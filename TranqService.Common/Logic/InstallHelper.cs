
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO.Compression;

namespace TranqService.Common.Logic;

public static class InstallHelper
{
    static InstallHelper()
    {
        SystemEvents.SessionEnding += (_, _) => SessionIsEnding = true;
    }

    // VersionAccess class for update check funcs

    const string RegistryStartupFolder = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    const string RegistryStartupKey = "TranqService Background Runner"; // must match installer script

    /// <summary>
    /// Indicates windows is shutting down or logging off. We may end up with a corrupt install if we update now.
    /// Likely to happen since service will wait for UI to close, and UI may remain open until shutdown.
    /// </summary>
    public static bool SessionIsEnding { get; private set; }

    public static bool DoesServiceStartOnBoot() // todo: linux support
        => Registry.GetValue(RegistryStartupFolder, RegistryStartupKey, null) is null;

    public static bool IsServiceRunning()
        => System.Diagnostics.Process.GetProcessesByName("TranqService").Length > 0;

    public static bool IsUiRunning()
        => System.Diagnostics.Process.GetProcessesByName("TranqService.Ui").Length > 0;

    /// <summary>
    /// Should not be called from service. Will close Service instantly if there is an update.
    /// Keep in mind cmd window pops up whenever the service starts up.
    /// Restarts the service if it was running.
    /// </summary>
    /// <returns></returns>
    public static async Task TryUpdateServiceAsync()
    {
        // Do nothing if no update
        if (!await VersionAccess.IsServiceUpdateAvailableAsync()) return;

        // Service is allowed to be shut down
        var processes = Process.GetProcessesByName("TranqService.Ui");
        int foundRunningProcesses = processes.Length;
        if (foundRunningProcesses > 0)
            foreach (var p in processes)
                p.Kill();

        // Download update and install it
        await InstallUpdateAsync(
            AppConstants.LatestServiceVersionUrl,
            PathHelper.GetUiDeployDirectory());

        // Restart service if it was running
        if (foundRunningProcesses > 0)
            Process.Start(Path.Combine(PathHelper.GetUiDeployDirectory(), "TranqService.exe"));
    }

    /// <summary>
    /// Should not be called from UI. Will wait for UI to close by user before installing update
    /// </summary>
    /// <returns></returns>
    public static async Task TryUpdateUiAsync()
    {
        // Do nothing if no update
        if (!await VersionAccess.IsServiceUpdateAvailableAsync()) return;

        // Service is allowed to be shut down
        while (Process.GetProcessesByName("TranqService.Ui").Length > 0)
            await Task.Delay(TimeSpan.FromMinutes(5));

        // Download update and install it
        await InstallUpdateAsync(
            AppConstants.LatestServiceVersionUrl,
            PathHelper.GetUiDeployDirectory());
    }



    /// <summary>
    /// Should only be called if the exe is not running
    /// </summary>
    /// <param name="url"></param>
    /// <param name="deployProgramDir"></param>
    /// <returns></returns>
    private static async Task InstallUpdateAsync(string url, string deployProgramDir)
    {
        string zipPath = Path.GetTempFileName();
        try
        {
            // Download update
            using (var s = await AppConstants.HTTPCLIENT.GetStreamAsync(AppConstants.LatestServiceVersionUrl))
                using (var fs = new FileStream(zipPath, FileMode.CreateNew))
                    await s.CopyToAsync(fs);

            // Extract it // Check if the destination folder exists. If it doesn't, create it.
            if (!Directory.Exists(deployProgramDir))
                Directory.CreateDirectory(deployProgramDir);

            // Abort if system is about to shut down
            if (SessionIsEnding)
                return;

            // Extract the zip file to the destination folder
            ZipFile.ExtractToDirectory(zipPath, deployProgramDir, true);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }
}
