using System.Windows;

namespace PoliceReportAdmin
{
    public partial class EditCarWindow : Window
    {
        public Voiture VoitureModifiee { get; set; }

        public EditCarWindow(Voiture voiture)
        {
            InitializeComponent();
            VoitureModifiee = voiture;
            DataContext = VoitureModifiee;
        }

        private void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour enregistrer les modifications
            DialogResult = true;
        }
    }
}
