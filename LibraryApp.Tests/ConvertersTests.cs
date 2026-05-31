using System.Globalization;
using LibraryApp.Converters;
using Xunit;

namespace LibraryApp.Tests;

public class ConvertersTests
{
    [Theory]
    [InlineData(0, "☆☆☆☆☆")]
    [InlineData(3, "★★★☆☆")]
    [InlineData(5, "★★★★★")]
    [InlineData(7, "☆☆☆☆☆")] // вне диапазона 0–5
    public void RatingToStars_Converts(int rating, string expected)
    {
        var conv = new RatingToStarsConverter();
        var result = conv.Convert(rating, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(true, "Да")]
    [InlineData(false, "Нет")]
    public void BoolToYesNo_Converts(bool value, string expected)
    {
        var conv = new BoolToYesNoConverter();
        var result = conv.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }
}
