using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Threading;
using System.Diagnostics;

namespace AudioDuplicateFinder.Core.ScanUtil
{
    internal class FileSearch
    {
        public static readonly string[] VideoExtensions = {
            ".mp4",
            ".wmv",
            ".avi",
            ".mkv",
            ".flv",
            ".mov",
            ".mpg",
            ".mpeg", // Need to figure out a way to programmatically differentiate between Video and Audio
			".m4v",
            ".asf",
            ".f4v",
            ".webm",
            ".divx",
            ".m2t",
            ".m2ts",
            ".vob",
            ".ts"
        };
        public static readonly string[] ImageExtensions = {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".bmp",
            ".tiff",
            ".webp"};
        public static readonly string[] AudioExtensions = {
            ".aac",
            ".aiff",
            ".amr",
            ".flac",
            ".m4a",
            ".m4b",
            ".mp2",
            ".mp3",
            ".mpeg", // Need to figure out a way to programmatically differentiate between Video and Audio
			".ogg",
            ".voc",
            ".wav",
            ".wma"
        };
        public static readonly string[] TextExtensions = {// Text only file types (raw text with no formatting)
			".txt",
            ".log",		// Log File - Very-Common
			".faq",		// Frequently Asked Questions Document
			".err",		// Error Message Log
			//Readme text file types
			".1st",		// Readme Text
			".readme",  // Readme Text
			".now",		// ReadMe Text
		};
        public static readonly string[] DocExtensions = { // Formatted text documents
			".doc",		// Microsoft Word Document - Very-Common
			".docm",	// Microsoft Office Word (2007+) Document (Macro Enabled)Common
			".docx",	// Microsoft Office Word (2007+) Document - Very-Common
			".dot",		// Microsoft Word Document TemplateCommon
			".dotm",	// Microsoft Office Word (2007+) Template (Macro Enabled)Common
			".dotx",	// Microsoft Office Word (2007+) TemplateCommon
			".odf",		// Open Document Format 
			".odm",		// OpenDocument Master Document
			".odt",		// OpenOffice/StarOffice OpenDocument (V.2) Text Doc - Very-Common
			".ott",		// OpenDocument Text Document Template
			".pdf",		// Portable Document Format | Adobe Acrobat
			".htm",		// Hypertext Markup Language
			".html",	// Hypertext Markup Language
			".rtf",		// Rich Text Format file (MS-Windows)
		};
        public static readonly string[] SpreadSheetExtensions = {// Spread sheet type files
			".xls",
            ".xlsx",
            ".ods",
        };
        public static readonly string[] PresentationExtensions = { // Presentation type files
			".odp",
            ".pptx",
            ".pptm",
            ".ppt"
        };
        static readonly string[] AllExtensions = VideoExtensions.Concat(ImageExtensions).Concat(AudioExtensions).ToArray();
        static readonly string[] VideoAudioExtensions = VideoExtensions.Concat(AudioExtensions).ToArray();
        static readonly string[] VideoImagesExtensions = VideoExtensions.Concat(ImageExtensions).ToArray();
        public enum FileTypes
        {
            ImageFiles          = 1 << 0,
            AudioFiles          = 1 << 1,
            VideoFiles          = 1 << 2,
            TextFiles           = 1 << 3,
            DocFiles            = 1 << 4,
            SpreadSheetFiles    = 1 << 5,
            PresentationFiles   = 1 << 6,
            ImageAndAudio       = ImageFiles | AudioFiles,
            AllMediaFiles       = ImageAndAudio | VideoFiles,
            AllOfficeFiles      = TextFiles | DocFiles | SpreadSheetFiles | PresentationFiles,
            AllFiles            = AllMediaFiles | AllOfficeFiles
        }
        private String[] IncludeDir;
        private String[] ExcludeDir;
        private List<FileInfo> _fileInfos = new List<FileInfo>();
        private CancellationTokenSource cts = new ();
        private ParallelOptions parallelOptions;
        private EnumerationOptions enumerationOptions;
        private EventLog eventLog = new EventLog ();
        private int QtyFilesProcessed = 0;
        private FileTypes filetypes = FileTypes.AllMediaFiles;
        public FileSearch(StringCollection i, StringCollection x, bool ignoreReadonly, bool ignoreReparsePoints, int MaxParallel = 20) // ToDo: Set MaxParallel to a program settings value
        { 
            this.IncludeDir = new string[i.Count];
            i.CopyTo(this.IncludeDir, 0);
            this.ExcludeDir = new string[x.Count];
            x.CopyTo(this.ExcludeDir, 0);
            //token = tokenSource.Token;
            parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = MaxParallel, 
                //Set the CancellationToken value
                CancellationToken = cts.Token
            };
            enumerationOptions = new ()
            {
                IgnoreInaccessible = true,
            };
            enumerationOptions.AttributesToSkip = FileAttributes.System;
            if ( ignoreReadonly )
                enumerationOptions.AttributesToSkip |= FileAttributes.ReadOnly;
            if ( ignoreReparsePoints )
                enumerationOptions.AttributesToSkip |= FileAttributes.ReparsePoint;
            eventLog.Source = "AudioDuplicateFinder";
            eventLog.Clear();
            eventLog.WriteEntry("This is a test from AudioDuplicateFinder");
        }
        public void StartSearch()
        {
            StartSearch(IncludeDir, parallelOptions, 0);
        }

        private void StartSearch(String[] includeDir, ParallelOptions options, int Tier)
        {
            Parallel.ForEach(includeDir, options, item =>
            {
                Console.WriteLine("Sequential iteration on item '{0}' running on thread {1}.", item, Thread.CurrentThread.ManagedThreadId);
                if ( !options.CancellationToken.IsCancellationRequested )
                {
                    List<FileInfo> files = new (); // An instance for each thread which will get appended to _fileInfos before thread ends.
                    Queue<DirectoryInfo> subFolders = new();
                    subFolders.Enqueue(new(item));
                    DirectoryInfo currentFolder = subFolders.Dequeue();
                    try
                    {
                        bool includeImages = (filetypes & FileTypes.ImageFiles) != 0;
                        bool includeAudio = (filetypes & FileTypes.AudioFiles) != 0;
                        files.AddRange(currentFolder.EnumerateFiles("*", enumerationOptions)
                            .Where(f => ((filetypes & FileTypes.ImageAndAudio) != 0 ? AllExtensions : (includeImages ? VideoImagesExtensions : (includeAudio ? VideoAudioExtensions : VideoExtensions)))
                            .Any(x => f.FullName.EndsWith(x, StringComparison.OrdinalIgnoreCase))));

                        foreach ( DirectoryInfo subFolder in currentFolder.EnumerateDirectories("*", enumerationOptions)
                            .Where(d => !ExcludeDir.Any(x => d.FullName.Equals(x, StringComparison.OrdinalIgnoreCase))) )
                            subFolders.Enqueue(subFolder);
                        String[] folders = new String[subFolders.Count];
                        foreach(var folder in subFolders)
                            folders.Append(folder.FullName);
                        StartSearch(folders, options, Tier + 1);
                    }
                    catch ( DirectoryNotFoundException ) { }
                    catch ( Exception e )
                    {
                        lock ( eventLog )
                        {
                            eventLog.WriteEntry($"Failed to enumerate '{currentFolder.FullName}' because of: {e}");
                        }
                    }

                    lock (_fileInfos)
                    {
                        _fileInfos.AddRange(files);
                    }
                }
            });
        }

        public List<FileInfo> GetFileList()
        {
            lock ( _fileInfos )
            {
                List<FileInfo> f = new(_fileInfos);
                return f; // return a copy so-as to avoid multithread race issues.
            }
        }

        private void ProccessFile(string filename)
        {

        }
        public void StopSearch() => cts.Cancel();
    }
}
