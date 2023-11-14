using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

/// <summary>
/// Класс, описывающий заполняющий эффект
/// </summary>
public class ModbusStaticColor : BaseModbusEffect
{
    private CompositeColor _backgroundColor;
    private CompositeColor _color;

    /// <summary>
    /// Изменение цвета фона
    /// </summary>
    public event EventHandler BackgroundColorChanged;

    /// <summary>
    /// Изменение цвета заливки
    /// </summary>
    public event EventHandler ColorChanged;


    public ModbusStaticColor()
    {
        _color = CompositeColor.RedColor;
        _backgroundColor = CompositeColor.BlackColor;
    }

    public ModbusStaticColor(int id, string name, int parentId, CompositeColor backgroundColor, CompositeColor color,
        long totalTicks, long riseTime, long fadeTime, float dimmingLevel, string ipAddress, ushort port, byte unitId,
        ushort register, int interval, float min, float max, SensorUnits sensorUnits) : this()
    {
        Id = id;
        Name = name;
        _parentId = parentId;
        BackgroundColor = backgroundColor;
        Color = color;
        TotalTicks = totalTicks;
        RiseTime = riseTime;
        FadeTime = fadeTime;
        DimmingLevel = dimmingLevel;
            
        IpAddress = ipAddress;
        Port = port;
        UnitId = unitId;
        Register = register;
        Interval = interval;
        Minimum = min;
        Maximum = max;
        SensorUnits = sensorUnits;
    }

    /// <summary>
    /// Категория
    /// </summary>
    public override Category Category => Category.Modbus;

    /// <summary>
    /// Тип
    /// </summary>
    public override Enum Type => ModbusTypes.ModbusStatic;


    /// <summary>
    /// Цвет фона
    /// </summary>
    public CompositeColor BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            if (_backgroundColor == value)
            {
                return;
            }

            _backgroundColor = value;
            BackgroundColorChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Цвет заливки
    /// </summary>
    public CompositeColor Color
    {
        get => _color;
        set
        {
            if (_color == value)
            {
                return;
            }

            _color = value;
            ColorChanged?.Invoke(this, EventArgs.Empty);
        }
    }

        
           

    /// <summary>
    /// Вызывается при сохранении проекта
    /// </summary>
    /// <param name="projectFolderPath">Путь к папке проекта</param>
    public override void Save(string projectFolderPath)
    {
    }


    protected override Effect Clone(IPlayingEntity parent)
    {
        var clone = new ModbusStaticColor
        {
            Parent = parent,
            Color = _color,
            BackgroundColor = _backgroundColor,
            TotalTicks = TotalTicks
        };

        return clone;
    }

    protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
    {
        ReadValue();
        
        float value = FloatValue;

        float l1 = Minimum;
        float l2 = Maximum;

        if (Maximum < Minimum)
        {
            l1 = Maximum;
            l2 = Minimum;
        }

        CompositeColor resultColor = value >= l1 && value <= l2 ? _color : _backgroundColor;

        frame.FillByColor(resultColor * riseOrFadeLevel);
        return true;
    }
}