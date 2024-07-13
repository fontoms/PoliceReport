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

        private static AdministrationWindow? _instance;
        public static AdministrationWindow Instance
        {
            get
            {
                _instance ??= new AdministrationWindow();
                return _instance;
            }
        }

        private AdministrationWindow()
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
                // Récupère l'instance de la classe DAO
                dynamic daoInstance = daoType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);

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
            if (TableSelector.SelectedItem != null)
            {
                string selectedTable = ((ComboBoxItem)TableSelector.SelectedItem).Content.ToString();
                string tableName = selectedTable;

                if (selectedTable.EndsWith("s"))
                {
                    selectedTable = selectedTable.Remove(selectedTable.Length - 1);
                }

                string logicLayerNamespace = "LogicLayer." + selectedTable + "." + selectedTable;
                Type logicLayerType = Type.GetType(logicLayerNamespace + ", LogicLayer");

                if (logicLayerType != null)
                {
                    if (!estAjout && DataGridItems.SelectedItem == null)
                    {
                        MessageBox.Show("Veuillez sélectionner un élément à modifier.", tableName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    dynamic addWindow = new Window();
                    addWindow.Title = estAjout ? "Ajouter " + selectedTable : "Modifier " + selectedTable;
                    addWindow.Width = Width / 2;
                    addWindow.SizeToContent = SizeToContent.Height;
                    addWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    addWindow.ResizeMode = ResizeMode.NoResize;
                    addWindow.Background = Background;
                    addWindow.Icon = Icon;

                    Grid grid = new Grid();
                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = Color.FromRgb(240, 240, 240);
                    brush.Opacity = 0.85;
                    grid.Background = brush;

                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Vertical;
                    stackPanel.Margin = new Thickness(50, 10, 50, 10);
                    grid.Children.Add(stackPanel);

                    var columns = DataGridItems.Columns;

                    foreach (var column in columns)
                    {
                        Label label = new Label();
                        label.Content = column.Header.ToString();
                        label.Margin = new Thickness(0, 10, 0, 0);

                        string tableForeignKey = CheckIfForeignKey(column.Header.ToString());

                        if (tableForeignKey != null)
                        {
                            var selectedItem = estAjout ? null : DataGridItems.SelectedItem;
                            object[] args = Array.Empty<object>();

                            if (selectedItem != null)
                            {
                                var properties = selectedItem.GetType().GetProperties();
                                args = properties.Select(prop =>  prop.GetValue(selectedItem)).ToArray();
                            }

                            dynamic classe = Activator.CreateInstance(logicLayerType, args);
                            PropertyInfo selectedProperty = classe.GetType().GetProperty(column.Header.ToString());
                            object value = selectedProperty.GetValue(classe);

                            ComboBox comboBox = new ComboBox();
                            comboBox.Name = "ComboBox_" + column.Header.ToString();
                            comboBox.Margin = new Thickness(0, 0, 0, 10);
                            comboBox.DisplayMemberPath = "Nom";
                            comboBox.SelectedValuePath = "Type";
                            comboBox.ItemsSource = GetForeignKeyData(tableForeignKey);

                            if (comboBox.Items.Count > 0)
                            {
                                comboBox.SelectedIndex = 0;
                                if (value != null) comboBox.SelectedValue = value;
                            }
                            else
                            {
                                comboBox.IsEnabled = false;
                            }

                            stackPanel.Children.Add(label);
                            stackPanel.Children.Add(comboBox);
                        }
                        else
                        {
                            TextBox textBox = new TextBox();
                            textBox.Name = "TextBox_" + column.Header.ToString();
                            textBox.Margin = new Thickness(0, 0, 0, 10);

                            if (estAjout && column.Header.ToString() == "Id")
                            {
                                int? nextId = TrouverProchainIdDisponible() as int?;
                                if (nextId.HasValue)
                                {
                                    textBox.Text = nextId.Value.ToString();
                                    textBox.IsEnabled = false;
                                }
                                else
                                {
                                    textBox.Text = "";
                                    textBox.IsEnabled = true;
                                }
                            }
                            else if (!estAjout && column.Header.ToString() == "Id")
                            {
                                dynamic selectedItem = DataGridItems.SelectedItem;
                                textBox.Text = selectedItem.Id.ToString();
                                textBox.IsEnabled = false;
                            }
                            else if (!estAjout)
                            {
                                dynamic selectedItem = DataGridItems.SelectedItem;
                                object propertyValue = selectedItem.GetType().GetProperty(column.Header.ToString()).GetValue(selectedItem).ToString();
                                textBox.Text = propertyValue.ToString();
                            }

                            stackPanel.Children.Add(label);
                            stackPanel.Children.Add(textBox);
                        }
                    }

                    Button actionButton = new Button();
                    actionButton.Content = estAjout ? "Ajouter" : "Modifier";
                    actionButton.Margin = new Thickness(0, 10, 0, 20);
                    actionButton.Padding = new Thickness(10, 5, 10, 5);
                    actionButton.Click += (obj, args) =>
                    {
                        bool allFieldsFilled = true;
                        foreach (var child in stackPanel.Children)
                        {
                            if (child is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
                            {
                                allFieldsFilled = false;
                                break;
                            }
                            else if (child is ComboBox comboBox && comboBox.SelectedItem == null)
                            {
                                allFieldsFilled = false;
                                break;
                            }
                        }

                        if (allFieldsFilled)
                        {
                            dynamic newItem = Activator.CreateInstance(logicLayerType);

                            foreach (var child in stackPanel.Children)
                            {
                                if (child is TextBox textBox)
                                {
                                    string propertyName = textBox.Name.Replace("TextBox_", "");
                                    PropertyInfo property = logicLayerType.GetProperty(propertyName);
                                    if (property != null)
                                    {
                                        property.SetValue(newItem, Convert.ChangeType(textBox.Text, property.PropertyType));
                                    }
                                }
                                else if (child is ComboBox comboBox)
                                {
                                    string propertyName = comboBox.Name.Replace("ComboBox_", "");
                                    PropertyInfo property = logicLayerType.GetProperty(propertyName);
                                    if (property != null)
                                    {
                                        dynamic classe = comboBox.SelectedItem;
                                        PropertyInfo selectedProperty = classe.GetType().GetProperty("Type");
                                        object value = selectedProperty.GetValue(classe);


                                        property.SetValue(newItem, Convert.ChangeType(value, property.PropertyType));
                                    }
                                }
                            }

                            string daoClassName = tableName + "Dao";
                            Type daoType = Type.GetType("StorageLayer.Dao." + daoClassName + ", StorageLayer");

                            if (daoType != null)
                            {
                                dynamic daoInstance = daoType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
                                MethodInfo actionMethod = estAjout ? daoType.GetMethod("Add") : daoType.GetMethod("Update", [newItem.GetType()]);

                                if (actionMethod != null)
                                {
                                    actionMethod.Invoke(daoInstance, new object[] { newItem });
                                    AfficherTable(tableName);
                                }
                            }

                            addWindow.Close();
                        }
                        else
                        {
                            MessageBox.Show("Veuillez remplir tous les champs.", tableName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    };

                    stackPanel.Children.Add(actionButton);
                    addWindow.Content = grid;
                    addWindow.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une table.", Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        // Méthode pour vérifier si une colonne correspond à une table de la base de données
        private string CheckIfForeignKey(string columnName)
        {
            string searchString = columnName + "s";
            List<string> tables = Database.GetTables();
            foreach (string table in tables)
            {
                if (searchString.Contains(table, StringComparison.Ordinal))
                {
                    return table;
                }
            }
            return null;
        }


        // Méthode pour récupérer les données de la table correspondante
        private dynamic GetForeignKeyData(string tableName)
        {
            string daoClassName = tableName + "Dao";

            Type daoType = Type.GetType("StorageLayer.Dao." + daoClassName + ", StorageLayer");

            if (daoType != null)
            {
                dynamic daoInstance = daoType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);

                // Vérifie si la classe DAO a une méthode GetAll()
                MethodInfo getAllRowsMethod = daoType.GetMethod("GetAll");

                if (getAllRowsMethod != null)
                {
                    dynamic rows = getAllRowsMethod.Invoke(daoInstance, null);
                    return rows;
                }
                else
                {
                    MessageBox.Show("Erreur : Impossible de récupérer les données de la table.", daoClassName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return null;
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
            if (DataGridItems.SelectedItems != null && DataGridItems.SelectedItems.Count > 0)
            {
                if (TableSelector.SelectedItem != null)
                {
                    string selectedTable = ((ComboBoxItem)TableSelector.SelectedItem).Content.ToString();

                    // Construire le chemin complet de la classe dans StorageLayer
                    string daoClassName = "StorageLayer.Dao." + selectedTable + "Dao";
                    Type daoType = Type.GetType(daoClassName + ", StorageLayer");

                    if (daoType != null)
                    {
                        dynamic daoInstance = daoType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);

                        List<dynamic> selectedItemsCopy = new List<dynamic>(DataGridItems.SelectedItems.Cast<dynamic>());

                        foreach (dynamic selectedItem in selectedItemsCopy)
                        {
                            MethodInfo deleteMethod = daoType.GetMethod("Remove");
                            if (deleteMethod != null)
                            {
                                deleteMethod.Invoke(daoInstance, new object[] { selectedItem });
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
