using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace LCSVersionControl;

public static class LCMath
    {
        /// <summary>
        /// Машинное эпсилон.
        /// </summary>
        public const float Eps = 1E-6f;

        public const float PImul2 = MathF.PI * 2;

        public const float PIdiv2 = MathF.PI  / 2;
        public const float PIdiv4 = MathF.PI  / 4;
        public const float PIdiv180 = MathF.PI  / 180;

        public const float _180divPI = 180 / MathF.PI ;
        
        public static CultureInfo StringCulture => CultureInfo.InvariantCulture;
        
        
        /// <summary>
        /// Приведение к диапазону (ограничение по min max)
        /// T - byte int float long ...
        /// иcпользоваение var int2 = int1.Clamped(min,max)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static float Clamped(this float value, float min, float max)
        {
            //If min <= max, clamp
            if (min.CompareTo(max) <= 0) return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
            //If min > max, clamp on swapped min and max
            return value.CompareTo(max) < 0 ? max : value.CompareTo(min) > 0 ? min : value;
        }
        
        public static double Clamped(this double  value, double  min, double max)
        {
            //If min <= max, clamp
            if (min.CompareTo(max) <= 0) return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
            //If min > max, clamp on swapped min and max
            return value.CompareTo(max) < 0 ? max : value.CompareTo(min) > 0 ? min : value;
        }
        
        public static long Clamped(this long value, long min, long max)
        {
            if (min.CompareTo(max) <= 0) return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
            return value.CompareTo(max) < 0 ? max : value.CompareTo(min) > 0 ? min : value;
        }
        
        public static byte Clamped(this byte value, byte min, byte max)
        {
            if (min.CompareTo(max) <= 0) return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
            return value.CompareTo(max) < 0 ? max : value.CompareTo(min) > 0 ? min : value;
        }
        
        public static int Clamped(this int value, int min, int max)
        {
            if (min.CompareTo(max) <= 0) return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
            return value.CompareTo(max) < 0 ? max : value.CompareTo(min) > 0 ? min : value;
        }

        /// <summary>
        /// Сравнение с машинным нулём
        /// </summary>
        public static bool IsZero(this float value)
        {
            return Math.Abs(value) < Eps;
        }
        
        /// <summary>
        /// Сравнение с машинным нулём
        /// </summary>
        public static bool IsZero(this double value)
        {
            return Math.Abs(value) < Eps;
        }
        
        /// <summary>
        /// Сравнение с машинным нулём
        /// </summary>
        public static bool NearEqual(this float value1, float value2)
        {
            return Math.Abs(value1 - value2) < Eps;
        }

        public static bool NearEqual(this Vector3 vector1, Vector3 vector2)
        {
            return Math.Abs(vector1.X - vector2.X) < 0.0001 && 
                   Math.Abs(vector1.Y - vector2.Y) < 0.0001 &&
                   Math.Abs(vector1.Z - vector2.Z) < 0.0001;
        }
        
        /// <summary>
        /// Проверяет вектор Vector3 на валидность
        /// </summary>
        /// <param name="vector">Производный вектор</param>
        /// <returns>false - если хотя бы один компонент вектора NaN, 
        ///          true - в обратном случае
        /// </returns>
        public static bool IsValid(this Vector3 vector)
        {
            return !(float.IsNaN(vector.X) || float.IsNaN(vector.Y) || float.IsNaN(vector.Z));
        }

        /// <summary>
        /// Проверяет вектор Vector4 на валидность
        /// </summary>
        /// <param name="value">Производный вектор</param>
        /// <returns>false - если хотя бы один компонент вектора NaN, 
        ///          true - в обратном случае
        /// </returns>
        public static bool IsValid(this Vector4 value)
        {
            return new Vector3(value.X, value.Y, value.Z).IsValid() && !float.IsNaN(value.W);
        }

        /// <summary>
        /// Нормализация вектора
        /// </summary>
        /// <param name="input">Исходный вектор</param>
        /// <returns>Нормализованный вектор</returns>
        // public static Vector3 Normalize(this Vector3 input)
        // {
        //     float length = input.Length();
        //     if (IsZero(length)) return input;
        //     return input / length;
        // }

        /// <summary>
        /// Расчет среднего вектора в массиве
        /// </summary>
        /// <param name="vectors">Коллекция векторов</param>
        /// <returns>Среднее значение из коллекции</returns>
        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            float x = 0f;
            float y = 0f;
            float z = 0f;
            int count = 0;
            foreach (Vector3 pos in vectors)
            {
                x += pos.X;
                y += pos.Y;
                z += pos.Z;
                count++;
            }
            return count == 0 ? Vector3.Zero : new Vector3(x/count,y/count,z/count);
            //return vectors.Aggregate(Vector3.Zero, (current, vector) => current + vector)/ vectors.Count;
        }

        /// <summary>
        /// Поучает ориентацию в виде кватерниона по вектору направления.
        /// Использует расчет углов Эйлера по направлению первым способом.
        /// Добавляет поправку к углу тонгажа.
        /// </summary>
        /// <param name="direction">Входной вектор направления.</param>
        /// <param name="pitchCorrection">Поправка к углу тонгажа.</param>
        /// <returns>Кватернион ориентации</returns>
        public static Quaternion GetOrientation(this Vector3 direction, float pitchCorrection)
        {
            if (direction == Vector3.Zero)
            {
                return Quaternion.Identity;
            }
            Vector3 eulerAngles = direction.ToEulerAngles();
            float yaw = eulerAngles.X;
            float pitch = eulerAngles.Y + pitchCorrection;
            float roll = eulerAngles.Z;
            Quaternion result = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            return result;
        }

        /// <summary>
        /// Расчет углов Эйлера для произвольного вектора первым способом: 
        /// https://stackoverflow.com/questions/2782647/how-to-get-yaw-pitch-and-roll-from-a-3d-vector
        /// pitch = asin(-direction.Y);
        /// yaw = atan2(direction.X, direction.Z);
        /// roll = 0. Невозможно расчитать непосредственно из вектора
        /// </summary>
        /// <param name="direction">Произвольный вектор направления</param>
        /// <returns>Углы Эйлера в виде Vector3(yaw, pitch, roll)</returns>
        public static Vector3 ToEulerAngles(this Vector3 direction)
        {
            Vector3 d = Vector3.Normalize(direction);
            float yaw = (float)Math.Atan2(d.X, d.Z);
            float pitch = (float)Math.Asin(-d.Y);
            float roll = 0.0f;
            return new Vector3(yaw, pitch, roll);
        }

        
        /// <summary>
        /// Конвертирует градусы в радианы
        /// </summary>
        /// <param name="degrees">Значение в градусах</param>
        /// <returns>Результат в радианах</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(this float degrees)
        {
            return PIdiv180 * degrees;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(this int degrees)
        {
            return PIdiv180 * degrees;
        }


        /// <summary>
        /// Конвертирует радианы в градусы
        /// </summary>
        /// <param name="radians">Значение в радианах</param>
        /// <returns>Результат в градусах</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(this float radians)
        {
            return radians * _180divPI;
        }

        /// <summary>
        /// Расчитывает угол между векторами
        /// </summary>
        /// <param name="a">Вектор a</param>
        /// <param name="b">Вектор b</param>
        /// <returns>Значение угла</returns>
        public static float GetAngle(Vector2 a, Vector2 b)
        {
            return MathF.Atan2(Cross(a, b), Dot(a, b));
        }

        /// <summary>
        /// Скалярное произведение векторов
        /// </summary>
        private static float Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// Векторное произведение векторов
        /// </summary>
        private static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        /// <summary>
        /// Возвращает нормаль к плоскости заданной тремя точками
        /// </summary>
        /// <param name="v1">Точка 1</param>
        /// <param name="v2">Точка 2</param>
        /// <param name="v3">Точка 3</param>
        /// <returns>Вектор нормали</returns>
        public static Vector3 GetNormal(this Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 vector1 = v1 - v2;
            Vector3 vector2 = v2 - v3;
            Vector3 normal = Vector3.Normalize(Vector3.Cross(vector1, vector2));
            return normal;
        }

        public static Quaternion GetRotationBetween(Vector3 u, Vector3 v)
        {
            float kCosTheta = Vector3.Dot(u, v);
            float ul = u.Length();
            float vl = v.Length();
            float k = (float)Math.Sqrt(ul * ul + vl * vl);

            if ((kCosTheta / k).Equals(-1.0f))
            {
                Vector3 uNorm = Vector3.Normalize(u);
                // 180 degree rotation around any orthogonal vector
                return new Quaternion(uNorm.X, uNorm.Y, uNorm.Z, 0);
            }

            var crossUV = Vector3.Cross(u, v);
            return Quaternion.Inverse(Quaternion.Normalize(new Quaternion(crossUV.X, crossUV.Y, crossUV.Z, kCosTheta + k)));
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FloatToString(float value)
        {
            //decimal result = (decimal)value;
            //return result.ToString("0.###", StringCulture);
            return value.ToString("G6", StringCulture);
        }
        
        public static string Vector4ToString(Vector4 vector)
        {
            return $"{FloatToString(vector.X)};{FloatToString(vector.Y)};{FloatToString(vector.Z)};{FloatToString(vector.W)}";
        }

        public static string Vector3ToString(Vector3 vector)
        {
            return $"{FloatToString(vector.X)};{FloatToString(vector.Y)};{FloatToString(vector.Z)}";
        }

        public static string Vector2ToString(Vector2 vector)
        {
            return $"{FloatToString(vector.X)};{FloatToString(vector.Y)}";
        }
        
        // public static string ToStr(this float value)
        // {
        //     return value.ToString("G9", StringCulture); // Максимальная точность float это 9 цифр
        // }
        
        public static string ToStr(this Quaternion vector, string delim = " ")
        {
            return $"{FloatToString(vector.X)}{delim}{FloatToString(vector.Y)}{delim}{FloatToString(vector.Z)}{delim}{FloatToString(vector.W)}";
        }
        
        // public static string ToStr(this Vector4 vector, string delim = " ", bool toVec3 = false)
        // {
        //     if (toVec3)
        //     {
        //         return $"{vector.X.ToStr()}{delim}{vector.Y.ToStr()}{delim}{vector.Z.ToStr()}";
        //     }
        //     return $"{vector.X.ToStr()}{delim}{vector.Y.ToStr()}{delim}{vector.Z.ToStr()}{delim}{vector.W.ToStr()}";
        // }
        
        public static string ToStr(this Vector3 vector, string delim = " ")
        {
            return $"{FloatToString(vector.X)}{delim}{FloatToString(vector.Y)}{delim}{FloatToString(vector.Z)}";
        }

        public static string ToStr(Vector2 vector, string delim = " ")
        {
            return $"{FloatToString(vector.X)}{delim}{FloatToString(vector.Y)}";
        }
        
        
        /// <summary>
        /// Возвращает исходное значение, если значения равны или дефолтное значение в ином случае
        /// </summary>
        /// <param name="value1">Исходное значение</param>
        /// <param name="value2">Значение с которым сравниваем</param>
        /// <param name="decimals">Число цифр для округления</param>
        public static float SameValueOrNan(this float value1, float value2, int decimals = 4)
        {
            if (float.IsNaN(value1) || float.IsNaN(value2))
                return float.NaN;

            if (Math.Round(value1, decimals).CompareTo(Math.Round(value2, decimals)) == 0)
                return value1;

            return float.NaN;
        }

        /// <summary>
        /// Конвертирует вектор в квартенион
        /// </summary>
        /// <param name="vector">Вектор поворота</param>
        /// <returns>Поворот в виде кватерниона</returns>
        public static Quaternion ToQuaternion(this Vector3 vector) => ToQuaternion(vector.Y, vector.X, vector.Z);

        /// <summary>
        /// Создает кватернион из углов поворота вокруг осей координат
        /// </summary>
        /// <param name="yaw">Угол поворота по Z</param>
        /// <param name="pitch">Угол поворота по Y</param>
        /// <param name="roll">Угол поворота по X</param>
        /// <returns>Кватернион</returns>
        private static Quaternion ToQuaternion(float yaw, float pitch, float roll)
        {
            double rollOver2 = roll * 0.5f;
            double sinRollOver2 = Math.Sin(rollOver2);
            double cosRollOver2 = Math.Cos(rollOver2);
            double pitchOver2 = pitch * 0.5f;
            double sinPitchOver2 = Math.Sin(pitchOver2);
            double cosPitchOver2 = Math.Cos(pitchOver2);
            double yawOver2 = yaw * 0.5f;
            double sinYawOver2 = Math.Sin(yawOver2);
            double cosYawOver2 = Math.Cos(yawOver2);
            Quaternion result;
            result.W = (float)(cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2);
            result.X = (float)(cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2);
            result.Y = (float)(sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2);
            result.Z = (float)(cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2);

            return result;
        }

        /// <summary>
        /// Конвертирует Vector3 в Vector4. Четвертой координате присваивается значение 1
        /// converts Vector3 to Vector4 by adding a homogenous coordinate w = 1.0f;
        /// </summary>
        // public static Vector4 ToVector4(this Vector3 vector)
        // {
        //     return new Vector4(vector.X, vector.Y, vector.Z, 1.0f);
        // }

        /// <summary>
        /// Создает Matrix4x4 из кватерниона
        /// </summary> 
        public static Matrix4x4 ToMatrix4X4(this Quaternion q)
        {
            return Matrix4x4.CreateFromQuaternion(q);
        }

        /// <summary>
        /// Десериализует Vector3 из строки
        /// </summary>
        /// <param name="vector">Строковое представление вектора в формате <X;Y;Z> </param>
        /// <returns>Объект Vector3</returns>
        public static Vector3 ToVector3(this string vector)
        {
            if (string.IsNullOrEmpty(vector))
            {
                return Vector3.Zero;
            }

            // if (vector.StartsWith("<") && vector.EndsWith(">"))
            // {
            //     vector = vector.Substring(1, vector.Length - 2);
            // }

            Vector3 result = new Vector3();
            int i = 0;
            foreach (ReadOnlySpan<char> item in vector.SplitLines(';'))
            {
                switch (i)
                {
                    case 0: result.X = ParseFloat(item);
                        break;
                    case 1: result.Y = ParseFloat(item);
                        break;
                    case 2: result.Z = ParseFloat(item);
                        break;
                }
                if (++i == 3)
                {
                    break;
                }
            }

            return result;
            //
            // string[] sArray = vector.Split(';');
            //
            // float x = ParseFloat(sArray[0]);
            // float y = ParseFloat(sArray[1]);
            // float z = ParseFloat(sArray[2]);
            //
            // return new Vector3(x, y, z);
        }

        /// <summary>
        /// Десериализует Vector2 из строки
        /// </summary>
        /// <param name="vector">Строковое представление вектора в формате <X;Y> </param>
        /// <returns>Объект Vector2</returns>
        public static Vector2 ToVector2(this string vector)
        {
            Vector2 result = new Vector2();
            int i = 0;
            foreach (ReadOnlySpan<char> item in vector.SplitLines(';'))
            {
                switch (i)
                {
                    case 0: result.X = ParseFloat(item);
                        break;
                    case 1: result.Y = ParseFloat(item);
                        break;
                }
                if (++i == 2)
                {
                    break;
                }
            }

            return result;
            
            // string[] parts = vector.Split(';');
            // float x = ParseFloat(parts[0]);
            // float y = ParseFloat(parts[1]);
            //
            // return new Vector2(x, y);
        }

        /// <summary>
        /// Десериализует Vector4 из строки
        /// </summary>
        /// <param name="vector">Строковое представление вектора в формате <X;Y;Z;W> </param>
        /// <returns>Объект Vector4</returns>
        public static Vector4 ToVector4(this string vector)
        {
            Vector4 result = new Vector4();
            int i = 0;
            foreach (ReadOnlySpan<char> item in vector.SplitLines(';'))
            {
                switch (i)
                {
                    case 0: result.X = ParseFloat(item);
                        break;
                    case 1: result.Y = ParseFloat(item);
                        break;
                    case 2: result.Z = ParseFloat(item);
                        break;
                    case 3: result.W = ParseFloat(item);
                        break;
                }
                if (++i == 4)
                {
                    break;
                }
            }

            return result;
            // string[] parts = vector.Split(';');
            // float x = ParseFloat(parts[0]);
            // float y = ParseFloat(parts[1]);
            // float z = ParseFloat(parts[2]);
            // float w = ParseFloat(parts[3]);
            // return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Конвертирует кватернион в углы Эйлера в виде Vector3 
        /// </summary>
        /// <param name="q1">Кватернион</param>
        /// <returns>Углы Эйлера в виде Vector3</returns>
        public static Vector3 ToEulerAngles(this Quaternion q1)
        {
            float sqw = q1.W * q1.W;
            float sqx = q1.X * q1.X;
            float sqy = q1.Y * q1.Y;
            float sqz = q1.Z * q1.Z;
            float unit = sqx + sqy + sqz + sqw;
            float test = q1.X * q1.W - q1.Y * q1.Z;
            Vector3 v;

            if (test > (0.5f-Eps) * unit)
            { // singularity at north pole
                v.Y = (float)(2f * Math.Atan2(q1.Y, q1.X));
                v.X = PIdiv2;
                v.Z = 0;
                return v;
            }
            if (test < (Eps-0.5f) * unit)
            { // singularity at south pole
                v.Y = (float)(-2f * Math.Atan2(q1.Y, q1.X));
                v.X = -PIdiv2;
                v.Z = 0;
                return v;
            }
            var q = new Quaternion(q1.W, q1.Z, q1.X, q1.Y);
            v.Y = (float)Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W));     // Yaw
            v.X = (float)Math.Asin(2f * (q.X * q.Z - q.W * q.Y));                                           // Pitch
            v.Z = (float)Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z));     // Roll
            return v;
        }

        /// <summary>
        /// Возвращает компоненты вектора в виде массива
        /// </summary>
        public static float[] ToFloatArray(this Vector3 vec)
        {
            return new[] { vec.X, vec.Y, vec.Z, };
        }

        /// <summary>
        /// Возвращает компоненты вектора в виде массива
        /// </summary>
        public static float[] ToFloatArray(this Vector4 vec)
        {
            return new[] { vec.X, vec.Y, vec.Z, vec.W, };
        }

        /// <summary>
        /// Возвращает компоненты вектора в виде массива
        /// </summary>
        public static float[] ToFloatArray(this Vector2 vec)
        {
            return new[] { vec.X, vec.Y, };
        }

        public static int GetAngleSquareIndex(float angle)
        {
            while (angle > PImul2)
            {
                angle -= PImul2;
            }
            if (angle >= 0 && angle < PIdiv2)
                return 1;
            if (angle >= PIdiv2 && angle < MathF.PI )
                return 2;
            if (angle >= MathF.PI && angle < PIdiv2 * 3)
                return 3;
            if (angle >= PIdiv2 * 3 && angle <= PImul2)
                return 4;

            return 0;
        }

        /// <summary>
        /// Вернуть точку на эллипсе в зависимости от градуса
        /// </summary>
        /// <param name="bigSemiAxis">Большая полуось эллипса</param>
        /// <param name="smallSemiAxis">Меньшая полуось эллипса</param>
        /// <param name="angleInRadians">Угол в радианах</param>
        /// <returns></returns>
        public static Vector2 GetEllipsePoint(double bigSemiAxis, double smallSemiAxis, double angleInRadians)
        {
            float x = (float)(bigSemiAxis * Math.Cos(angleInRadians));
            float y = (float)(smallSemiAxis * Math.Sin(angleInRadians));

            return new Vector2(x, y);
        }

        /// <summary>
        /// Длина дуги эллипса
        /// </summary>
        /// <param name="bigSemiAxis">Большая полуось эллипса</param>
        /// <param name="smallSemiAxis">Меньшая полуось эллипса</param>
        /// <param name="startAngle">Угол(в радианах) с которого надо рассчитать длину дуги</param>
        /// <param name="sweepAngle">Угол поворота(в радианах)</param>
        /// <returns></returns>
        public static double GetEllipseArcLength(double bigSemiAxis, double smallSemiAxis, double startAngle, double sweepAngle)
        {
            double result = CalculateIntegral(bigSemiAxis, smallSemiAxis, startAngle, startAngle + sweepAngle, 100);

            return result;
        }

        /// <summary>
        /// Высчитываем длину дуги эллипса, интеграл считается по методу Симпсона
        /// </summary>
        /// <param name="bigSemiAxis">Большая полуось эллипса</param>
        /// <param name="smallSemiAxis">Меньшая полуось эллипса</param>
        /// <param name="start">Начало интегирования</param>
        /// <param name="stop">Конец интегрирования</param>
        /// <param name="n">Кол-во отрезков</param>
        /// <returns></returns>
        private static double CalculateIntegral(double bigSemiAxis, double smallSemiAxis, double start, double stop, int n)
        {
            double h = (stop - start) / n;
            double s = 0;
            double x = start + h;
            while (x < stop)
            {
                s = s + 4 * EllipseArcLengthIntegralFunction(bigSemiAxis, smallSemiAxis, x);
                x = x + h;
                s = s + 2 * EllipseArcLengthIntegralFunction(bigSemiAxis, smallSemiAxis, x);
                x = x + h;
            }
            s = h / 3 * (s + EllipseArcLengthIntegralFunction(bigSemiAxis, smallSemiAxis, start) - EllipseArcLengthIntegralFunction(bigSemiAxis, smallSemiAxis, start));

            return s;
        }

        /// <summary>
        /// Функция внутри интеграла (формула вычисления длины дуги эллипса)
        /// </summary>
        /// <param name="bigSemiAxis"></param>
        /// <param name="smallSemiAxis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static double EllipseArcLengthIntegralFunction(double bigSemiAxis, double smallSemiAxis, double angle)
        {
            double sinA = Math.Sin(angle);
            double cosA = Math.Cos(angle);
            return Math.Sqrt(bigSemiAxis * bigSemiAxis * sinA * sinA + smallSemiAxis * smallSemiAxis * cosA * cosA);
        }

        private static Vector3 Orthogonal(Vector3 v)
        {
            float x = Math.Abs(v.X);
            float y = Math.Abs(v.Y);
            float z = Math.Abs(v.Z);

            Vector3 other = x < y ? x < z ? Vector3.UnitX : Vector3.UnitZ : y < z ? Vector3.UnitY : Vector3.UnitZ;
            return Vector3.Cross(v, other);
        }

        //public delegate float RadiusFunction(float value, float factor);

        /// <summary>
        /// Инвертирование матрицы
        /// </summary>
        public static Matrix4x4 Inverse(this Matrix4x4 m)
        {
            if (!Matrix4x4.Invert(m, out var result))
            {
                //LogManager.GetCurrentClassLogger().Error($"Matrix inverse error! input={m.ToString()}");
                //throw new ArgumentException($"Matrix inverse error! result={result.ToString()}");
            }
            return result;
        }

        /// <summary>
        /// Парсит float c использованием объекта культуры по умолчанию
        /// </summary>
        /// <param name="inputString">Входная строка с . либо , как разделитель</param>
        /// <returns>Float value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ParseFloat(string inputString)
        {
            //string _float = inputString.Contains(',') ? inputString.Replace(',', '.') : inputString;
            if (float.TryParse(inputString, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }

            //LogManager.GetCurrentClassLogger().Error($"Parse float error! input={inputString}");
            return 0f;
            //throw new ArgumentException($"Parse float error! result={result.ToString(CultureInfo.InvariantCulture)}");
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ParseFloat(ReadOnlySpan<char> inputString)
        {
            //string _float = inputString.Contains(',') ? inputString.Replace(',', '.') : inputString;
            if (float.TryParse(inputString, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }

            // Старый костыль был в ToVector3
            inputString = inputString.TrimStart("<").TrimEnd(">");
            if (float.TryParse(inputString, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            //LogManager.GetCurrentClassLogger().Error($"Parse float error! input={inputString}");
            return 0f;
            //throw new ArgumentException($"Parse float error! result={result.ToString(CultureInfo.InvariantCulture)}");
        }
    }
    public static class FastSplit
{
    public static LineSplitEnumerator SplitLines(this string str, char separator)
    {
        // LineSplitEnumerator is a struct so there is no allocation here
        return new LineSplitEnumerator(str.AsSpan(), separator);
    }

    // Must be a ref struct as it contains a ReadOnlySpan<char>
    public ref struct LineSplitEnumerator
    {
        private ReadOnlySpan<char> _str;
        private char _separator;

        public LineSplitEnumerator(ReadOnlySpan<char> str, char separator)
        {
            _str = str;
            _separator = separator;
            Current = default;
        }

        // Needed to be compatible with the foreach operator
        public LineSplitEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            var span = _str;
            if (span.Length == 0) // Reach the end of the string
                return false;

            var index = span.IndexOf(_separator);
            if (index == -1) // The string is composed of only one line
            {
                _str = ReadOnlySpan<char>.Empty; // The remaining string is an empty string
                Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
                return true;
            }

            // if (index < span.Length - 1 && span[index] == '\r')
            // {
            //     // Try to consume the '\n' associated to the '\r'
            //     var next = span[index + 1];
            //     if (next == '\n')
            //     {
            //         Current = new LineSplitEntry(span[..index], span.Slice(index, 2));
            //         _str = span[(index + 2)..];
            //         return true;
            //     }
            // }

            Current = new LineSplitEntry(span[..index], span.Slice(index, 1));
            _str = span[(index + 1)..];
            return true;
        }

        public LineSplitEntry Current { get; private set; }
    }

    public readonly ref struct LineSplitEntry
    {
        public LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
        {
            Line = line;
            Separator = separator;
        }

        public ReadOnlySpan<char> Line { get; }
        public ReadOnlySpan<char> Separator { get; }

        // This method allow to deconstruct the type, so you can write any of the following code
        // foreach (var entry in str.SplitLines()) { _ = entry.Line; }
        // foreach (var (line, endOfLine) in str.SplitLines()) { _ = line; }
        // https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct?WT.mc_id=DT-MVP-5003978#deconstructing-user-defined-types
        public void Deconstruct(out ReadOnlySpan<char> line, out ReadOnlySpan<char> separator)
        {
            line = Line;
            separator = Separator;
        }

        // This method allow to implicitly cast the type into a ReadOnlySpan<char>, so you can write the following code
        // foreach (ReadOnlySpan<char> entry in str.SplitLines())
        public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry) => entry.Line;
    }
}