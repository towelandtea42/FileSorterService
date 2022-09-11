namespace FileSorterService.Services.Abstract;

public interface IFilterService
{
    bool ShouldMove(FileInfo file, out string ToPath);
}