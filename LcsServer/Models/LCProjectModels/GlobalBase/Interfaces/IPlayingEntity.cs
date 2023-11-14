using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

namespace LCSVersionControl.Interfaces;

public interface IPlayingEntity 
{
    /// <summary>
    /// Общее кол-во тиков изменено
    /// </summary>
    event EventHandler TotalTicksChanged; 
    event EventHandler RiseTimeChanged;
    event EventHandler FadeTimeChanged;

    /// <summary>
    /// Id
    /// </summary>
    int Id { get; }
    /// <summary>
    /// Общее кол-во тиков
    /// </summary>
    long TotalTicks { get; }
    /// <summary>
    /// Родитель
    /// </summary>
    IPlayingEntity Parent { get; set; }
    /// <summary>
    /// Попытка получить кадр проигрывания
    /// </summary>
    /// <param name="tick"> Тик </param>
    /// <param name="frame"> Кадр </param>
    /// <param name="calculateDefaultImage"> Рассчитать дефолное изображение </param>
    /// <param name="riseOrFadeLevel">Уровень затухания или розжига</param>
    /// <returns> Получилось ли рассчитать кадр </returns>
    bool TryGetFrame(long tick, out ScenarioFrame frame, bool calculateDefaultImage, float riseOrFadeLevel);

    //bool GetEmptyFrame(long tick, out ScenarioFrame frame);

    public long RiseTime { get; set; }

    public long FadeTime { get; set; }
}