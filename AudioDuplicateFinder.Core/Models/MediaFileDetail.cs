namespace AudioDuplicateFinder.Core.Models;

// Remove this class once your pages/features are using your data.
// This is used by the MediaFileDataService.
// It is the model class we use to display data on pages like Grid, Chart, and List Details.
public class MediaFileDetail
{
    public string Name { get; set; }
    public string Ext { get; set; }
    public string DirectoryName { get; set; }
    public long Size { get; set; }
    public long Duration { get; set; }
    public bool IsReadOnly { get; set; }
    public DirectoryInfo Directory { get; set; }

    public string ShortDescription => $"File ID: {Name} - {DirectoryName}";
}
