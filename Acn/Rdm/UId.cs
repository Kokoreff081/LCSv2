namespace Acn.Rdm
{
    public class UId:IComparable
    {
        protected UId()
        {
        }

        public UId(ushort manufacturerId,uint deviceId)
        {
            ManufacturerId = manufacturerId;
            DeviceId = deviceId;
        }

        protected UId(UId source)
        {
            ManufacturerId = source.ManufacturerId;
            DeviceId = source.DeviceId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UId"/> class. Creates a device ID from the combination
        /// of the specified <paramref name="productId"/> and <paramref name="deviceCode"/>.
        /// </summary>
        /// <param name="manufacturerId">The manufacturer identifier.</param>
        /// <param name="productId">The product identifier, stored in the high byte of the <see cref="DeviceId"/>.</param>
        /// <param name="deviceCode">The device code, stored in the other 3 bytes of the <see cref="DeviceId"/>.</param>
        public UId(ushort manufacturerId, byte productId, uint deviceCode)
            : this(manufacturerId, (uint)((productId << 24) + (deviceCode & 0x00FFFFFF)))
        {
        }

        public ushort ManufacturerId { get; }

        public uint DeviceId { get; }

        #region Predefined Values

        public static UId Broadcast { get; } = new UId(0xFFFF, 0xFFFFFFFF);

        public static UId Empty { get; } = new UId();

        public static UId ManufacturerBroadcast(ushort manufacturerId)
        {
            return new UId(manufacturerId, 0xFFFFFFFF);
        }

        /// <summary>
        /// Gets the minimum possible UId value.
        /// </summary>
        public static UId MinValue { get; } = new UId(0x1, 0x0);

        /// <summary>
        /// Gets the maximum possible UId value.
        /// </summary>
        public static UId MaxValue { get; } = new UId(0x7FFF, 0xFFFFFFFF);

        #endregion
        

        public override string ToString()
        {
            return $"{ManufacturerId:X4}:{DeviceId:X8}";
        }

        public static UId NewUId(ushort manufacturerId, int min = 1, int max = 0x7FFFFFFF)
        {
            Random randomId = new Random();
            return new UId(manufacturerId, (uint) randomId.Next(min, max));
        }

        /// <summary>
        /// Generates a new random <see cref="UId"/> with the specified <paramref name="manufacturerId"/>. The high
        /// byte of <see cref="DeviceId"/> will be the <paramref name="productId"/>, the other 3 bytes will be a
        /// randomly generated number.
        /// </summary>
        /// <param name="manufacturerId">The manufacturer identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <returns>The new <see cref="UId"/>.</returns>
        public static UId NewUId(ushort manufacturerId, byte productId)
        {
            Random randomId = new Random();
            return new UId(manufacturerId, productId, (uint)randomId.Next(1, 0x00FFFFFF));
        }

        public static UId Parse(string value)
        {
            string[] parts = value.Split(':');
            return new UId((ushort) int.Parse(parts[0], System.Globalization.NumberStyles.HexNumber), (uint) int.Parse(parts[1], System.Globalization.NumberStyles.HexNumber));
        }

        public static UId ParseUrl(string url)
        {           
            string[] parts = url.Split('/');
            string idPart = parts[^1];
            
            //Normalize the string
            idPart = idPart.Replace("0x",string.Empty).Replace(":",string.Empty);

            return new UId((ushort) int.Parse(idPart[..4], System.Globalization.NumberStyles.HexNumber), (uint) int.Parse(idPart.Substring(4, 8), System.Globalization.NumberStyles.HexNumber));
        }

        public override int GetHashCode()
        {
            return ManufacturerId.GetHashCode() + DeviceId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is UId id)
                return id.ManufacturerId.Equals(ManufacturerId) && id.DeviceId.Equals(DeviceId);

            return base.Equals(obj);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is UId id)
                return ManufacturerId.CompareTo(id.ManufacturerId) + DeviceId.CompareTo(id.DeviceId);

            return -1;
        }

        #endregion
    }
}
