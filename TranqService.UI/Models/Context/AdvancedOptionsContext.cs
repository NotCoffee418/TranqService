using System;
using System.Threading.Tasks;
using TranqService.Common.Models.Configs;

namespace TranqService.UI.Models.Context;

/// <summary>
/// These values are empty by default, only used to set, never read.
/// </summary>
public class AdvancedOptionsContext : NotificationObject
{
    public AdvancedOptionsContext()
    {
        // Load known values from config
        AppPaths.GetAsync().ContinueWith(task =>
        {
            AppPaths paths = task.Result;
            OverrideConfigDir = paths.AppSettingsDir;
            OverrideDatabasePath = paths.DatabaseFile;
        });
    }

    public bool CanSave
    {
        get => Get<bool>(nameof(CanSave), overrideDefault: true);
        set => Set(nameof(CanSave), value);
    }

    public string OverrideConfigDir
    {
        get => Get<string>(nameof(OverrideConfigDir));
        set => Set(nameof(OverrideConfigDir), value);
    }
    
    public string OverrideDatabasePath
    {
        get => Get<string>(nameof(OverrideDatabasePath));
        set => Set(nameof(OverrideDatabasePath), value);
    }

    public void Save()
    {
        CanSave = false;
        AppPaths.GetAsync().ContinueWith(task =>
        {
            // Load file first to ensure integrity of any other properties
            AppPaths paths = task.Result;
            paths.AppSettingsDir = OverrideConfigDir;
            paths.DatabaseFile = OverrideDatabasePath;

            // Update the file with the updated properties
            paths.SaveAsync().ContinueWith(task =>
            {
                CanSave = true;
            });
        });
    }

}
