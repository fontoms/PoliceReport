using System.Globalization;
using System.Windows.Data;

namespace PoliceReport.Converters
{
    public class NullToDisabledConverter : IValueConverter
    {
        private static NullToDisabledConverter _instance;

        public static NullToDisabledConverter Instance
        {
            get
            {
                _instance ??= new NullToDisabledConverter();
                return _instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
