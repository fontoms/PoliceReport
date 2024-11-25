namespace PoliceReport.Core.Tools.Updates
{
    public interface IApplicationUpdater
    {
        Task<bool> CheckForUpdatesAsync();
        Task<bool> DownloadUpdateAsync();
        Task InstallUpdateAsync();
    }
}
