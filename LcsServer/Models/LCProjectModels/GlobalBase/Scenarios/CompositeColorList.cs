namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public class CompositeColorList
    {
        private const int PalSize = 256;
        private readonly List<CompositeColor> _colorStack;

        private IReadOnlyList<CompositeColor> _readOnlyColl;

        public CompositeColorList(IEnumerable<CompositeColor> colors)
        {
            Pallete = new CompositeColor[PalSize];
            _colorStack = new List<CompositeColor>();

            ReplaceStack(colors);
        }

        /// <summary>
        /// Возвращает список композитных цветов
        /// </summary>
        public IReadOnlyList<CompositeColor> Get => _readOnlyColl;

        public CompositeColor[] Pallete { get; }

        /// <summary>
        /// Кол-во композитных цветов
        /// </summary>
        public int Count => _readOnlyColl.Count;

        /// <summary>
        /// Заменить список композитных цветов
        /// </summary>
        /// <param name="colors"></param>
        public void ReplaceStack(IEnumerable<CompositeColor> colors)
        {
            _colorStack.Clear();
            _colorStack.AddRange(colors);
            OnColorStackChange();
        }

        /// <summary>
        /// Добавить композитный цвет
        /// </summary>
        /// <param name="composite"></param>
        public void Add(CompositeColor composite)
        {
            _colorStack.Add(composite);
            OnColorStackChange();
        }

        /// <summary>
        /// Заменить композитный цвет
        /// </summary>
        /// <param name="colorId">Id цвета</param>
        /// <param name="newComposite">Новый композитный цвет</param>
        public void Replace(int colorId, CompositeColor newComposite)
        {
            _colorStack[colorId] = newComposite;
            OnColorStackChange();
        }

        /// <summary>
        /// Удалить композитный цвет
        /// </summary>
        /// <param name="colorId">Id цвета</param>
        public void Remove(int colorId)
        {
            _colorStack.RemoveAt(colorId);
            OnColorStackChange();
        }

        public void InsertAt(CompositeColor newComposite, int index)
        {
            _colorStack.Insert(index, newComposite);
        }
        /// <summary>
        /// Клонировать объект
        /// </summary>
        /// <returns>Новый склонированный объект</returns>
        public CompositeColorList Clone()
        {
            var lst = new CompositeColorList();
            foreach (var color in _colorStack)
            {
                lst.Add(color);
            }

            for (int i = 0; i < PalSize; i++)
            {
                lst.Pallete[i] = Pallete[i];
            }

            lst.UpdateReadOnlyCollection();
            return lst;
        }

        /// <summary>
        /// Проверяет данный объект на идентичность с другим объектом
        /// </summary>
        /// <param name="obj">Объект для сравнения</param>
        /// <returns>Идентичные или нет объекты</returns>
        public override bool Equals(object obj)
        {
            var c2 = obj as CompositeColorList;
            if (c2 == null || c2.Count != Count) return false;

            for (int i = 0; i < c2.Count; i++)
            {
                if (c2._colorStack[i] == _colorStack[i]) continue;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Возвращает хеш код объекта
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private void UpdateReadOnlyCollection()
        {
            _readOnlyColl = _colorStack.AsReadOnly();
        }

        private CompositeColorList()
        {
            Pallete = new CompositeColor[PalSize];
            _colorStack = new List<CompositeColor>();
            UpdateReadOnlyCollection();
        }

        private void OnColorStackChange()
        {
            CalculatePalette();
            UpdateReadOnlyCollection();
        }

        private void CalculatePalette()
        {
            int n_bands = _colorStack.Count;

            if(n_bands <= 1)
            {
                var color = n_bands == 0
                    ? CompositeColor.BlackColor
                    : _colorStack[0];

                for(int i = 0; i < PalSize; i++)
                {
                    Pallete[i] = color;
                }

                return;
            }

            n_bands--;
            int band_width = PalSize / n_bands;
            int rest = PalSize % n_bands;
            for(int i = 0; i < n_bands; i++)
            {
                int offset = (int)(i * band_width);
                int next = i + 1;
                if(next >= n_bands)
                {
                    band_width += rest;
                }
                CalculateColor(_colorStack[i], _colorStack[next], offset, band_width);
            }
        }

        private void CalculateColor(CompositeColor c1, CompositeColor c2, int offset, int bandWidth)
        {
            //var argb1 = c1.CalculateResultColor();
            //var argb2 = c2.CalculateResultColor();

            for(int i = 0; i < bandWidth; i++)
            {
                byte r = (byte)(c1.Red - i * (c1.Red - c2.Red) / bandWidth);
                byte g = (byte)(c1.Green - i * (c1.Green - c2.Green) / bandWidth);
                byte b = (byte)(c1.Blue - i * (c1.Blue - c2.Blue) / bandWidth);

                byte a = (byte)(c1.Amber - i * (c1.Amber - c2.Amber) / bandWidth);
                byte cct = (byte)(c1.CCT - i * (c1.CCT - c2.CCT) / bandWidth);
                byte w = (byte)(c1.White - i * (c1.White - c2.White) / bandWidth);

                Pallete[offset + i] = new CompositeColor(r, g, b, a, cct, w);
            }
        }

        public List<CompositeColor> ToList()
        {
            return _colorStack;
        }
    }