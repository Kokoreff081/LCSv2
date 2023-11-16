using System.ComponentModel;
using LcsServer.Models.LCProjectModels.GlobalBase.Utils;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LightCAD.UI.Strings;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Addressing.CreationParams;

public partial class AddressingCreationParams : LCObject
{
    private OrderTypes _orderType;

    public event Action<OrderTypes> OrderTypeChanged;

    public AddressingCreationParams()
    {
        LampProjection = new Raster();
        CreationOption = new ContinuousAddressingOption { StartDmxAddress = 0 };
    }

    public BaseOption CreationOption { get; set; }

    public Raster LampProjection { get; }

    /// <summary>
    /// Порядок адрессации
    /// </summary>
    public OrderTypes OrderType
    {
        get => _orderType;
        set
        {
            _orderType = value;
            OrderTypeChanged?.Invoke(_orderType);
        }
    }

    public IEnumerable<LampProjection> GetOrderedLampProjection()
    {
        IEnumerable<LampProjection> orderedProjection = OrderType == OrderTypes.LeftToRight ? 
            LampProjection.RotatedProjectionMapping.OrderBy(x => x.GetFirstPoint(), new IntPointComparer()) : 
            LampProjection.RotatedProjectionMapping.OrderBy(x => x.GetFirstPoint(), new IntPointSnakeComparer());

        return orderedProjection;
    }
}
/// <summary>
/// Порядок адрессации
/// </summary>
public enum OrderTypes
{
    [Description(nameof(Resources.LeftToRight))]
    LeftToRight,
    [Description(nameof(Resources.Snake))]
    Snake
}

public enum CreationParams
{
    [Description(nameof(Resources.Continuous))]
    ContinuousAddressing,
    [Description(nameof(Resources.IntegerRowsInUniverse))]
    IntegerRowsPerUniverse,
    [Description(nameof(Resources.LimitedRowsInUniverse))]
    LimitRowsPerUniverse,

}