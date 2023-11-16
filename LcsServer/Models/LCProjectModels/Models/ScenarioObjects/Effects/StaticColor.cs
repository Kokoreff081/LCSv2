using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;


namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public class StaticColor : Effect
{
    private CompositeColor _backgroundColor;

    public event EventHandler BackgroundColorChanged;

    public StaticColor()
    {
        _backgroundColor = CompositeColor.WhiteColor;
    }

    public StaticColor(int id, string name, int parentId, CompositeColor background, long totalTicks, long riseTime,
        long fadeTime, float dimmingLevel)
    {
        Id = id;
        Name = name;
        _parentId = parentId;
        BackgroundColor = background;
        TotalTicks = totalTicks;
        RiseTime = riseTime;
        FadeTime = fadeTime;
        DimmingLevel = dimmingLevel;
    }

    public override Category Category => Category.Filling;
    public override Enum Type => FillingTypes.Static;

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

    public override void Save(string projectFolderPath)
    {
    }

    protected override Effect Clone(IPlayingEntity parent)
    {
        var clone = new StaticColor {
            Parent = parent,
            BackgroundColor = _backgroundColor,
            TotalTicks = TotalTicks,
            Name = Name
        };


        return clone;
    }

    protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
    {
        frame.FillByColor(_backgroundColor * riseOrFadeLevel);
        return true;
    }
}