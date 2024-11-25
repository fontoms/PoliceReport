using PoliceReport.Core.Tools.Updates;
using System.Windows.Controls;

namespace PoliceReport.Updates
{
    public class WpfUpdateUserInterface : IUpdateUserInterface
    {
        private readonly ProgressBar _progressBar;
        private readonly Button _updateButton;

        public WpfUpdateUserInterface(ProgressBar progressBar, Button updateButton)
        {
            _progressBar = progressBar;
            _updateButton = updateButton;
        }

        public void ShowProgressBar(bool visible)
        {
            _progressBar.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void UpdateProgressBar(int percentage)
        {
            _progressBar.Value = percentage;
        }

        public void EnableUpdateButton(bool enabled)
        {
            _updateButton.IsEnabled = enabled;
        }

        public void DisplayErrorMessage(Exception ex)
        {
            System.Windows.MessageBox.Show($"Erreur : {ex.Message}", "Erreur de mise à jour",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}
