
using System.Data;

namespace TranqService.UI.Models.Context;

public class FailedDownloadsContext : NotificationObject
{
    public List<FailedDatatableModel> FailedDownloadsDataView
    {
        get => Get<List<FailedDatatableModel>>(nameof(FailedDownloadsDataView), new());
        set => Set(nameof(FailedDownloadsDataView), value);
    }
}
