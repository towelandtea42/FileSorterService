using FileSorterService.Services.Abstract;

namespace FileSorterService.Services.Concrete;
public class FileSortingService : IFileSortingService
{
    private DirectoryInfo _directory;
    private List<FileInfo> _files;
    private readonly IConfigurationSection _configSection;
    private readonly IFilterService _filter;

    public FileSortingService(IConfiguration config, IFilterService filter)
    {
        _filter = filter;
        _configSection = config.GetRequiredSection("Environment");
        _directory = GetDirectoryInfo();
        _files = _directory.GetFiles().ToList();
    }

    public void SortFiles()
    {
        foreach(var file in _files)
        {
            if(_filter.ShouldMove(file, out var toPath))
                TryMove(file, toPath);
        }
    }
    
    public void Refresh()
    {
        _filter.Refresh();
        _directory = GetDirectoryInfo();
        _files.Clear();
        _files = _directory.GetFiles().ToList();
    }

    private void TryMove(FileInfo file, string toPath)
    {
        if (!Directory.Exists(toPath))
            Directory.CreateDirectory(toPath);

        if(!File.Exists(toPath + "\\" + file.Name))
            File.Move(file.FullName, toPath + "\\" + file.Name);
    }

    private DirectoryInfo GetDirectoryInfo()
    {
        var fromPath = Environment.GetEnvironmentVariable(
            _configSection.GetValue<string>("FromPath"));
        return new DirectoryInfo(fromPath!);
    }

}
