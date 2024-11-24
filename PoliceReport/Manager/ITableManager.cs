using System.Windows.Controls;

namespace PoliceReport.Manager
{
    public interface ITableManager
    {
        void LoadTables(ComboBox tableSelector);
        void DisplayTable(string tableName, DataGrid dataGridItems, IServiceProvider serviceProvider);
    }
}
