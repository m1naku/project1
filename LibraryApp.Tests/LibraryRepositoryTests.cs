using System;
using System.IO;
using LibraryApp.Models;
using LibraryApp.Repository;
using Xunit;

namespace LibraryApp.Tests;

public class LibraryRepositoryTests : IDisposable
{
    private readonly string _tempFile;

    public LibraryRepositoryTests()
    {
        _tempFile = Path.Combine(Path.GetTempPath(), $"libtest_{Guid.NewGuid():N}.json");
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
    }

    /// <summary>Создаёт репозиторий поверх пустого файла (без демо-данных).</summary>
    private LibraryRepository EmptyRepo()
    {
        File.WriteAllText(_tempFile, "[]");
        return new LibraryRepository(_tempFile);
    }

    [Fact]
    public void NewRepo_NonexistentFile_SeedsSampleData()
    {
        var repo = new LibraryRepository(_tempFile);
        Assert.NotEmpty(repo.GetAll());
    }

    [Fact]
    public void Add_IncreasesCountAndPersists()
    {
        var repo = EmptyRepo();
        repo.Add(new Book { Title = "Тест", Author = "Автор" });

        Assert.Single(repo.GetAll());

        var reloaded = new LibraryRepository(_tempFile);
        Assert.Single(reloaded.GetAll());
        Assert.Equal("Тест", reloaded.GetAll()[0].Title);
    }

    [Fact]
    public void Add_AssignsId_WhenEmpty()
    {
        var repo = EmptyRepo();
        repo.Add(new Book { Id = Guid.Empty, Title = "X", Author = "A" });

        Assert.NotEqual(Guid.Empty, repo.GetAll()[0].Id);
    }

    [Fact]
    public void GetById_ReturnsNull_WhenMissing()
    {
        var repo = EmptyRepo();
        Assert.Null(repo.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void Update_ChangesFields()
    {
        var repo = EmptyRepo();
        var book = new Book { Title = "Старое", Author = "A" };
        repo.Add(book);

        repo.Update(new Book
        {
            Id = book.Id, Title = "Новое", Author = "B",
            Genre = "Роман", Year = 2001, Pages = 50, IsRead = true, Rating = 3
        });

        var result = repo.GetById(book.Id);
        Assert.NotNull(result);
        Assert.Equal("Новое", result!.Title);
        Assert.Equal("B", result.Author);
        Assert.True(result.IsRead);
    }

    [Fact]
    public void Update_Missing_DoesNothing()
    {
        var repo = EmptyRepo();
        repo.Update(new Book { Id = Guid.NewGuid(), Title = "Нет" });
        Assert.Empty(repo.GetAll());
    }

    [Fact]
    public void Delete_RemovesBook()
    {
        var repo = EmptyRepo();
        var book = new Book { Title = "Удалить", Author = "A" };
        repo.Add(book);

        repo.Delete(book.Id);

        Assert.Empty(repo.GetAll());
    }
}
