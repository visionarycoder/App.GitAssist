namespace vc.GitHelper.Models;

public class ProcessCommand
{
    public string Prefix { get; init; } = string.Empty;
    public string TargetFolder { get; init; } = string.Empty;
    public string Suffix { get; init; } = string.Empty;
}