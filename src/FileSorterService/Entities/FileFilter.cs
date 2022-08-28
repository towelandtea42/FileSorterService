using System.Text.RegularExpressions;

namespace FileSorterService.Entities;

public class FileFilter
{
    public string ToPath { get; set; } = "";
    public Regex Rule { get; set; } = new Regex(".*");
}
