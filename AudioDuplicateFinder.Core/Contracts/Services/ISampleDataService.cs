using AudioDuplicateFinder.Core.Models;

namespace AudioDuplicateFinder.Core.Contracts.Services;

public interface ISampleDataService
{
    Task<IEnumerable<SampleOrder>> GetContentGridDataAsync();

    Task<IEnumerable<SampleOrder>> GetGridDataAsync();

    Task<IEnumerable<SampleOrder>> GetListDetailsDataAsync();
}
