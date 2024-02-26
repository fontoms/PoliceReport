using StorageLayer;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PoliceReport
{
    public partial class AdministrationWindow : Window
    {
        private BaseDao Database;

        public AdministrationWindow()
        {
            InitializeComponent();
            Database = new BaseDao();
            LoadTables();
        }

        private void LoadTables()
        {
            // Charger les tables de la base de données
            List<string> tables = Database.GetTables();
            foreach (string table in tables)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = table;
                TableSelector.Items.Add(item);
            }
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

            // Nom de la classe DAO est formé en concaténant le nom de la table avec "Dao" à la fin
            string daoClassName = tableName + "Dao";

            // Assurez-vous que la classe DAO existe dans le namespace approprié (ici, j'utilise StorageLayer.Dao)
            Type daoType = Type.GetType("StorageLayer.Dao." + daoClassName + ", StorageLayer");

            if (daoType != null)
            {
                // Crée une instance de la classe DAO
                dynamic daoInstance = Activator.CreateInstance(daoType);

                // Vérifie si la classe DAO a une méthode GetAll()
                MethodInfo getAllRowsMethod = daoType.GetMethod("GetAll");

                if (getAllRowsMethod != null)
                {
                    // Appel de la méthode GetAll() pour récupérer les données de la table
                    dynamic rows = getAllRowsMethod.Invoke(daoInstance, null);

                    // Récupérer les colonnes de la table
                    List<string> columns = Database.GetColumnsOfTable(tableName);

                    if (columns.Count > 0)
                    {
                        // Créer des colonnes pour le DataGrid
                        foreach (string column in columns)
                        {
                            DataGridTextColumn textColumn = new DataGridTextColumn();
                            textColumn.Header = column;
                            textColumn.Binding = new Binding(column);

                            // Appliquer le style pour le retour à la ligne
                            textColumn.ElementStyle = Resources["WrapCellStyle"] as Style;

                            if (column != "Id")
                            {
                                textColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star); // Ajuster la largeur des autres colonnes
                            }

                            DataGridItems.Columns.Add(textColumn);
                        }
                    }

                    // Affichage des données où vous le souhaitez
                    // Par exemple, supposons que vous ayez une DataGrid nommée "DataGridItems" dans votre interface
                    DataGridItems.ItemsSource = rows;

                    // Mettre à jour le label d'information
                    UpdateInfoLabel();
                }
                else
                {
                    // Si la classe DAO correspondante n'a pas de méthode GetAll()
                    // Affichez un message d'erreur
                    MessageBox.Show("Erreur : Impossible de récupérer les données de la table.", daoClassName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Si la classe DAO correspondante n'est pas trouvée ou n'a pas de méthode GetAll()
                // Affichez un message d'erreur
                MessageBox.Show("Erreur : Impossible de récupérer les données de la table.", tableName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGridItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataGridItems.SelectedItem != null)
            {
                GérerAjoutModification(false);
            }
        }

        private void UpdateInfoLabel()
        {
            if (DataGridItems.Items != null)
            {
                int totalItems = DataGridItems.Items.Count;
                int selectedItems = DataGridItems.SelectedItems.Count;

                CountLabel.Content = selectedItems > 0 ? $"Total : {totalItems}, Sélectionnés : {selectedItems}" : (object)$"Total : {totalItems}";
            }
        }

        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            GérerAjoutModification(true);
        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            GérerAjoutModification(false);
        }

        private void GérerAjoutModification(bool estAjout)
        {
            // Logique pour ajouter ou modifier un élément dans la liste
            if (TableSelector.SelectedItem != null)
            {
                string selectedTable = ((ComboBoxItem)TableSelector.SelectedItem).Content.ToString();
                string tableName = selectedTable;

                // Vérifier si le nom se termine par "s", si oui, le supprimer
                if (selectedTable.EndsWith("s"))
                {
                    selectedTable = selectedTable.Remove(selectedTable.Length - 1);
                }

                // Construire le chemin complet de la classe dans LogicLayer
                string logicLayerNamespace = "LogicLayer." + selectedTable + "." + selectedTable;
                Type logicLayerType = Type.GetType(logicLayerNamespace + ", LogicLayer");

                if (logicLayerType != null)
                {
                    if (!estAjout && DataGridItems.SelectedItem == null)
                    {
                        // Si aucune ligne n'est sélectionnée pour la modification
                        MessageBox.Show("Veuillez sélectionner un élément à modifier.", tableName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    // Créer une fenêtre dynamique pour ajouter/modifier un élément
                    dynamic addWindow = new Window();
                    addWindow.Title = estAjout ? "Ajouter " + selectedTable : "Modifier " + selectedTable;
                    addWindow.Width = Width / 2;
                    addWindow.SizeToContent = SizeToContent.Height;
                    addWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    addWindow.ResizeMode = ResizeMode.NoResize;
                    addWindow.Background = Background;
                    addWindow.Icon = Icon;

                    // Créer un Grid pour contenir les contrôles
                    Grid grid = new Grid();
                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = Color.FromRgb(240, 240, 240);
                    brush.Opacity = 0.85;
                    grid.Background = brush;

                    // Créer un StackPanel pour contenir les contrôles
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Vertical;
                    stackPanel.Margin = new Thickness(50, 10, 50, 10);

                    // Ajouter le StackPanel au Grid
                    grid.Children.Add(stackPanel);

                    // Récupérer les colonnes de DataGridItems
                    var columns = DataGridItems.Columns;

                    foreach (var column in columns)
                    {
                        // Créer un label pour le nom de la colonne
                        Label label = new Label();
                        label.Content = column.Header.ToString();
                        label.Margin = new Thickness(0, 10, 0, 0);

                        // Créer une TextBox pour l'utilisateur à saisir
                        TextBox textBox = new TextBox();
                        textBox.Name = "TextBox_" + column.Header.ToString(); // Nom unique pour référencer plus tard
                        textBox.Margin = new Thickness(0, 0, 0, 10);

                        // Si la colonne est "Id", pré-remplir avec un numéro incrémenté
                        if (estAjout && column.Header.ToString() == "Id")
                        {
                            // Trouver le prochain numéro d'Id disponible
                            int? nextId = TrouverProchainIdDisponible() as int?;

                            if (nextId.HasValue)
                            {
                                // Pré-remplir le champ "Id" avec le prochain numéro
                                textBox.Text = nextId.Value.ToString();
                                textBox.IsEnabled = false; // Désactiver la modification de l'Id
                            }
                            else
                            {
                                textBox.Text = ""; // Réinitialiser le champ "Id" si aucun ID disponible
                                textBox.IsEnabled = true; // Activer la modification de l'Id
                            }
                        }
                        else if (!estAjout && column.Header.ToString() == "Id")
                        {
                            // Si c'est une modification, remplir le champ "Id" avec la valeur existante
                            dynamic selectedItem = DataGridItems.SelectedItem;
                            textBox.Text = selectedItem.Id.ToString();
                            textBox.IsEnabled = false; // Désactiver la modification de l'Id
                        }
                        else if (!estAjout)
                        {
                            // Si c'est une modification, remplir les champs avec les valeurs existantes
                            dynamic selectedItem = DataGridItems.SelectedItem;
                            object propertyValue = selectedItem.GetType().GetProperty(column.Header.ToString()).GetValue(selectedItem).ToString();
                            textBox.Text = propertyValue.ToString();
                        }

                        // Ajouter label et textBox au StackPanel
                        stackPanel.Children.Add(label);
                        stackPanel.Children.Add(textBox);
                    }

                    // Créer un bouton "Ajouter" ou "Modifier" en fonction de l'opération
                    Button actionButton = new Button();
                    actionButton.Content = estAjout ? "Ajouter" : "Modifier";
                    actionButton.Margin = new Thickness(0, 10, 0, 20);
                    actionButton.Padding = new Thickness(10, 5, 10, 5);
                    actionButton.Click += (obj, args) =>
                    {
                        // Vérifier que tous les champs sont remplis
                        bool allFieldsFilled = true;
                        foreach (var child in stackPanel.Children)
                        {
                            if (child is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
                            {
                                allFieldsFilled = false;
                                break;
                            }
                        }

                        // Si tous les champs sont remplis, procéder à l'ajout/modification
                        if (allFieldsFilled)
                        {
                            // Créer un nouvel objet avec les valeurs des TextBox
                            dynamic newItem = Activator.CreateInstance(logicLayerType);

                            foreach (var child in stackPanel.Children)
                            {
                                if (child is TextBox textBox)
                                {
                                    // Récupérer le nom de la propriété à partir du nom de la TextBox
                                    string propertyName = textBox.Name.Replace("TextBox_", "");
                                    // Trouver la propriété correspondante dans l'objet et définir sa valeur
                                    PropertyInfo property = logicLayerType.GetProperty(propertyName);
                                    if (property != null)
                                    {
                                        property.SetValue(newItem, Convert.ChangeType(textBox.Text, property.PropertyType));
                                    }
                                }
                            }

                            string daoClassName = tableName + "Dao";

                            // Assurez-vous que la classe DAO existe dans le namespace approprié (ici, j'utilise StorageLayer.Dao)
                            Type daoType = Type.GetType("StorageLayer.Dao." + daoClassName + ", StorageLayer");

                            if (daoType != null)
                            {
                                // Crée une instance de la classe DAO
                                dynamic daoInstance = Activator.CreateInstance(daoType);

                                // Vérifie si la classe DAO a une méthode Add ou Update en fonction de estAjout
                                MethodInfo actionMethod = estAjout ? daoType.GetMethod("Add") : daoType.GetMethod("Update");

                                if (actionMethod != null)
                                {
                                    actionMethod.Invoke(daoInstance, new object[] { newItem });

                                    // Rafraîchir l'affichage
                                    AfficherTable(tableName);
                                }
                            }

                            addWindow.Close(); // Fermer la fenêtre une fois l'ajout/modification terminé
                        }
                        else
                        {
                            MessageBox.Show("Veuillez remplir tous les champs.", tableName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    };

                    // Ajouter le bouton "Ajouter" ou "Modifier" au StackPanel
                    stackPanel.Children.Add(actionButton);

                    // Ajouter le StackPanel à la fenêtre
                    addWindow.Content = grid;

                    // Afficher la fenêtre
                    addWindow.ShowDialog();
                }
            }
            else
            {
                // Si aucune table n'est sélectionnée
                MessageBox.Show("Veuillez sélectionner une table.", Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        // Méthode pour trouver le prochain Id disponible
        private object TrouverProchainIdDisponible()
        {
            // Vérifier le type de l'ID
            if (DataGridItems.Items.Count > 0)
            {
                var firstItem = DataGridItems.Items[0];
                dynamic dynamicFirstItem = firstItem;
                object idValue = dynamicFirstItem.Id;

                if (idValue is int)
                {
                    // Si c'est un integer, chercher le prochain ID disponible
                    int maxId = 0;
                    foreach (var item in DataGridItems.Items)
                    {
                        dynamic dynamicItem = item;
                        int id = dynamicItem.Id;

                        if (id > maxId)
                        {
                            maxId = id;
                        }
                    }

                    return maxId + 1;
                }
                else if (idValue is string)
                {
                    // Si c'est une chaîne de caractères, indiquer qu'aucun ID n'est disponible
                    return null;
                }
            }

            // Si aucun élément dans la liste, retourner 1 comme premier ID
            return 1;
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            // Logique pour supprimer les éléments sélectionnés
            if (DataGridItems.SelectedItems != null && DataGridItems.SelectedItems.Count > 0)
            {
                if (TableSelector.SelectedItem != null)
                {
                    string selectedTable = ((ComboBoxItem)TableSelector.SelectedItem).Content.ToString();

                    // Construire le chemin complet de la classe dans StorageLayer
                    string storageLayerNamespace = "StorageLayer.Dao." + selectedTable + "Dao";
                    Type storageLayerType = Type.GetType(storageLayerNamespace + ", StorageLayer");

                    if (storageLayerType != null)
                    {
                        // Créer une instance de la classe de logique correspondante
                        dynamic logicInstance = Activator.CreateInstance(storageLayerType);

                        // Créer une copie temporaire des éléments sélectionnés
                        List<dynamic> selectedItemsCopy = new List<dynamic>(DataGridItems.SelectedItems.Cast<dynamic>());

                        // Parcourir les éléments sélectionnés et les supprimer un par un
                        foreach (dynamic selectedItem in selectedItemsCopy)
                        {
                            // Appeler la méthode de suppression appropriée dans la classe de logique
                            MethodInfo deleteMethod = storageLayerType.GetMethod("Remove"); // La méthode de suppression est généralement nommée "Remove"
                            if (deleteMethod != null)
                            {
                                deleteMethod.Invoke(logicInstance, new object[] { selectedItem });
                            }
                        }

                        // Rafraîchir l'affichage une fois la suppression terminée
                        AfficherTable(selectedTable);
                    }
                }
                else
                {
                    // Si aucune table n'est sélectionnée
                    MessageBox.Show("Veuillez sélectionner une table.", Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                // Si aucun élément n'est sélectionné
                MessageBox.Show("Veuillez sélectionner un ou plusieurs éléments à supprimer.", Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateInfoLabel();
        }
    }
}
