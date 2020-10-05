using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MqttListener.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return IsInverted;

            bool result = (bool)value;
            if (IsInverted)
            {
                return result ? Visibility.Collapsed : Visibility.Visible;
            }

            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}