using System.Reflection;
using System.IO;

namespace PDFerter.Contracts
{
    public static class ApiRoutes
    {
        public const string Base = "api";

        public const string Convert = Base + "/" + "convert";

        public const string Split = Base + "/" + "split/{index}";
    }

    public static class LocalPaths
    {
        public static string localPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"../../../"));
        public static string workFilesPath = Path.GetFullPath(Path.Combine(localPath, @"WorkFiles/"));
        public static string resultFilesPath = Path.GetFullPath(Path.Combine(workFilesPath, @"Resultfiles/"));
    }
}