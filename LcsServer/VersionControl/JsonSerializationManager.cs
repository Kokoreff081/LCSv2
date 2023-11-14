using System.Text;
using System.IO;
using LCSVersionControl.Converters;
using Newtonsoft.Json;


namespace LCSVersionControl;

internal class JsonSerializationManager
    {
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly JsonSerializer _jsonSerializer;

        public JsonSerializationManager()
        {
            _jsonSettings = new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.None };

            _jsonSettings.Converters.Add(new BaseVcJsonConverter());
            _jsonSettings.Converters.Add(new Vector2Converter());
            _jsonSettings.Converters.Add(new Vector3Converter());
            _jsonSettings.Converters.Add(new Vector4Converter());

            _jsonSerializer = JsonSerializer.Create(_jsonSettings);
        }

        /// <summary>
        /// Сериализация объекта в массив байтов
        /// </summary>
        /// <param name="obj">Объект для сериализации</param>
        /// <returns></returns>
        public byte[] Serialize(object obj)
        { 
            string jsonData = JsonConvert.SerializeObject(obj, _jsonSettings);
            return Encode(jsonData);
        }

        /// <summary>
        /// Десериализация объекта
        /// </summary>
        /// <typeparam name="T">Тип объекта после десериализации</typeparam>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public T Deserialize<T>(string jsonData)
        {
            var result = JsonConvert.DeserializeObject<T>(jsonData, _jsonSettings);
            return result;
        }
        
        public T Deserialize<T>(Stream jsonStrean)
        {
            using StreamReader sr = new StreamReader(jsonStrean);
            using JsonReader reader = new JsonTextReader(sr);
            T result = _jsonSerializer.Deserialize<T>(reader);
            return result;
        }

        /// <summary>
        /// Перевод строки в массив байтов с использованием кодировки UTF8
        /// </summary>
        private static byte[] Encode(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// Получение строки из массива байтов с использованием кодировки UTF8
        /// </summary>
        private static string Decode(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

    }