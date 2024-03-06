using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Nice3point.Revit.AddIn.Views.Converters;

public class InverseBoolConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value!;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}