using PoliceReport.Views;
using System.Windows;

namespace PoliceReport
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MiseAJourWindow.Instance.Show();
        }
    }

}
