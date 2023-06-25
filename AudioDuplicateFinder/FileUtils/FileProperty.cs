/*
//      Copyright © 2023 David Maisonave
//      GPLv3 License
*/
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using AudioDuplicateFinder.AudioUtils;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.FileProviders;

namespace AudioDuplicateFinder.FileUtils
{    
    public partial class FileProperty
    {
        public FileProperty() { }
        public FileProperty(string name) => fileInfo = new FileInfo(name);
        public FileProperty(FileInfo fi) => fileInfo=fi;

        [JsonInclude]
        public string Name { get; private set; } = null!;
        [JsonInclude]
        public string ParentDir { get; private set; } = null!;
        public string Folder { get => ParentDir; } // Alias
        private string _Path;
        public string Path { get => string.IsNullOrEmpty(_Path) ? _Path = ParentDir + "\\" + Name : _Path; private set => _Path = value; }
        public string FullPath { get => Path; } // Alias
        public string Ext { get; private set; }
        [JsonInclude]
        public FileTypes MediaType { get; private set; }
        [JsonInclude]
        public long Size { get; private set; }
        [JsonInclude]
        public TimeSpan Duration { get; set; }
        private FileInfo _fileInfo;
        public FileInfo fileInfo { get => _fileInfo; set =>UpdateFileInfo(value); }
        public long CheckSum { get; set; }
        public byte[] FingerPrint { get; set; }
        private WavFile _waveControl;
        public WavFile waveControl 
        { 
            get
            {
                lock( this )
                {
                    if ( _waveControl == null && Ext.EndsWith(".wav") ) 
                    {
                        _waveControl = new(FullPath);
                    }
                    return _waveControl;
                }
            }
            private set => _waveControl = value;
        }
        public bool IsVideo { get => (MediaType & FileTypes.VideoFiles) != 0; }
        public bool IsImage { get => (MediaType & FileTypes.ImageFiles) != 0; }
        public bool IsAudio { get => (MediaType & FileTypes.AudioFiles) != 0; }
        public bool IsText { get => (MediaType & FileTypes.TextFiles) != 0; }
        public bool IsDoc { get => (MediaType & FileTypes.DocFiles) != 0; }
        public bool IsManuallyExcluded { get => (MediaType & FileTypes.IsManuallyExcluded) != 0; }
        public bool IsTooDark { get => (MediaType & FileTypes.IsTooDark) != 0; }
        public bool HasMetadataError { get => (MediaType & FileTypes.HasMetadataError) != 0; }
        public bool HasThumbNailError { get => (MediaType & FileTypes.HasThumbNailError) != 0; }

        private FileInfo UpdateFileInfo(FileInfo fi)
        {
            _fileInfo = fi;
            Name = fi.Name;
            ParentDir = fi.DirectoryName;
            Ext = fi.Extension.ToLower();
            Size = fi.Length;
            if ( FileExtensions.VideoExtensions.Any(x => Ext.EndsWith(x)) )
                MediaType = FileTypes.VideoFiles;
            else if ( FileExtensions.ImageExtensions.Any(x => Ext.EndsWith(x)) )
                MediaType = FileTypes.ImageFiles;
            else if ( FileExtensions.AudioExtensions.Any(x => Ext.EndsWith(x)) )
                MediaType = FileTypes.AudioFiles;
            else if ( FileExtensions.TextExtensions.Any(x => Ext.EndsWith(x)) )
                MediaType = FileTypes.TextFiles;
            else if ( FileExtensions.DocExtensions.Any(x => Ext.EndsWith(x)) )
                MediaType = FileTypes.DocFiles;
            return fi;
        }
    }
    public enum FileTypes
    {
        VideoFiles          = 1 << 0,
        ImageFiles          = 1 << 1,
        AudioFiles          = 1 << 2,
        TextFiles           = 1 << 3,
        DocFiles            = 1 << 4,
        SpreadSheetFiles    = 1 << 5,
        PresentationFiles   = 1 << 6,
        AudioAndImage       = AudioFiles | ImageFiles,
        VideoAndImage       = VideoFiles | ImageFiles,
        MediaFiles          = VideoFiles | ImageFiles | AudioFiles,
        OfficeFiles         = DocFiles | SpreadSheetFiles | PresentationFiles,
        OfficeAndTextFiles  = OfficeFiles | TextFiles,
        AllFiles            = 255, // All lower bits on
        IsManuallyExcluded  = 1 << 8,
        IsTooDark           = 1 << 9,
        HasMetadataError    = 1 << 10,
        HasThumbNailError   = 1 << 11
    }
    public static class CoreUtils
    {
        public static bool IsWindows;
        public static string CurrentFolder;
        static CoreUtils()
        {
            IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            CurrentFolder = Path.GetDirectoryName(typeof(CoreUtils).Assembly.Location)!;
        }
    }
}