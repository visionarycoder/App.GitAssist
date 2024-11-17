namespace vc.GitHelper;


public static class ProcessCommandFactory
{

    public static ProcessCommand CreatePullCommand(string targetFolder)
    {
        var command = new ProcessCommand
        {
            Prefix = "/C git -C",
            TargetFolder = targetFolder,
            Suffix = "pull",
        };
        return command;
    }

    public static ProcessCommand CreateResetCommand(string targetFolder)
    {
        var command = new ProcessCommand
        {
            Prefix = "/C git -C",
            TargetFolder = targetFolder,
            Suffix = "reset --hard",
        };
        return command;
    }

    public static ProcessCommand CreateCloneCommand(string targetFolder, string repoName)
    {
        var command = new ProcessCommand
        {
            Prefix = "/C git -C",
            TargetFolder = targetFolder,
            Suffix = $"clone https://git.vspglobal.com/scm/pm/{repoName}.git",
        };
        return command;
    }


}