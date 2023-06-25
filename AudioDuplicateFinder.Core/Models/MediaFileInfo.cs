namespace AudioDuplicateFinder.Core.Models;

// Remove this class once your pages/features are using your data.
// This is used by the MediaFileDataService.
// It is the model class we use to display data on pages like Grid, Chart, and List Details.
public class MediaFileInfo
{
    private static int SessionCounter = 0;
    private static Dictionary<Guid, int> GuidIndex = new ();
    public string _ID = null;
    public string ID // Single session ID
    { 
        get
        {
            if ( _ID == null )
            {
                if ( GUID != null )
                {
                    lock ( this )
                    {
                        if ( !GuidIndex.ContainsKey( GUID ) )
                            GuidIndex.Add(GUID, SessionCounter++);
                        _ID = GuidIndex[GUID].ToString("D4");
                    }
                }
            }
            return _ID;
        }
        private set { _ID = value; }
    }
    public Guid GUID { get; set; } // Multi-session ID
    public long Size { get; set; }
    public string Name { get; set; }
    public string Ext { get; set; }
    public string DirectoryName { get; set; }
    public double Duration { get; set; }
    public bool IsReadOnly { get; set; }
    public DirectoryInfo Directory { get; set; }
    public string FullPath { get => $"{DirectoryName}\\{Name}{Ext}"; }
    public bool IsChecked { get; set; }

    // Use this for a one to many data member
    // public ICollection<MediaFileDetail> Details { get; set; }

    public override string ToString() => FullPath;

    public string ShortDescription => $"File ID: {ID}";
}
