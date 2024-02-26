using System.Globalization;
using System.Windows.Data;

namespace PoliceReport.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        private static readonly DateTimeToStringConverter _instance = new DateTimeToStringConverter();

        public static DateTimeToStringConverter Instance
        {
            get { return _instance; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DateTime dateTime ? dateTime.ToString("dd/MM/yyyy HH:mm") : (object)"Toujours en service";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
