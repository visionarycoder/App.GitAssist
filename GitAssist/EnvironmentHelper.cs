using System.ComponentModel.DataAnnotations;

namespace vc.GitHelper;

public static class EnvironmentHelper
{

    public static IEnumerable<DirectoryInfo> GetDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {

        return Directory
            .GetDirectories(path, searchPattern, searchOption)
            .Select(p => new DirectoryInfo(p))
            .Where(p => p.Exists);

    }

    public static ValidationResult? VerifyPath(string path)
    {

        if (string.IsNullOrWhiteSpace(path))
        {
            return new ValidationResult("Path is empty");
        }
        return !new DirectoryInfo(path).Exists
            ? new ValidationResult("Directory does not exist")
            : ValidationResult.Success;

    }

    public static DirectoryInfo GetDirectoryInfo(string path)
    {
        return new DirectoryInfo(path);
    }

}