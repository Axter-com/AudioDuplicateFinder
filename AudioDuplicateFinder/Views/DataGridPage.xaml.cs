using System.Windows.Controls;

using AudioDuplicateFinder.ViewModels;

namespace AudioDuplicateFinder.Views;

public partial class DataGridPage : Page
{
    public DataGridPage(DataGridViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
