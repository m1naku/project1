using System;
using LibraryApp.Models;
using Xunit;

namespace LibraryApp.Tests;

public class BookTests
{
    [Fact]
    public void IsStale_UnreadAndOld_ReturnsTrue()
    {
        var book = new Book { IsRead = false, DateAdded = DateTime.Now.AddDays(-100) };
        Assert.True(book.IsStale(90));
    }

    [Fact]
    public void IsStale_Read_ReturnsFalse()
    {
        var book = new Book { IsRead = true, DateAdded = DateTime.Now.AddDays(-100) };
        Assert.False(book.IsStale(90));
    }

    [Fact]
    public void IsStale_RecentlyAdded_ReturnsFalse()
    {
        var book = new Book { IsRead = false, DateAdded = DateTime.Now.AddDays(-10) };
        Assert.False(book.IsStale(90));
    }

    [Fact]
    public void Clone_CopiesAllFieldsIncludingId()
    {
        var book = new Book
        {
            Title = "Название", Author = "Автор", Genre = "Роман",
            Year = 2000, Pages = 100, IsRead = true, Rating = 4
        };

        var clone = book.Clone();

        Assert.Equal(book.Id, clone.Id);
        Assert.Equal(book.Title, clone.Title);
        Assert.Equal(book.Author, clone.Author);
        Assert.Equal(book.Genre, clone.Genre);
        Assert.Equal(book.Year, clone.Year);
        Assert.Equal(book.Pages, clone.Pages);
        Assert.Equal(book.IsRead, clone.IsRead);
        Assert.Equal(book.Rating, clone.Rating);
        Assert.Equal(book.DateAdded, clone.DateAdded);
    }

    [Fact]
    public void CopyFrom_CopiesDataButKeepsId()
    {
        var target = new Book { Title = "Старое" };
        var originalId = target.Id;

        var source = new Book
        {
            Title = "Новое", Author = "Автор", Genre = "Наука",
            Year = 1999, Pages = 200, IsRead = true, Rating = 5
        };

        target.CopyFrom(source);

        Assert.Equal("Новое", target.Title);
        Assert.Equal("Автор", target.Author);
        Assert.Equal("Наука", target.Genre);
        Assert.Equal(1999, target.Year);
        Assert.Equal(200, target.Pages);
        Assert.True(target.IsRead);
        Assert.Equal(5, target.Rating);
        Assert.Equal(originalId, target.Id);
    }

    [Fact]
    public void Genres_All_IsNotEmpty()
    {
        Assert.NotEmpty(Genres.All);
    }
}
