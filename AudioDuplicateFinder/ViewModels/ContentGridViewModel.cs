using System.Collections.ObjectModel;
using System.Windows.Input;

using AudioDuplicateFinder.Contracts.Services;
using AudioDuplicateFinder.Contracts.ViewModels;
using AudioDuplicateFinder.Core.Contracts.Services;
using AudioDuplicateFinder.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AudioDuplicateFinder.ViewModels;

public class ContentGridViewModel : ObservableObject, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly IMediaFileDataService _mediaFileDataService;
    private ICommand _navigateToDetailCommand;

    public ICommand NavigateToDetailCommand => _navigateToDetailCommand ?? (_navigateToDetailCommand = new RelayCommand<MediaFileInfo>(NavigateToDetail));

    public ObservableCollection<MediaFileInfo> MediaFileItems { get; } = new ObservableCollection<MediaFileInfo>();

    public ContentGridViewModel(IMediaFileDataService mediaFileDataService, INavigationService navigationService)
    {
        _mediaFileDataService = mediaFileDataService;
        _navigationService = navigationService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        MediaFileItems.Clear();

        // Replace this with your actual data
        var data = await _mediaFileDataService.GetContentGridDataAsync();
        foreach (var item in data)
        {
            MediaFileItems.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    private void NavigateToDetail(MediaFileInfo order)
    {
        _navigationService.NavigateTo(typeof(ContentGridDetailViewModel).FullName, order.Name);
    }
}
