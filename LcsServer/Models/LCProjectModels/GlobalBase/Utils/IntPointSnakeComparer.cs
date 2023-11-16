namespace LcsServer.Models.LCProjectModels.GlobalBase.Utils;

public class IntPointSnakeComparer : IComparer<IntPoint>
{
    public int Compare(IntPoint pointA, IntPoint pointB)
    {
        if (pointA.Equals(pointB))
            return 0;

        if (pointA.Y > pointB.Y)
            return 1;

        if (pointA.Y == pointB.Y)
        {
            if (pointA.Y % 2 == 0)
                return pointA.X > pointB.X ? 1 : -1;

            return pointA.X > pointB.X ? -1 : 1;
        }

        return -1;
    }
}