using Acn.Rdm;
using Acn.Rdm.Packets;
using Acn.Rdm.Packets.Parameters;
using Acn.Rdm.Packets.Sensors;
using Acn.Sockets;
using LcsServer.DevicePollingService.Enums;

namespace LcsServer.DevicePollingService.Models;

public class ParameterInformation : BaseObject
    {
        private readonly RdmDeviceBroker _deviceBroker;
        private int? _defaultValue;
        private object _value;
        private int? _maxValidValue;
        private int? _minValidValue;
        private SensorDefinition.SensorPrefix _prefix;
        private SensorDefinition.SensorUnit _unit;
        private byte _type;
        private ParameterDescription.CommandClass _commandClass;
        private ParameterDefinition _dataType;
        private byte _pdlSize;
        private string _description;

        //public ParameterInformation() { }

        public ParameterInformation(RdmDeviceBroker deviceBroker, RdmParameters parameterId, RdmEndPoint address, UId uId, string parentId) : base(parameterId.ToString(), parentId)
        {
            _deviceBroker = deviceBroker;
            ParameterId = parameterId;
            UId = uId;
            Address = address;
        }

        public RdmParameters ParameterId { get; }

        public UId UId { get; }

        public RdmEndPoint Address { get; }

        /// <summary>
        /// PDL Size defines the number used for the PDL field in all GET_RESPONSE and SET messages
        /// associated with this PID. In the case of the value of DS_ASCII, the PDL Size represents the
        /// maximum length of a variable sized ASCII string.
        /// </summary>
        public byte PdlSize
        {
            get => _pdlSize;
            set => _pdlSize = value;
        }

        /// <summary>
        /// Data Type defines the size of the data entries in the PD of the message for this PID. For
        /// example: unsigned 8-bit character versus signed 16-bit word. Table A-15 enumerates the field
        /// codes.
        /// </summary>
        public ParameterDefinition DataType
        {
            get => _dataType;
            set => _dataType = value;
        }

        /// <summary>
        /// Command Class defines whether Get and or Set messages are implemented for the specified
        /// PID. Table A-16 enumerates the field codes.
        /// </summary>
        public ParameterDescription.CommandClass CommandClass
        {
            get => _commandClass;
            set => _commandClass = value;
        }

        /// <summary>
        /// Type is an unsigned 8-bit value enumerated by Table A-12. It defines the type of data that is
        /// described by the specified PID.
        /// </summary>
        public byte ParameterType
        {
            get => _type;
            set => _type = value;
        }

        /// <summary>
        /// Unit is an unsigned 8-bit value enumerated by Table A-13. It defines the SI (International System
        /// of units) unit of the specified PID data.
        /// </summary>
        public SensorDefinition.SensorUnit Unit
        {
            get => _unit;
            set => _unit = value;
        }

        /// <summary>
        /// Prefix is an unsigned 8-bit value enumerated by Table A-14. It defines the SI Prefix and
        /// multiplication factor of the units.
        /// </summary>
        public SensorDefinition.SensorPrefix Prefix
        {
            get => _prefix;
            set => _prefix = value;
        }

        /// <summary>
        /// This is a 32-bit field that represents the lowest value that data can reach. The format of the
        /// number is defined by DATA TYPE. This field has no meaning for a Data Type of DS_BIT_FIELD
        /// or DS_ASCII. For Data Types less than 32-bits, the Most Significant Bytes shall be padded with
        /// 0x00 out to 32-bits. For example, an 8-bit data value of 0x12 shall be represented in the field as:
        /// 0x00000012.
        /// </summary>
        public int? MinValidValue
        {
            get => _minValidValue;
            set => _minValidValue = value;
        }

        /// <summary>
        /// This is a 32-bit field that represents the highest value that data can reach. The format of the
        /// number is defined by DATA TYPE. This field has no meaning for a Data Type of DS_BIT_FIELD
        /// or DS_ASCII. For Data Types less than 32-bits, the Most Significant Bytes shall be padded with
        /// 0x00 out to 32-bits. For example, an 8-bit data value of 0x12 shall be represented in the field as:
        /// 0x00000012.
        /// </summary>
        public int? MaxValidValue
        {
            get => _maxValidValue;
            set => _maxValidValue = value;
        }

        /// <summary>
        /// This is a 32-bit field that represents the default value of that data. This field has no meaning for a
        /// Data Type of DS_BIT_FIELD or DS_ASCII. The default value shall be within the minimum and
        /// maximum range. For Data Types less than 32-bits, the Most Significant Bytes shall be padded
        /// with 0x00 out to 32-bits. For example, an 8-bit data value of 0x12 shall be represented in the field
        /// as: 0x00000012.
        /// </summary>
        public int? DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        /// <summary>
        /// The current value of the parameter
        /// </summary>
        public object Value
        {
            get => _value;
            set => _value = value;
        }

        public void ChangeValueRequest(byte[] value)
        {
            RdmRawPacket setRequest = new RdmRawPacket(RdmCommands.Set, ParameterId);

            setRequest.Data = value;

            _deviceBroker.ForceSendRdm(setRequest, Address, UId);

            Value = null;

            // Запускаем задачу на получение Value через 1 сек., чтобы запрос на изменение Value успел отработать
            Task.Delay(1000).ContinueWith((x) =>
            {
                RdmRawPacket getRequest = new RdmRawPacket(RdmCommands.Get, ParameterId);
                getRequest.Data = new byte[0];
                _deviceBroker.ForceSendRdm(getRequest, Address, UId);
            });
        }

        /// <summary>
        /// The Description field is used to describe the function of the specified PID. This text field shall be
        /// variable up to 32 characters in length.
        /// </summary>
        public string Description
        {
            get => _description;
            set => _description = value;
        }
        public enum ParameterDefinition
        {
            None = 0x00, // Data type is not defined
            Bit = 0x01, // Data is bit packed
            ASCII = 0x02,// Data is a string
            UByte = 0x03, // Data is an array of unsigned bytes,
            SByte = 0x04, // Data is an array of signed bytes
            UWord = 0x05, // Data is an array of unsigned 16-bit words
            SWord = 0x06, // Data is an array of signed 16-bit words
            UDWord = 0x07, // Data is an array of unsigned 32-bit words
            SDWord = 0x08, // Data is an array of signed 32-bit words
            ManufactureSpecificData = 0x80 // Manufacturer-Specific Data Types
        }

        

    }