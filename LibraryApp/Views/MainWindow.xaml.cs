using System.Windows;
using LibraryApp.Repository;
using LibraryApp.ViewModels;

namespace LibraryApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var repo = new LibraryRepository();
        var vm = new MainViewModel(repo);

        vm.ShowEditorDialog = editorVm =>
        {
            var dlg = new BookEditorWindow
            {
                Owner = this,
                DataContext = editorVm
            };
            editorVm.RequestClose += () => dlg.Close();
            dlg.ShowDialog();
            return editorVm.IsConfirmed;
        };

        vm.ConfirmDialog = (title, message) =>
        {
            var result = MessageBox.Show(this, message, title,
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        };

        DataContext = vm;
    }
}
