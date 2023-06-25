using AudioDuplicateFinder.Contracts.ViewModels;
using AudioDuplicateFinder.Core.Contracts.Services;
using AudioDuplicateFinder.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AudioDuplicateFinder.ViewModels;

public class ContentGridDetailViewModel : ObservableObject, INavigationAware
{
    private readonly IMediaFileDataService _mediaFileDataService;
    private MediaFileInfo _item;
    public MediaFileInfo Item
    {
        get => _item;
        set => SetProperty(ref _item, value);
    }

    public ContentGridDetailViewModel(IMediaFileDataService mediaFileDataService) =>_mediaFileDataService = mediaFileDataService;

    public async void OnNavigatedTo(object parameter)
    {
        if ( parameter is string Name )
        {
            var data = await _mediaFileDataService.GetContentGridDataAsync();
            Item = data.First(i => i.Name == Name);
        }
        else if ( parameter is long Size )
        {
            var data = await _mediaFileDataService.GetContentGridDataAsync();
            Item = data.First(i => i.Size == Size);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
