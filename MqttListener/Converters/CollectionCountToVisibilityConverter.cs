using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Visibility = System.Windows.Visibility;

namespace MqttListener.Converters
{
    public class CollectionCountToVisibilityConverter : IValueConverter
    {
        private Dictionary<CollectionCountKind, Func<int, int, bool>> _dictKindFuncs = new Dictionary<CollectionCountKind, Func<int, int, bool>>()
        {
            {CollectionCountKind.Less, (count, dest) => count < dest},
            {CollectionCountKind.LessOrEqual, (count, dest) => count <= dest},
            {CollectionCountKind.Equal, (count, dest) => count == dest},
            {CollectionCountKind.Great, (count, dest) => count > dest},
            {CollectionCountKind.GreatOrEqual, (count, dest) => count >= dest},
        };

        public enum CollectionCountKind
        {
            Less,
            LessOrEqual,
            Equal,
            Great,
            GreatOrEqual
        }

        public int Count { get; set; }

        public bool IsInverted { get; set; }
        public CollectionCountKind Kind { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as ICollection;
            if (collection == null)
                return Result(false);

            if (_dictKindFuncs.TryGetValue(Kind, out Func<int, int, bool> _func))
            {
                return Result(_func(collection.Count, Count));
            }

            return Result(false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private Visibility Result(bool result)
        {
            if (IsInverted)
            {
                return result ? Visibility.Collapsed : Visibility.Visible;
            }

            return result ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}