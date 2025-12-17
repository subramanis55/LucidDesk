using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace LucidDesk.FeaturesFuntionMangerClasses
{
    public class ColorReductionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                Color originalColor = brush.Color;
                byte newR = (byte)Math.Max(0, originalColor.R - 15);
                byte newG = (byte)Math.Max(0, originalColor.G - 15);
                byte newB = (byte)Math.Max(0, originalColor.B - 15);

                return new SolidColorBrush( Color.FromArgb(originalColor.A, newR, newG, newB));
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ColorIncreaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                Color originalColor = brush.Color;
                byte newR = (byte)Math.Max(0, originalColor.R + 15);
                byte newG = (byte)Math.Max(0, originalColor.G + 15);
                byte newB = (byte)Math.Max(0, originalColor.B + 15);
                return new SolidColorBrush( Color.FromArgb(originalColor.A, newR, newG, newB));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
