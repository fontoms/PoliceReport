using LibGit2Sharp;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Windows;
using System.Windows.Controls;

namespace PoliceReportAdmin
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, List<string>> tablesColumns = new Dictionary<string, List<string>>
        {
            {
                "Personne", new List<string> { "Nom", "Prenom", "Age" }
            },
            {
                "Voiture", new List<string> { "Marque", "Modele", "Annee" }
            }
        };

        private Repository repo;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TableSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TableSelector.SelectedItem != null)
            {
                string selectedTable = ((ComboBoxItem)TableSelector.SelectedItem).Content.ToString();
                AfficherTable(selectedTable);
            }
        }

        private void AfficherTable(string tableName)
        {
            // Supprimer les colonnes existantes
            DataGridItems.Columns.Clear();

            // Ajouter les colonnes pour la table sélectionnée
            if (tablesColumns.ContainsKey(tableName))
            {
                List<string> columns = tablesColumns[tableName];
                foreach (string column in columns)
                {
                    DataGridTextColumn dataGridColumn = new DataGridTextColumn();
                    dataGridColumn.Header = column;
                    dataGridColumn.Binding = new System.Windows.Data.Binding(column);
                    DataGridItems.Columns.Add(dataGridColumn);
                }
            }

            // Calculer la largeur des colonnes
            double totalWidth = DataGridItems.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            if (DataGridItems.Columns.Count > 0)
            {
                double columnWidth = totalWidth / DataGridItems.Columns.Count;
                foreach (var column in DataGridItems.Columns)
                {
                    column.Width = new DataGridLength(columnWidth, DataGridLengthUnitType.Pixel);
                }
            }

            // Exemple de données pour affichage
            List<object> data = new List<object>();
            if (tableName == "Personne")
            {
                data.Add(new Personne { Nom = "Doe", Prenom = "John", Age = 30 });
                data.Add(new Personne { Nom = "Smith", Prenom = "Alice", Age = 25 });
            }
            else if (tableName == "Voiture")
            {
                data.Add(new Voiture { Marque = "Toyota", Modele = "Corolla", Annee = 2020 });
                data.Add(new Voiture { Marque = "Honda", Modele = "Civic", Annee = 2019 });
            }

            // Afficher les données dans le DataGrid
            DataGridItems.ItemsSource = data;
        }


        private void DataGridItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataGridItems.SelectedItem != null)
            {
                // Logique pour gérer le double-clic sur une ligne
            }
        }

        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour ajouter un nouvel élément à la liste
            if (TableSelector.SelectedItem != null)
            {
                string selectedTable = ((ComboBoxItem)TableSelector.SelectedItem).Content.ToString();
                if (selectedTable == "Personne")
                {
                    // Exemple d'ajout d'une personne
                    ((List<Personne>)DataGridItems.ItemsSource).Add(new Personne());
                }
                else if (selectedTable == "Voiture")
                {
                    // Exemple d'ajout d'une voiture
                    ((List<Voiture>)DataGridItems.ItemsSource).Add(new Voiture());
                }
            }
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour supprimer les éléments sélectionnés
            if (DataGridItems.SelectedItems != null && DataGridItems.SelectedItems.Count > 0)
            {
                if (TableSelector.SelectedItem != null)
                {
                    string selectedTable = ((ComboBoxItem)TableSelector.SelectedItem).Content.ToString();
                    if (selectedTable == "Personne")
                    {
                        // Supprimer les personnes sélectionnées
                        foreach (Personne personne in DataGridItems.SelectedItems)
                        {
                            ((List<Personne>)DataGridItems.ItemsSource).Remove(personne);
                        }
                    }
                    else if (selectedTable == "Voiture")
                    {
                        // Supprimer les voitures sélectionnées
                        foreach (Voiture voiture in DataGridItems.SelectedItems)
                        {
                            ((List<Voiture>)DataGridItems.ItemsSource).Remove(voiture);
                        }
                    }
                }
            }
        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour modifier l'élément sélectionné
            if (DataGridItems.SelectedItem != null)
            {
                if (TableSelector.SelectedItem != null)
                {
                    string selectedTable = ((ComboBoxItem)TableSelector.SelectedItem).Content.ToString();
                    if (selectedTable == "Personne")
                    {
                        // Ouvrir une fenêtre pour modifier les détails de la personne sélectionnée
                        Personne selectedPerson = (Personne)DataGridItems.SelectedItem;
                        EditPersonWindow editWindow = new EditPersonWindow(selectedPerson);
                        editWindow.ShowDialog();
                    }
                    else if (selectedTable == "Voiture")
                    {
                        // Ouvrir une fenêtre pour modifier les détails de la voiture sélectionnée
                        Voiture selectedCar = (Voiture)DataGridItems.SelectedItem;
                        EditCarWindow editWindow = new EditCarWindow(selectedCar);
                        editWindow.ShowDialog();
                    }
                }
            }
        }

        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            if (repo != null)
            {
                // Créer un nouvel objet Commit
                string message = "Nouvelles modifications depuis l'application";
                var signature = new LibGit2Sharp.Signature("Votre Nom", "votre@email.com", DateTimeOffset.Now);
                var changes = repo.RetrieveStatus().Staged;

                if (changes.Any())
                {
                    // Commit les changements
                    Commit commit = repo.Commit(message, signature, signature);

                    // Push vers GitHub
                    string remoteName = "origin"; // Nom de votre dépôt distant
                    string branchName = "master"; // Nom de la branche
                    Branch branch = repo.Branches[branchName];
                    if (branch == null)
                    {
                        MessageBox.Show("Branche non trouvée !");
                        return;
                    }

                    var options = new PushOptions
                    {
                        CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                        {
                            Username = "VotreNomDUtilisateurGitHub",
                            Password = GetGitHubPassword()
                        }
                    };

                    repo.Network.Push(branch, options);

                    MessageBox.Show("Commit et Push vers GitHub réussis !");
                }
                else
                {
                    MessageBox.Show("Aucun changement à commit !");
                }
            }
            else
            {
                MessageBox.Show("Dépôt Git non initialisé !");
            }
        }

        private string GetGitHubPassword()
        {
            // Récupérer le mot de passe à partir du Secret Manager
            string password = null;

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                password = Configuration["GitHub:Password"];
            }
            else
            {
                password = Environment.GetEnvironmentVariable("GitHub:Password");
            }

            return password;
        }

    }

    public class Personne
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public int Age { get; set; }
    }

    public class Voiture
    {
        public string Marque { get; set; }
        public string Modele { get; set; }
        public int Annee { get; set; }
    }
}
