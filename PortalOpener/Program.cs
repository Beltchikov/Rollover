namespace PortalOpener
{
    internal static class Program
    {
        
        public static string CHROME_PATH=@"C:\Program Files\Google\Chrome\Application\chrome.exe";
        
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}