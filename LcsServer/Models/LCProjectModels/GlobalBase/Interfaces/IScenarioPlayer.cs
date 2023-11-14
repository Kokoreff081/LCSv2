using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

namespace LCSVersionControl.Interfaces;

 /// <summary>
    /// Интерфейс описыающий работу проигрывателя сценариев
    /// </summary>
    public interface IScenarioPlayer
    {
        /// <summary>
        /// Событие изменения кадра
        /// </summary>
        event EventHandler FrameChanged;

        /// <summary>
        /// Событие изменения истекших тиков
        /// </summary>
        event EventHandler ElapsedTicksChanged;

        /// <summary>
        /// Событие изменения проинициализированного объекта
        /// </summary>
        event EventHandler InitializeItemChanged;

        /// <summary>
        /// Событие изменения флага Проигрывания
        /// </summary>
        event EventHandler IsPlayChanged;

        /// <summary>
        /// Последний кадр
        /// </summary>
        ScenarioFrame Frame { get; }

        /// <summary>
        /// Обновлять кадр 
        /// </summary>
        bool UpdateFrame { get; set; }

        /// <summary>
        /// Флаг проигрывания
        /// </summary>
        bool IsPlay { get; }

        /// <summary>
        /// Получение числа истекших тиков
        /// </summary>
        long ElapsedTicks { get; }

        /// <summary>
        /// Повторять сценарий (зацикливание проигрывания сценария)
        /// </summary>
        bool Repeat { get; set; }

        /// <summary>
        /// Получение Id проинициализированного объекта
        /// </summary>
        int InitializeItemId { get; }

        /// <summary>
        /// Инициализация проигрывателя
        /// </summary>
        /// <param name="playItem"> Объект для проигрывания </param>
        void Initialize(IPlayingEntity playItem);

        /// <summary>
        /// Запуск проигрывания
        /// </summary>
        void Start();

        /// <summary>
        /// Остановка проигрывания
        /// </summary>
        void Stop(bool switchOffLamps = true);
        
        void Pause(bool silent = false);

        /// <summary>
        /// Прокрутка
        /// </summary>
        /// <param name="tick"> тик </param>
        void Rewind(long tick);

        /// <summary>
        /// Сброс цветов
        /// </summary>
        void ResetColors();
        void SwitchOffLamps();
        void ShowLastFrame();
        void CloseCurrent();
    }