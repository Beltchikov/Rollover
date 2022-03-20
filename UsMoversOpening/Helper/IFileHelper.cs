namespace UsMoversOpening.Helper
{
    public interface IFileHelper
    {
        public string ReadAllText(string path);
        bool FileExists(string path);
        void WriteAllText(string path, string text);
    }
}