using System.Diagnostics;

const string unknown = "Unknown";
const string notProcessed = "NotProcessed";
const string finished = "Finished";
const string clone = "Clone";
const string reset = "Reset";

const string prefix = @"https://github.com/visionarycoder";
const string suffix = @".git";
const string defaultLocal = @"C:\Dev\GitHub\VisionaryCoder";

var separator = new String('-', 80);
var title = "Git Assist";

Console.WriteLine(separator);
Console.WriteLine($"{title}");
Console.WriteLine(separator);

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

        if(string.IsNullOrWhiteSpace(commandArgument))
        {
            break;
        }

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
            _ when errOut.Contains("error:") && errOut.Contains("unmerged") => reset,
            _ when errOut.Contains("fatal:") && errOut.Contains("not a git repository") => clone,
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

Console.WriteLine(separator);
Console.WriteLine("Press any key to exit");
Console.WriteLine(separator);
Console.ReadKey();