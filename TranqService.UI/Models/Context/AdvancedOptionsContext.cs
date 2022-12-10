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
            OverrideAppsettingsPath = paths.AppSettingsPath;
            OverrideDatabasePath = paths.DatabasePath;
        });
    }

    public bool CanSave
    {
        get => Get<bool>(nameof(CanSave), overrideDefault: true);
        set => Set(nameof(CanSave), value);
    }

    public string OverrideAppsettingsPath
    {
        get => Get<string>(nameof(OverrideAppsettingsPath));
        set => Set(nameof(OverrideAppsettingsPath), value);
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
            paths.AppSettingsPath = OverrideAppsettingsPath;
            paths.DatabasePath = OverrideDatabasePath;

            // Update the file with the updated properties
            paths.SaveAsync().ContinueWith(task =>
            {
                CanSave = true;
            });
        });
    }

}
