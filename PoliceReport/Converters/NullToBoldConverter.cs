using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PoliceReport.Converters
{
    public class NullToBoldConverter : IValueConverter
    {
        private static NullToBoldConverter _instance;

        public static NullToBoldConverter Instance
        {
            get
            {
                _instance ??= new NullToBoldConverter();
                return _instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
