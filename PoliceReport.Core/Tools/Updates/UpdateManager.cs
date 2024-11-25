namespace PoliceReport.Core.Tools.Updates
{
    public class UpdateManager
    {
        private readonly IApplicationUpdater _updater;

        public UpdateManager(IApplicationUpdater updater)
        {
            _updater = updater;
        }

        public async Task PerformUpdateAsync()
        {
            if (await _updater.CheckForUpdatesAsync())
            {
                if (await _updater.DownloadUpdateAsync())
                {
                    await _updater.InstallUpdateAsync();
                }
            }
        }
    }
}
