using System.Numerics;
using LCSVersionControl;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public struct CompositeColor
    {
        private const float MaxCCT = 15000f;
        private const string Separator = "|";
        private const float CCT3K = 3000;
        private const float CCT5K = 5000;
        private const float CCT4K = 4000;

        //private const int GradientSize = 256 * 3;

        public CompositeColor(byte red, byte green, byte blue, byte amber = 0, float cct = 0, byte white = 0) : this()
        {
            Red = red;
            Green = green;
            Blue = blue;

            Amber = amber;
            White = white;

            CCT = cct;
        }

        public static readonly CompositeColor RedColor = new CompositeColor(255, 0, 0);
        public static readonly CompositeColor BlackColor = new CompositeColor(0, 0, 0);
        public static readonly CompositeColor GreenColor = new CompositeColor(0, 255, 0);
        public static readonly CompositeColor BlueColor = new CompositeColor(0, 0, 255);
        public static readonly CompositeColor YellowColor = new CompositeColor(255, 255, 0);
        public static readonly CompositeColor WhiteColor = new CompositeColor(255, 255, 255);
        public static readonly CompositeColor MagentaColor = new CompositeColor(255, 0, 255);
        public static readonly CompositeColor CyanColor = new CompositeColor(0, 158, 224);
        public static readonly CompositeColor AmberColor = new CompositeColor(255, 191, 0);
        public static readonly CompositeColor Max = new CompositeColor(255, 255, 255, 255, MaxCCT, 255);

        public float CCT;

        public byte White;

        public byte Amber;

        public byte Blue;

        public byte Green;

        public byte Red;

        public Vector4 ToVector4()
        {
            var rgba = CalculateResultColor();

            float r = rgba.R / 255.0f;
            float g = rgba.G / 255.0f;
            float b = rgba.B / 255.0f;
            //float Brightness = 0.212f * R + 0.7152f * G + 0.072f * B;
            float brightness = (float)Math.Sqrt(r * r * 0.241f + g * g * 0.691f + b * b * 0.068f);
            return new Vector4(r, g, b, brightness);
        }

        public override bool Equals(object obj)
        {
            return obj != null && this == (CompositeColor)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var rgba = (uint)(255 << 24 | Red << 16 | Green << 8 | Blue); // TODO WTF - 255 << 24
            return $"{rgba}{Separator}{CCT}{Separator}{White}{Separator}{Amber}";
        }

        public RGB CalculateResultColor()
        {
            RGB color = new RGB(Red, Green, Blue);
            if (Amber == 0 && White == 0)
            {
                return color;
            }

            float rgbLevel = color.GetLevel();
            if (Amber != 0)
            {
               RGB amber = Amber2Rgba();
               float amberLevel = amber.GetLevel();
               color = Lerp(color, amber, amberLevel / (rgbLevel + amberLevel));
            }

            if (White != 0)
            {
                RGB white;
                if (Red == 0 && Green == 0 && Blue == 0)
                    white = CctToRgba();
                else
                    white = WhiteToRgba();
                float whiteLevel = white.GetLevel();
                float rgbaLevel = color.GetLevel();
                return Lerp(color, white, whiteLevel / (rgbaLevel + whiteLevel));
            }

            return color;
        }

        public void GetHSB(out int hue, out byte saturation, out byte brightness)
        {
            RgbToHsv(out float h, out float s, out float b, Red, Green, Blue);

            hue = (int)h;
            saturation = (byte)s;
            brightness = (byte)b;
        }

        public static CompositeColor FromString(string value)
        {
            CompositeColor color = new CompositeColor();

            if (string.IsNullOrWhiteSpace(value))
            {
                return color;
            }

            string[] strs = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            if (strs.Length > 0 && uint.TryParse(strs[0], out uint rgba))
            {
                color = FromUInt32(rgba);
            }
            else
            {
                return color;
            }

            if (strs.Length > 1 && float.TryParse(strs[1], out float cct))
            {
                color.CCT = cct;
            }
            else
            {
                return color;
            }

            if (strs.Length > 2 && byte.TryParse(strs[2], out byte white))
            {
                color.White = white;
            }
            else
            {
                return color;
            }

            if (strs.Length > 3 && byte.TryParse(strs[3], out byte amber))
            {
                color.Amber = amber;
            }
            else
            {
                return color;
            }

            return color;
        }

        public static CompositeColor FromUInt32(uint c)
        {
            //var alpha = (byte)((c & 0xff000000) >> 24); // WTF???
            var red = (byte)((c & 0xff0000) >> 16);
            var green = (byte)((c & 0xff00) >> 8);
            var blue = (byte)(c & 0xff);

            return new CompositeColor(red, green, blue);
        }

        public static bool operator ==(CompositeColor x, CompositeColor y)
        {
            return x.Red == y.Red &&
                   x.Green == y.Green &&
                   x.Blue == y.Blue &&
                   (x.CCT - y.CCT).IsZero() &&
                   x.White == y.White &&
                   x.Amber == y.Amber;
        }

        public static bool operator !=(CompositeColor x, CompositeColor y)
        {
            return !(x == y);
        }

        public static CompositeColor operator *(CompositeColor x, float multiplier)
        {
            return new CompositeColor((byte)(x.Red * multiplier), (byte)(x.Green * multiplier), (byte)(x.Blue * multiplier), (byte)(x.Amber * multiplier), (byte)(x.CCT * multiplier), (byte)(x.White * multiplier));
        }

        public static CompositeColor Lerp(CompositeColor c1, CompositeColor c2, float rel)
        {
            var red = c1.Red - rel * (c1.Red - c2.Red);
            var green = c1.Green - rel * (c1.Green - c2.Green);
            var blue = c1.Blue - rel * (c1.Blue - c2.Blue);
            var amber = c1.Amber - rel * (c1.Amber - c2.Amber);
            var white = c1.White - rel * (c1.White - c2.White);
            var cct = c1.CCT - rel * (c1.CCT - c2.CCT);

            return new CompositeColor((byte)red, (byte)green, (byte)blue, (byte)amber, cct, (byte)white);
        }

        public static void Lerp(ref CompositeColor target, CompositeColor c1, CompositeColor c2, float rel)
        {
            target.Red = (byte)(c1.Red - rel * (c1.Red - c2.Red));
            target.Green = (byte)(c1.Green - rel * (c1.Green - c2.Green));
            target.Blue = (byte)(c1.Blue - rel * (c1.Blue - c2.Blue));
            target.Amber = (byte)(c1.Amber - rel * (c1.Amber - c2.Amber));
            target.White = (byte)(c1.White - rel * (c1.White - c2.White));
            target.CCT = (byte)(c1.CCT - rel * (c1.CCT - c2.CCT));
        }

        public static CompositeColor GetAttenuate(CompositeColor color, int mult)
        {
            byte red = (byte)(color.Red * mult >> 8);
            byte green = (byte)(color.Green * mult >> 8);
            byte blue = (byte)(color.Blue * mult >> 8);
            byte amber = (byte)(color.Amber * mult >> 8);
            byte white = (byte)(color.White * mult >> 8);

            return new CompositeColor(red, green, blue, amber, color.CCT, white);
        }

        private static RGB Lerp(RGB c1, RGB c2, float rel)
        {
            var red = c1.R - rel * (c1.R - c2.R);
            var green = c1.G - rel * (c1.G - c2.G);
            var blue = c1.B - rel * (c1.B - c2.B);

            return new RGB((byte)red, (byte)green, (byte)blue);
        }

        private RGB Amber2Rgba()
        {
            var constAmberColor = new RGB(255, 192, 0);

            var k = (float)Amber / 255;

            byte r = (byte)(constAmberColor.R * k);
            byte g = (byte)(constAmberColor.G * k);
            byte b = (byte)(constAmberColor.B * k);

            return new RGB(r, g, b);
        }

        private RGB WhiteToRgba()
        {
            var k = (float)White / 255;

            var r = (byte)(255 * k);
            var g = (byte)(255 * k);
            var b = (byte)(255 * k);

            return new RGB(r, g, b);
        }

        private RGB CctToRgba()
        {
            var cct = CCT.Clamped(CCT3K, CCT5K);
            cct -= CCT3K;

            var constWhite30Color = new RGB(255, 224, 192);
            var constWhite50Color = new RGB(224, 224, 255);

            float diff = CCT5K - CCT3K;
            var color = Lerp(constWhite30Color, constWhite50Color, cct / diff);

            var k = (float)White / 255;

            var r = (byte)(color.R * k);
            var g = (byte)(color.G * k);
            var b = (byte)(color.B * k);

            return new RGB(r, g, b);
        }

        private static void RgbToHsv(out float hue, out float saturation, out float brightness, float r, float g, float b)
        {
            float rgbMax = Math.Max(Math.Max(r, g), b);
            brightness = rgbMax;
            if (brightness < LCMath.Eps)
            {
                hue = saturation = 0.0f;
                return;
            }

            /* Normalize value to 1 */
            r /= brightness;
            g /= brightness;
            b /= brightness;
            float rgbMin = Math.Min(Math.Min(r, g), b);
            rgbMax = Math.Max(Math.Max(r, g), b);

            saturation = rgbMax - rgbMin;
            if (saturation < LCMath.Eps)
            {
                hue = 0;
                brightness = brightness / 255 * 100;
                return;
            }

            /* Normalize saturation to 1 */
            r = (r - rgbMin) / (rgbMax - rgbMin);
            g = (g - rgbMin) / (rgbMax - rgbMin);
            b = (b - rgbMin) / (rgbMax - rgbMin);
            if (r > g && r > b)
            {
                hue = 0.0f + 60.0f * (g - b);
                if (hue < 0.0f)
                {
                    hue += 360.0f;
                }
            }
            else if (g > b)
            {
                hue = 120.0f + 60.0f * (b - r);
            }
            else
            {
                hue = 240.0f + 60.0f * (r - g);
            }

            saturation *= 100;
            brightness = brightness / 255 * 100;
        }

        public readonly CompositeColor GetLampColor(List<LampColor> colorSets)
        {
            CompositeColor color = new CompositeColor();

            foreach (var cs in colorSets)
            {
                switch (cs.ColorCode)
                {
                    case ColorCodes.Red:
                        color.Red = Red;
                        color.CCT = CCT4K;
                        break;
                    case ColorCodes.Green:
                        color.Green = Green;
                        color.CCT = CCT4K;
                        break;
                    case ColorCodes.Blue:
                        color.Blue = Blue;
                        color.CCT = CCT4K;
                        break;
                    case ColorCodes.Cian:
                        // todo Nadya: внести в цвет
                        color.Green = Green;
                        color.Blue = Blue;
                        color.CCT = CCT4K;
                        break;
                    case ColorCodes.Amber:
                        color.Amber = Amber;
                        color.CCT = CCT4K;
                        break;
                    case ColorCodes.White3000:
                        color.White = White;
                        color.CCT = CCT3K;
                        break;
                    case ColorCodes.White4000:
                        color.White = White;
                        color.CCT = CCT4K;
                        break;
                    case ColorCodes.White5000:
                        color.White = White;
                        color.CCT = CCT5K;
                        break;
                }
            }

            return color;
        }

        public bool Contains(List<LampColor> colorSets)
        {
            var colorCodes = colorSets.ConvertAll(x => x.ColorCode);

            if (colorCodes.Count== 1) // Если только один компонент цвета
            {
                ColorCodes colorCode = colorCodes[0];

                switch (colorCode)
                {
                    case ColorCodes.Red:
                        return Green == 0 && Blue == 0 && Amber == 0 && White == 0;
                    case ColorCodes.Green:
                        return Red == 0 && Blue == 0 && Amber == 0 && White == 0;
                    case ColorCodes.Blue:
                        return Red == 0 && Green == 0 && Amber == 0 && White == 0;
                    case ColorCodes.Amber:
                        return Red == 0 && Green == 0 && Blue == 0 && White == 0;
                    case ColorCodes.Cian:
                        return Red == 0 && Amber == 0;
                    case ColorCodes.White3000:
                        return Red == 0 && Green == 0 && Blue == 0 && Amber == 0;
                    case ColorCodes.White4000:
                        return Red == 0 && Green == 0 && Blue == 0 && Amber == 0;
                    case ColorCodes.White5000:
                        return Red == 0 && Green == 0 && Blue == 0 && Amber == 0;
                }
                return true;
            }

            if (Red != 0 && !colorCodes.Contains(ColorCodes.Red))
                return false;
            if (Green != 0 && !colorCodes.Contains(ColorCodes.Green))
                return false;
            if (Blue != 0 && !colorCodes.Contains(ColorCodes.Blue))
                return false;
            if (Amber != 0 && !colorCodes.Contains(ColorCodes.Amber))
                return false;

            if (colorCodes.Contains(ColorCodes.White3000) && CCT.NearEqual(3000f))
                return true;
            if (colorCodes.Contains(ColorCodes.White4000) && CCT.NearEqual(4000f))
                return true;
            if (colorCodes.Contains(ColorCodes.White5000) && CCT.NearEqual(5000f))
                return true;

            return !colorCodes.Contains(ColorCodes.White3000) && !colorCodes.Contains(ColorCodes.White4000) && !colorCodes.Contains(ColorCodes.White5000);
            //return false;
            //return colorCodes.Contains(ColorCodes.White3000) || colorCodes.Contains(ColorCodes.White4000) || colorCodes.Contains(ColorCodes.White5000) || White <= 0;
        }

        /// <summary>
        /// Конвертирование представление цвета из hsv(hsb) в rgb
        /// http://www.splinter.com.au/converting-hsv-to-rgb-colour-using-c/
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="val"></param>
        public static void HsvToRgb(out float r, out float g, out float b, float hue, float sat, float val)
        {
            sat /= 100;
            val /= 100;
            double h = hue;
            while (h < 0)
            {
                h += 360;
            }

            while (h >= 360)
            {
                h -= 360;
            }
            double red, green, blue;
            if (val <= 0)
            {
                red = green = blue = 0;
            }
            else if (sat <= 0)
            {
                red = green = blue = val;
            }
            else
            {
                double hf = h / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = val * (1 - sat);
                double qv = val * (1 - sat * f);
                double tv = val * (1 - sat * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        red = val;
                        green = tv;
                        blue = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        red = qv;
                        green = val;
                        blue = pv;
                        break;
                    case 2:
                        red = pv;
                        green = val;
                        blue = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        red = pv;
                        green = qv;
                        blue = val;
                        break;
                    case 4:
                        red = tv;
                        green = pv;
                        blue = val;
                        break;

                    // Red is the dominant color

                    case 5:
                        red = val;
                        green = pv;
                        blue = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        red = val;
                        green = tv;
                        blue = pv;
                        break;
                    case -1:
                        red = val;
                        green = pv;
                        blue = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        red = green = blue = val; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(red * 255.0));
            g = Clamp((int)(green * 255.0));
            b = Clamp((int)(blue * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        private static int Clamp(int i)
        {
            if (i < 0) return 0;
            return i > 255 ? 255 : i;
        }
    }