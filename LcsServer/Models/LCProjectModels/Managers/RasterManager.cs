using System;
using System.Collections.Generic;
using System.Linq;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LCSVersionControl;

namespace LcsServer.Models.LCProjectModels.Managers;

public class RasterManager : BaseLCObjectsManager
    {
        //private readonly ISerializationManager _serializer;
        private readonly VersionControlManager _versionControlManagerEx;

        public virtual event EventHandler<UpdateObjectsEventArgs> ObjectsUpdated;

        public RasterManager(VersionControlManager versionControlManagerEx)
        {
            //_serializer = serializer;
            _versionControlManagerEx = versionControlManagerEx;
        }

        /// <summary>
        /// Временный растр
        /// </summary>
        public Raster TempRaster { get; set; }

        public override void AddObjects(params LCObject[] objects)
        {
            var result = AddRangeToLcObjectsList(objects);
            ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(result, InformAction.Add));
        }

        public override void RemoveObjects(LCObject[] objects)
        {
            List<LCObject> rasters = objects.OfType<Raster>().Cast<LCObject>().ToList();
            RemoveRangeFromLcObjectsList(rasters.ToArray());
            ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(rasters, InformAction.Remove));
        }

        public override void RemoveAllObjects()
        {
            foreach (Raster raster in GetPrimitives<Raster>())
                raster.Clear();

            ClearLcObjectsList();
            ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(null, InformAction.RemoveAll));
        }

        #region Save/Load

        /// <summary>
        /// Сохранить растры
        /// </summary>
        /// <param name="projectPath">Путь до папки проекта</param>
        /// <param name="projectName">Имя проекта</param>
        /// <param name="autoSave">Автосохранение</param>
        public void Save(string projectPath, string projectName, bool autoSave)
        {
            string rasterFolder = FileManager.GetRastersFolderPath(projectPath);
            var rasterFile = FileManager.GetRastersFilePath(projectPath);
            FileManager.CreateIfNotExist(rasterFolder);

            _versionControlManagerEx.ConvertToVCAndSave(GetPrimitives<Raster>(), projectPath, rasterFile, true);
        }

        /// <summary>
        /// Загрузить растры
        /// </summary>
        /// <param name="projectPath">Путь до папки проекта</param>
        /// <param name="isNewFormat">Новый формат проекта</param>
        /// <returns>Список растров</returns>
        public void Load(string projectPath, bool isNewFormat)
        {
            FileManager.CreateIfNotExist(FileManager.GetRastersFolderPath(projectPath));
            string rasterFileName = FileManager.GetRastersFilePath(projectPath);

            var rasterItems = _versionControlManagerEx.LoadAndConvertFromVC(rasterFileName, true);

            for (var i = 0; i < rasterItems.Count; i++)
            {
                var saveLoadObject = rasterItems[i];
                saveLoadObject.Load(rasterItems, i, projectPath);
            }

            List<LCObject> rasters = rasterItems.Cast<LCObject>().ToList();

            AddObjects(rasters.ToArray());
        }

        #endregion

    }