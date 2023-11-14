namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public record struct RGB(byte R, byte G, byte B)
{
    public readonly byte R = R;
    public readonly byte G = G;
    public readonly byte B = B;

    public float GetLevel()
    {
        var result = (R + G + B) / 3f;
        return result;
    }
}