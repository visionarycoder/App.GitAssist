using vc.GitHelper.Models;

namespace vc.GitHelper.Helpers;

public static class ProcessCommandExtension
{
    
    public static string Argument(this ProcessCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        return $"{command.Prefix} {command.TargetFolder} {command.Suffix}";
    }

}