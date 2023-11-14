namespace LcsServer.Models.LCProjectModels.GlobalBase;

public class IntPoint //: ICloneable//record struct IntPoint(int X, int Y)
{
    public IntPoint(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public IntPoint()
    {
        this.X = 0;
        this.Y = 0;
    }
    /// <summary>
    /// Создание структуры с заданными координатами
    /// </summary>
    /// <param name="z"> Координаты, запакованные в long-значение </param>
    public IntPoint(long z) : this((int)(z >> 32), (int)(z & 0xffffffff))
    {
    }

    public IntPoint(IntPoint pointToCopy) : this(pointToCopy.X, pointToCopy.Y)
    {
           
    }
    /// <summary>
    /// Координата X
    /// </summary>
    public readonly int X;

    /// <summary>
    /// Координата Y
    /// </summary>
    public readonly int Y;


    public static IntPoint MinusOne = new(-1, -1);

    /*public object Clone()
    {
        return (IntPoint)new IntPoint(this);
    }*/
}