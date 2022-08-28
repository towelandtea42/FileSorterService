using FileSorterService.Entities;
using FileSorterService.Services.Abstract;
using System.Text.RegularExpressions;

namespace FileSorterService.Services.Concrete;
public class FilterService : IFilterService
{
    private readonly IConfigurationSection _configSection;
    private List<FileFilter> _filters;
    private readonly ILogger<FilterService> _logger;

    public FilterService(IConfiguration config, ILogger<FilterService> logger)
    {
        _configSection = config.GetRequiredSection("Environment");
        _filters = GetFilters();
        _logger = logger;
    }

    public bool ShouldMove(FileInfo file, out string ToPath)
    {
        foreach(var filter in _filters)
        {
            if(filter.Rule.IsMatch(file.Name))
            {
                ToPath = filter.ToPath;
                Console.WriteLine($"Found match for {file.Name}.");
                Console.WriteLine($"Rule number: {_filters.IndexOf(filter)}\n");
                return true;
            }
        }
        ToPath = String.Empty;
        return false;
    }

    public void Refresh()
    {
        _filters.Clear();
        _filters = GetFilters();
    }

    /// <summary>
    /// Gets filters from FilterFile.
    /// </summary>
    /// <returns></returns>
    private List<FileFilter> GetFilters()
    {
        var filters = new List<FileFilter>();
        try
        {
            string filterFileName = GetFilterFileName();
            var fileLines = File.ReadAllLines(filterFileName!).ToList();

            foreach (var line in fileLines)
                if (!String.IsNullOrEmpty(line))
                    filters.Add(GetFilter(line));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            Environment.Exit(1);
        }

        return filters;
    }

    /// <summary>
    /// Gets a filter from the fileLine obtained.
    /// </summary>
    /// <param name="fileLine"></param>
    /// <returns></returns>
    private static FileFilter GetFilter(string fileLine)
    {
        var aux = fileLine.Split('"', 2, StringSplitOptions.RemoveEmptyEntries);

        var pattern = aux.Length > 1 ? aux[1].Trim() : ".*";

        return new FileFilter()
        {
            ToPath = aux[0].Trim(),
            Rule = new Regex(pattern)
        };
    }

    private string GetFilterFileName()
    {
        string fileName = Environment.GetEnvironmentVariable(
            _configSection.GetValue<string>("FilterFile"))!;

        if(!fileName.EndsWith(".txt"))
            throw new FormatException("Filter file must be a .txt file.");

        return fileName;
    }
}
