using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LibraryApp.Models;

/// <summary>
/// Модель книги в личной библиотеке.
/// Использует ObservableObject из CommunityToolkit.Mvvm для INotifyPropertyChanged.
/// </summary>
public partial class Book : ObservableObject
{
    [ObservableProperty] private Guid _id = Guid.NewGuid();
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _author = string.Empty;
    [ObservableProperty] private string _genre = string.Empty;
    [ObservableProperty] private int _year = DateTime.Now.Year;
    [ObservableProperty] private int _pages;
    [ObservableProperty] private bool _isRead;
    [ObservableProperty] private int _rating;
    [ObservableProperty] private DateTime _dateAdded = DateTime.Now;

    /// <summary>
    /// Возвращает true, если книга добавлена более N дней назад и до сих пор не прочитана.
    /// </summary>
    public bool IsStale(int olderThanDays = 90)
        => !IsRead && (DateTime.Now - DateAdded).TotalDays > olderThanDays;

    public Book Clone() => new()
    {
        Id = Id,
        Title = Title,
        Author = Author,
        Genre = Genre,
        Year = Year,
        Pages = Pages,
        IsRead = IsRead,
        Rating = Rating,
        DateAdded = DateAdded,
    };

    public void CopyFrom(Book other)
    {
        Title = other.Title;
        Author = other.Author;
        Genre = other.Genre;
        Year = other.Year;
        Pages = other.Pages;
        IsRead = other.IsRead;
        Rating = other.Rating;
    }
}

/// <summary>
/// Предопределённые жанры для удобства выбора в UI.
/// </summary>
public static class Genres
{
    public static readonly string[] All =
    {
        "Фантастика",
        "Детектив",
        "Наука",
        "Роман",
        "Поэзия",
        "История",
        "Биография",
        "Психология",
        "Программирование",
        "Бизнес",
        "Фэнтези",
        "Прочее"
    };
}
