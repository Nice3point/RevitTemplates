using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Nice3point.Revit.AddIn.Views.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolVisibilityConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool) value! ? Visibility.Visible : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (Visibility) value! == Visibility.Visible;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}

[ValueConversion(typeof(bool), typeof(Visibility))]
public class InverseBoolVisibilityConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool) value! == false ? Visibility.Visible : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (Visibility) value! != Visibility.Visible;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}

[ValueConversion(typeof(bool), typeof(bool))]
public class InverseBooleanConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool) value!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool) value!;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}