using System.Diagnostics;

namespace vc.GitHelper;

public static class ProcessHelper
{

    public static Process CreateProcess(ProcessStartInfo startInfo)
    {
        var process = new Process { StartInfo = startInfo };
        return process;
    }

    public static ProcessStartInfo CreateStartInfo(ProcessCommand processCommand)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = processCommand.Argument(),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Normal,
        };
        return startInfo;
    }

    public static (string stdOut, string errOut) ExecuteProcess(Process process)
    {

        process.Start();
        process.WaitForExit();
        var stdOut = process.StandardOutput.ReadToEnd().Trim();
        var errOut = process.StandardError.ReadToEnd().Trim();

        return (stdOut, errOut);

    }
}