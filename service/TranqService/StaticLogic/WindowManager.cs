namespace TranqService.StaticLogic;

internal class WindowManager
{
    // Failed import shouldn't break on linux unless called, but not tested
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    internal static void HideConsole()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;
        
        IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
        ShowWindow(h, 0);
     } 
}
