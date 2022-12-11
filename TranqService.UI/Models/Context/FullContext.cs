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
}
