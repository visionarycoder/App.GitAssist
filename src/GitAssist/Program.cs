using System.Diagnostics;

const string UNKNOWN = "Unknown";
const string NOT_PROCESSED = "NotProcessed";
const string FINISHED = "Finished";
const string CLONE = "Clone";
const string RESET = "Reset";

const string SUFFIX = @".git";

const string REMOTE = @"https://github.com/visionarycoder";
const string LOCAL = @"C:\Dev\GitHub\VisionaryCoder";

var separator = new string('-', Console.WindowWidth);

Console.CancelKeyPress += (sender, eventArgs) =>
{
    Console.WriteLine(separator);
    Console.WriteLine("Press any key to exit");
    Console.WriteLine(separator);
};

Console.WriteLine(separator);
Console.WriteLine("VisionaryCoder: Git Helper");
Console.WriteLine(separator);

Console.Write("Enter the root directory (press Enter to use default): ");
var inputLocal = Console.ReadLine();
var input = string.IsNullOrWhiteSpace(inputLocal) ? LOCAL : inputLocal.Trim();

if (!Directory.Exists(input))
{
    Console.WriteLine($"The directory '{input}' does not exist.");
    Console.WriteLine("Press any key to exit");
    Console.ReadKey();
    return;
}

var root = new DirectoryInfo(input);
foreach (var directoryInfo in root.EnumerateDirectories())
{

    Console.WriteLine($"Processing {directoryInfo.FullName}");

    var source = $"{REMOTE}/{directoryInfo.Name.Trim().ToLower()}{SUFFIX}";
    var workflowStatus = NOT_PROCESSED;

    do
    {
        var commandArgument = workflowStatus switch
        {
            NOT_PROCESSED => $"/C git -C {directoryInfo.FullName} pull",
            RESET => $"/C git -C {directoryInfo.FullName} reset --hard",
            CLONE => $"/C git -C {directoryInfo.Parent!.FullName} clone {source}",
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
            _ when errOut.Contains("error:") && errOut.Contains("unmerged") => RESET,
            _ when errOut.Contains("fatal:") && errOut.Contains("not a git repository") => CLONE,
            _ => stdOut.Contains("already up to date") ? FINISHED : UNKNOWN
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

    } while (workflowStatus != FINISHED);

    Console.WriteLine();

}

Console.WriteLine(separator);
Console.WriteLine("Press any key to exit");
Console.WriteLine(separator);

Console.ReadKey();