using System.Collections.ObjectModel;

using AudioDuplicateFinder.Contracts.ViewModels;
using AudioDuplicateFinder.Core.Contracts.Services;
using AudioDuplicateFinder.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AudioDuplicateFinder.ViewModels;

public class ListDetailsViewModel : ObservableObject, INavigationAware
{
    private readonly IMediaFileDataService _mediaFileDataService;
    private MediaFileInfo _selected;

    public MediaFileInfo Selected
    {
        get { return _selected; }
        set { SetProperty(ref _selected, value); }
    }

    public ObservableCollection<MediaFileInfo> MediaFileItems { get; private set; } = new ObservableCollection<MediaFileInfo>();

    public ListDetailsViewModel(IMediaFileDataService mediaFileDataService) => _mediaFileDataService = mediaFileDataService;

    public async void OnNavigatedTo(object parameter)
    {
        MediaFileItems.Clear();

        var data = await _mediaFileDataService.GetListDetailsDataAsync();

        foreach (var item in data)
        {
            MediaFileItems.Add(item);
        }

        Selected = MediaFileItems.First();
    }

    public void OnNavigatedFrom()
    {
    }
}
