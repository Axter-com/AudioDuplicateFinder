using System.Windows.Controls;

using AudioDuplicateFinder.ViewModels;

namespace AudioDuplicateFinder.Views;

public partial class DuplicateListPage : Page
{
    public DuplicateListPage(DuplicateListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
