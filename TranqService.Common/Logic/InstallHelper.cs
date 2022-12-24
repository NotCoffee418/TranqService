
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

    public static Process[] GetServiceProcesses()
        => Process.GetProcessesByName("TranqService")
        .Where(x => !x.ProcessName.Contains(".Ui"))
        .ToArray();

    public static Process[] GetUiProcesses()
        => Process.GetProcessesByName("TranqService.Ui");

    public static bool IsServiceRunning()
        => GetServiceProcesses().Length > 0;

    public static bool IsUiRunning()
        => GetUiProcesses().Length > 0;

    /// <summary>
    /// Should not be called from service. Will close Service instantly if there is an update.
    /// Keep in mind cmd window pops up whenever the service starts up.
    /// Restarts the service if it was running.
    /// </summary>
    /// <returns></returns>
    public static async Task TryUpdateServiceAsync()
    {
        // Do nothing if no update
        (bool hasUpdate, DateTime? latestVersionTime) = await VersionAccess.IsServiceUpdateAvailableAsync();
        if (!hasUpdate) return;

        // Service is allowed to be shut down
        var processes = GetServiceProcesses();
        int foundRunningProcesses = processes.Length;
        if (foundRunningProcesses > 0)
            foreach (var p in processes)
                p.Kill();

        // Download update and install it
        await InstallUpdateAsync(
            AppConstants.LatestServiceVersionUrl,
            PathHelper.GetServiceDeployDirectory());

        // Update version
        await VersionAccess.UpdateServiceVersionAsync(latestVersionTime.Value);

        // Restart service if it was running
        if (foundRunningProcesses > 0)
            Process.Start(Path.Combine(PathHelper.GetServiceDeployDirectory(), "TranqService.exe"));
    }

    /// <summary>
    /// Should not be called from UI. Will wait for UI to close by user before installing update
    /// </summary>
    /// <returns></returns>
    public static async Task TryUpdateUiAsync()
    {
        // Do nothing if no update
        (bool hasUpdate, DateTime? latestVersionTime) = await VersionAccess.IsUiUpdateAvailableAsync();
        if (!hasUpdate) return;

        // Service is allowed to be shut down
        while (GetUiProcesses().Length > 0)
            await Task.Delay(TimeSpan.FromMinutes(5));

        // Download update and install it
        await InstallUpdateAsync(
            AppConstants.LatestUiVersionUrl,
            PathHelper.GetUiDeployDirectory());

        // Update version
        await VersionAccess.UpdateUiVersionAsync(latestVersionTime.Value);
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
            // Prevent concurrent temp paths existing
            while (File.Exists(zipPath)) zipPath = zipPath + "_";

            // Download update
            using (var s = await AppConstants.HTTPCLIENT.GetStreamAsync(url))
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
        catch (Exception ex)
        {
            Console.WriteLine($"There was an error updating to '{deployProgramDir}'. " + ex.Message);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }
}
