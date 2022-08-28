using FileSorterService.Services;
using FileSorterService.Services.Abstract;
using FileSorterService.Services.Concrete;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Automatic File Sorting Service";
    })
    .ConfigureServices(services =>
    {
        LoggerProviderOptions.RegisterProviderOptions<
            EventLogSettings, EventLogLoggerProvider>(services);
        services.AddHostedService<WorkerService>();
        services.AddSingleton<IFilterService, FilterService>();
        services.AddSingleton<IFileSortingService, FileSortingService>();
    })
    .ConfigureLogging((context, logging) =>
    {
        // See: https://github.com/dotnet/runtime/issues/47303
        logging.AddConfiguration(
            context.Configuration.GetSection("Logging"));
    })
    .Build();

try
{
    await host.RunAsync();
}
catch
{
    //log
    Environment.Exit(1);
}