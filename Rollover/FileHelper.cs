using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Rollover
{
    [ExcludeFromCodeCoverage]
    public class FileHelper : IFileHelper
    {
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
