using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LibraryApp.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LibraryApp.ViewModels;

/// <summary>
/// Агрегированные метрики библиотеки и серия данных для круговой диаграммы жанров.
/// </summary>
public partial class LibraryStatsViewModel : ObservableObject
{
    [ObservableProperty] private int _totalBooks;
    [ObservableProperty] private int _readBooks;
    [ObservableProperty] private double _readPercentage;
    [ObservableProperty] private double _averageRating;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private string _topAuthor = "—";
    [ObservableProperty] private string _topGenre = "—";

    public ObservableCollection<ISeries> GenreSeries { get; } = new();

    private static readonly SKColor[] Palette =
    {
        SKColor.Parse("#4C8BF5"), SKColor.Parse("#F5A623"), SKColor.Parse("#7ED321"),
        SKColor.Parse("#D0021B"), SKColor.Parse("#9013FE"), SKColor.Parse("#50E3C2"),
        SKColor.Parse("#8B572A"), SKColor.Parse("#999999"), SKColor.Parse("#F8E71C"),
        SKColor.Parse("#BD10E0"), SKColor.Parse("#417505"), SKColor.Parse("#B8E986"),
    };

    public void Update(IEnumerable<Book> books)
    {
        var list = books.ToList();
        TotalBooks = list.Count;
        ReadBooks = list.Count(b => b.IsRead);
        ReadPercentage = TotalBooks == 0 ? 0 : ReadBooks * 100.0 / TotalBooks;
        TotalPages = list.Sum(b => b.Pages);

        var rated = list.Where(b => b.Rating > 0).ToList();
        AverageRating = rated.Count == 0 ? 0 : rated.Average(b => b.Rating);

        TopAuthor = Top(list.GroupBy(b => b.Author));
        TopGenre = Top(list.GroupBy(b => b.Genre));

        BuildGenreSeries(list);
    }

    private static string Top(IEnumerable<IGrouping<string, Book>> groups)
        => groups.OrderByDescending(g => g.Count())
                 .Select(g => $"{g.Key} ({g.Count()})")
                 .FirstOrDefault() ?? "—";

    private void BuildGenreSeries(List<Book> books)
    {
        GenreSeries.Clear();
        var groups = books
            .GroupBy(b => string.IsNullOrWhiteSpace(b.Genre) ? "Без жанра" : b.Genre)
            .OrderByDescending(g => g.Count())
            .ToList();

        var i = 0;
        foreach (var g in groups)
        {
            GenreSeries.Add(new PieSeries<int>
            {
                Values = new[] { g.Count() },
                Name = g.Key,
                Fill = new SolidColorPaint(Palette[i % Palette.Length]),
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 12,
                DataLabelsFormatter = p => $"{p.Coordinate.PrimaryValue}",
            });
            i++;
        }
    }
}
