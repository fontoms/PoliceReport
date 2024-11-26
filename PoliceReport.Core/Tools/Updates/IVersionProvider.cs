namespace PoliceReport.Core.Tools.Updates
{
    public interface IVersionProvider
    {
        Task<string> GetLatestReleaseInfoAsync();
        string ExtractDownloadUrl(string releaseInfo);
    }
}
