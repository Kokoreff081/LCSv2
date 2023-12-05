using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Addressing.Enums;
using LcsServer.Models.ProjectModels;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;


namespace LcsServer.Models.LCProjectModels.Models.Addressing;

/// <summary>
    /// Модель порта устройства
    /// </summary>
    public class LCAddressDevicePort : LCAddressObject, ISaveLoad
    {
        [SaveLoad]
        private int _saveUniverseId;

        private LCAddressUniverse _universe;
        private int _portNumber;

        public event Action UniverseChanged;

        public LCAddressDevicePort(int parentId, int portNumber, DmxSizeTypes dmxSizeType)
        {
            SaveParentId = parentId;
            PortNumber = portNumber;
            DmxSize = dmxSizeType;
        }

        public LCAddressDevicePort(int id, string name, int parentId, int portNumber, int universeId, DmxSizeTypes dmxSizeType)
        {
            Id = id;
            Name = name;
            SaveParentId = parentId;
            PortNumber = portNumber;
            _saveUniverseId = universeId;
            DmxSize = dmxSizeType;
        }


        /// <summary>
        /// Размер пространства в одном порту
        /// </summary>
        public DmxSizeTypes DmxSize { get; set; }

        public override string DisplayName => $"Port {PortNumber}";

        /// <summary>
        /// Номер порта
        /// </summary>
        [SaveLoad]
        public int PortNumber
        {
            get => _portNumber;
            set
            {
                _portNumber = value;
                OnNameChanged();
            }
        }

        public LCAddressUniverse Universe
        {
            get => _universe;
            set
            {
                if (_universe != null)
                    _universe.UniverseChanged -= OnUniverseChanged;

                _universe = value;

                if (_universe != null)
                    _universe.UniverseChanged += OnUniverseChanged;

                OnUniverseChanged();
            }
        }

        private void OnUniverseChanged()
        {
            UniverseChanged?.Invoke();
            OnNameChanged();
        }

        public void Save(string projectFolderPath)
        {
        }

        public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
        {
            Universe = primitives.OfType<LCAddressUniverse>().FirstOrDefault(x => x.Id == _saveUniverseId);
        }

        public override void UnsubscribeAllEvents()
        {
            if (_universe != null)
                _universe.UniverseChanged -= OnUniverseChanged;
        }
    }