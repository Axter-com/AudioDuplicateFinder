using AudioDuplicateFinder.Core.Models;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AudioDuplicateFinder.Core.Contracts.Services;
using AudioDuplicateFinder.Contracts.ViewModels;
using System.Windows.Data;

namespace AudioDuplicateFinder.ViewModels;

public class DuplicateGroupsViewModel : ObservableObject, INavigationAware
{
    private readonly IMediaFileDataService _mediaFileDataService;
    private MediaFileInfo _selected;
    public MediaFileInfo Selected{ get => _selected; set => SetProperty(ref _selected, value); }
    public ObservableCollection<MediaFileInfo> MediaFileItems { get; private set; } = new ObservableCollection<MediaFileInfo>();
    public DuplicateGroupsViewModel(IMediaFileDataService mediaFileDataService) => _mediaFileDataService = mediaFileDataService;
    public async void OnNavigatedTo(object parameter)
    {
        MediaFileItems.Clear();

        var data = await _mediaFileDataService.GetListDetailsDataAsync();

        foreach ( var item in data )
        {
            MediaFileItems.Add(item);
        }
        Selected = MediaFileItems.First();
    }
    public void OnNavigatedFrom()
    {
    }
}
