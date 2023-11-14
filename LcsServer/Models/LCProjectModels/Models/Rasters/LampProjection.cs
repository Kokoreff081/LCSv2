using LcsServer.Models.LCProjectModels.GlobalBase;

namespace LcsServer.Models.LCProjectModels.Models.Rasters;

public class LampProjection : ICloneable
{
    /// <summary>
    /// Создание проекции
    /// </summary>
    /// <param name="lampId"> Id светильника</param>
    /// <param name="pixelsCount">Кол-во источников в светильнике</param>
    /// <param name="colorsCount">Кол-во цветов которыми может светить светильник</param>
    public LampProjection(int lampId, int pixelsCount = 0, int colorsCount = 0)
    {
        LampId = lampId;
        PixelsCount = pixelsCount;
        ColorsCount = colorsCount;
        Points = new List<IntPoint>();
    }

    /// <summary>
    /// Id-лампы
    /// </summary>
    public int LampId { get; }

    /// <summary>
    /// Кол-во источников в светильнике
    /// </summary>
    public int PixelsCount { get; }

    /// <summary>
    /// Кол-во цветов подерживает лампа
    /// </summary>
    public int ColorsCount { get; }

    /// <summary>
    /// Координаты на растровом изображнении
    /// </summary>
    public List<IntPoint> Points { get; private set; }

    /// <summary>
    /// Добавление точки в коллекция точек
    /// </summary>
    /// <param name="point"></param>
    public void AddPoint(IntPoint point)
    {
        Points.Add(point);
    }

    public object Clone()
    {
        var other = (LampProjection) MemberwiseClone();
        other.Points = new List<IntPoint>(Points);
            
        return other;
    }

    public IntPoint GetFirstPoint()
    {
        //return _points.Count == 0 ? new IntPoint() : _points[0]; TODO zergud
        return Points.FirstOrDefault();
    }
}