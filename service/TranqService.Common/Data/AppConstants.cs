namespace TranqService.Common.Data;

public class AppConstants
{
    public static readonly HttpClient HTTPCLIENT = new HttpClient();

    // -- Update sources
    public const string LatestServiceVersionUrl = "https://s3.eu-central-1.amazonaws.com/tranqservice-deploy/service-windows-latest.zip";
    public const string LatestUiVersionUrl = "https://s3.eu-central-1.amazonaws.com/tranqservice-deploy/ui-windows-latest.zip";
}
