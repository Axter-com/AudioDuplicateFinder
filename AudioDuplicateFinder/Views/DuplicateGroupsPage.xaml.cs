using System.Windows.Controls;

using AudioDuplicateFinder.ViewModels;
using AudioDuplicateFinder.Core.Models;
using System.Windows.Data;

namespace AudioDuplicateFinder.Views;

public partial class DuplicateGroupsPage : Page
{
    public DuplicateGroupsPage(DuplicateGroupsViewModel viewModel)
    {
        InitializeComponent();
        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(viewModel.MediaFileItems);
        PropertyGroupDescription groupDescription = new ("ID");
        view.GroupDescriptions.Add(groupDescription);
        DataContext = viewModel;
    }
}
