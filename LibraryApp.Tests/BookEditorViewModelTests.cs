using LibraryApp.Models;
using LibraryApp.ViewModels;
using Xunit;

namespace LibraryApp.Tests;

public class BookEditorViewModelTests
{
    [Fact]
    public void Save_EmptyTitle_SetsError_NotConfirmed()
    {
        var vm = new BookEditorViewModel { Title = "", Author = "Автор" };
        var closed = false;
        vm.RequestClose += () => closed = true;

        vm.SaveCommand.Execute(null);

        Assert.False(vm.IsConfirmed);
        Assert.False(closed);
        Assert.False(string.IsNullOrEmpty(vm.ErrorMessage));
    }

    [Fact]
    public void Save_EmptyAuthor_Fails()
    {
        var vm = new BookEditorViewModel { Title = "Название", Author = "" };
        vm.SaveCommand.Execute(null);
        Assert.False(vm.IsConfirmed);
    }

    [Fact]
    public void Save_ValidInput_ConfirmsAndCloses()
    {
        var vm = new BookEditorViewModel
        {
            Title = "Название", Author = "Автор", Year = 2000, Pages = 100, Rating = 3
        };
        var closed = false;
        vm.RequestClose += () => closed = true;

        vm.SaveCommand.Execute(null);

        Assert.True(vm.IsConfirmed);
        Assert.True(closed);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(99999)]
    public void Save_InvalidYear_Fails(int year)
    {
        var vm = new BookEditorViewModel { Title = "T", Author = "A", Year = year };
        vm.SaveCommand.Execute(null);
        Assert.False(vm.IsConfirmed);
    }

    [Fact]
    public void Save_NegativePages_Fails()
    {
        var vm = new BookEditorViewModel { Title = "T", Author = "A", Pages = -5 };
        vm.SaveCommand.Execute(null);
        Assert.False(vm.IsConfirmed);
    }

    [Fact]
    public void ToBook_TrimsAndReturnsValues()
    {
        var vm = new BookEditorViewModel
        {
            Title = "  Название  ", Author = "  Автор  ", Genre = "Роман",
            Year = 2010, Pages = 321, IsRead = true, Rating = 4
        };

        var book = vm.ToBook();

        Assert.Equal("Название", book.Title);
        Assert.Equal("Автор", book.Author);
        Assert.Equal("Роман", book.Genre);
        Assert.Equal(2010, book.Year);
        Assert.Equal(321, book.Pages);
        Assert.True(book.IsRead);
        Assert.Equal(4, book.Rating);
    }

    [Fact]
    public void Constructor_FromExisting_PopulatesFieldsAndPreservesId()
    {
        var existing = new Book
        {
            Title = "Существующая", Author = "Автор", Genre = "Наука",
            Year = 1990, Pages = 150, IsRead = true, Rating = 5
        };

        var vm = new BookEditorViewModel(existing);

        Assert.Equal("Существующая", vm.Title);
        Assert.Equal("Автор", vm.Author);
        Assert.Equal(existing.Id, vm.EditingBookId);
        Assert.Equal(existing.Id, vm.ToBook().Id);
    }

    [Fact]
    public void Cancel_NotConfirmed_Closes()
    {
        var vm = new BookEditorViewModel { Title = "T", Author = "A" };
        var closed = false;
        vm.RequestClose += () => closed = true;

        vm.CancelCommand.Execute(null);

        Assert.False(vm.IsConfirmed);
        Assert.True(closed);
    }
}
