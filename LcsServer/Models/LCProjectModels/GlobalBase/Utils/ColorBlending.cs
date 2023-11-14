using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Utils;

public static class ColorBlending
    {
        public static CompositeColor Blend(CompositeColor c1, CompositeColor c2, Mixing mix, float opacity)
        {
            //return Actions[mix].Invoke(c1, c2);
            switch (mix)
            {
                case Mixing.Dissolve:
                    return DissolveBlendColors(c1, c2);
                case Mixing.Darken:
                    return DarkenBlendColors(c1, c2, opacity);
                case Mixing.Multiply:
                    return MultiplyBlendColors(c1, c2, opacity);
                case Mixing.Lighten:
                    return LightenBlendColors(c1, c2, opacity);
                case Mixing.LastReceived:
                    return c2.Equals(CompositeColor.BlackColor) ? c1 : c2;
                case Mixing.Normal:
                default:
                    return NormalBlendColors(c1, c2, opacity);
            }
        }

        private static CompositeColor LightenBlendColors(CompositeColor c1, CompositeColor c2, float opacity)
        {
            var r = (byte)Math.Max(c1.Red, c2.Red   * opacity);
            var g = (byte)Math.Max(c1.Green, c2.Green * opacity);
            var b = (byte)Math.Max(c1.Blue, c2.Blue  * opacity);
            return new CompositeColor(r, g, b);
        }

        private static CompositeColor DissolveBlendColors(CompositeColor c1, CompositeColor c2)
        {
            return LCRandom.RandomNext(100) < 20 ? c1 : c2;
        }

        private static CompositeColor MultiplyBlendColors(CompositeColor c1, CompositeColor c2, float opacity)
        {
            var r = (byte)(c1.Red * c2.Red * opacity / 255.0);
            var g = (byte)(c1.Green * c2.Green * opacity / 255.0);
            var b = (byte)(c1.Blue * c2.Blue * opacity / 255.0);
            return new CompositeColor(r, g, b);
        }

        private static CompositeColor NormalBlendColors(CompositeColor c1, CompositeColor c2, float opacity)
        {
            var r = (byte)(c1.Red   * (1 - opacity) + c2.Red   * opacity);
            var g = (byte)(c1.Green * (1 - opacity) + c2.Green * opacity);
            var b = (byte)(c1.Blue  * (1 - opacity) + c2.Blue  * opacity);
            return new CompositeColor(r, g, b);
        }

        private static CompositeColor DarkenBlendColors(CompositeColor c1, CompositeColor c2, float opacity)
        {
            var r = (byte)Math.Min(c1.Red, c2.Red   * opacity);
            var g = (byte)Math.Min(c1.Green, c2.Green * opacity);
            var b = (byte)Math.Min(c1.Blue, c2.Blue  * opacity);
            return new CompositeColor(r, g, b);
        }
    }