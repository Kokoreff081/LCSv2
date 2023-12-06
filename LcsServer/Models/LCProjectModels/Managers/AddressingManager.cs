using System.Net.NetworkInformation;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Addressing;
using LcsServer.Models.LCProjectModels.GlobalBase.Addressing.CreationParams;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.Models.Addressing;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LCSVersionControl;
using LightCAD.UI.Strings;
using LLcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

namespace LcsServer.Models.LCProjectModels.Managers;

public class AddressingManager : BaseLCObjectsManager, IAddressingManager
{
    private readonly VersionControlManager _versionControlManagerEx;

    //private readonly IColorsMap _colorsMap;
    //private readonly HardwareManager _hardwareManager;

    private List<LCAddressObject> _selectedUniverseObjects;
    private List<LCAddressObject> _selectedAddressDevices;

    public event EventHandler<UpdateObjectsEventArgs> ObjectsUpdated;

    public event Action SelectedUniverseObjectsChanged;
    public event Action SelectedDeviceObjectsChanged;

    //public LCMap LcMap { get; set; } = new LCMap();

    public LCAddressRegisteredMap LCAddressRegisteredMap { get; set; }

    public event Action LicenseChanged;

    public AddressingManager(VersionControlManager versionControlManagerEx/*,
                             IColorsMap colorsMap,
                             HardwareManager hardwareManager*/)
    {
        _versionControlManagerEx = versionControlManagerEx;

        SelectedAddressDevices = new List<LCAddressObject>();
        SelectedUniverseObjects = new List<LCAddressObject>();

        /*_colorsMap = colorsMap;
        _colorsMap.OnColorsInBytesChange += ColorsMap_OnColorsInBytesChange;

        _hardwareManager = hardwareManager; 
        _hardwareManager.LicenseChanged += HardwareManager_LicenseChanged;*/
    }


    public List<LCAddressObject> SelectedAddressDevices
    {
        get => _selectedAddressDevices;
        set
        {
            _selectedAddressDevices = value;
            SelectedDeviceObjectsChanged?.Invoke();
        }
    }

    public List<LCAddressObject> SelectedUniverseObjects
    {
        get => _selectedUniverseObjects;
        set
        {
            if (_selectedUniverseObjects == null || !_selectedUniverseObjects.SequenceEqual(value))
            {
                _selectedUniverseObjects = value;
                SelectedUniverseObjectsChanged?.Invoke();
            }
        }
    }

    public LCAddressUniverse SelectedUniverse => SelectedUniverseObjects.OfType<LCAddressUniverse>().FirstOrDefault();

    public AddressingCreationParams AddressingCreationParams { get; set; }

    /*public bool AreAllLampsNotAddressed(IEnumerable<LCLamp> lamps)
    {
        Dictionary<int, LCAddressLamp> addressLamps = GetPrimitives<LCAddressLamp>().ToDictionary(x => x.LampId, x => x);
        return lamps.All(lamp => !addressLamps.ContainsKey(lamp.Id)); // TODO зачем словарь!!!
    }*/

    public override void AddObjects(params LCObject[] objects)
    {
        var result = AddRangeToLcObjectsList(objects);
        ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(result, InformAction.Add));
    }

    public override void RemoveObjects(LCObject[] objects)
    {
        objects.OfType<LCAddressObject>().ForEach(x => x.UnsubscribeAllEvents());

        RemoveRangeFromLcObjectsList(objects);

        List<LCAddressObject> addressObjects = objects.OfType<LCAddressObject>().ToList();
        SelectedUniverseObjects = SelectedUniverseObjects.Except(addressObjects).ToList();
        SelectedAddressDevices = SelectedAddressDevices.Except(addressObjects).ToList();

        ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(new List<LCObject>(objects), InformAction.Remove));
    }

    public override void RemoveAllObjects()
    {
        GetPrimitives<LCAddressObject>().ForEach(x => x.UnsubscribeAllEvents());

        //LcMap = new LCMap();
        SelectedUniverseObjects.Clear();
        SelectedAddressDevices.Clear();

        LCAddressRegisteredMap?.Clear();

        ClearLcObjectsList();
        ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(null, InformAction.RemoveAll));
    }

    /*public void AutoAddressing(LCAddressUniverse selectedUniverse, AddressingCreationParams creationParams)
    {
        var orderedProjection = creationParams.GetOrderedLampProjection();

        if (creationParams.CreationOption is ContinuousAddressingOption)
            ContinuousAutoAddressing(orderedProjection, selectedUniverse, creationParams);
        else
            MaxRowAutoAddressing(orderedProjection, selectedUniverse, creationParams);
    }

    private void ContinuousAutoAddressing(IEnumerable<LampProjection> orderedProjection, LCAddressUniverse selectedUniverse, AddressingCreationParams creationParams)
    {
        var continuousAddressingOption = (ContinuousAddressingOption)creationParams.CreationOption;

        int address = continuousAddressingOption.StartDmxAddress;

        int prevAddress = 0;
        IntPoint prevProjection = IntPoint.MinusOne;

        List<string> usedUniverseNames = new List<string>();

        LCAddressUniverse universe = selectedUniverse;

        List<LCObject> addedObjects = new List<LCObject>();

        int universeId = GetNextUniverse();

        foreach (LampProjection projection in orderedProjection)
        {
            IntPoint currentPoint = projection.GetFirstPoint();

            int lampAddress;
            int footPrint = projection.PixelsCount * projection.ColorsCount;
            if (prevProjection.Equals(currentPoint))
            {
                lampAddress = prevAddress;
                if (prevAddress + footPrint > address)
                {
                    address = prevAddress + footPrint;
                }
            }
            else
            {
                lampAddress = address;
                prevAddress = lampAddress;
                address += footPrint;
            }

            if (continuousAddressingOption.EndDmxAddress < address)
            {
                LCAddressUniverse newUniverse = new LCAddressUniverse(universe.ParentId, universeId);
                newUniverse.DmxSize = universe.DmxSize;

                lampAddress = 0;
                prevAddress = lampAddress;
                address = lampAddress + footPrint;

                if ((int)newUniverse.DmxSize < address)
                    throw new AutoAddressingException("Auto addressing options are invalid");

                newUniverse.Id = GetNewCurrentId();
                newUniverse.Name = GetNewName("Universe", newUniverse.ParentId, usedUniverseNames);
                usedUniverseNames.Add(newUniverse.Name);

                addedObjects.Add(newUniverse);

                universe = newUniverse;
                universeId++;
            }

            int id = GetNewCurrentId();
            LCAddressLamp addressLamp = new LCAddressLamp(universe.Id)
            {
                Id = id,
                PixelsCount = projection.PixelsCount,
                LampId = projection.LampId,
                ColorsCount = projection.ColorsCount,
                LampAddress = lampAddress,
            };

            addedObjects.Add(addressLamp);

            prevProjection = currentPoint;
        }

        HistoryCommandBuilder.Current.AddAddressingObjects(addedObjects, Resources.AutoAddressingLuminaries);
    }

    private void MaxRowAutoAddressing(IEnumerable<LampProjection> orderedProjection, LCAddressUniverse selectedUniverse, AddressingCreationParams creationParams)
    {
        MaxRowsAddressingOption maxRowAddressingOption = (MaxRowsAddressingOption)creationParams.CreationOption;

        int address = 0;
        int prevAddress = 0;
        IntPoint prevProjection = IntPoint.MinusOne;

        int rowsCountPerUniverse = 1;
        int checkedRowNumber = -1;

        List<string> usedUniverseNames = new List<string>();
        LCAddressUniverse universe = selectedUniverse;

        List<LCObject> addedObjects = new List<LCObject>();

        int universeId = GetNextUniverse();

        var lampProjections = orderedProjection as LampProjection[] ?? orderedProjection.ToArray();
        foreach (var projection in lampProjections)
        {
            IntPoint currentPoint = projection.GetFirstPoint();
            int rowNumber = currentPoint.Y;

            if (checkedRowNumber != rowNumber)
            {
                int checkAddress = address;
                int checkPrevAddress = address;
                IntPoint checkPrevProjection = IntPoint.MinusOne;
                foreach (LampProjection projectionForCheck in lampProjections.Where(x => x.GetFirstPoint().Y == rowNumber))
                {
                    int checkFootPrint = projectionForCheck.PixelsCount * projectionForCheck.ColorsCount;
                    if (checkPrevProjection.Equals(projectionForCheck.GetFirstPoint()))
                    {
                        if (checkPrevAddress + checkFootPrint > checkAddress)
                        {
                            checkAddress = checkPrevAddress + checkFootPrint;
                        }
                    }
                    else
                    {
                        checkPrevAddress = checkAddress;
                        checkAddress += checkFootPrint;
                    }

                    checkPrevProjection = projectionForCheck.GetFirstPoint();
                }

                if ((int)universe.DmxSize < checkAddress - address)
                    throw new AutoAddressingException("Auto addressing options are invalid");

                if (maxRowAddressingOption.MaxRowsCount < rowsCountPerUniverse || (int)universe.DmxSize < checkAddress)
                {
                    LCAddressUniverse newUniverse = new LCAddressUniverse(universe.ParentId, universeId);
                    newUniverse.DmxSize = universe.DmxSize;

                    address = 0;
                    newUniverse.Id = GetNewCurrentId();
                    newUniverse.Name = GetNewName("Universe", newUniverse.ParentId, usedUniverseNames);
                    usedUniverseNames.Add(newUniverse.Name);
                    addedObjects.Add(newUniverse);
                    universe = newUniverse;
                    rowsCountPerUniverse = 1;
                    universeId++;
                }

                checkedRowNumber = rowNumber;
                rowsCountPerUniverse++;
            }

            int id = GetNewCurrentId();
            int lampAddress;
            int footPrint = projection.PixelsCount * projection.ColorsCount;

            if (prevProjection.Equals(currentPoint))
            {
                lampAddress = prevAddress;
                if (prevAddress + footPrint > address)
                {
                    address = prevAddress + footPrint;
                }
            }
            else
            {
                lampAddress = address;
                prevAddress = lampAddress;
                address += footPrint;
            }
            LCAddressLamp addressLamp = new LCAddressLamp(universe.Id)
            {
                Id = id,
                PixelsCount = projection.PixelsCount,
                LampId = projection.LampId,
                ColorsCount = projection.ColorsCount,
                LampAddress = lampAddress,
            };

            addedObjects.Add(addressLamp);

            prevProjection = currentPoint;
        }

        HistoryCommandBuilder.Current.AddAddressingObjects(addedObjects, Resources.AutoAddressingLuminaries);
    }

    private void ColorsMap_OnColorsInBytesChange(byte[] data)
    {
        _hardwareManager.SendFrame(data);
    }

    public void FindLampAddress(LCLamp lamp)
    {
        if (lamp?.AddressData == null)
            return;

        LCAddressUniverse universe = GetPrimitives<LCAddressUniverse>(x => x.Id == lamp.AddressData.ParentId).FirstOrDefault();
        SelectedUniverseObjects = new List<LCAddressObject>(new[] { universe });

    }

    /// <summary>
    /// пересраивает адресную карты и передает ее в модуль управления
    /// </summary>
    /// <param name="addressObjects"></param>
    /// <returns></returns>
    public void RebuildLcMap(List<LCObject> addressObjects)
    {
        List<LCAddressLamp> addressLamps = addressObjects.OfType<LCAddressLamp>().ToList();

        LCAddressRegisteredMap = new LCAddressRegisteredMap();

        var registeredLamps = new Dictionary<int, LCAddressRegisteredLamp>();

        int colorIndex = 0;

        LcMap = new LCMap();

        foreach (LCAddressDevice device in addressObjects.OfType<LCAddressDevice>())
        {
            LCDevice dcoDevice = device switch
            {
                LCArtNetAddressDevice artNetDevice => new LCArtNetDevice(artNetDevice.IpAddress),
                _ => throw new NotSupportedException("Device is not supported")
            };

            foreach (LCAddressDevicePort devicePort in addressObjects.OfType<LCAddressDevicePort>().Where(x => x.ParentId == device.Id))
            {
                if (devicePort.Universe == null)
                    continue;

                LCUniverse universe = new LCUniverse(devicePort.PortNumber, (int)devicePort.DmxSize);
                dcoDevice.AddUniverse(universe, devicePort.PortNumber);

                foreach (LCAddressLamp addressLamp in addressLamps.Where(x => x.ParentId == devicePort.Universe.Id))
                {
                    bool isAddressLampRegistered = false;
                    int lampColorIndex = colorIndex;

                    if (registeredLamps.TryGetValue(addressLamp.LampId, out var registeredLamp))
                    {
                        lampColorIndex = registeredLamp.ArrayIndex;
                        isAddressLampRegistered = true;
                    }
                    else
                    {
                        colorIndex += addressLamp.GetFootprint();
                    }

                    universe.Add(addressLamp.LampAddress, addressLamp.GetFootprint(), lampColorIndex);
                    if (!isAddressLampRegistered)
                    {
                        registeredLamps.Add(addressLamp.LampId, new LCAddressRegisteredLamp
                        {
                            LampId = addressLamp.LampId,
                            ArrayIndex = lampColorIndex,
                            ArraySize = addressLamp.GetFootprint(),
                            IsReverse = addressLamp.IsReverse
                        });
                    }

                }
            }

            LcMap.Add(dcoDevice);
        }

        LCAddressRegisteredMap.RegisteredLamps = registeredLamps.Values.ToList();

        RegisterDeviceMap(LCAddressRegisteredMap, LcMap);

        LicenseChanged?.Invoke();
    }

    /// <summary>
    ///  Кол-во использованных адресных ячеек
    /// </summary>
    /// <param name="addressLamps"></param>
    /// <returns></returns>
    private static int GetUsedChanelsCount(List<LCAddressLamp> addressLamps)
    {
        HashSet<string> usedAddresses = new HashSet<string>();
        int usedChannels = 0;

        foreach (var addressLamp in addressLamps.OrderBy(x => x.ParentId).ThenBy(x => x.LampAddress).ThenBy(x => x.GetFootprint()))
        {
            for (int i = 0; i < addressLamp.GetFootprint(); i++)
            {
                string address = $"{addressLamp.ParentId.ToString()}:{(addressLamp.LampAddress + i).ToString()}";
                if (!usedAddresses.Contains(address))
                {
                    usedAddresses.Add(address);
                    usedChannels++;
                }
            }
        }

        return usedChannels;
    }*/

    /*public void RegisterDeviceMap(LCAddressRegisteredMap lcAddressRegisteredMap, LCMap lcMap)
    {
        _colorsMap.RegisteredMap = lcAddressRegisteredMap;
        _hardwareManager.SetMap(lcMap, lcAddressRegisteredMap?.ByteArraySize ?? 0);
    }*/

    public int GetNextUniverse()
    {
        var universes = GetPrimitives<LCAddressUniverse>().ToList();
        return universes.Count > 0 ? universes.Max(x => x.UId) + 1 : 1;
    }
/*
    public bool UniverseUidExists(int uid)
    {
        return GetPrimitives<LCAddressUniverse>().Any(x => x.UId == uid);
    }

    public void PingDevices()
    {
        Task.Factory.StartNew(() =>
        {
            foreach (var artNetDevice in GetPrimitives<LCArtNetAddressDevice>())
            {
                artNetDevice.IsOnline = IsPortOpen(artNetDevice.IpAddress, 0x1936, TimeSpan.FromSeconds(1));
            }
            foreach (var modbusDevice in GetPrimitives<LCModbusAddressDevice>())
            {
                modbusDevice.IsOnline = IsPortOpen(modbusDevice.Address.ToString(), modbusDevice.Port, TimeSpan.FromSeconds(1));
            }
        });
    }

    private bool IsPortOpen(string host, int port, TimeSpan timeout)
    {
        bool pingable = false;
        using Ping pinger = new Ping();
        try
        {

            PingReply reply = pinger.Send(host, (int)timeout.TotalMilliseconds);
            pingable = reply is { Status: IPStatus.Success };
        }
        catch (PingException)
        {
            // Discard PingExceptions and return false;
        }

        return pingable;
    }


    #region License

    /// <summary>
    /// Не превышает ли адресная карта кол-ву доступных адресов по лицензии
    /// </summary>
    /// <returns>true - не превышает, false - превышает</returns>
    public bool IsCurrentAddressMapLicensed()
    {
        return LicensedChannelsCount > GetUsedChanelsCount(GetPrimitives<LCAddressLamp>().ToList());
    }

    private uint LicensedChannelsCount => _hardwareManager.GetLicensedChannelsCount;

    public uint LicensedDmxCount => _hardwareManager.GetLicensedDmxCount;

    public bool LicenseKeyChanged => _hardwareManager.LicenseKeyChanged;

    public bool IsDemoModeFinished => _hardwareManager.IsDemoModeFinished;

    public bool IsDemoModeStarted => _hardwareManager.IsDemoModeStarted;

    public void ActivateDemoMode()
    {
        _hardwareManager.ActivateDemoMode();
    }

    private void HardwareManager_LicenseChanged()
    {
        LicenseChanged?.Invoke();
    }

    #endregion
*/
    #region Save/Load

    /// <summary>
    /// Сохранить Объекты адресации
    /// </summary>
    /// <param name="projectPath">Путь до папки проекта</param>
    /// <param name="projectName">Имя проекта</param>
    /// <param name="autoSave">Автосохранение</param>
    public void Save(string projectPath, string projectName, bool autoSave)
    {
        string addressingFolder = FileManager.GetAddressingFolderPath(projectPath);
        var addressingFile = FileManager.GetAddressingFilePath(projectPath);
        FileManager.CreateIfNotExist(addressingFolder);

        _versionControlManagerEx.ConvertToVCAndSave(GetPrimitives<LCObject>().OfType<ISaveLoad>(), projectPath, addressingFile, true);
    }

    /// <summary>
    /// Загрузить адресацию
    /// </summary>
    /// <param name="projectPath">Путь до папки проекта</param>
    /// <param name="lamps">Список светильников в проекте, для восстановления объектов адресации</param>
    /// <param name="isNewFormat">Новый формат проекта</param>
    /// <returns>Список объектов адресации</returns>
    public void Load(string addressingFileName, /*List<LCLamp> lamps,*/ bool isNewFormat)
    {
        /*FileManager.CreateIfNotExist(FileManager.GetAddressingFolderPath(projectPath));
        string addressingFileName = FileManager.GetAddressingFilePath(projectPath);*/

        if (!File.Exists(addressingFileName)) return;

        var addressingObjects = _versionControlManagerEx.LoadAndConvertFromVC(addressingFileName, false);

        RestoreAddressingObjects(addressingFileName, ref addressingObjects);//, lamps);

        AddObjects(addressingObjects.Cast<LCObject>().ToArray());
    }


    private static void RestoreAddressingObjects(string projectPath, ref List<ISaveLoad> addressingObjects)//, List<LCLamp> lamps)
    {
        List<LCAddressLamp> removesAddressLamps = new List<LCAddressLamp>();
        for (var i = 0; i < addressingObjects.Count; i++)
        {
            var addressingObject = addressingObjects[i];
            if (addressingObject != null)
            {
                addressingObject.Load(addressingObjects, i);
                /*if (addressingObject is LCAddressLamp addressLamp)
                {
                    LCLamp lamp = lamps.FirstOrDefault(x => x.Id == addressLamp.LampId);
                    if (lamp != null)
                    {
                        lamp.AddressData = addressLamp;
                    }
                    else
                    {
                        removesAddressLamps.Add(addressLamp);
                    }
                }*/
            }
        }

        //addressingObjects = addressingObjects.Except(removesAddressLamps).ToList();
    }


    #endregion

}