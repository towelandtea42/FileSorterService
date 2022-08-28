# FileSorterService
 A Windows Service for sorting files. 

 Create a filter file with the following syntax:
 "C:\Path" regexExpression
 (1 rule per line)

 Set an environment variable (FROM_PATH) for the folder you wish to sort.
 Set another environment variable (FILTER_FILE_PATH) to indicate where the filter file is located.
 Change how often it runs in appsettings.json (Section "TimeSpanParameters").

 Code currently runs (and works) in Visual Studio, but I haven't tried installing and running as an actual Windows Service yet. 

 Still plan on adding more options, like filtering by dates, ignoring files/folders, checking subfolders, writing in a log file instead of overwriting existing files and rule priority levels.
