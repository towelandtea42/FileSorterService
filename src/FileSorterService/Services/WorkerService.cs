using FileSorterService.Services.Abstract;

namespace FileSorterService.Services;

public class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IFileSortingService _sortingService;
    private readonly IConfigurationSection _configSection;

    public WorkerService(ILogger<WorkerService> logger, IFileSortingService service, IConfiguration config)
    {
        _logger = logger;
        _sortingService = service;
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
                _sortingService.SortFiles();
                Thread.Sleep(sleepTime);
                _sortingService.Refresh();
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
