using System.Globalization;
using System.Windows.Data;

namespace PoliceReport.Converters
{
    public class DoubleToIntConverter : IValueConverter
    {
        private static DoubleToIntConverter _instance;

        public static DoubleToIntConverter Instance
        {
            get
            {
                _instance ??= new DoubleToIntConverter();
                return _instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is double ? System.Convert.ToInt64((double)value) : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
