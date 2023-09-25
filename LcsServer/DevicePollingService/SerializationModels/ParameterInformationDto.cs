using Acn.Rdm;
using Acn.Rdm.Packets.Parameters;
using Acn.Rdm.Packets.Sensors;
using Acn.Sockets;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.SerializationModels;

public class ParameterInformationDto : BaseObjectDto
    { 
        // For json deserialization
        public ParameterInformationDto() { }

        public ParameterInformationDto(ParameterInformation model)
        {
            ParentId = model.ParentId;
            ParameterId = model.ParameterId;
            UId = model.UId;
            Address = model.Address;
            PdlSize = model.PdlSize;
            DataType = (byte)model.DataType;
            CommandClass = model.CommandClass;
            ParameterType = model.ParameterType;
            Unit = model.Unit;
            Prefix = model.Prefix;
            MinValidValue = model.MinValidValue;
            MaxValidValue = model.MaxValidValue;
            DefaultValue = model.DefaultValue;
            if (model.Value is int intValue)
                Value = intValue;
            
            Description = model.Description;
        }

        public override string Type => nameof(ParameterInformation);

        public override BaseObject ToBaseObject()
        {
            ParameterInformation parameterInformation = new ParameterInformation(null, ParameterId, Address, UId, ParentId)
            {
                PdlSize = PdlSize,
                DataType = (ParameterInformation.ParameterDefinition)DataType,
                CommandClass = CommandClass,
                ParameterType = ParameterType,
                Unit = Unit,
                Prefix = Prefix,
                MinValidValue = MinValidValue,
                MaxValidValue = MaxValidValue,
                DefaultValue = DefaultValue,
                Value = Value,
                Description = Description
            };

            return parameterInformation;
        }

        public string ParentId { get; set; }

        public RdmParameters ParameterId { get; set; }

        public UId UId { get; set; }

        public RdmEndPoint Address { get; set; }

        public byte PdlSize { get; set; }

        public byte DataType { get; set; }

        public ParameterDescription.CommandClass CommandClass { get; set; }

        public byte ParameterType { get; set; }

        public SensorDefinition.SensorUnit Unit { get; set; }

        public SensorDefinition.SensorPrefix Prefix { get; set; }

        public int? MinValidValue { get; set; }

        public int? MaxValidValue { get; set; }

        public int? DefaultValue { get; set; }

        public int? Value { get; set; }

        public string Description { get; set; }
    }