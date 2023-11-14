using System.Numerics;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.Rasters;

[JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 1)]
    public class RasterV1 : BaseVC
    {
        private const string ModelClassName = "Raster";

        public int DimensionX { get; set; }
        public int DimensionY { get; set; }
        public int Angle { get; set; }
        public bool IsFlipHorizontal { get; set; }
        public bool IsFlipVertical { get; set; }
        public int[] IdMap { get; set; }
        public long[][] ProjectionMap { get; set; }
        public bool ManualSet { get; set; }
        public string Rotation { get; set; }

        public override ISaveLoad ToConcreteObject()
        {
            Quaternion? rotation = null;
            if (!string.IsNullOrEmpty(Rotation))
            { 
                string[] orientationParams = Rotation.Split(';');

                float orientationX = LCMath.ParseFloat(orientationParams[0]);
                float orientationY = LCMath.ParseFloat(orientationParams[1]);
                float orientationZ = LCMath.ParseFloat(orientationParams[2]);
                float orientationW = LCMath.ParseFloat(orientationParams[3]);

                rotation = new Quaternion(orientationX, orientationY, orientationZ, orientationW);
            }

            Raster raster = new Raster(Id, Name, DimensionX, DimensionY, 
                Angle, IsFlipHorizontal, IsFlipVertical, IdMap, ProjectionMap, ManualSet, rotation);

            return raster;
        }

        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            Raster raster = (Raster)o;

            int[] idMap = new int[raster.ProjectionMapping.Count];
            long[][] projectionMap = new long[raster.ProjectionMapping.Count][];
            int i = 0;
            foreach (var pr in raster.ProjectionMapping)
            {
                idMap[i] = pr.LampId;
                projectionMap[i] = new long[pr.Points.Count];
                for (int j = 0; j < pr.Points.Count; j++)
                {
                    var point = pr.Points[j];
                    var longValue = ((long)point.X << 32) + point.Y;
                    projectionMap[i][j] = longValue;
                }
                i++;
            }

            string rotation = null;
            if (raster.Rotation != null)
            {
                rotation = string.Join(";", LCMath.FloatToString(raster.Rotation.Value.X),
                                            LCMath.FloatToString(raster.Rotation.Value.Y), 
                                            LCMath.FloatToString(raster.Rotation.Value.Z), 
                                            LCMath.FloatToString(raster.Rotation.Value.W));
            }

            RasterV1 rasterVc = new RasterV1
            {
                Id = raster.Id,
                Name = raster.Name,
                DimensionX = raster.DimensionX,
                DimensionY = raster.DimensionY,
                Angle = raster.Angle,
                IsFlipHorizontal = raster.IsFlipHorizontal,
                IsFlipVertical = raster.IsFlipVertical,
                IdMap = idMap,
                ProjectionMap = projectionMap,
                ManualSet = raster.ManualSet,
                Rotation = rotation
            };

            return rasterVc;
        }
    }