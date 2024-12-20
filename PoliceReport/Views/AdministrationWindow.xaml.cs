﻿using Microsoft.Extensions.DependencyInjection;
using PoliceReport.Core.Role;
using PoliceReport.Core.Tools.Encryption;
using PoliceReport.Core.Utilisateur;
using PoliceReport.Database;
using PoliceReport.Manager;
using PoliceReport.Views;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PoliceReport
{
    public partial class AdministrationWindow : Window
    {
        private readonly ITableManager _tableManager;
        private readonly IDatabaseConnection _database;
        private Utilisateur _user;
        private readonly IServiceProvider _serviceProvider;

        public Utilisateur User
        {
            get => _user;
            set
            {
                if (value != _user)
                {
                    _user = value;
                    SetRoles(_user);
                }
            }
        }

        public AdministrationWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _database = serviceProvider.GetRequiredService<IDatabaseConnection>();
            _tableManager = serviceProvider.GetRequiredService<ITableManager>();
            _tableManager.LoadTables(tableSelector);
            LoadSettings();
        }

        private void SetRoles(Utilisateur? utilisateur)
        {
            // Modifie les accès suivant le rôle
            switch (utilisateur.Role)
            {
                case (int)Role.Administrateur:
                    break;
                case (int)Role.Editeur:
                    deleteBtn.IsEnabled = false;
                    break;
                case (int)Role.Lecteur:
                    chkIsDisplayList.IsEnabled = false;
                    ajoutBtn.IsEnabled = false;
                    deleteBtn.IsEnabled = false;
                    editBtn.IsEnabled = false;
                    dataGridItems.IsReadOnly = true;
                    dataGridItems.IsHitTestVisible = true;
                    dataGridItems.MouseDoubleClick -= DataGridItems_MouseDoubleClick;
                    break;
            }
        }

        private void LoadSettings()
        {
            chkIsDisplayList.IsChecked = Settings.Default.VehDisplayList;
        }

        private void TableSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tableSelector.SelectedItem != null)
            {
                string selectedTable = ((ComboBoxItem)tableSelector.SelectedItem).Content.ToString();
                _tableManager.DisplayTable(selectedTable, dataGridItems, _serviceProvider);

                if (selectedTable.Equals("Utilisateur", StringComparison.OrdinalIgnoreCase) && dataGridItems.Items.Count > 0)
                {
                    dataGridItems.UpdateLayout();

                    DataGridRow firstRow = dataGridItems.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                    if (firstRow != null)
                    {
                        firstRow.IsEnabled = false;
                    }

                    Utilisateur ownerUser = dataGridItems.Items[0] as Utilisateur;
                    bool isOwner = User.Id == ownerUser.Id;

                    foreach (var item in dataGridItems.Items)
                    {
                        Utilisateur utilisateur = (Utilisateur)item;
                        if (!isOwner && utilisateur.Role <= User.Role)
                        {
                            DataGridRow row = (DataGridRow)dataGridItems.ItemContainerGenerator.ContainerFromItem(item);
                            if (row != null)
                            {
                                row.IsEnabled = false;
                            }
                        }
                    }
                }
            }
        }

        private void DataGridItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataGridItems.SelectedItem != null)
            {
                GérerAjoutModification(false);
            }
        }

        private void UpdateInfoLabel()
        {
            if (dataGridItems.Items != null)
            {
                int totalItems = dataGridItems.Items.Count;
                int selectedItems = dataGridItems.SelectedItems.Count;

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
            if (tableSelector.SelectedItem != null)
            {
                string selectedTable = ((ComboBoxItem)tableSelector.SelectedItem).Content.ToString();
                string tableName = selectedTable;

                string coreNamespace = "PoliceReport.Core." + selectedTable + "." + selectedTable;
                Type coreType = Type.GetType(coreNamespace + ", PoliceReport.Core");

                if (coreType != null)
                {
                    if (!estAjout && dataGridItems.SelectedItem == null)
                    {
                        MessageBox.Show("Veuillez sélectionner un élément à modifier.", tableName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    Window addWindow = new Window();
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

                    var columns = dataGridItems.Columns;

                    foreach (var column in columns)
                    {
                        Label label = new Label();
                        label.Content = column.Header.ToString();
                        label.Margin = new Thickness(0, 10, 0, 0);

                        string tableForeignKey = CheckIfForeignKey(column.Header.ToString());
                        List<object> tableEnumKey = CheckIfEnum(column.Header.ToString());

                        if (tableForeignKey != null || tableEnumKey.Count > 0)
                        {
                            var selectedItem = estAjout ? null : dataGridItems.SelectedItem;
                            object[] args = Array.Empty<object>();

                            if (selectedItem != null)
                            {
                                var properties = selectedItem.GetType().GetProperties();
                                args = properties.Select(prop => prop.GetValue(selectedItem)).ToArray();
                            }

                            dynamic classe = Activator.CreateInstance(coreType, args);
                            PropertyInfo selectedProperty = classe.GetType().GetProperty(column.Header.ToString());
                            object value = selectedProperty.GetValue(classe);

                            dynamic items = null;
                            if (tableForeignKey != null) items = GetForeignKeyData(tableForeignKey);
                            if (tableEnumKey.Count > 0) items = tableEnumKey;

                            ComboBox comboBox = new ComboBox();
                            comboBox.Name = "ComboBox_" + column.Header.ToString();
                            comboBox.Margin = new Thickness(0, 0, 0, 10);
                            comboBox.DisplayMemberPath = "Nom";
                            comboBox.ItemsSource = items;

                            if (comboBox.Items.Count > 0)
                            {
                                comboBox.SelectedIndex = estAjout ? (int)value : (int)value - 1;
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
                            if (column.Header.ToString() == "Password")
                            {
                                PasswordBox passwordBox = new PasswordBox();
                                passwordBox.Name = "PasswordBox_" + column.Header.ToString();
                                passwordBox.Margin = new Thickness(0, 0, 0, 10);

                                if (!estAjout)
                                {
                                    dynamic selectedItem = dataGridItems.SelectedItem;
                                    passwordBox.Password = selectedItem.Password;
                                }

                                stackPanel.Children.Add(label);
                                stackPanel.Children.Add(passwordBox);
                            }
                            else
                            {
                                TextBox textBox = new TextBox();
                                textBox.Name = "TextBox_" + column.Header.ToString();
                                textBox.Margin = new Thickness(0, 0, 0, 10);

                                if (estAjout && column.Header.ToString() == "Id")
                                {
                                    int? nextId = FindNextIdAvailable() as int?;
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
                                    dynamic selectedItem = dataGridItems.SelectedItem;
                                    textBox.Text = selectedItem.Id.ToString();
                                    textBox.IsEnabled = false;
                                }
                                else if (!estAjout)
                                {
                                    dynamic selectedItem = dataGridItems.SelectedItem;
                                    object propertyValue = selectedItem.GetType().GetProperty(column.Header.ToString()).GetValue(selectedItem).ToString();
                                    textBox.Text = propertyValue.ToString();
                                }

                                stackPanel.Children.Add(label);
                                stackPanel.Children.Add(textBox);
                            }
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
                            dynamic newItem = Activator.CreateInstance(coreType);

                            foreach (var child in stackPanel.Children)
                            {
                                if (child is TextBox textBox)
                                {
                                    string propertyName = textBox.Name.Replace("TextBox_", "");
                                    PropertyInfo property = coreType.GetProperty(propertyName);
                                    if (property != null)
                                    {
                                        property.SetValue(newItem, Convert.ChangeType(textBox.Text, property.PropertyType));
                                    }
                                }
                                else if (child is PasswordBox passwordBox)
                                {
                                    string propertyName = passwordBox.Name.Replace("PasswordBox_", "");
                                    PropertyInfo property = coreType.GetProperty(propertyName);
                                    if (property != null)
                                    {
                                        passwordBox.Password = HashHelper.CalculateSHA256(passwordBox.Password);
                                        property.SetValue(newItem, Convert.ChangeType(passwordBox.Password, property.PropertyType));
                                    }
                                }
                                else if (child is ComboBox comboBox)
                                {
                                    string propertyName = comboBox.Name.Replace("ComboBox_", "");
                                    PropertyInfo property = coreType.GetProperty(propertyName);
                                    if (property != null)
                                    {
                                        dynamic classe = comboBox.SelectedItem;
                                        PropertyInfo selectedProperty = classe.GetType().GetProperty("Id");
                                        object value = selectedProperty.GetValue(classe);

                                        property.SetValue(newItem, Convert.ChangeType(value, property.PropertyType));
                                    }
                                }
                            }

                            string daoInterfaceName = $"I{tableName}Dao";
                            Type daoType = Type.GetType($"PoliceReport.Core.{tableName}.{daoInterfaceName}, PoliceReport.Core");

                            if (daoType != null)
                            {
                                // Utilisation du service provider pour obtenir l'instance du DAO
                                dynamic daoInstance = _serviceProvider.GetService(daoType);
                                MethodInfo actionMethod = estAjout ? daoType.GetMethod("Add") : daoType.GetMethod("Update", [newItem.GetType()]);

                                if (actionMethod != null)
                                {
                                    actionMethod.Invoke(daoInstance, new object[] { newItem });
                                    _tableManager.DisplayTable(tableName, dataGridItems, _serviceProvider);
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
            List<string> tables = _database.GetTables();
            foreach (string table in tables)
            {
                if (searchString.Contains(table, StringComparison.Ordinal))
                {
                    return table;
                }
            }
            return null;
        }

        // Méthode pour vérifier si une colonne possède une énumération
        private List<dynamic> CheckIfEnum(string columnName)
        {
            Type enumType = Type.GetType($"PoliceReport.Core.{columnName}.{columnName}, PoliceReport.Core");
            if (enumType != null && enumType.IsEnum)
            {
                string[] names = Enum.GetNames(enumType);
                Array values = Enum.GetValues(enumType);

                List<dynamic> enumList = new List<dynamic>();

                for (int i = 0; i < names.Length; i++)
                {
                    dynamic enumItem = new
                    {
                        Id = (int)values.GetValue(i),
                        Nom = names[i]
                    };
                    enumList.Add(enumItem);
                }
                return enumList;
            }
            return new List<dynamic>();
        }

        // Méthode pour récupérer les données de la table correspondante
        private dynamic GetForeignKeyData(string tableName)
        {
            string daoInterfaceName = $"I{tableName}Dao";
            Type daoType = Type.GetType($"PoliceReport.Core.{tableName}.{daoInterfaceName}, PoliceReport.Core");

            if (daoType != null)
            {
                // Utilisation du service provider pour obtenir l'instance du DAO
                dynamic daoInstance = _serviceProvider.GetService(daoType);

                // Vérifie si la classe DAO a une méthode GetAll()
                MethodInfo getAllRowsMethod = daoType.GetMethod("GetAll");

                if (getAllRowsMethod != null)
                {
                    dynamic rows = getAllRowsMethod.Invoke(daoInstance, null);
                    return rows;
                }
                else
                {
                    MessageBox.Show("Erreur : Impossible de récupérer les données de la table.", daoInterfaceName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return null;
        }

        // Méthode pour trouver le prochain Id disponible
        private object FindNextIdAvailable()
        {
            // Vérifier le type de l'ID
            if (dataGridItems.Items.Count > 0)
            {
                var firstItem = dataGridItems.Items[0];
                dynamic dynamicFirstItem = firstItem;
                object idValue = dynamicFirstItem.Id;

                if (idValue is int)
                {
                    // Si c'est un integer, chercher le prochain ID disponible
                    int maxId = 0;
                    foreach (var item in dataGridItems.Items)
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
            if (dataGridItems.SelectedItems != null && dataGridItems.SelectedItems.Count > 0)
            {
                if (tableSelector.SelectedItem != null)
                {
                    string selectedTable = ((ComboBoxItem)tableSelector.SelectedItem).Content.ToString();
                    string daoInterfaceName = $"I{selectedTable}Dao";
                    Type daoType = Type.GetType($"PoliceReport.Core.{selectedTable}.{daoInterfaceName}, PoliceReport.Core");

                    if (daoType != null)
                    {
                        // Utilisation du service provider pour obtenir l'instance du DAO
                        dynamic daoInstance = _serviceProvider.GetService(daoType);
                        List<dynamic> selectedItemsCopy = new List<dynamic>(dataGridItems.SelectedItems.Cast<dynamic>());

                        foreach (dynamic selectedItem in selectedItemsCopy)
                        {
                            MethodInfo deleteMethod = daoType.GetMethod("Remove");
                            deleteMethod?.Invoke(daoInstance, new object[] { selectedItem });
                        }

                        _tableManager.DisplayTable(selectedTable, dataGridItems, _serviceProvider);
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner une table.", Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un ou plusieurs éléments à supprimer.", Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DataGridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateInfoLabel();
        }

        private void ChkIsDisplayList_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.VehDisplayList = chkIsDisplayList.IsChecked ?? false;
            Settings.Default.Save();
        }
    }
}
