using PoliceReport.Core.Tools.Updates;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace PoliceReport.Updates
{
    public class ApplicationUpdater : IApplicationUpdater
    {
        private readonly IVersionProvider _versionProvider;
        private readonly IUpdateUserInterface _uiManager;
        private readonly string _setupFileName;

        public ApplicationUpdater(
            IVersionProvider versionProvider,
            IUpdateUserInterface uiManager,
            string setupFileName = "PoliceReportSetup.exe")
        {
            _versionProvider = versionProvider;
            _uiManager = uiManager;
            _setupFileName = Path.Combine(Path.GetTempPath(), setupFileName);
        }

        public async Task<bool> CheckForUpdatesAsync()
        {
            try
            {
                var releaseInfo = await _versionProvider.GetLatestReleaseInfoAsync();
                return !string.IsNullOrEmpty(releaseInfo);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DownloadUpdateAsync()
        {
            try
            {
                _uiManager.EnableUpdateButton(false);
                _uiManager.ShowProgressBar(true);

                var releaseInfo = await _versionProvider.GetLatestReleaseInfoAsync();
                var downloadUrl = _versionProvider.ExtractDownloadUrl(releaseInfo);

                using (var downloadClient = new HttpClient())
                {
                    var response = await downloadClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(_setupFileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        var isMoreDataToRead = true;

                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                            {
                                isMoreDataToRead = false;
                                _uiManager.UpdateProgressBar(100);
                                continue;
                            }

                            await fileStream.WriteAsync(buffer, 0, read);

                            totalRead += read;
                            if (canReportProgress)
                            {
                                _uiManager.UpdateProgressBar((int)((totalRead * 100) / totalBytes));
                            }
                        }
                        while (isMoreDataToRead);
                    }
                }

                _uiManager.ShowProgressBar(false);
                return true;
            }
            catch (Exception ex)
            {
                _uiManager.DisplayErrorMessage(ex);
                return false;
            }
            finally
            {
                _uiManager.EnableUpdateButton(true);
            }
        }

        public async Task InstallUpdateAsync()
        {
            try
            {
                var setupStartInfo = new ProcessStartInfo
                {
                    FileName = _setupFileName,
                    UseShellExecute = true,
                    Verb = "runas" // Élévation de privilèges
                };

                var setupProcess = Process.Start(setupStartInfo);
                if (setupProcess != null)
                {
                    setupProcess.WaitForExit();
                    if (setupProcess.ExitCode == 0) // Vérifie si l'utilisateur a validé l'élévation de privilège
                    {
                        // Fermeture de l'application actuelle
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        _uiManager.DisplayErrorMessage(new Exception("L'installation a été annulée par l'utilisateur."));
                    }
                }
                else
                {
                    throw new Exception("Erreur lors du lancement du processus d'installation.");
                }
            }
            catch (Exception ex)
            {
                _uiManager.DisplayErrorMessage(ex);
            }
        }
    }
}
