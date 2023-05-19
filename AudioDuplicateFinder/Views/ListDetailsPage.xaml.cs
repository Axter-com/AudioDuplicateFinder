using System.Windows.Controls;

using AudioDuplicateFinder.ViewModels;

namespace AudioDuplicateFinder.Views;

public partial class ListDetailsPage : Page
{
    public ListDetailsPage(ListDetailsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
