namespace HotkeyTyper;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        
        // Enable dark theme support based on Windows system settings (.NET 9+)
        Application.SetColorMode(SystemColorMode.System);
        
        Application.Run(new Form1());
    }    
}