using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Rollover.Helper
{
    [ExcludeFromCodeCoverage]
    public class FileHelper : IFileHelper
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path,string text)
        {
            File.WriteAllText(path, text);
        }
    }
}
