using System.Collections.Concurrent;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.Interfaces;

public interface IStorageManager
{
    public ConcurrentDictionary<string, BaseDevice> Devices { get; set; }
    /// <summary>
    /// Список устроств изменился
    /// </summary>
    event Action<ActionTypes, BaseDevice[]> DeviceListUpdated;

    /// <summary>
    /// Путь к файлу snapshot
    /// </summary>
    string SnapshotFilePath { get; set; }
    public bool scanActive { get; set; }
    /// <summary>
    /// Вернуть устройство по его id
    /// </summary>
    /// <param name="id">id устройства</param>
    /// <returns>Найденое устройство</returns>
    BaseDevice GetDeviceById(string id);

    /// <summary>
    /// Список устройств
    /// </summary>
    /// <returns></returns>
    IEnumerable<BaseDevice> GetDevices();

    /// <summary>
    /// Вернуть словарь устройств, где id устройство это ключ в словаре
    /// </summary>
    /// <returns></returns>
    Dictionary<string, BaseDevice> GetDevicesDictionary();

    /// <summary>
    /// Добавить устройства в кеш
    /// </summary>
    /// <param name="devices">Устройства для добавления</param>
    void AddDevices(BaseDevice[] devices);

    /// <summary>
    /// Вернуть всех потомков
    /// </summary>
    /// <param name="parent">Родитель</param>
    /// <param name="devices">Список в который будут добавлены все найденые потомки</param>
    void GetAllChildren(BaseDevice parent, List<BaseDevice> devices);

    /// <summary>
    /// Запросить все параметры rdm устройств
    /// </summary>
    /// <param name="parentId">Id родительского устройства</param>
    void RefreshAllRdmDevices(string parentId);

    /// <summary>
    /// Проверить какие устройства не были найдены с момента последнего обхода всех устройств.
    /// </summary>
    void CheckWhichDevicesLost();

    /// <summary>
    /// Очистить кеш(список) всех устройств
    /// </summary>
    void ClearDevices();
}