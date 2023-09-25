using Acn.Rdm.Packets.Sensors;

namespace LcsServer.DevicePollingService.Models;

public class Sensor : BaseObject
    {
        private short? _presentValue;
        private short _minValue;
        private short _maxValue;
        private short _recordedValue;
        private string _description;
        private byte _recordValueSupport;
        private short _normalMaxValue;
        private short _normalMinValue;
        private short _rangeMaxValue;
        private short _rangeMinValue;
        private SensorDefinition.SensorPrefix _prefix;
        private SensorDefinition.SensorUnit _unit;
        private SensorDefinition.SensorTypes _sensorType;
        private byte _sensorNumber;
        private double? _sensorValue;

        public Sensor(string id, string parentId) : base(id, parentId)
        {

        }

        /// <summary>
        /// The sensor number requested is in the range from 0x00 to 0xFE
        /// </summary>
        public byte SensorNumber
        {
            get => _sensorNumber;
            set => _sensorNumber = value;
        }

        /// <summary>
        /// It defines the type of data that is measured by the sensor
        /// </summary>
        public SensorDefinition.SensorTypes SensorType
        {
            get => _sensorType;
            set => _sensorType = value;
        }

        /// <summary>
        ///  It defines the SI unit of the sensor data
        /// </summary>
        public SensorDefinition.SensorUnit Unit
        {
            get => _unit;
            set => _unit = value;
        }

        /// <summary>
        /// It defines the SI Prefix and multiplication factor of the units
        /// </summary>
        public SensorDefinition.SensorPrefix Prefix
        {
            get => _prefix;
            set => _prefix = value;
        }

        /// <summary>
        /// Range Minimum Value:
        /// This is a 2’s compliment signed 16-bit value that represents the lowest value the sensor can
        /// report.A value of –32768 indicates that the minimum is not defined.
        /// </summary>
        public short RangeMinValue
        {
            get => _rangeMinValue;
            set => _rangeMinValue = value;
        }

        /// <summary>
        /// Range Maximum Value:
        /// This is a 2’s compliment signed 16-bit value that represents the highest value the sensor can
        /// report.This also defines the maximum capacity.A value of +32767 indicates that the maximum is
        /// not defined. 
        /// </summary>
        public short RangeMaxValue
        {
            get => _rangeMaxValue;
            set => _rangeMaxValue = value;
        }

        /// <summary>
        /// Normal Minimum Value:
        /// This is a 2’s compliment signed 16-bit value that defines the lowest sensor value for which the
        /// device is in normal operation.A value of –32768 indicates that the minimum is not defined
        /// </summary>
        public short NormalMinValue
        {
            get => _normalMinValue;
            set
            {
                _normalMinValue = value;
            }
        }

        /// <summary>
        /// Normal Maximum Value:
        /// This is a 2’s compliment signed 16-bit value that defines the highest value that for which the
        /// device is in normal operation.A value of +32767 indicates that the maximum is not defined. 
        /// </summary>
        public short NormalMaxValue
        {
            get => _normalMaxValue;
            set
            {
                _normalMaxValue = value;
            }
        }

        /// <summary>
        /// Recorded Value Support:
        /// This field is a bit-masked field to indicate the support for recorded values
        /// </summary>
        public byte RecordValueSupport
        {
            get => _recordValueSupport;
            set => _recordValueSupport = value;
        }

        /// <summary>
        /// Description:
        /// The Description field is used to describe the function of the specified Sensor.This text field shall
        /// be variable up to 32 characters in length.
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = value;
        }

        /// <summary>
        /// Present Value:
        /// This is a 2’s compliment signed 16-bit value that represents the present value of the sensor data.
        /// </summary>
        public short? PresentValue
        {
            get => _presentValue;
            set
            {
                _presentValue = value;
                _sensorValue = ConvertValueWithPrefix(value);
            }
        }
        public double? SensorValue
        {
            get => _sensorValue;
            set
            {
                _sensorValue = ConvertValueWithPrefix(PresentValue);
            }
        }
        /// <summary>
        /// Lowest Detected Value:
        /// This is a 2’s compliment signed 16-bit value that represents the lowest value registered by the
        /// sensor.Support for this data is optional.If this value is not supported, then this field shall be set
        /// to 0x0000.
        /// </summary>
        public short MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        /// <summary>
        /// Highest Detected Value:
        /// This is a 2’s compliment signed 16-bit value that represents the highest value registered by the
        /// sensor.Support for this data is optional.If this value is not supported, then this field shall be set
        /// to 0x0000.
        /// </summary>
        public short MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        /// <summary>
        /// Recorded Value:
        /// This is a 2’s compliment signed 16-bit value that represents the value that was recorded when the
        /// last RECORD_SENSORS was issued.Support for this data is optional.If this value is not
        /// supported, then this field shall be set to 0x0000
        /// </summary>
        public short RecordedValue
        {
            get => _recordedValue;
            set => _recordedValue = value;
        }

        public bool PresentValueIsOutOfNormalRange => PresentValue != null && (PresentValue > NormalMaxValue || PresentValue < NormalMinValue);

        public double? ConvertValueWithPrefix(short? nullableValue)
        {
            if (nullableValue == null)
                return null;

            return ConvertValueWithPrefix(nullableValue.Value);
        }

        /// <summary>
        /// Возвращает значение с учетом префикса сенсора
        /// </summary>
        /// <param name="value">значение которое надо преобразовать</param>
        /// <returns></returns>
        public double ConvertValueWithPrefix(short value)
        {
            switch (Prefix)
            {
                case SensorDefinition.SensorPrefix.None:
                    return value;
                case SensorDefinition.SensorPrefix.Deci:
                    return value / 10d; // 10−1
                case SensorDefinition.SensorPrefix.Centi:
                    return value / 100d; // 10−2
                case SensorDefinition.SensorPrefix.Milli:
                    return value / 1000d; // 10−3
                case SensorDefinition.SensorPrefix.Micro:
                    return value / 1000000d; // 10−6
                case SensorDefinition.SensorPrefix.Nano:
                    return value / 1000000000d; // 10−9
                case SensorDefinition.SensorPrefix.Pico:
                    return value / 1000000000000d; // 10−12
                case SensorDefinition.SensorPrefix.Fempto:
                    return value / 1000000000000000d; // 10−15
                case SensorDefinition.SensorPrefix.Atto:
                    return value / 1000000000000000000d; // 10−18
                case SensorDefinition.SensorPrefix.Zepto:
                    return value / 1000000000000000000000d; // 10−21
                case SensorDefinition.SensorPrefix.Yocto:
                    return value / 1000000000000000000000000d; // 10−24
                case SensorDefinition.SensorPrefix.Deca:
                    return value * 10d; // 10^1
                case SensorDefinition.SensorPrefix.Hecto:
                    return value * 100d; // 10^2
                case SensorDefinition.SensorPrefix.Kilo:
                    return value * 1000d; // 10^3
                case SensorDefinition.SensorPrefix.Mega:
                    return value * 1000000d; // 10^6
                case SensorDefinition.SensorPrefix.Giga:
                    return value * 1000000000d; // 10^9
                case SensorDefinition.SensorPrefix.Terra:
                    return value * 1000000000000d; // 10^12
                case SensorDefinition.SensorPrefix.Peta:
                    return value * 1000000000000000d; // 10^15
                case SensorDefinition.SensorPrefix.Exa:
                    return value * 1000000000000000000d; // 10^18
                case SensorDefinition.SensorPrefix.Zetta:
                    return value * 1000000000000000000000d; // 10^21
                case SensorDefinition.SensorPrefix.Yotta:
                    return value * 1000000000000000000000000d; // 10^24
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }