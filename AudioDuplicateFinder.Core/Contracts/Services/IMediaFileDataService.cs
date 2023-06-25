using AudioDuplicateFinder.Core.Models;

namespace AudioDuplicateFinder.Core.Contracts.Services;

public interface IMediaFileDataService
{
    Task<IEnumerable<MediaFileInfo>> GetContentGridDataAsync();

    Task<IEnumerable<MediaFileInfo>> GetGridDataAsync();

    Task<IEnumerable<MediaFileInfo>> GetListDetailsDataAsync();
}
