using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using LightCAD.UI.Strings;

namespace LcsServer.Models.LCProjectModels.GlobalBase;

public static class Helper
    {
        public static readonly string ApplicationName;
        public const string LcLightMesh = "lc_lightmesh";
        public const string MaskLetter = "g";
        //public const long MillisecondsInSecond = 1000L;

        public const int DefaultWidth = 100;
        public const int DefaultHeight = 50;
        private const long _minPlayItemDuration = 100L;
        public const long TimeLineStepMs = 100L;

        public static CultureInfo Culture => CultureInfo.InvariantCulture;

        static Helper()
        {
            ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? "LightCAD";
        }

        /// <summary>
        /// Флаг, указывающий, что приложение запущено расширенном режиме.
        /// В этом режиме включаются некоторые специальные возможности.
        /// Можно использовать для включения отладочных механизмов.
        /// </summary>
        public static bool IsDevelopingMode { get; set; }
        public static bool IsDx11DebugEnabled { get; set; }

        /// <summary>
        /// Флаг, указывающий сохранять ли кеш и восстанавливать его при загрузке проекта
        /// TODO плавающий баг на json конвертере
        /// </summary>
        public static bool UseCache => false;

        public static long MinEffectDuration => Math.Max(_minPlayItemDuration, TimeLineStepMs);
        public static long DefaultEffectDuration => Math.Max(MinEffectDuration, 10000); // 10min
        public static long DefaultUnitDuration => DefaultEffectDuration;

        // public static int GetDecimalDigit()
        // {
        //     var digit = (int)Math.Ceiling(Math.Log10(1000)) - (int)Math.Ceiling(Math.Log10(TimeLineStepMs));
        //     return digit < 0 ? 0 : digit;
        // }
        //
        // public static long ValidatePlayItemDuration(long value)
        // {
        //     value -= value % TimeLineStepMs;
        //     return Math.Max(value, TimeLineStepMs);
        // }

        public static string DateTimeToString(DateTime value)
        {
            return value.ToString("s");
        }

        public static Dictionary<T, V> ToDictionary<T, V>(this IReadOnlyDictionary<T, V> readOnlyDictionary)
        {
            return readOnlyDictionary.ToDictionary(a => a.Key, a => a.Value);
        }

        public static List<T> GetEnumValues<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }


        /// <summary>
        /// Возвращает имя директории, в которой находится указанный файл
        /// </summary>
        /// <param name="fileName">Путь к файлу</param>
        /// <returns>Имя директории. Если указанного файла не существует, возвращается переданную строку</returns>
        public static string GetDirectoryName(string fileName)
        {
            return File.Exists(fileName) ? Path.GetDirectoryName(fileName) : fileName;
        }


        /// <summary>
        /// Extension method ForEach for IEnumerable
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        //public delegate float RadiusFunction(float value, float factor);
        public static Vector3[,] GetLightZoneData(float[] iesData, int iesWidth, int iesHeight, Quaternion orientation, float scale = 1.0f)
        {
            Vector3[,] data = new Vector3[iesWidth, iesHeight];
            for (int i = 0; i < iesWidth; ++i)
            {
                for (int j = 0; j < iesHeight; ++j)
                {
                    // Индекс в линеаризованном массиве данных текстуры:
                    int index = iesWidth * j + i;

                    // Радиус в зависимости от значения силы света в данном направлении:

                    float r = MathF.Sqrt(iesData[index] * scale);

                    // Азимутальный угол:
                    float phi = MathF.PI * 2.0f * i / (iesWidth - 1);

                    // Полярный угол:
                    float theta = MathF.PI * j / (iesHeight - 1);

                    float x = r * MathF.Sin(theta) * MathF.Sin(phi);
                    float y = -r * MathF.Cos(theta);
                    float z = r * MathF.Sin(theta) * MathF.Cos(phi);

                    Vector3 pos = new Vector3(x, y, z);
                    pos = Vector3.Transform(pos, orientation);
                    data[i, j] = pos;
                }
            }
            return data;
        }


        /// <summary>
        /// Пытается получить имя значения перечисления через атрибут Description
        /// </summary>
        /// <param name="value">Значение перечисления</param>
        /// <returns>Значение атрибута Description если он указан. В противном случае результат метода ToString() переданного значения</returns>
        public static string GetEnumFriendlyName(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length == 0) return value.ToString();

            string resourceName = attributes[0].Description;
            string result = Resources.ResourceManager.GetString(resourceName);

            //if (!string.IsNullOrEmpty(result)) return result;

            //result = strEngine.ResourceManager.GetString(resourceName);

            return string.IsNullOrEmpty(result) ? resourceName : result;
        }

        /// <summary>
        /// Сombine two vectors by the "action" and return new vector
        /// </summary>
        public static Vector3 CompareTo(this Vector3 src, Vector3 dst, Func<float, float, float> action)
        {
            return new Vector3(action(src.X, dst.X), action(src.Y, dst.Y), action(src.Z, dst.Z));
        }

        public static int AlphaNumComparator(this string str1, string str2)
        {
            var alphaNumComparator = new AlphanumComparatorFast();
            return alphaNumComparator.Compare(str1, str2);
        }

        public static IntPointer GetIntPtr(this object obj)
        {
            return new IntPointer(obj);
        }

        /// <summary>
        /// Конвертирует значение перечисления в строку
        /// </summary>
        /// <param name="eff">Значение перечисления</param>
        /// <returns>Значение перечисления строкой</returns>
        public static string ConvertToString(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

        /// <summary>
        /// Convert arbitrary object to Array of T
        /// </summary>
        /// <typeparam name="T">Incoming type of array</typeparam>
        /// <param name="obj">Object to convert</param>
        /// <returns></returns>
        // public static T[] GetUnits<T>(this object obj) where T : struct
        // {
        //     int size = Marshal.SizeOf(obj);
        //
        //     if (size % Marshal.SizeOf(typeof(T)) != 0) return null;
        //
        //     byte[] src = new byte[size];
        //
        //     IntPtr ptr = Marshal.AllocHGlobal(size);
        //     Marshal.StructureToPtr(obj, ptr, true);
        //     Marshal.Copy(ptr, src, 0, size);
        //     Marshal.FreeHGlobal(ptr);
        //
        //     T[] dest = new T[size / Marshal.SizeOf(typeof(T))];
        //     Buffer.BlockCopy(src, 0, dest, 0, size);
        //
        //     return dest;
        // }

        // public static int GetSequenceHashCode<T>(this IEnumerable<T> sequence)
        // {
        //     const int seed = 487;
        //     const int modifier = 31;
        //
        //     unchecked
        //     {
        //         return sequence.Aggregate(seed, (current, item) => (current * modifier) + item.GetHashCode());
        //     }
        // }

        public static int GetArrayHashCode(params byte[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            if (array.Length == 0) return 0;

            int result = array[0];

            for (int i = 1; i < array.Length; i++)
                result = CombineHashCodes(result, array[i]);

            return result;
        }

        public static int GetArrayHashCode(params int[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Length == 0)
            {
                return 0;
            }

            int result = array[0];

            for (int i = 1; i < array.Length; i++)
            {
                result = CombineHashCodes(result, array[i]);
            }

            return result;
        }

        public static int GetListHashCode(List<int> array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            if (array.Count == 0) return 0;

            int result = array[0];

            for (int i = 1; i < array.Count; i++)
                result = CombineHashCodes(result, array[i] + i);
            return result;
            // UGLY HACK
            //return string.Join(",", array).GetHashCode();
        }

        // public static long GetLongHashCode(int int1, int int2)
        // {
        //     return ((long)int1 << 32) + int2;
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }


        /// <summary>
        /// Копирование массива
        /// </summary>
        /// <typeparam name="T">Тип исходного массива</typeparam>
        /// <param name="source">Исходный массив</param>
        /// <returns>Копия исходного массива</returns>
        // public static T[] Copy<T>(this T[] source) where T : struct
        // {
        //     if (source == null) return null;
        //     if (source.Length <= 0) return null;
        //     T[] result = new T[source.Length];
        //     int size = source.Length * Marshal.SizeOf<T>();
        //     Buffer.BlockCopy(source, 0, result, 0, size);
        //     return result;
        // }

        public static int ComputeHash(byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (byte t in data)
                {
                    hash = (hash ^ t) * p;
                }
                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        public static Type[] GetDerivedTypes(this Type type)
        {
            return type.Assembly.ExportedTypes.Where(type.IsAssignableFrom).ToArray();
        }

        // ReSharper disable once InconsistentNaming
        // public static void GCCollectCompactLOH()
        // {
        //     GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        //     GC.Collect();
        //     GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.Default;
        // }

        /// <summary>
        /// Конвертирует матрицу в одномерный массив (построчно)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float[] ToArray(this Matrix4x4 source)
        {
            var result = new float[16];

            result[0] = source.M11;
            result[1] = source.M12;
            result[2] = source.M13;
            result[3] = source.M14;
            result[4] = source.M21;
            result[5] = source.M22;
            result[6] = source.M23;
            result[7] = source.M24;
            result[8] = source.M31;
            result[9] = source.M32;
            result[10] = source.M33;
            result[11] = source.M34;
            result[12] = source.M41;
            result[13] = source.M42;
            result[14] = source.M43;
            result[15] = source.M44;

            return result;
        }

        public static void AddMatrix(this float[] list, ref int idx, Matrix4x4 source)
        {
            list[idx++] = source.M11;
            list[idx++] = source.M12;
            list[idx++] = source.M13;
            list[idx++] = source.M14;
            list[idx++] = source.M21;
            list[idx++] = source.M22;
            list[idx++] = source.M23;
            list[idx++] = source.M24;
            list[idx++] = source.M31;
            list[idx++] = source.M32;
            list[idx++] = source.M33;
            list[idx++] = source.M34;
            list[idx++] = source.M41;
            list[idx++] = source.M42;
            list[idx++] = source.M43;
            list[idx++] = source.M44;
        }

        public static string ToHexString(this Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }