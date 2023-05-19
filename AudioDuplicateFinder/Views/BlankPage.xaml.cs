using System.Windows.Controls;

using AudioDuplicateFinder.ViewModels;

namespace AudioDuplicateFinder.Views;

public partial class BlankPage : Page
{
    public BlankPage(BlankViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
