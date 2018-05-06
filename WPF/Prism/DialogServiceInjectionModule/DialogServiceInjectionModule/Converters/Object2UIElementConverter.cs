using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DialogServiceInjectionModule
{
    public class Object2UIElementConverter : IValueConverter
    {
        /// <summary>
        /// IEnumerable<Object>からIEnumerable<UIElement>への変換
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is null)
                return null;

            var src = value as IEnumerable<object>;
            if (src is null)
                return null;

            return src.OfType<UIElement>();
        }

        /// <summary>
        /// IEnumerable<UIElement>からIEnumerable<Object>への変換
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // oneway想定なので例外を投げる
            throw new NotImplementedException();
        }
    }
}
