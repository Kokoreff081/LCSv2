using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.Models.Rasters;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public class EffectFrame
{
    public EffectFrame(int width, int height, List<LampProjection> projection)
    {
        Width = width;
        Height = height;
        Projection = projection;
        //int count = Projection.Sum(lampProjection => lampProjection.Points.Count);
        Colors = new Dictionary<IntPoint, CompositeColor>();
    }

    /// <summary>
    /// Растровая проекция
    /// </summary>
    public List<LampProjection> Projection { get; }

    /// <summary>
    /// Коллекция цветов проекции
    /// </summary>
    public Dictionary<IntPoint, CompositeColor> Colors { get; }

    /// <summary>
    /// Размер проекции
    /// </summary>
    //public IntSize Size { get; }

    /// <summary>
    /// Ширина проекции
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Высота проекции
    /// </summary>
    public int Height { get; }


    /// <summary>
    /// закрасить в один цвет
    /// </summary>
    /// <param name="color"> Цвет </param>
    public void FillByColor(CompositeColor color)
    {
        foreach (var lampProjection in Projection)
        {
            foreach (var point in lampProjection.Points)
            {
                Colors[point] = color;
            }
        }
    }
}