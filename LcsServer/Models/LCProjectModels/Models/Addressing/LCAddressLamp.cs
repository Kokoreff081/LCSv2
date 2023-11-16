using System.Diagnostics;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;


namespace LcsServer.Models.LCProjectModels.Models.Addressing;

 /// <summary>
    /// Описание светильника в адресном пространстве
    /// </summary>
    public class LCAddressLamp : LCAddressObject, ISaveLoad
    {
        private bool _isReverse;

        public event Action<LCAddressLamp> IsReverseChanged;

        public LCAddressLamp(int universeId)
        {
            SaveParentId = universeId;
        }

        public LCAddressLamp(int id, string name, int universeId, int colorsCount, int lampAddress, int lampId, int pixelsCount)
        {
            Id = id;
            Name = name;
            SaveParentId = universeId;
            ColorsCount = colorsCount;
            LampAddress = lampAddress;
            LampId = lampId;
            PixelsCount = pixelsCount;
        }

        /// <summary>
        /// Адрес светильника в DMX пространстве
        /// </summary>
        [SaveLoad]
        public int LampAddress { get; set; }

        /// <summary>
        /// Кол-во источников
        /// </summary>
        [SaveLoad]
        public int PixelsCount { get; set; }

        /// <summary>
        /// Id светильника на сцене
        /// </summary>
        [SaveLoad]
        public int LampId { get; set; }

        /// <summary>
        /// Кол-во цветов
        /// </summary>
        [SaveLoad]
        public int ColorsCount { get; set; }

        /// <summary>
        /// Указывает начальный адрес(пиксель) светильника (Если IsReverse = true последний становится первым адресом и адресация начинается с него)
        /// </summary>
        [SaveLoad]
        public bool IsReverse
        {
            get => _isReverse;
            set
            {
                if (PixelsCount <= 1 && value)
                {
                    Debug.WriteLine("WARNING! IsReverse can not be set to Luminaries where PixelsCount = 1");
                    return;
                }

                _isReverse = value;
                IsReverseChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Кол-во занимаемых адресов
        /// </summary>
        /// <returns></returns>
        public int GetFootprint()
        {
            return PixelsCount * ColorsCount;
        }

        public int[] GetAddressCells()
        {
            int footprint = GetFootprint();
            int[] addresses = new int[footprint];
            for (int i = 0; i < footprint; i++)
            {
                addresses[i] = LampAddress + i;
            }

            return addresses;
        }

        public void Save(string projectFolderPath)
        {
        }

        public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
        {
        }

        public override void UnsubscribeAllEvents()
        {
            
        }
    }