namespace LCSVersionControl;

public static class LCRandom
{
    private static readonly float[] FastFloatRandomArray;
    private static int _indexRandomArray;
    private static readonly int SizeRandomArray;

    static LCRandom()
    {
        SizeRandomArray = 1000000;
        FastFloatRandomArray = new float[SizeRandomArray];

        var r = new Random();
        for (int i = 0; i < SizeRandomArray; i++)
        {
            FastFloatRandomArray[i] = (float)r.NextDouble();
        }
    }

    public static int RandomNext(int maximum)
    {
        var value = FastFloatRandomArray[_indexRandomArray++];
        if (_indexRandomArray >= SizeRandomArray)
        {
            _indexRandomArray = 0;
        }
        return (int)((maximum - 1) * value);
    }

    public static int RandomNext(int minimum, int maximum)
    {
        return minimum + RandomNext(maximum - minimum);
    }

    public static byte RandomNextByte()
    {
        var value = FastFloatRandomArray[_indexRandomArray++];
        if (_indexRandomArray >= SizeRandomArray)
        {
            _indexRandomArray = 0;
        }
        return (byte)((255 - 1) * value);
    }
}