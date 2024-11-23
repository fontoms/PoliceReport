using PoliceReport.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PoliceReport.Manager
{
    public interface ITableManager
    {
        void LoadTables(ComboBox tableSelector);
        void DisplayTable(string tableName, DataGrid dataGridItems, IServiceProvider serviceProvider);
    }
}
