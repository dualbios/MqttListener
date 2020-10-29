using System;
using System.Globalization;
using System.Windows.Data;
using Windows.Web.UI;
using MqttListener.Core;

namespace MqttListener.Converters
{
    public class ViewTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is ViewType))
                return false;
            if (!(value is ViewType))
                return false;

            ViewType vt = (ViewType) parameter;
            ViewType v = (ViewType)value;

            return v == vt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? parameter : null;
        }
    }
}