using System.Collections.Generic;
using LibraryApp.Models;
using LibraryApp.ViewModels;
using Xunit;

namespace LibraryApp.Tests;

public class LibraryStatsViewModelTests
{
    private static List<Book> Sample() => new()
    {
        new Book { Title = "A", Author = "Автор1", Genre = "Роман", Pages = 100, IsRead = true, Rating = 4 },
        new Book { Title = "B", Author = "Автор1", Genre = "Роман", Pages = 200, IsRead = true, Rating = 2 },
        new Book { Title = "C", Author = "Автор2", Genre = "Наука", Pages = 300, IsRead = false, Rating = 0 },
    };

    [Fact]
    public void Update_ComputesMetrics()
    {
        var vm = new LibraryStatsViewModel();
        vm.Update(Sample());

        Assert.Equal(3, vm.TotalBooks);
        Assert.Equal(2, vm.ReadBooks);
        Assert.Equal(600, vm.TotalPages);
        Assert.Equal(2.0 / 3 * 100, vm.ReadPercentage, 3);
        Assert.Equal(3.0, vm.AverageRating, 3); // (4 + 2) / 2; оценка 0 не учитывается
        Assert.StartsWith("Автор1", vm.TopAuthor);
        Assert.StartsWith("Роман", vm.TopGenre);
        Assert.Equal(2, vm.GenreSeries.Count); // Роман и Наука
    }

    [Fact]
    public void Update_EmptyList_ResetsMetrics()
    {
        var vm = new LibraryStatsViewModel();
        vm.Update(new List<Book>());

        Assert.Equal(0, vm.TotalBooks);
        Assert.Equal(0, vm.ReadBooks);
        Assert.Equal(0, vm.ReadPercentage);
        Assert.Equal(0, vm.AverageRating);
        Assert.Equal(0, vm.TotalPages);
        Assert.Equal("—", vm.TopAuthor);
        Assert.Equal("—", vm.TopGenre);
        Assert.Empty(vm.GenreSeries);
    }
}
