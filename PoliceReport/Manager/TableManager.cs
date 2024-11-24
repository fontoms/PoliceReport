using PoliceReport.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using PoliceReport.Database.Dao;
using Microsoft.Extensions.DependencyInjection;

namespace PoliceReport.Manager
{
    public class TableManager : ITableManager
    {
        private readonly IDatabaseConnection _database;

        public TableManager(IDatabaseConnection database)
        {
            _database = database;
        }

        public void LoadTables(ComboBox tableSelector)
        {
            List<string> tables = _database.GetTables();
            foreach (string table in tables)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = table
                };
                tableSelector.Items.Add(item);
            }
        }

        public void DisplayTable(string tableName, DataGrid dataGridItems, IServiceProvider serviceProvider)
        {
            dataGridItems.Columns.Clear();
            string daoInterfaceName = "I" + tableName + "Dao";
            Type daoType = Type.GetType($"PoliceReport.Core.{tableName}.{daoInterfaceName}, PoliceReport.Core");

            if (daoType != null)
            {
                dynamic daoInstance = serviceProvider.GetService(daoType);
                MethodInfo getAllRowsMethod = daoType.GetMethod("GetAll");

                if (getAllRowsMethod != null)
                {
                    dynamic rows = getAllRowsMethod.Invoke(daoInstance, null);
                    List<(string Name, Type Type)> columns = _database.GetColumnsOfTable(tableName);

                    if (columns.Count > 0)
                    {
                        foreach (var column in columns)
                        {
                            DataGridTextColumn textColumn = new DataGridTextColumn
                            {
                                Header = column.Name,
                                Binding = new Binding(column.Name),
                                ElementStyle = Application.Current.Resources["WrapCellStyle"] as Style,
                                Width = column.Type == typeof(int) ? new DataGridLength(1, DataGridLengthUnitType.Auto) : new DataGridLength(1, DataGridLengthUnitType.Star)
                            };
                            dataGridItems.Columns.Add(textColumn);
                        }
                    }

                    dataGridItems.ItemsSource = rows;

                    if (tableName.Equals("Utilisateur", StringComparison.OrdinalIgnoreCase) && dataGridItems.Items.Count > 0)
                    {
                        dataGridItems.UpdateLayout();
                        var firstRow = dataGridItems.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                        if (firstRow != null)
                        {
                            firstRow.IsEnabled = false;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Erreur : Impossible de récupérer les données de la table.", daoInterfaceName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Erreur : Impossible de récupérer les données de la table.", tableName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
