using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryApp.Models;

namespace LibraryApp.ViewModels;

/// <summary>
/// ViewModel диалога добавления/редактирования книги.
/// Работает с копией книги; изменения применяются только при подтверждении.
/// </summary>
public partial class BookEditorViewModel : ObservableObject
{
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _author = string.Empty;
    [ObservableProperty] private string _genre = Genres.All[0];
    [ObservableProperty] private int _year = DateTime.Now.Year;
    [ObservableProperty] private int _pages = 0;
    [ObservableProperty] private bool _isRead;
    [ObservableProperty] private int _rating;
    [ObservableProperty] private string _windowTitle = "Новая книга";
    [ObservableProperty] private string? _errorMessage;

    public IReadOnlyList<string> AvailableGenres => Genres.All;
    public IReadOnlyList<int> AvailableRatings => new[] { 0, 1, 2, 3, 4, 5 };

    public Guid EditingBookId { get; private set; } = Guid.Empty;
    public bool IsConfirmed { get; private set; }

    public event Action? RequestClose;

    public BookEditorViewModel() { }

    public BookEditorViewModel(Book existing)
    {
        EditingBookId = existing.Id;
        Title = existing.Title;
        Author = existing.Author;
        Genre = existing.Genre;
        Year = existing.Year;
        Pages = existing.Pages;
        IsRead = existing.IsRead;
        Rating = existing.Rating;
        WindowTitle = $"Редактирование: {existing.Title}";
    }

    [RelayCommand]
    private void Save()
    {
        if (!Validate(out var error))
        {
            ErrorMessage = error;
            return;
        }

        IsConfirmed = true;
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        IsConfirmed = false;
        RequestClose?.Invoke();
    }

    private bool Validate(out string error)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            error = "Название не может быть пустым.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Author))
        {
            error = "Автор не может быть пустым.";
            return false;
        }
        if (Year < 0 || Year > DateTime.Now.Year + 1)
        {
            error = $"Год должен быть в диапазоне 0–{DateTime.Now.Year + 1}.";
            return false;
        }
        if (Pages < 0)
        {
            error = "Количество страниц не может быть отрицательным.";
            return false;
        }
        if (Rating < 0 || Rating > 5)
        {
            error = "Оценка должна быть от 0 до 5.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public Book ToBook()
    {
        var book = new Book
        {
            Title = Title.Trim(),
            Author = Author.Trim(),
            Genre = Genre,
            Year = Year,
            Pages = Pages,
            IsRead = IsRead,
            Rating = Rating,
        };
        if (EditingBookId != Guid.Empty) book.Id = EditingBookId;
        return book;
    }
}
