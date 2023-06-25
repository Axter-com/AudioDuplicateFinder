using System.Collections.Immutable;
using System.Collections.ObjectModel;

using AudioDuplicateFinder.Contracts.ViewModels;
using AudioDuplicateFinder.Core.Contracts.Services;
using AudioDuplicateFinder.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AudioDuplicateFinder.ViewModels;

public class DuplicateListViewModel : ObservableObject, INavigationAware
{
    private readonly IMediaFileDataService _mediaFileDataService;

    public ObservableCollection<MediaFileInfo> MediaFileItems { get; private set; } = new ObservableCollection<MediaFileInfo>();

    public DuplicateListViewModel(IMediaFileDataService mediaFileDataService) => _mediaFileDataService = mediaFileDataService;

    public async void OnNavigatedTo(object parameter)
    {
        MediaFileItems.Clear();

        // Replace this with your actual data
        var data = await _mediaFileDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            MediaFileItems.Add(item);
        }
        MediaFileItems = new ObservableCollection<MediaFileInfo>(MediaFileItems.OrderBy(i => i.FullPath));
    }

    public void OnNavigatedFrom()
    {
    }
}
