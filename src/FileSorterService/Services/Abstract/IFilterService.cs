namespace FileSorterService.Services.Abstract;

public interface IFilterService : IUpdate
{
    bool ShouldMove(FileInfo file, out string ToPath);
}