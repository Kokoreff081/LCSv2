using System.Net;
using System.Reflection;
using System.Text;
using Acn.Sockets;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Interfaces;
using LcsServer.DevicePollingService.Models;
using LcsServer.DevicePollingService.SerializationModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LightControlServiceV._2.DevicePollingService.Models;

internal class PrivateSetterJsonContractResolver : DefaultContractResolver
    {
        
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }
    public class JsonSerializationManagerRDM : ISerializationManagerRDM
    {
        
        private readonly JsonSerializerSettings _settings;

        public JsonSerializationManagerRDM()
        {
            _settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterJsonContractResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };
            _settings.Converters.Add(new BaseDeviceConverter());
            _settings.Converters.Add(new IPEndPointConverter());
            _settings.Converters.Add(new RdmEndPointConverter());
            _settings.Converters.Add(new IPAddressConverter());
            
        }

        /// <summary>
        /// Сериализация объекта в массив байтов
        /// </summary>
        /// <param name="obj">Объект для сериализации</param>
        /// <returns></returns>
        public byte[] Serialize(object obj)
        {

            string jsonData;

            if (obj is IEnumerable<BaseObject> devices)
            {
                jsonData = JsonConvert.SerializeObject(ConvertToDto(devices), _settings);
            }
            else
            {
                jsonData = JsonConvert.SerializeObject(obj, _settings);
            }


            return Encode(jsonData);
        }

        /// <summary>
        /// Десериализация объекта
        /// </summary>
        /// <typeparam name="T">Тип объекта после десериализации</typeparam>
        /// <param name="buff">Массив байтов</param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] buff)
        {
            var jsonData = Decode(buff);

            if (typeof(IEnumerable<BaseDevice>).IsAssignableFrom(typeof(T)))
            {
                IEnumerable<BaseObjectDto> dtoList = JsonConvert.DeserializeObject<List<BaseObjectDto>>(jsonData, _settings);
                return (T)(ConvertToBaseObjects(dtoList) as object);
            }

            return JsonConvert.DeserializeObject<T>(jsonData, _settings);

        }

        private List<BaseObjectDto> ConvertToDto(IEnumerable<BaseObject> items)
        {
            List<BaseObjectDto> dtoList = new List<BaseObjectDto>();
            foreach (BaseObject item in items)
            {
                switch (item)
                {
                    case ArtNetGateway artNetGateway:
                        dtoList.Add(new ArtNetGatewayDto(artNetGateway));
                        break;
                    case ArtNetGatewayNode artNetGatewayNode:
                        dtoList.Add(new ArtNetGatewayNodeDto(artNetGatewayNode));
                        break;
                    case GatewayInputUniverse gatewayInputUniverse:
                        dtoList.Add(new GatewayInputUniverseDto(gatewayInputUniverse));
                        break;
                    case GatewayOutputUniverse gatewayOutputUniverse:
                        dtoList.Add(new GatewayOutputUniverseDto(gatewayOutputUniverse));
                        break;
                    case ParameterInformation parameterInformation:
                        dtoList.Add(new ParameterInformationDto(parameterInformation));
                        break;
                    case Sensor sensor:
                        dtoList.Add(new SensorDto(sensor));
                        break;
                    case RdmDevice rdmDevice:
                        dtoList.Add(new RdmDeviceDto(rdmDevice));
                        break;
                }
            }

            return dtoList;
        }

        private List<BaseDevice> ConvertToBaseObjects(IEnumerable<BaseObjectDto> deviceDtos)
        {
            return deviceDtos.Select(deviceDto => deviceDto.ToBaseObject() as BaseDevice).ToList();
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

    public class BaseDeviceConverter : JsonConverter<BaseObjectDto>
    {
        private static readonly Dictionary<string, Type> NameTypesDictionary;

        static BaseDeviceConverter()
        {
            NameTypesDictionary = new Dictionary<string, Type>()
            {
                {nameof(ArtNetGateway), typeof(ArtNetGatewayDto) },
                {nameof(ArtNetGatewayNode), typeof(ArtNetGatewayNodeDto) },
                {nameof(GatewayInputUniverse), typeof(GatewayInputUniverseDto) },
                {nameof(GatewayOutputUniverse), typeof(GatewayOutputUniverseDto) },
                {nameof(RdmDevice), typeof(RdmDeviceDto) },
                {nameof(Sensor), typeof(SensorDto) },
                {nameof(ParameterInformation), typeof(ParameterInformationDto) }
            };

        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, BaseObjectDto value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override BaseObjectDto ReadJson(JsonReader reader, Type objectType, BaseObjectDto existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            string type = jObject["Type"].Value<string>();

            // Create target object based on JObject
            BaseObjectDto target = Activator.CreateInstance(NameTypesDictionary[type]) as BaseObjectDto;

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }
    }

    public class IPEndPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPEndPoint));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IPEndPoint ep = (IPEndPoint)value;
            JObject jo = new JObject();
            jo.Add("Address", JToken.FromObject(ep.Address, serializer));
            jo.Add("Port", ep.Port);
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
            int port = (int)jo["Port"];
            return new IPEndPoint(address, port);
        }
    }

    class RdmEndPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(RdmEndPoint));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RdmEndPoint rdmEndPoint = (RdmEndPoint)value;
            JObject jo = new JObject();
            jo.Add("Address", JToken.FromObject(rdmEndPoint.Address, serializer));
            jo.Add("Port", rdmEndPoint.Port);
            jo.Add("Universe", rdmEndPoint.Universe);
            jo.Add("Net", rdmEndPoint.Net);

            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
            int port = (int)jo["Port"];
            int universe = (int)jo["Universe"];
            byte net = (byte)jo["Net"];

            return new RdmEndPoint(address, port, universe, net);
        }
    }

    public class IPAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPAddress));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader.Value);
        }
    }