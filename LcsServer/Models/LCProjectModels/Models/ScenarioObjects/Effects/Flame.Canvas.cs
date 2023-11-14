using LCSVersionControl;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public partial class Flame
{
    private struct Canvas
    {
        public Canvas(int dimX, int dimY, int intense)
        {
            Width = dimX;
            Height = dimY;

            BotLine = new int[dimX];

            Intense = intense;

            Flame1 = null;
            Flame2 = null;

            Flame1 = GetArray(dimX, dimY);
            Flame2 = GetArray(dimX, dimY);
        }

        public int Intense;
        public readonly int[] BotLine;
        public readonly int[][] Flame1;
        public readonly int[][] Flame2;

        public readonly int Width;
        public readonly int Height;

        public void RecalculateHottestSpots(int stability)
        {
            //int indent = 1;
            int interval = Width;
            int maxWidth = 1 + Width / 10;
            
            HotSpot[] hots = new HotSpot[maxWidth];
            for(int i = 0; i < maxWidth; i++)
            {
                hots[i].Width = 1 + LCRandom.RandomNext(maxWidth);
                hots[i].Pos = LCRandom.RandomNext(interval);
            }

            for(int i = 0; i < maxWidth; i++)
            {
                hots[i].Width += LCRandom.RandomNext(3) - 1;
                if(hots[i].Width < 1)
                {
                    hots[i].Width = 1;
                }
                if(hots[i].Width > maxWidth)
                {
                    hots[i].Width = maxWidth;
                }

                int initTemperature = MAX_HOT_VAL * stability / MAX_STABILITY + LCRandom.RandomNext(MAX_HOT_VAL * (MAX_STABILITY - stability) / 10);

                int x = hots[i].Pos;
                if(x >= Width)
                { x = Width - 1; }
                if(x < 0)
                { x = 0; }
                for(int j = 0; j < hots[i].Width; j++)
                {
                    BotLine[x] = initTemperature;
                    if(++x >= Width)
                    {
                        break;
                    }
                }
            }
        }

        private static int[][] GetArray(int dimX, int dimY)
        {
            var array = new int[dimY][];
            for(int i = 0; i < dimY; i++)
            {
                array[i] = new int[dimX];
            }

            return array;
        }
    }

    private struct HotSpot
    {
        public int Width;
        public int Pos;
    };
}