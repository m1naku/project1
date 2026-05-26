using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryApp.Models;
using LibraryApp.Repository;

namespace LibraryApp.ViewModels;

/// <summary>
/// Главная ViewModel: список книг, фильтры, поиск, команды CRUD,
/// агрегированная статистика и уведомления о "забытых" непрочитанных книгах.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    public const string AllGenresLabel = "Все жанры";
    public const string StatusAll = "Все";
    public const string StatusRead = "Прочитанные";
    public const string StatusUnread = "Непрочитанные";

    private readonly LibraryRepository _repository;
    private readonly ObservableCollection<Book> _books = new();

    public ICollectionView BooksView { get; }
    public LibraryStatsViewModel Stats { get; } = new();
    public ObservableCollection<Book> StaleBooks { get; } = new();

    public IReadOnlyList<string> AvailableGenres { get; }
    public IReadOnlyList<string> AvailableStatuses { get; } =
        new[] { StatusAll, StatusRead, StatusUnread };

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _selectedGenreFilter = AllGenresLabel;
    [ObservableProperty] private string _selectedStatusFilter = StatusAll;
    [ObservableProperty] private Book? _selectedBook;
    [ObservableProperty] private int _staleThresholdDays = 90;

    public bool HasStaleBooks => StaleBooks.Count > 0;

    /// <summary>
    /// Делегат, открывающий диалог редактирования. Подставляется View'ом,
    /// чтобы ViewModel не зависела напрямую от UI.
    /// </summary>
    public Func<BookEditorViewModel, bool>? ShowEditorDialog { get; set; }

    /// <summary>
    /// Делегат, показывающий диалог подтверждения. Возвращает true если пользователь подтвердил.
    /// </summary>
    public Func<string, string, bool>? ConfirmDialog { get; set; }

    public MainViewModel(LibraryRepository repository)
    {
        _repository = repository;

        AvailableGenres = new[] { AllGenresLabel }.Concat(Genres.All).ToList();

        foreach (var book in _repository.GetAll())
            _books.Add(book);

        BooksView = CollectionViewSource.GetDefaultView(_books);
        BooksView.Filter = FilterBook;
        BooksView.SortDescriptions.Add(new SortDescription(nameof(Book.Title), ListSortDirection.Ascending));

        RefreshStaleBooks();
        Stats.Update(_books);
    }

    partial void OnSearchTextChanged(string value) => BooksView.Refresh();
    partial void OnSelectedGenreFilterChanged(string value) => BooksView.Refresh();
    partial void OnSelectedStatusFilterChanged(string value) => BooksView.Refresh();
    partial void OnStaleThresholdDaysChanged(int value) => RefreshStaleBooks();

    private bool FilterBook(object obj)
    {
        if (obj is not Book book) return false;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var s = SearchText.Trim();
            var matches = book.Title.Contains(s, StringComparison.OrdinalIgnoreCase)
                          || book.Author.Contains(s, StringComparison.OrdinalIgnoreCase);
            if (!matches) return false;
        }

        if (!string.IsNullOrWhiteSpace(SelectedGenreFilter) && SelectedGenreFilter != AllGenresLabel)
        {
            if (!string.Equals(book.Genre, SelectedGenreFilter, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return SelectedStatusFilter switch
        {
            StatusRead => book.IsRead,
            StatusUnread => !book.IsRead,
            _ => true,
        };
    }

    [RelayCommand]
    private void AddBook()
    {
        var vm = new BookEditorViewModel();
        if (ShowEditorDialog?.Invoke(vm) == true)
        {
            var book = vm.ToBook();
            _repository.Add(book);
            _books.Add(book);
            RefreshAggregates();
            SelectedBook = book;
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedBook))]
    private void EditBook()
    {
        if (SelectedBook == null) return;

        var vm = new BookEditorViewModel(SelectedBook);
        if (ShowEditorDialog?.Invoke(vm) == true)
        {
            var updated = vm.ToBook();
            _repository.Update(updated);
            SelectedBook.CopyFrom(updated);
            BooksView.Refresh();
            RefreshAggregates();
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedBook))]
    private void DeleteBook()
    {
        if (SelectedBook == null) return;

        var confirm = ConfirmDialog?.Invoke(
            "Удалить книгу?",
            $"Вы уверены, что хотите удалить «{SelectedBook.Title}»?") ?? true;
        if (!confirm) return;

        _repository.Delete(SelectedBook.Id);
        _books.Remove(SelectedBook);
        SelectedBook = null;
        RefreshAggregates();
    }

    [RelayCommand(CanExecute = nameof(HasSelectedBook))]
    private void ToggleRead()
    {
        if (SelectedBook == null) return;

        SelectedBook.IsRead = !SelectedBook.IsRead;
        _repository.Update(SelectedBook);
        BooksView.Refresh();
        RefreshAggregates();
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = string.Empty;
        SelectedGenreFilter = AllGenresLabel;
        SelectedStatusFilter = StatusAll;
    }

    [RelayCommand]
    private void Refresh()
    {
        BooksView.Refresh();
        RefreshAggregates();
    }

    [RelayCommand]
    private void DismissStaleBook(Book book)
    {
        if (book == null) return;
        StaleBooks.Remove(book);
        OnPropertyChanged(nameof(HasStaleBooks));
    }

    private bool HasSelectedBook() => SelectedBook != null;

    partial void OnSelectedBookChanged(Book? value)
    {
        EditBookCommand.NotifyCanExecuteChanged();
        DeleteBookCommand.NotifyCanExecuteChanged();
        ToggleReadCommand.NotifyCanExecuteChanged();
    }

    private void RefreshAggregates()
    {
        RefreshStaleBooks();
        Stats.Update(_books);
    }

    private void RefreshStaleBooks()
    {
        StaleBooks.Clear();
        foreach (var book in _books.Where(b => b.IsStale(StaleThresholdDays))
                                   .OrderBy(b => b.DateAdded))
            StaleBooks.Add(book);
        OnPropertyChanged(nameof(HasStaleBooks));
    }
}
