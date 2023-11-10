using TranqService.Common.Abstract;

namespace TranqService.UI.Models.Context;

public class FullContext : NotificationObject
{
    public SetupContext SetupContext
    {
        get => Get<SetupContext>(nameof(SetupContext));
        set => Set(nameof(SetupContext), value);
    }
    
    public AdvancedOptionsContext AdvancedOptionsContext
    {
        get => Get<AdvancedOptionsContext>(nameof(AdvancedOptionsContext));
        set => Set(nameof(AdvancedOptionsContext), value);
    }

    public PlaylistSetupContext PlaylistSetupContext
    {
        get => Get<PlaylistSetupContext>(nameof(PlaylistSetupContext));
        set => Set(nameof(PlaylistSetupContext), value);
    }

    public FailedDownloadsContext FailedDownloadsContext
    {
        get => Get<FailedDownloadsContext>(nameof(FailedDownloadsContext));
        set => Set(nameof(FailedDownloadsContext), value);
    }

    /// <summary>
    /// Assume healthy by default.
    /// Don't rely on this directly for anything other than display purposes.
    /// It is updated when config changes
    /// </summary>
    public bool IndicateConfigAcceptable
    {
        get => Get<bool>(nameof(IndicateConfigAcceptable), true);
        set => Set(nameof(IndicateConfigAcceptable), value);
    }

    public string IsServiceRunningIndicator
    {
        get => Get<string>(nameof(IsServiceRunningIndicator), "Checking service status...");
        set => Set(nameof(IsServiceRunningIndicator), value);
    }

    /// <summary>
    /// Can be updated to show a load bar and status text.
    /// Should be cleared when the status has completed
    /// </summary>
    public string StatusbarText
    {
        get => Get<string>(nameof(StatusbarText), string.Empty);
        set
        {
            Set(nameof(StatusbarText), value);
            RaisePropertyChanged(nameof(ShowStatusbar));
        }
    }
    public bool ShowStatusbar { get => !string.IsNullOrEmpty(StatusbarText); }
}
