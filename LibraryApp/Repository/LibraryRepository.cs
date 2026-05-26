using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibraryApp.Models;
using Newtonsoft.Json;

namespace LibraryApp.Repository;

/// <summary>
/// Репозиторий для управления коллекцией книг с персистентным хранилищем в JSON-файле.
/// </summary>
public class LibraryRepository
{
    private readonly string _filePath;
    private readonly List<Book> _books = new();

    public LibraryRepository(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LibraryApp",
            "library.json");

        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        Load();
    }

    public IReadOnlyList<Book> GetAll() => _books;

    public Book? GetById(Guid id) => _books.FirstOrDefault(b => b.Id == id);

    public void Add(Book book)
    {
        if (book.Id == Guid.Empty) book.Id = Guid.NewGuid();
        if (book.DateAdded == default) book.DateAdded = DateTime.Now;
        _books.Add(book);
        Save();
    }

    public void Update(Book book)
    {
        var existing = GetById(book.Id);
        if (existing == null) return;
        existing.CopyFrom(book);
        Save();
    }

    public void Delete(Guid id)
    {
        var book = GetById(id);
        if (book == null) return;
        _books.Remove(book);
        Save();
    }

    public void Save()
    {
        try
        {
            var json = JsonConvert.SerializeObject(_books, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Не удалось сохранить библиотеку: {ex.Message}");
        }
    }

    private void Load()
    {
        if (!File.Exists(_filePath))
        {
            SeedSampleData();
            Save();
            return;
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            var list = JsonConvert.DeserializeObject<List<Book>>(json);
            if (list != null)
            {
                _books.Clear();
                _books.AddRange(list);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Не удалось загрузить библиотеку: {ex.Message}");
        }
    }

    private void SeedSampleData()
    {
        _books.AddRange(new[]
        {
            new Book { Title = "Мастер и Маргарита", Author = "Михаил Булгаков", Genre = "Роман",
                Year = 1967, Pages = 480, IsRead = true, Rating = 5, DateAdded = DateTime.Now.AddDays(-200) },
            new Book { Title = "1984", Author = "Джордж Оруэлл", Genre = "Фантастика",
                Year = 1949, Pages = 328, IsRead = true, Rating = 5, DateAdded = DateTime.Now.AddDays(-150) },
            new Book { Title = "Чистый код", Author = "Роберт Мартин", Genre = "Программирование",
                Year = 2008, Pages = 464, IsRead = true, Rating = 4, DateAdded = DateTime.Now.AddDays(-90) },
            new Book { Title = "Шерлок Холмс", Author = "Артур Конан Дойл", Genre = "Детектив",
                Year = 1887, Pages = 600, IsRead = false, Rating = 0, DateAdded = DateTime.Now.AddDays(-180) },
            new Book { Title = "Краткая история времени", Author = "Стивен Хокинг", Genre = "Наука",
                Year = 1988, Pages = 256, IsRead = false, Rating = 0, DateAdded = DateTime.Now.AddDays(-15) },
            new Book { Title = "Властелин колец", Author = "Дж.Р.Р. Толкин", Genre = "Фэнтези",
                Year = 1954, Pages = 1216, IsRead = true, Rating = 5, DateAdded = DateTime.Now.AddDays(-300) },
            new Book { Title = "Думай медленно... решай быстро", Author = "Даниэль Канеман", Genre = "Психология",
                Year = 2011, Pages = 656, IsRead = false, Rating = 0, DateAdded = DateTime.Now.AddDays(-250) },
            new Book { Title = "Преступление и наказание", Author = "Фёдор Достоевский", Genre = "Роман",
                Year = 1866, Pages = 672, IsRead = true, Rating = 5, DateAdded = DateTime.Now.AddDays(-60) },
        });
    }
}
