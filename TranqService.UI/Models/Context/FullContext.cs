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
}
