using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TowerDefense1
{
    public class MarginConverter : IValueConverter
    {
        public Thickness BaseThickness { get; set; }


        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = System.Convert.ToDouble(value);
            return new Thickness(val * BaseThickness.Left, val * BaseThickness.Top, val * BaseThickness.Right, val * BaseThickness.Bottom);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
