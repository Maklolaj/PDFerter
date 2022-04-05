using System.Reflection;
using System.IO;

namespace PDFerter.Contracts
{
    public static class ApiRoutes
    {
        public const string Base = "api";

        public const string Merge = Base + "/" + "merge";

        public const string Split = Base + "/" + "split/{index}";
    }
}