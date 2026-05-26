using System;
using System.Globalization;
using System.Windows.Data;

namespace LibraryApp.Converters;

/// <summary>Конвертирует int (0–5) в строку звёздочек "★★★☆☆".</summary>
public class RatingToStarsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        => value is int r && r is >= 0 and <= 5
            ? new string('★', r) + new string('☆', 5 - r)
            : "☆☆☆☆☆";

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Конвертирует bool в "Да"/"Нет".</summary>
public class BoolToYesNoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && b ? "Да" : "Нет";

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
