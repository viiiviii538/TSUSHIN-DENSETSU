using System.Windows;
using TsushinDensetsu.App.ViewModels;

namespace TsushinDensetsu.App.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
