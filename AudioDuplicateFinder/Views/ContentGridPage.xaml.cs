using System.Windows.Controls;

using AudioDuplicateFinder.ViewModels;

namespace AudioDuplicateFinder.Views;

public partial class ContentGridPage : Page
{
    public ContentGridPage(ContentGridViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
