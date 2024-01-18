namespace vc.GitHelper;

public class Constant
{
    
    public class Status
    {
        public const string NOT_PROCESSED = "Not Processed";
        public const string PULL = "Pull";
        public const string RESET = "Reset";
        public const string CLONE = "Clone";
        public const string UNKNOWN = "Unknown";
        public const string FINISHED = "Finished";
    }

    public class Source
    {
        public const string PREFIX = @"https://git.vspglobal.com/scm/pm/";
        public const string SUFFIX = @".git";
    }

    public class FileSystem
    {
        public const string ROOT_FOLDER = @"C:\Dev\VSP\Eyefinity.PM";
    }


}