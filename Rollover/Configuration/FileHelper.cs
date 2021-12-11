using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Rollover.Configuration
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
