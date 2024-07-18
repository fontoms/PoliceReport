using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PoliceReport.Converters
{
    public class ZeroToBoldConverter : IValueConverter
    {
        private static ZeroToBoldConverter _instance;

        public static ZeroToBoldConverter Instance
        {
            get
            {
                _instance ??= new ZeroToBoldConverter();
                return _instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
