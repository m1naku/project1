using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibraryApp.Converters;
using LibraryApp.Repository;
using LibraryApp.ViewModels;
using LibraryApp.Views;

namespace LibraryApp;

public class App : Application
{
    [STAThread]
    public static void Main()
    {
        var app = new App();
        app.Run();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        RegisterResources();

        var repo = new LibraryRepository();
        var viewModel = new MainViewModel(repo);
        var window = new MainWindow { DataContext = viewModel };

        viewModel.ShowEditorDialog = editorVm =>
        {
            var dialog = new BookEditorWindow
            {
                Owner = window,
                DataContext = editorVm
            };
            editorVm.RequestClose += () => dialog.Close();
            dialog.ShowDialog();
            return editorVm.IsConfirmed;
        };

        viewModel.ConfirmDialog = (title, message) =>
            MessageBox.Show(window, message, title,
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

        window.Show();
    }

    private void RegisterResources()
    {
        Resources["RatingToStars"] = new RatingToStarsConverter();
        Resources["BoolToYesNo"] = new BoolToYesNoConverter();
        Resources["BoolToVis"] = new BooleanToVisibilityConverter();

        Resources["Caption"] = BuildStyle(typeof(TextBlock),
            (TextBlock.ForegroundProperty, Brush("#666")),
            (TextBlock.FontSizeProperty, 12.0),
            (TextBlock.MarginProperty, new Thickness(0, 0, 0, 2)));

        Resources["StatValue"] = BuildStyle(typeof(TextBlock),
            (TextBlock.FontSizeProperty, 20.0),
            (TextBlock.FontWeightProperty, FontWeights.SemiBold));

        Resources[typeof(Button)] = BuildStyle(typeof(Button),
            (Control.PaddingProperty, new Thickness(10, 5, 10, 5)),
            (FrameworkElement.MarginProperty, new Thickness(4, 0, 0, 0)),
            (FrameworkElement.MinWidthProperty, 80.0));

        Resources[typeof(TextBox)] = BuildStyle(typeof(TextBox),
            (Control.PaddingProperty, new Thickness(4, 3, 4, 3)));

        Resources[typeof(ComboBox)] = BuildStyle(typeof(ComboBox),
            (Control.PaddingProperty, new Thickness(4, 3, 4, 3)),
            (FrameworkElement.MinWidthProperty, 130.0));
    }

    private static Style BuildStyle(Type targetType, params (DependencyProperty Property, object Value)[] setters)
    {
        var style = new Style(targetType);
        foreach (var (property, value) in setters)
            style.Setters.Add(new Setter(property, value));
        return style;
    }

    private static Brush Brush(string hex)
        => (Brush)new BrushConverter().ConvertFromString(hex)!;
}
