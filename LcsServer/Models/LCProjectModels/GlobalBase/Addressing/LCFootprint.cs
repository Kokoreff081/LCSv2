using System.Runtime.CompilerServices;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Addressing;

 /// <summary>
    /// Определяет место светильника в адресной карте - его адрес, размер и индекс в массиве источника
    /// </summary>
    public sealed class LCFootprint : IEquatable<LCFootprint>, ILCContainer
    {
        public int DmxAddress { get; }
        public int Size { get; }
        public int SourceIndex { get; }

        private readonly int _hashCode;

        public LCFootprint(int address, int size, int index)
        {
            DmxAddress = address;
            Size = size;
            SourceIndex = index;

            _hashCode = GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PlaceData(byte[] dest, byte[] source, bool shiftAddress)
        {
            int sIndex = SourceIndex;
            int dmxAddr = shiftAddress ? DmxAddress + 1 : DmxAddress;
            for(int i = dmxAddr; i < dmxAddr + Size; i++)
            {
                if (sIndex < source.Length)
                {
                    dest[i] = source[sIndex++];
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Возврашает представление в виде массива байт
        /// </summary>
        /// <returns></returns>
        //public List<byte> ToByteList()
        //{
        //    List<byte> result = new List<byte> 
        //    {
        //        (byte)(DmxAddress >> 8),
        //        (byte)DmxAddress,
        //        (byte)(Size >> 8),
        //        (byte)Size,
        //        (byte)(SourceIndex >> 8),
        //        (byte)SourceIndex
        //    };

        //    return result;
        //}

        public override int GetHashCode()
        {
            if (_hashCode != 0)
            {
                return _hashCode;
            }

            var hashCode = -1772587053;
            hashCode = hashCode * -1521134295 + DmxAddress.GetHashCode();
            hashCode = hashCode * -1521134295 + Size.GetHashCode();
            hashCode = hashCode * -1521134295 + SourceIndex.GetHashCode();
            return hashCode;
        }

        public bool Equals(LCFootprint other)
        {
            return other != null &&
                   DmxAddress == other.DmxAddress &&
                   Size == other.Size &&
                   SourceIndex == other.SourceIndex;
        }

        public override bool Equals(object other)
        {
            return Equals(other as LCFootprint);
        }

        public override string ToString()
        {
            return string.Format($"Address: {DmxAddress}, Size: {Size}, SourceIndex: {SourceIndex}");
        }

        public static bool operator ==(LCFootprint footprint1, LCFootprint footprint2)
        {
            return Comparator.EqualsOperator(footprint1,footprint2);
        }

        public static bool operator !=(LCFootprint footprint1, LCFootprint footprint2)
        {
            return !(footprint1 == footprint2);
        }

    }