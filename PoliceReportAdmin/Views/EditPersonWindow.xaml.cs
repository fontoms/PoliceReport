using System.Windows;

namespace PoliceReportAdmin
{
    public partial class EditPersonWindow : Window
    {
        public Personne PersonneModifiee { get; set; }

        public EditPersonWindow(Personne personne)
        {
            InitializeComponent();
            PersonneModifiee = personne;
            DataContext = PersonneModifiee;
        }

        private void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour enregistrer les modifications
            DialogResult = true;
        }
    }
}
