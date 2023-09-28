namespace LcsServer.DevicePollingService;

public class BackgroundDevicePolling : BackgroundService
{
    private DevicePollService _service;

    public BackgroundDevicePolling(DevicePollService service)
    {
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if(!_service.IsWorking && !_service.IsCommandStopped)
                    await Task.Run(() => _service.Start());
            }
            catch (Exception ex)
            {
                int point = 0;
                // обработка ошибки однократного неуспешного выполнения фоновой задачи
            }

            await Task.Delay(121000);
        }
    }
}