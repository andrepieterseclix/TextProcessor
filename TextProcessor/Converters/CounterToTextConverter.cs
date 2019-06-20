using System;
using System.Windows.Data;

namespace TextProcessor.Converters
{
    class CounterToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null)
                return value.ToString();
            
            string input = parameter.ToString();
            int counter = (int)value;

            return string.Format("{0} {1}{2}", counter, input, counter != 1 ? "s" : "").TrimEnd();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
