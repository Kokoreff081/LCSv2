using System;
using System.Collections.Generic;
using System.IO;
using LCSVersionControl.Interfaces;

namespace LCSVersionControl;

public class VersionControlManager
{
    private readonly JsonSerializationManager _jsonSerializationManager;

        public VersionControlManager()
        {
            _jsonSerializationManager = new JsonSerializationManager();
        }

        /// <summary>
        /// Сконвертировать объекты в соответствующие объекты Version control
        /// </summary>
        /// <param name="lst">Список объектов</param>
        /// <param name="projectPath">Путь до папки с проектом</param>
        /// <returns>Возвращает список объектов Version control</returns>
        private List<BaseVC> ConvertToVC(IEnumerable<ISaveLoad> lst, string projectPath)
        {
            List<BaseVC> lstVc = new List<BaseVC>();
            foreach (var item in lst)
            {
                item.Save(projectPath);
                BaseVC obj = BaseVC.CreateFromConcreteObject(item);
                lstVc.Add(obj);
            }
            return lstVc;
        }

        /// <summary>
        /// Сконвертировать объекты Version control в соответствующие объекты
        /// </summary>
        /// <param name="lstVc">Список объектов Version control</param>
        /// <returns>Возвращает список объектов сконвертированных из Version control</returns>
        private List<ISaveLoad> ConvertFromVC(List<BaseVC> lstVc)
        {
            List<ISaveLoad> lst = new List<ISaveLoad>();

            foreach (var item in lstVc)
            {
                BaseVC lastVcItemVersion = BaseVC.VCItemToLastVersion(item);
                var obj = lastVcItemVersion.ToConcreteObject();

                lst.Add(obj);
            }
            return lst;
        }

        /// <summary>
        /// Сконвертировать объекты в соответствующие объекты Version control и сохранить в файл(Файл формата json и архивирован)
        /// </summary>
        /// <param name="lst">Список объектов</param>
        /// <param name="projectPath">Путь до папки с проектом</param>
        /// <param name="filePath">Путь к файлу(Файл формата json и архивирован)</param>
        /// <param name="zipFile">Зиповать файл или нет</param>
        public void ConvertToVCAndSave(IEnumerable<ISaveLoad> lst, string projectPath, string filePath, bool zipFile)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath can not be null");

            List<BaseVC> baseVcObjectsList = ConvertToVC(lst, projectPath);

            byte[] data = _jsonSerializationManager.Serialize(baseVcObjectsList);

            if (zipFile)
                data = ZipHelper.ZipByteArrayToByteArray(data);

            string tempFilePath = Path.Combine(Path.GetDirectoryName(filePath), $"{Path.GetFileNameWithoutExtension(filePath)}_temp{Path.GetExtension(filePath)}");
            File.WriteAllBytes(tempFilePath, data);

            File.Delete(filePath);
            File.Move(tempFilePath, filePath);
        }

        /// <summary>
        /// Загрузить с файла и сконвертировать объекты Version control в соответствующие объекты
        /// </summary>
        /// <param name="filePath">Путь к файлу(Файл формата json и архивирован)</param>
        /// <param name="isZipFile">Зип файл или нет</param>
        /// <returns>Возвращает список объектов сконвертированных из Version control</returns>
        public List<ISaveLoad> LoadAndConvertFromVC(string filePath, bool isZipFile)
        {
            using Stream unzippedString = isZipFile ? ZipHelper.UnzipFileToStream(filePath) : File.OpenRead(filePath);
            
            List<ISaveLoad> result = new List<ISaveLoad>();
            
            if (unzippedString == null)
            {
                return result;
            }
            
            var list = _jsonSerializationManager.Deserialize<List<BaseVC>>(unzippedString);
            
            // var unzippedString = isZipFile ? ZipHelper.UnzipFileToString(filePath) : File.ReadAllText(filePath);
            //
            // List<ISaveLoad> result = new List<ISaveLoad>();
            //
            // if (string.IsNullOrEmpty(unzippedString))
            // {
            //     return result;
            // }
            //
            // var list = _jsonSerializationManager.Deserialize<List<BaseVC>>(unzippedString);
           
            foreach (var baseVc in list)
            {
                if (baseVc != null)
                {
                    result.Add(BaseVC.VCItemToLastVersion(baseVc).ToConcreteObject());
                }
            }

            return result;
            //return _jsonSerializationManager.Deserialize<List<BaseVC>>(unzippedString).Select(BaseVC.VCItemToLastVersion).Select(lastVcItemVersion => lastVcItemVersion.ToConcreteObject()).ToList();
        }
}