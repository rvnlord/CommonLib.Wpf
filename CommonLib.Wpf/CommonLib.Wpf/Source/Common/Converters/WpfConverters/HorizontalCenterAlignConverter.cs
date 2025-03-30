using System;
using System.Globalization;
using System.Windows.Data;

namespace CommonLib.Wpf.Source.Common.Converters.WpfConverters;

public class HorizontalCenterAlignConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is double elementToCenter && values[1] is double elementToCenterAgainst)
            return (elementToCenterAgainst - elementToCenter) / 2;
        return 0;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}