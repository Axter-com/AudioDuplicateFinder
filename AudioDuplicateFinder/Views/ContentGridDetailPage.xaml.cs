using System.Windows.Controls;

using AudioDuplicateFinder.ViewModels;

namespace AudioDuplicateFinder.Views;

public partial class ContentGridDetailPage : Page
{
    public ContentGridDetailPage(ContentGridDetailViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
