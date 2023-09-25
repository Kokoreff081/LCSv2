using Acn.Rdm;
using Acn.Rdm.Broker;
using Acn.Rdm.Packets;
using Acn.Rdm.Packets.Configuration;
using Acn.Rdm.Packets.Control;
using Acn.Rdm.Packets.DMX;
using Acn.Rdm.Packets.Management;
using Acn.Rdm.Packets.Parameters;
using Acn.Rdm.Packets.Power;
using Acn.Rdm.Packets.Product;
using Acn.Rdm.Packets.Sensors;
using Acn.Sockets;
using LcsServer.DevicePollingService.Interfaces;
using LLcsServer.DevicePollingService.Models;
using SlotIds = LcsServer.DevicePollingService.Enums.SlotIds;
using SlotTypes = LcsServer.DevicePollingService.Enums.SlotTypes;

namespace LcsServer.DevicePollingService.Models;

public class RdmDeviceBroker : RdmMessageBroker
    {
        private readonly IRdmSocket _socket;
        private readonly IStorageManager _storageManager;
        private readonly ParametersSettings _parametersSettings;

        public event Action<DeviceFoundEventArgs> NewRdmDeviceFound;

        public RdmDeviceBroker(IRdmSocket socket, IStorageManager storageManager, ParametersSettings parametersSettings)
        {
            _socket = socket;
            _storageManager = storageManager;
            _parametersSettings = parametersSettings;

            RegisterHandlers(this);

            socket.NewRdmPacket += OnNewRdmPacket;
        }

        public void Unsubscribe()
        {
            _socket.NewRdmPacket -= OnNewRdmPacket;
        }

        private void OnNewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {

            RdmDevice rdmDevice = _storageManager.GetDeviceById(e.Packet.Header.SourceId.ToString()) as RdmDevice;

            //Exit if not from the target device or correct sub device.
            if (rdmDevice == null)
                return;

            RdmPacket replyPacket = ProcessPacket(e.Packet);

            SetParameterStatus(e.Packet.Header.ParameterId, ParameterStatus.Valid);

            if (replyPacket != null)
            {
                SendRdm(replyPacket, rdmDevice.Address, rdmDevice.UId);
            }

            if (e.Packet is DeviceInfo.GetReply info)
            {
                //if (rdmDevice.ForceUpdate)
                //{
                //    ForceUpdateRdmDevice(rdmDevice, info.SensorCount, info.SubDeviceCount);
                //    rdmDevice.ForceUpdate = false;
                //}
                //else
                //{
                UpdateRdmDevice(rdmDevice, info.SensorCount, info.SubDeviceCount);
                //}
            }
        }

        private void UpdateRdmDevice(RdmDevice rdmDevice, byte sensorsCount, short subDeviceCount)
        {
            HashSet<RdmParameters> unsupportedParameters = rdmDevice.GetUnsupportedParameters();

            RequestDetails(rdmDevice.Address, rdmDevice.UId);

            RequestLabel(rdmDevice.Address, rdmDevice.UId, unsupportedParameters);
            RequestSoftwareVersionLabel(rdmDevice.Address, rdmDevice.UId, unsupportedParameters);
            RequestBootSoftwareVersionId(rdmDevice.Address, rdmDevice.UId, unsupportedParameters);
            RequestBootSoftwareVersionLabel(rdmDevice.Address, rdmDevice.UId, unsupportedParameters);

            RequestIsIdentifyOn(rdmDevice.Address, rdmDevice.UId);

            RequestConfiguration(rdmDevice.Address, rdmDevice.UId, unsupportedParameters);
            RequestHistory(rdmDevice.Address, rdmDevice.UId, unsupportedParameters);
            RequestPersonality(rdmDevice.Address, rdmDevice);
            RequestParameters(rdmDevice.Address, rdmDevice.UId);

            if (sensorsCount > 0)
                RequestSensors(rdmDevice.Address, rdmDevice.UId, sensorsCount);

            if (!SubDeviceUId.IsSubDevice(rdmDevice.UId))
            {
                for (short n = 1; n <= subDeviceCount; n++)
                {
                    var deviceFoundArgs = new DeviceFoundEventArgs()
                    {
                        Address = rdmDevice.Address.IpAddress,
                        Id = new SubDeviceUId(rdmDevice.UId, n),
                    };
                    NewRdmDeviceFound?.Invoke(deviceFoundArgs);
                }
            }

        }

        public void ForceUpdateRdmDevice(RdmDevice rdmDevice, byte sensorsCount, short subDeviceCount)
        {
            ForceRequestDetails(rdmDevice.Address, rdmDevice.UId);

            ForceRequestLabel(rdmDevice.Address, rdmDevice.UId);
            ForceRequestSoftwareVersionLabel(rdmDevice.Address, rdmDevice.UId);
            ForceRequestBootSoftwareVersionId(rdmDevice.Address, rdmDevice.UId);
            ForceRequestBootSoftwareVersionLabel(rdmDevice.Address, rdmDevice.UId);

            ForceRequestIsIdentifyOn(rdmDevice.Address, rdmDevice.UId);

            ForceRequestConfiguration(rdmDevice.Address, rdmDevice.UId);
            ForceRequestHistory(rdmDevice.Address, rdmDevice.UId);
            ForceRequestPersonality(rdmDevice.Address, rdmDevice);
            ForceRequestParameters(rdmDevice.Address, rdmDevice.UId);

            if (sensorsCount > 0)
                ForceRequestSensors(rdmDevice.Address, rdmDevice.UId, sensorsCount);

            if (!SubDeviceUId.IsSubDevice(rdmDevice.UId))
            {
                for (short n = 1; n <= subDeviceCount; n++)
                {
                    var deviceFoundArgs = new DeviceFoundEventArgs()
                    {
                        Address = rdmDevice.Address.IpAddress,
                        Id = new SubDeviceUId(rdmDevice.UId, n),
                    };
                    NewRdmDeviceFound?.Invoke(deviceFoundArgs);
                }
            }

        }

        #region Packet Handlers

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.ManufacturerLabel)]
        private RdmPacket ProcessManufacturerLabel(RdmPacket packet)
        {
            ManufacturerLabel.GetReply response = packet as ManufacturerLabel.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            rdmDevice.Manufacturer = response.Label;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DeviceModelDescription)]
        private RdmPacket ProcessDeviceModelDescription(RdmPacket packet)
        {
            DeviceModelDescription.GetReply response = packet as DeviceModelDescription.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            rdmDevice.Model = response.Description;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DeviceLabel)]
        private RdmPacket ProcessDeviceLabel(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.Label)))
                return null;

            DeviceLabel.GetReply response = packet as DeviceLabel.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;

            if (rdmDevice == null)
                return null;

            rdmDevice.Label = response.Label;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.SoftwareVersionLabel)]
        private RdmPacket ProcessSoftwareVersionLabel(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.SoftwareVersionLabel)))
                return null;

            SoftwareVersionLabel.GetReply response = packet as SoftwareVersionLabel.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;

            if (rdmDevice == null)
                return null;

            rdmDevice.SoftwareVersionLabel = response.VersionLabel;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.BootSoftwareVersionId)]
        private RdmPacket ProcessBootSoftwareVersionId(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.BootSoftwareVersionId)))
                return null;

            BootSoftwareVersionId.GetReply response = packet as BootSoftwareVersionId.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;

            if (rdmDevice == null)
                return null;

            rdmDevice.BootSoftwareVersionId = response.VersionId;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.BootSoftwareVersionLabel)]
        private RdmPacket ProcessBootSoftwareVersionLabel(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.BootSoftwareVersionLabel)))
                return null;

            BootSoftwareVersionLabel.GetReply response = packet as BootSoftwareVersionLabel.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;

            if (rdmDevice == null)
                return null;

            rdmDevice.BootSoftwareVersionLabel = response.VersionLabel;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DmxStartAddress)]
        private RdmPacket ProcessDmxStartAddress(RdmPacket packet)
        {
            DmxStartAddress.GetReply response = packet as DmxStartAddress.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            rdmDevice.DmxAddress = response.DmxAddress;

            return null;
        }

        [RdmMessage(RdmCommands.SetResponse, RdmParameters.DmxStartAddress)]
        private RdmPacket ProcessSetDmxStartAddress(RdmPacket packet)
        {
            return new DmxStartAddress.Get();
        }

        [RdmMessage(RdmCommands.SetResponse, RdmParameters.DmxPersonality)]
        private RdmPacket ProcessSetDmxPersonality(RdmPacket packet)
        {
            return new DeviceInfo.Get();
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.TiltInvert)]
        private RdmPacket ProcessTiltInvert(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.TiltInvert)))
                return null;

            TiltInvert.GetReply response = packet as TiltInvert.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            rdmDevice.TiltInvert = response.Inverted;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.PanInvert)]
        private RdmPacket ProcessPanInvert(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.PanInvert)))
                return null;

            PanInvert.GetReply response = packet as PanInvert.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            rdmDevice.PanInvert = response.Inverted;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.PanTiltSwap)]
        private RdmPacket ProcessPanTiltSwap(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.PanTiltSwap)))
                return null;

            PanTiltSwap.GetReply response = packet as PanTiltSwap.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            rdmDevice.PanTiltSwap = response.Swapped;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DeviceHours)]
        private RdmPacket ProcessDeviceHours(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.DeviceHours)))
                return null;

            if (packet is DeviceHours.GetReply deviceHoursResponse)
            {
                //if (!_snoopService.IsWorking)
                //    return null;

                RdmDevice rdmDevice = _storageManager.GetDeviceById(deviceHoursResponse.Header.SourceId.ToString()) as RdmDevice;
                if (rdmDevice == null)
                    return null;

                rdmDevice.DeviceHours = deviceHoursResponse.DeviceHours;
            }

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DevicePowerCycles)]
        private RdmPacket ProcessPowerCycles(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.PowerCycles)))
                return null;

            DevicePowerCycles.GetReply response = packet as DevicePowerCycles.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            rdmDevice.PowerCycles = response.PowerCycles;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.LampHours)]
        private RdmPacket ProcessLampHours(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.LampHours)))
                return null;

            LampHours.GetReply response = packet as LampHours.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            rdmDevice.LampHours = response.LampHours;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.LampStrikes)]
        private RdmPacket ProcessLampStrikes(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.LampStrikes)))
                return null;

            LampStrikes.GetReply response = packet as LampStrikes.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            rdmDevice.LampStrikes = response.LampStrikes;

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DmxPersonalityDescription)]
        private RdmPacket ProcessPersonalityDescription(RdmPacket packet)
        {
            if (IsNackResponse(packet, nameof(RdmDevice.Mode)))
                return null;

            DmxPersonalityDescription.GetReply response = packet as DmxPersonalityDescription.GetReply;
            if (response == null)
                return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            if (response.PersonalityIndex == rdmDevice.DmxPersonality)
                rdmDevice.Mode = response.Description;

            rdmDevice.AddPersonalityDescriptionIfNotExists(new PersonalityDescription(response.PersonalityIndex, rdmDevice.Id, response.DmxSlotsRequired, response.Description));

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.SlotInfo)]
        private RdmPacket ProcessSlotInfo(RdmPacket packet)
        {
            Acn.Rdm.Packets.DMX.SlotInfo.GetReply response = packet as Acn.Rdm.Packets.DMX.SlotInfo.GetReply;
            if (response == null)
                return null;


            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            foreach (Acn.Rdm.Packets.DMX.SlotInfo.SlotInformation slot in response.Slots)
            {
                PersonalitySlotInformation slotInfo = new PersonalitySlotInformation((SlotIds)slot.Id, rdmDevice.Id);
                slotInfo.Offset = slot.Offset;
                slotInfo.Type = (SlotTypes)slot.Type;
                slotInfo.SlotLink = slot.SlotLink;

                rdmDevice.PersonalitySlots.Add(slotInfo);

                //if (_snoopService.IsWorking)
                //{
                //Request the slot description.
                SlotDescription.Get slotDescription = new SlotDescription.Get();
                slotDescription.SlotOffset = slot.Offset;
                SendRdm(slotDescription, rdmDevice.Address, rdmDevice.UId);
                //}
            }

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.SlotDescription)]
        private RdmPacket ProcessSlotDescription(RdmPacket packet)
        {
            SlotDescription.GetReply response = packet as SlotDescription.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            if (response.SlotOffset >= 0 && response.SlotOffset < rdmDevice.PersonalitySlots.Count)
            {
                rdmDevice.PersonalitySlots[response.SlotOffset].Description = response.Description;
            }

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.SupportedParameters)]
        private RdmPacket OnProcessSupportedParameters(RdmPacket packet)
        {
            SupportedParameters.GetReply response = packet as SupportedParameters.GetReply;
            if (response == null)
                return null;


            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            foreach (RdmParameters pid in response.ParameterIds.Where(item => item.IsManufacturerPID()))
            {
                ParameterInformation parameterInformation = new ParameterInformation(this, pid, rdmDevice.Address, rdmDevice.UId, rdmDevice.Id);
                string paramKey = $"{rdmDevice.UId}:{pid}";
                if (_parametersSettings.ParametersName.TryGetValue(paramKey, out string paramValue))
                    parameterInformation.Description = paramValue;

                rdmDevice.AddParameterIfNotExists(parameterInformation);

                //if (_snoopService.IsWorking)
                //{
                ParameterDescription.Get descriptionPacket = new ParameterDescription.Get();
                descriptionPacket.ParameterId = pid;
                SendRdm(descriptionPacket, rdmDevice.Address, rdmDevice.UId);

                SendRdm(new RdmRawPacket(RdmCommands.Get, pid) { Data = new byte[0] }, rdmDevice.Address, rdmDevice.UId);
                RegisterHandler(RdmCommands.GetResponse, pid, ProcessParameterValue);
                //}
            }

            return null;
        }

        private RdmPacket ProcessParameterValue(RdmPacket requestPacket)
        {
            RdmRawPacket response = requestPacket as RdmRawPacket;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            rdmDevice.ParameterValueReceived(requestPacket.Header.ParameterId.ToString(), response.Data);

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.ParameterDescription)]
        private RdmPacket ProcessParameterDescription(RdmPacket packet)
        {
            ParameterDescription.GetReply response = packet as ParameterDescription.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice == null)
                return null;

            ParameterInformation parameterInformation = new ParameterInformation(this, response.ParameterId, rdmDevice.Address, rdmDevice.UId, rdmDevice.Id)
            {
                Description = response.Description,
                Prefix = response.Prefix,
                Unit = response.Unit,
                CommandClass = response.CommandClass,
                DataType = (ParameterInformation.ParameterDefinition)response.DataType,
                MaxValidValue = response.MaxValidValue,
                MinValidValue = response.MinValidValue,
                DefaultValue = response.DefaultValue,
                PdlSize = response.PDLSize,
                ParameterType = response.Type,
            };

            rdmDevice.AddParameterIfNotExists(parameterInformation);
            rdmDevice.SetParameterInformation(parameterInformation);

            string paramKey = $"{rdmDevice.UId}:{response.ParameterId}";
            _parametersSettings.AddParameter(paramKey, response.Description);

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.SensorDefinition)]
        private RdmPacket ProcessSensorDefinition(RdmPacket packet)
        {
            SensorDefinition.GetReply response = packet as SensorDefinition.GetReply;
            if (response != null)
            {
                RdmDevice rdmDevice = _storageManager.GetDeviceById(packet.Header.SourceId.ToString()) as RdmDevice;

                if (rdmDevice == null)
                    return null;

                Sensor sensor = new Sensor($"{rdmDevice.Id}_{response.SensorNumber}", rdmDevice.Id)
                {
                    Description = response.Description,
                    NormalMaxValue = response.NormalMaxValue,
                    NormalMinValue = response.NormalMinValue,
                    Prefix = response.Prefix,
                    RangeMaxValue = response.RangeMaxValue,
                    RangeMinValue = response.RangeMinValue,
                    RecordValueSupport = response.RecordValueSupport,
                    SensorNumber = response.SensorNumber,
                    SensorType = response.Type,
                    Unit = response.Unit,
                };

                rdmDevice.SensorFound(sensor);

                //if (_snoopService.IsWorking)
                RequestSensorValue(rdmDevice.Address, rdmDevice.UId, response.SensorNumber);
            }

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.SensorValue)]
        private RdmPacket ProcessSensorValue(RdmPacket packet)
        {
            SensoreValue.GetReply response = packet as SensoreValue.GetReply;
            if (response != null)
            {
                //if (!_snoopService.IsWorking)
                //    return null;

                RdmDevice rdmDevice = _storageManager.GetDeviceById(packet.Header.SourceId.ToString()) as RdmDevice;

                if (rdmDevice == null)
                    return null;

                string sensorId = $"{rdmDevice.Id}_{response.SensorNumber}";

                rdmDevice.SensorValueReceived(sensorId, response.PresentValue, response.MinValue, response.MaxValue, response.RecordedValue);
            }

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.DeviceInfo)]
        private RdmPacket DeviceInfoDefinition(RdmPacket packet)
        {
            DeviceInfo.GetReply response = packet as DeviceInfo.GetReply;
            if (response == null)
                return null;

            //if (!_snoopService.IsWorking)
            //    return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            rdmDevice?.SetDeviceInformation(response);

            return null;
        }

        [RdmMessage(RdmCommands.GetResponse, RdmParameters.IdentifyDevice)]
        private RdmPacket IsIdentifyOn(RdmPacket packet)
        {
            IdentifyDevice.GetReply response = packet as IdentifyDevice.GetReply;
            if (response == null)
                return null;

            RdmDevice rdmDevice = _storageManager.GetDeviceById(response.Header.SourceId.ToString()) as RdmDevice;
            if (rdmDevice != null)
                rdmDevice.IsIdentifyOn = response.IdentifyEnabled;

            return null;
        }

        #endregion

        private void RequestLabel(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters)
        {
            DeviceLabel.Get label = new DeviceLabel.Get();
            SendRdm(label, address, uid, unsupportedParameters);
        }

        public void ForceRequestLabel(RdmEndPoint address, UId uid)
        {
            DeviceLabel.Get label = new DeviceLabel.Get();
            ForceSendRdm(label, address, uid);
        }

        private void RequestBootSoftwareVersionId(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters)
        {
            BootSoftwareVersionId.Get label = new BootSoftwareVersionId.Get();
            SendRdm(label, address, uid, unsupportedParameters);
        }

        public void ForceRequestBootSoftwareVersionId(RdmEndPoint address, UId uid)
        {
            BootSoftwareVersionId.Get label = new BootSoftwareVersionId.Get();
            ForceSendRdm(label, address, uid);
        }

        private void RequestBootSoftwareVersionLabel(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters)
        {
            BootSoftwareVersionLabel.Get label = new BootSoftwareVersionLabel.Get();
            SendRdm(label, address, uid, unsupportedParameters);
        }

        public void ForceRequestBootSoftwareVersionLabel(RdmEndPoint address, UId uid)
        {
            BootSoftwareVersionLabel.Get label = new BootSoftwareVersionLabel.Get();
            ForceSendRdm(label, address, uid);
        }

        private void RequestSoftwareVersionLabel(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters)
        {
            SoftwareVersionLabel.Get label = new SoftwareVersionLabel.Get();
            SendRdm(label, address, uid, unsupportedParameters);
        }

        public void ForceRequestSoftwareVersionLabel(RdmEndPoint address, UId uid)
        {
            SoftwareVersionLabel.Get label = new SoftwareVersionLabel.Get();
            ForceSendRdm(label, address, uid);
        }

        private void RequestConfiguration(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters)
        {
            RequestPanInvert(address, uid, unsupportedParameters);

            RequestTiltInvert(address, uid, unsupportedParameters);

            RequestPanTiltSwap(address, uid, unsupportedParameters);
        }

        private void ForceRequestConfiguration(RdmEndPoint address, UId uid)
        {
            ForceRequestPanInvert(address, uid);

            ForceRequestTiltInvert(address, uid);

            ForceRequestPanTiltSwap(address, uid);
        }

        private void RequestHistory(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters = null)
        {
            RequestDeviceHours(address, uid, unsupportedParameters);

            RequestPowerCycles(address, uid, unsupportedParameters);

            RequestLampHours(address, uid, unsupportedParameters);

            RequestLampStrikes(address, uid, unsupportedParameters);
        }

        private void ForceRequestHistory(RdmEndPoint address, UId uid)
        {
            ForceRequestDeviceHours(address, uid);

            ForceRequestPowerCycles(address, uid);

            ForceRequestLampHours(address, uid);

            ForceRequestLampStrikes(address, uid);
        }

        private void RequestPersonality(RdmEndPoint address, RdmDevice rdmDevice)
        {
            if (rdmDevice.DmxPersonality == null)
                return;

            //packet.PersonalityIndex = rdmDevice.DmxPersonality.Value;
            for (byte i = 1; i <= rdmDevice.DmxPersonalityCount; i++)
            {
                DmxPersonalityDescription.Get packet = new DmxPersonalityDescription.Get();

                packet.PersonalityIndex = i;
                SendRdm(packet, address, rdmDevice.UId);
            }


            //SlotInfo.Get slotPacket = new SlotInfo.Get();
            //SendRdm(slotPacket, address, rdmDevice.UId);
        }

        public void ForceRequestPersonality(RdmEndPoint address, RdmDevice rdmDevice)
        {
            if (rdmDevice.DmxPersonality == null)
                return;

            for (byte i = 1; i <= rdmDevice.DmxPersonalityCount; i++)
            {
                DmxPersonalityDescription.Get packet = new DmxPersonalityDescription.Get();

                packet.PersonalityIndex = i;
                ForceSendRdm(packet, address, rdmDevice.UId);
            }

            //DmxPersonalityDescription.Get packet = new DmxPersonalityDescription.Get();
            //if (rdmDevice.DmxPersonality == null)
            //    return;

            //packet.PersonalityIndex = rdmDevice.DmxPersonality.Value;
            //ForceSendRdm(packet, address, rdmDevice.UId);

            //SlotInfo.Get slotPacket = new SlotInfo.Get();
            //ForceSendRdm(slotPacket, address, rdmDevice.UId);
        }

        public void RequestParameters(RdmEndPoint address, UId uid)
        {
            SupportedParameters.Get packet = new SupportedParameters.Get();
            SendRdm(packet, address, uid);
        }

        public void ForceRequestParameters(RdmEndPoint address, UId uid)
        {
            SupportedParameters.Get packet = new SupportedParameters.Get();
            ForceSendRdm(packet, address, uid);
        }

        public void RequestSensors(RdmEndPoint address, UId uid, int sensorsCount)
        {
            for (int i = 0; i < sensorsCount; i++)
            {
                SensorDefinition.Get packet = new SensorDefinition.Get { SensorNumber = (byte)i };
                SendRdm(packet, address, uid);
            }
        }

        public void ForceRequestSensors(RdmEndPoint address, UId uid, int sensorsCount)
        {
            for (int i = 0; i < sensorsCount; i++)
            {
                SensorDefinition.Get packet = new SensorDefinition.Get { SensorNumber = (byte)i };
                ForceSendRdm(packet, address, uid);
            }
        }

        private void RequestSensorValue(RdmEndPoint address, UId uid, int sensorNumber)
        {
            SensoreValue.Get packet = new SensoreValue.Get { SensorNumber = (byte)sensorNumber };
            SendRdm(packet, address, uid);
        }

        private void RequestDetails(RdmEndPoint address, UId uid)
        {
            RequestManufacture(address, uid);
            RequestModelDescription(address, uid);
            RequestDmxAddress(address, uid);
        }

        private void ForceRequestDetails(RdmEndPoint address, UId uid)
        {
            ForceRequestManufacture(address, uid);
            ForceRequestModelDescription(address, uid);
            ForceRequestDmxAddress(address, uid);
        }

        private void RequestModelDescription(RdmEndPoint address, UId uid)
        {
            DeviceModelDescription.Get model = new DeviceModelDescription.Get();
            SendRdm(model, address, uid);
        }

        public void ForceRequestModelDescription(RdmEndPoint address, UId uid)
        {
            DeviceModelDescription.Get model = new DeviceModelDescription.Get();
            ForceSendRdm(model, address, uid);
        }

        private void RequestManufacture(RdmEndPoint address, UId uid)
        {
            ManufacturerLabel.Get manufacturer = new ManufacturerLabel.Get();
            SendRdm(manufacturer, address, uid);
        }

        public void ForceRequestManufacture(RdmEndPoint address, UId uid)
        {
            ManufacturerLabel.Get manufacturer = new ManufacturerLabel.Get();
            ForceSendRdm(manufacturer, address, uid);
        }

        private void RequestDmxAddress(RdmEndPoint address, UId uid)
        {
            DmxStartAddress.Get dmxAddress = new DmxStartAddress.Get();
            SendRdm(dmxAddress, address, uid);
        }

        public void ForceRequestDmxAddress(RdmEndPoint address, UId uid)
        {
            DmxStartAddress.Get dmxAddress = new DmxStartAddress.Get();
            ForceSendRdm(dmxAddress, address, uid);
        }

        private void RequestPowerCycles(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters = null)
        {
            DevicePowerCycles.Get cycles = new DevicePowerCycles.Get();
            SendRdm(cycles, address, uid, unsupportedParameters);
        }

        public void ForceRequestPowerCycles(RdmEndPoint address, UId uid)
        {
            DevicePowerCycles.Get cycles = new DevicePowerCycles.Get();
            ForceSendRdm(cycles, address, uid);
        }

        private void RequestDeviceHours(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters = null)
        {
            DeviceHours.Get hours = new DeviceHours.Get();
            SendRdm(hours, address, uid, unsupportedParameters);
        }

        public void ForceRequestDeviceHours(RdmEndPoint address, UId uid)
        {
            DeviceHours.Get hours = new DeviceHours.Get();
            ForceSendRdm(hours, address, uid);
        }

        private void RequestLampHours(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters = null)
        {
            LampHours.Get lampHours = new LampHours.Get();
            SendRdm(lampHours, address, uid, unsupportedParameters);
        }

        public void ForceRequestLampHours(RdmEndPoint address, UId uid)
        {
            LampHours.Get lampHours = new LampHours.Get();
            ForceSendRdm(lampHours, address, uid);
        }

        private void RequestLampStrikes(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters = null)
        {
            LampStrikes.Get lampStrikes = new LampStrikes.Get();
            SendRdm(lampStrikes, address, uid, unsupportedParameters);
        }

        public void ForceRequestLampStrikes(RdmEndPoint address, UId uid)
        {
            LampStrikes.Get lampStrikes = new LampStrikes.Get();
            ForceSendRdm(lampStrikes, address, uid);
        }

        private void RequestPanInvert(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters = null)
        {
            PanInvert.Get pan = new PanInvert.Get();
            SendRdm(pan, address, uid, unsupportedParameters);
        }

        public void ForceRequestPanInvert(RdmEndPoint address, UId uid)
        {
            PanInvert.Get pan = new PanInvert.Get();
            ForceSendRdm(pan, address, uid);
        }

        private void RequestTiltInvert(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters = null)
        {
            TiltInvert.Get tilt = new TiltInvert.Get();
            SendRdm(tilt, address, uid, unsupportedParameters);
        }

        public void ForceRequestTiltInvert(RdmEndPoint address, UId uid)
        {
            TiltInvert.Get tilt = new TiltInvert.Get();
            ForceSendRdm(tilt, address, uid);
        }

        private void RequestPanTiltSwap(RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters = null)
        {
            PanTiltSwap.Get swap = new PanTiltSwap.Get();
            SendRdm(swap, address, uid, unsupportedParameters);
        }

        public void ForceRequestPanTiltSwap(RdmEndPoint address, UId uid)
        {
            PanTiltSwap.Get swap = new PanTiltSwap.Get();
            ForceSendRdm(swap, address, uid);
        }

        public void RequestDeviceInfo(RdmEndPoint address, UId uid)
        {
            DeviceInfo.Get getInfo = new DeviceInfo.Get();
            SendRdm(getInfo, address, uid);
        }

        public void ForceRequestDeviceInfo(RdmEndPoint address, UId uid)
        {
            DeviceInfo.Get getInfo = new DeviceInfo.Get();
            ForceSendRdm(getInfo, address, uid);
        }

        private void RequestIsIdentifyOn(RdmEndPoint address, UId uid)
        {
            IdentifyDevice.Get getIdentifyDevice = new IdentifyDevice.Get();
            SendRdm(getIdentifyDevice, address, uid);
        }

        public void ForceRequestIsIdentifyOn(RdmEndPoint address, UId uid)
        {
            IdentifyDevice.Get getIdentifyDevice = new IdentifyDevice.Get();
            ForceSendRdm(getIdentifyDevice, address, uid);
        }

        private bool IsNackResponse(RdmPacket packet, string parameterName)
        {
            if (packet is RdmNack rdmNackResponse)
            {
                RdmDevice rdmDevice = _storageManager.GetDeviceById(rdmNackResponse.Header.SourceId.ToString()) as RdmDevice;
                rdmDevice?.AddUnsupportedParameter(packet.Header.ParameterId, parameterName);
                return true;
            }

            return false;
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint address, UId uid, HashSet<RdmParameters> unsupportedParameters = null)
        {
            if (unsupportedParameters == null || !unsupportedParameters.Contains(packet.Header.ParameterId))
            {
                _socket.SendRdm(packet, address, uid);
            }
        }

        public void ForceSendRdm(RdmPacket packet, RdmEndPoint address, UId uid)
        {
            if (_socket is RdmReliableSocket rdmReliableSocket)
                rdmReliableSocket.ForceSendRdm(packet, address, uid);
            else
                _socket.SendRdm(packet, address, uid);
        }
    }