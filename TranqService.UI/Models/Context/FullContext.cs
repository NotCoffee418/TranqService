namespace TranqService.UI.Models.Context;

public class FullContext : NotificationObject
{
    public AdvancedOptionsContext AdvancedOptionsContext
    {
        get => Get<AdvancedOptionsContext>(nameof(AdvancedOptionsContext));
        set => Set(nameof(AdvancedOptionsContext), value);
    }
}
