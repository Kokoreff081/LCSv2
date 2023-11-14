namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public class LampColor
{
    private static readonly Dictionary<ColorCodes, string> ColorCodeAndRepresentation;

    //private int _waveLength;

    static LampColor()
    {
        ColorCodeAndRepresentation = new Dictionary<ColorCodes, string>
        {
            {ColorCodes.Amber, "A"},
            {ColorCodes.Blue, "B"},
            {ColorCodes.Cian, "C"},
            {ColorCodes.Green, "G"},
            {ColorCodes.Red, "R"},
            {ColorCodes.White3000, "W3000"},
            {ColorCodes.White4000, "W4000"},
            {ColorCodes.White5000, "W5000"}
        };
    }
        
    public  LampColor(ColorCodes colorCode)
    {
        ColorCode = colorCode;
    }

    public LampColor(string color) 
    {
        string[] colorWaveLength = color.Split('_');
        if (colorWaveLength.Length < 1)
            return;
            
        var colorCode = ColorCodeAndRepresentation.FirstOrDefault(x => x.Value.Equals(colorWaveLength[0]));
        ColorCode = colorCode.Key;
//            if (colorWaveLength.Length < 2)
//                return;

//            if(int.TryParse(colorWaveLength[1], out int intValue))
//            {
//                _waveLength = intValue;
//            }
    }

    public ColorCodes ColorCode { get; set; }

    public override string ToString()
    {
        var colorCode = ColorCodeAndRepresentation.FirstOrDefault(x => x.Key == ColorCode);
        //if (ColorCode == ColorCodes.Amber || ColorCode == ColorCodes.Blue || ColorCode == ColorCodes.Cian ||
        //    ColorCode == ColorCodes.Green || ColorCode == ColorCodes.Red)
        //{
        //    return $"{colorCode.Value}_{_waveLength}";
        //}

        return colorCode.Value;

    }
}