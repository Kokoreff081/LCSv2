namespace LcsServer.DevicePollingService.Interfaces;

public interface ISerializationManagerRDM
{
    /// <summary>
    /// Сериализация объекта в массив байтов
    /// </summary>
    /// <param name="obj">Объект для сериализации</param>
    /// <returns></returns>
    byte[] Serialize(object obj);
    /// <summary>
    /// Десериализация объекта
    /// </summary>
    /// <typeparam name="T">Тип объекта после десериализации</typeparam>
    /// <param name="buff">Массив байтов</param>
    /// <returns></returns>
    T Deserialize<T>(byte[] buff);
}