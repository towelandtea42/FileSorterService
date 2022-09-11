using FileSorterService.Services.Abstract;

namespace FileSorterService.Services;

public class WindowsBackgroundService : BackgroundService
{
    private readonly ILogger<WindowsBackgroundService> _logger;
    //private readonly IFileSortingService _sortingService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfigurationSection _configSection;

    public WindowsBackgroundService
        (ILogger<WindowsBackgroundService> logger, 
        IServiceScopeFactory scopeFactory, 
        IConfiguration config)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configSection = config.GetRequiredSection("TimeSpanParameters");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            TimeSpan sleepTime = GetSleepDuration();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                using var scope = _scopeFactory.CreateScope();
                var sortingService = scope.ServiceProvider.GetService<IFileSortingService>();
                sortingService!.SortFiles();
                scope.Dispose();
                Thread.Sleep(sleepTime);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }

        return Task.CompletedTask;
    }

    private TimeSpan GetSleepDuration()
    {
        return new TimeSpan(_configSection.GetValue<int>("Days"),
                            _configSection.GetValue<int>("Hours"),
                            _configSection.GetValue<int>("Minutes"),
                            _configSection.GetValue<int>("Seconds"));
    }
}
