using System.Windows.Controls;

using AudioDuplicateFinder.ViewModels;

namespace AudioDuplicateFinder.Views;

public partial class MainPage : Page
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
