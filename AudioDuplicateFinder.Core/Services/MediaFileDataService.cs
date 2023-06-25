using AudioDuplicateFinder.Core.Contracts.Services;
using AudioDuplicateFinder.Core.Models;

namespace AudioDuplicateFinder.Core.Services;

// This class holds MediaFile data used by some generated pages to show how they can be used.
// TODO: The following classes have been created to display MediaFile data. Delete these files once your app is using real data.
// 1. Contracts/Services/IMediaFileDataService.cs
// 2. Services/MediaFileDataService.cs
// 3. Models/MediaFileInfo2.cs
// 4. Models/MediaFileInfo.cs
// 5. Models/MediaFileDetail.cs
public class MediaFileDataService : IMediaFileDataService
{
    private static List<MediaFileInfo> _mediaFileInfos = null;
    public static void SetMediaFileInfo(List<MediaFileInfo> m)=> _mediaFileInfos = m;
    public MediaFileDataService()
    {
    }

    private static IEnumerable<MediaFileInfo> AllFiles() => _mediaFileInfos == null ? AllFilesTest() : _mediaFileInfos;
    static private Guid[] guids = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
    private static IEnumerable<MediaFileInfo> AllFilesTest()
    {
        return new List<MediaFileInfo>() // Replace below with better dummy data
        {
            new MediaFileInfo()
            {
                GUID = guids[0] ,
                Size = 100,
                Name = "Test",
                Ext = ".wav",
                DirectoryName = "C:\\testpath\\testsubpath",
                Duration = 9123,
                IsReadOnly = false
            },            new MediaFileInfo()
            {
                GUID = guids[0],
                Size = 46,
                Name = "DavidFoo",
                Ext = ".wav",
                DirectoryName = "C:\\testpath\\testsubpxxath",
                Duration = 9123,
                IsReadOnly = false
            },
            new MediaFileInfo()
            {
                GUID = guids[1],
                Size = 157,
                Name = "Foofooxxx",
                Ext = ".mp3",
                DirectoryName = "C:\\foopath\\fsubpath",
                Duration = 8123,
                IsReadOnly = false
            },
            new MediaFileInfo()
            {
                GUID = guids[1],
                Size = 325,
                Name = "joo",
                Ext = ".wav",
                DirectoryName = "C:\\foopath\\fsubpath",
                Duration = 8123,
                IsReadOnly = false
            },
            new MediaFileInfo()
            {
                GUID = guids[3],
                Size = 696,
                Name = "uytiuty",
                Ext = ".mp3",
                DirectoryName = "C:\\foopath\\fsubpath",
                Duration = 8123,
                IsReadOnly = false
            },
            new MediaFileInfo()
            {
                GUID = guids[2],
                Size = 3800,
                Name = "wiget2",
                Ext = ".mp2",
                DirectoryName = "C:\\testpath\\wsubpath",
                Duration = 7123,
                IsReadOnly = false
            },
            new MediaFileInfo()
            {
                GUID = guids[2],
                Size = 3100,
                Name = "wiget88",
                Ext = ".mp2",
                DirectoryName = "C:\\testpath\\wsubpath",
                Duration = 7123,
                IsReadOnly = false
            },
            new MediaFileInfo()
            {
                GUID = guids[3],
                Size = 4100,
                Name = "david",
                Ext = ".wma",
                DirectoryName = "C:\\testpath\\testsubpath",
                Duration = 6123,
                IsReadOnly = false
            },
            new MediaFileInfo()
            {
                GUID = guids[1],
                Size = 4160,
                Name = "Bob",
                Ext = ".mp2",
                DirectoryName = "C:\\testpath\\testsubpath",
                Duration = 6123,
                IsReadOnly = false
            }
        };
    }

    // Remove this once your ContentGrid pages are displaying real data.
    public async Task<IEnumerable<MediaFileInfo>> GetContentGridDataAsync()
    {
        await Task.CompletedTask;
        return AllFiles();
    }

    // Remove this once your DuplicateList pages are displaying real data.
    public async Task<IEnumerable<MediaFileInfo>> GetGridDataAsync()
    {
        await Task.CompletedTask;
        return AllFiles();
    }

    // Remove this once your ListDetails pages are displaying real data.
    public async Task<IEnumerable<MediaFileInfo>> GetListDetailsDataAsync()
    {
        await Task.CompletedTask;
        return AllFiles();
    }
}
