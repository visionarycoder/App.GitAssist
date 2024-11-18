using System.Diagnostics;

string line = new('-', 80);
const string prefix = @"https://github.com/visionarycoder";
const string suffix = @".git";
const string defaultLocal = @"C:\Dev\GitHub\VisionaryCoder";

const string unknown = "Unknown";
const string notProcessed = "NotProcessed";
const string finished = "Finished";
const string clone = "Clone";
const string reset = "Reset";

Console.WriteLine(line);
Console.WriteLine("VisionaryCoder: Git Helper");
Console.WriteLine(line);

Console.Write("Enter the root directory (press Enter to use default): ");
var inputLocal = Console.ReadLine();
var local = string.IsNullOrWhiteSpace(inputLocal) 
    ? defaultLocal 
    : inputLocal;

if (! Directory.Exists(local))
{
    Console.WriteLine($"The directory '{local}' does not exist.");
    Console.WriteLine("Press any key to exit");
    Console.ReadKey();
    return;
}

var root = new DirectoryInfo(local);
foreach (var directoryInfo in root.EnumerateDirectories())
{
    Console.WriteLine($"Processing {directoryInfo.FullName}");

    var source = $"{prefix}/{directoryInfo.Name.Trim().ToLower()}{suffix}";
    var workflowStatus = notProcessed;

    do
    {
        var commandArgument = workflowStatus switch
        {
            notProcessed => $"/C git -C {directoryInfo.FullName} pull",
            reset => $"/C git -C {directoryInfo.FullName} reset --hard",
            clone => $"/C git -C {directoryInfo.Parent!.FullName} clone {source}",
            _ => string.Empty
        };

        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = commandArgument,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Normal,
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        process.WaitForExit();
        var stdOut = process.StandardOutput.ReadToEnd().ToLower().Trim();
        var errOut = process.StandardError.ReadToEnd().ToLower().Trim();

        workflowStatus = errOut switch
        {
            var err when err.Contains("error:") && err.Contains("unmerged") => reset,
            var err when err.Contains("fatal:") && err.Contains("not a git repository") => clone,
            _ => stdOut.Contains("already up to date") ? finished : unknown
        };

        Console.WriteLine(process.StartInfo.Arguments);
        Console.WriteLine(stdOut);

        if (!string.IsNullOrWhiteSpace(errOut))
        {
            var backgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = Console.ForegroundColor;
            Console.ForegroundColor = backgroundColor;
            Console.WriteLine($"{errOut}");
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = backgroundColor;
        }

    } while (workflowStatus != finished);

    Console.WriteLine();
}

Console.WriteLine(line);
Console.WriteLine("Press any key to exit");
Console.WriteLine(line);
Console.ReadKey();
