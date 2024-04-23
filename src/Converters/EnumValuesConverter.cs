using Avalonia.Data.Converters;
using System.Globalization;

namespace NxEditor.EpdPlugin.Converters;

public class EnumValuesConverter : IValueConverter
{
    public static EnumValuesConverter Shared { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value?.GetType() is Type type && type.IsEnum) {
            return Enum.GetValues(type);
        }

        return Array.Empty<string>();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException("""
            Cannot convert values to a constant enum.
            """);
    }
}
