using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LcsServer.Models.LCProjectModels.GlobalBase;

public abstract class BaseLCObjectsManager
    {
        private readonly Dictionary<int, LCObject> _lcObjects;
        private readonly object _lockObject = new object();
        private volatile int _currentId = 2;
        private readonly Regex regex = new Regex(@"(?<name>.*)_(?<id>[0-9]*$)", RegexOptions.Compiled);

        protected BaseLCObjectsManager()
        {
            _lcObjects = new Dictionary<int, LCObject>();
        }

        public int GetNewCurrentId()
        {
            return Interlocked.Increment(ref _currentId);
        }

        /// <summary>
        /// Возвращает уникальное имя 
        /// </summary>
        /// <param name="name">Предполагаемое имя объекта</param>
        /// <param name="parentId">Id родительского объекта</param>
        /// <param name="blackList">Черный список имен</param>
        /// <param name="addNumberForFirstElement">Первый объект с таким именим будет иметь в конце индекс или нет</param>
        /// <returns>Новое имя</returns>
        public string GetNewName(string name, int parentId = 0, IEnumerable<string> blackList = null, bool addNumberForFirstElement = true)
        {
            string originalName = regex.IsMatch(name) ? regex.Match(name).Groups["name"].Value : name;
            
            bool Predicate(LCObject h) => !string.IsNullOrEmpty(h.Name) && h.Name.StartsWith(originalName, StringComparison.InvariantCultureIgnoreCase) && regex.IsMatch(h.Name) && h.ParentId == parentId;
            
            //var primitives = GetPrimitives((Predicate<LCObject>) Predicate);
            bool primitiveFinded = false;
            List<int> objectsWithName = new List<int>();
            foreach (LCObject primitive in GetPrimitives((Predicate<LCObject>) Predicate))
            {
                primitiveFinded = true;
                if (int.TryParse(regex.Match(primitive.Name).Groups["id"].Value, out int id))
                {
                    objectsWithName.Add(id);
                }
            }
            
            if (blackList != null)
            {
                objectsWithName.AddRange(blackList.Where(x => regex.IsMatch(x)).Select(x => int.Parse(regex.Match(x).Groups["id"].Value)));
            }

            int max = 0;
            if (objectsWithName.Count > 0)
            {
                max = objectsWithName.Max() + 1;
            }
            else if (primitiveFinded)
            {
                max = 1;
            }

            if (addNumberForFirstElement && max == 0)
            {
                max = 1;
            }

            return max > 0 ? $"{originalName}_{max}" : originalName;
        }

        public LCObject GetLCObjectById(int id)
        {
            _lcObjects.TryGetValue(id, out var lcObject) ;
            return lcObject;
        }

        /// <summary>
        /// Получить объекты
        /// </summary>
        /// <param name="ids">Список id объектов</param>
        /// <returns>Возвращает список объектов по данным id</returns>
        public List<LCObject> GetLcObjectsById(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return new List<LCObject> ();
            }
            
            var res = new List<LCObject>(ids.Count);

            foreach (var id in ids)
            {
                if (_lcObjects.TryGetValue(id, out var lcObject))
                {
                    res.Add(lcObject);
                }
            }

            return res;// ids.Where(x => _lcObjects.ContainsKey(x)).Select(id => _lcObjects[id]).ToList();
        }

        /// <summary>
        /// Получить элементы по условию
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="predicate">Условие</param>
        /// <returns>Возвращает список найденых объектов</returns>
        public IReadOnlyCollection<T> GetPrimitives<T>([NotNull]Predicate<T> predicate) where T : LCObject
        {
            var primitives = _lcObjects.Values.OfType<T>().ToList();
            return primitives.FindAll(predicate);
        }
        
        public IReadOnlyCollection<T> GetPrimitives<T>() where T : LCObject
        {
            return _lcObjects.Values.OfType<T>().ToList();
        }
        

        /// <summary>
        /// Получить элементы отсортированные по id
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="predicate">Условие</param>
        /// <returns>Возвращает список найденых объектов</returns>
        protected IReadOnlyCollection<T> GetOrderedPrimitives<T>(Predicate<T> predicate = null) where T : LCObject
        {
           var orderedLcObjects = _lcObjects.OrderBy(x => x.Key).Select(x=>x.Value).OfType<T>().ToList();
            return predicate == null ? orderedLcObjects : orderedLcObjects.FindAll(predicate);
        }

        /// <summary>
        /// Добавить объекты
        /// </summary>
        /// <param name="objects">Объекты</param>
        public abstract void AddObjects(params LCObject[] objects);

        /// <summary>
        /// Удалить объекты
        /// </summary>
        /// <param name="objects">Объекты для удаления</param>
        public abstract void RemoveObjects(LCObject[] objects);

        /// <summary>
        /// Удалить все  объекты
        /// </summary>
        public abstract void RemoveAllObjects();

        public virtual bool CanRemoveObjects(params LCObject[] objects)
        {
            return true;
        }

        /// <summary>
        /// Очищает список объектов
        /// </summary>
        protected void ClearLcObjectsList()
        {
            _lcObjects.Clear();
            _currentId = 2;
        }

        /// <summary>
        /// Удалить объекты
        /// </summary>
        /// <param name="items">Объекты</param>
        protected void RemoveRangeFromLcObjectsList(params LCObject[] items)
        {
            lock (_lockObject)
            {
                foreach (LCObject lcObject in items)
                {
                    _lcObjects.Remove(lcObject.Id);
                }
            }
        }

        /// <summary>
        /// Добавить объекты
        /// </summary>
        /// <param name="items">Объекты</param>
        protected List<LCObject> AddRangeToLcObjectsList(params LCObject[] items)
        {

            List<LCObject> addedItems = new List<LCObject>();

            lock (_lockObject)
            {
                foreach (LCObject lcObject in items)
                {
                    if (_lcObjects.ContainsKey(lcObject.Id))
                    {
                        continue;
                    }

                    _lcObjects.Add(lcObject.Id, lcObject);
                    
                    addedItems.Add(lcObject);

                    if (lcObject.Id > _currentId)
                    {
                        _currentId = lcObject.Id;
                    }
                }
            }

            return addedItems;
        }

    }