﻿namespace Acn.Rdm
{
    public class SubDeviceUId:UId
    {
        protected SubDeviceUId()
        {
        }

        public SubDeviceUId(ushort manufacturerId,uint deviceId,short subDeviceId):base(manufacturerId,deviceId)
        {
            SubDeviceId = subDeviceId;            
        }

        public SubDeviceUId(UId source, short subDeviceId)
            : base(source)
        {
            SubDeviceId = subDeviceId;
        }

        public short SubDeviceId { get; set; }

        public static bool IsSubDevice(UId id)
        {
            if (id is SubDeviceUId uId)
                return uId.SubDeviceId != 0;
            return false;
        }

        public static bool IsMatch(UId sourceId, UId compareId, int subDeviceId)
        {
            if (IsSubDevice(sourceId))
            {
                return sourceId.Equals(compareId) && ((SubDeviceUId)sourceId).SubDeviceId == subDeviceId;
            }
            else
            {
                return sourceId.Equals(compareId) && subDeviceId == 0;
            }
        }

        public override string ToString()
        {
            return SubDeviceId > 0 ? $"{base.ToString()}-{SubDeviceId.ToString()}" : base.ToString();
        }
    }
}
