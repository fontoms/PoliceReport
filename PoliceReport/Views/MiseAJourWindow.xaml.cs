using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;

namespace PoliceReport.Views
{
    /// <summary>
    /// Logique d'interaction pour MiseAJourWindow.xaml
    /// </summary>
    public partial class MiseAJourWindow : Window
    {
        public MiseAJourWindow()
        {
            InitializeComponent();
            CheckForUpdates();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void CheckForUpdates()
        {
            string owner = "Fontom71";
            string repo = "PoliceReport";
            string repoUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "request");
                string releaseInfoJson = client.DownloadString(repoUrl);
                string latestVersionStr = releaseInfoJson.Split(new string[] { "\"tag_name\":" }, StringSplitOptions.None)[1].Split(',')[0].Trim().Replace("\"", "");
                Version latestVersion = new Version(latestVersionStr);
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                if (latestVersion > currentVersion)
                {
                    progressBar.Visibility = Visibility.Visible;

                    string latestSetupUrl = $"https://github.com/{owner}/{repo}/releases/latest/download/PoliceReportSetup.exe";
                    string setupFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "PoliceReportSetup.exe");
                    WebClient downloadClient = new WebClient();
                    downloadClient.DownloadProgressChanged += (sender, e) =>
                    {
                        progressBar.Value = e.ProgressPercentage;
                    };
                    downloadClient.DownloadFileCompleted += (sender, e) =>
                    {
                        Process setupProcess = Process.Start(setupFileName);
                        setupProcess.WaitForExit();
                        File.Delete(setupFileName);

                        Process.Start(Assembly.GetExecutingAssembly().Location);
                        Application.Current.Shutdown();
                    };
                    downloadClient.DownloadFileAsync(new Uri(latestSetupUrl), setupFileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification des mises à jour : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
