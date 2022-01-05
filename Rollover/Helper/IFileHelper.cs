namespace Rollover.Helper
{
    public interface IFileHelper
    {
        public string ReadAllText(string path);
        bool FileExists(string path);
    }
}