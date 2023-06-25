/*
//      Copyright © 2023 David Maisonave
//      GPLv3 License
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Data.Entity;
using AudioDuplicateFinder.SQL;
using AudioDuplicateFinder.FileUtils;
using AudioDuplicateFinder.AudioUtils;
using System.Windows;
using AudioDuplicateFinder.Properties;

namespace AudioDuplicateFinder.FileUtils
{
    public class FileSearch
    {
        // Public modifiable
        public FileTypes fileTypes { get; set; } = FileTypes.MediaFiles;
        public bool ignoreReadonly { get; set; } = true;
        public bool ignoreReparsePoints { get; set; } = true;
        public float minimumAccuracy { get; set; } = 96f;
        // Public read only access
        public HashSet<DuplicateFile> Duplicates { get; private set; } = new HashSet<DuplicateFile>();
        public List<FileProperty> fileInfoList { get; private set; } = new List<FileProperty>();
        public string[] AllowedExtensions { get; private set; } = null;

        public FileSearch(StringCollection i, StringCollection x, int MaxParallel = -1) // ToDo: Set MaxParallel to a program settings value
        {
            if ( MaxParallel == 0 )
                maxParallel = 1; 
            else if ( MaxParallel < 0 || MaxParallel > MAX_PARALLEL )
                maxParallel = MAX_PARALLEL;
            else
                maxParallel = MaxParallel;
            this.IncludeDir = new string[i.Count];
            i.CopyTo(this.IncludeDir, 0);
            this.ExcludeDir = new string[x.Count];
            x.CopyTo(this.ExcludeDir, 0);
            //token = tokenSource.Token;
            parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = maxParallel, 
                //Set the CancellationToken value
                CancellationToken = cts.Token
            };
            enumerationOptions = new()
            {
                IgnoreInaccessible = true,
                AttributesToSkip = FileAttributes.System
            };
            if ( ignoreReadonly )
                enumerationOptions.AttributesToSkip |= FileAttributes.ReadOnly;
            if ( ignoreReparsePoints )
                enumerationOptions.AttributesToSkip |= FileAttributes.ReparsePoint;
        }
        public void StartSearch()
        {
            List<string> list = new();
            if ( fileTypes == FileTypes.MediaFiles )
                list.AddRange(FileExtensions.MediaExtensions);
            else if ( fileTypes == FileTypes.AllFiles )
                list.AddRange(FileExtensions.AllExtensions);
            else if ( fileTypes == FileTypes.OfficeAndTextFiles )
                list.AddRange(FileExtensions.OfficeAndTextExtensions);
            else if ( fileTypes == FileTypes.OfficeFiles )
                list.AddRange(FileExtensions.OfficeExtensions);
            else
            {   // Do itemize add
                if ( (fileTypes & FileTypes.VideoFiles) != 0 )
                    list.AddRange(FileExtensions.VideoExtensions);
                if ( (fileTypes & FileTypes.ImageFiles) != 0 )
                    list.AddRange(FileExtensions.ImageExtensions);
                if ( (fileTypes & FileTypes.AudioFiles) != 0 )
                    list.AddRange(  FileExtensions.AudioExtensions);
                if ( (fileTypes & FileTypes.TextFiles) != 0 )
                    list.AddRange(FileExtensions.TextExtensions);
                if ( (fileTypes & FileTypes.DocFiles) != 0 )
                    list.AddRange(FileExtensions.DocExtensions);
                if ( (fileTypes & FileTypes.SpreadSheetFiles) != 0 )
                    list.AddRange(FileExtensions.SpreadSheetExtensions);
                if ( (fileTypes & FileTypes.PresentationFiles) != 0 )
                    list.AddRange(FileExtensions.PresentationExtensions);
            }

            AllowedExtensions = list.ToArray();
            lock ( lockUntilComplete )
            {
                DoFileSearch(IncludeDir, parallelOptions, 0);
                DoDuplicateCompare(parallelOptions);
                int count = Duplicates.Count;
                Debug.WriteLine($"Info: Found {count} duplicates");
            }
        }
        private void PauseIfOnHold()
        {
                while(OnHold)
                {
                    Thread.Sleep(3000);
                    // Task.Delay(3000);
                }
        }
        private DirectoryInfo PauseIfOnHold(DirectoryInfo f)
        {
            PauseIfOnHold();
            return f;
        }
        private string PauseIfOnHold(string f)
        {
            PauseIfOnHold();
            return f;
        }

        private void DoFileSearch(string[] includeDir, ParallelOptions options, int Tier)
        {
            ParallelOptions optionCpy = new (){ MaxDegreeOfParallelism = maxParallel,  CancellationToken = cts.Token };
            Parallel.ForEach(includeDir, optionCpy, item =>
            {
                if ( item != null )
                {
                    Debug.WriteLine($"Sequential iteration on item '{item}' running on thread {Thread.CurrentThread.ManagedThreadId} Tier={Tier}.");
                    PauseIfOnHold();

                    if ( !options.CancellationToken.IsCancellationRequested )
                    {
                        List<FileInfo> files = new (); // An instance for each thread which will get appended to fileInfoList before thread ends.
                        Queue<DirectoryInfo> subFolders = new();
                        subFolders.Enqueue(new(item));
                        DirectoryInfo currentFolder = subFolders.Dequeue();
                        try
                        {
                            bool includeImages = (fileTypes & FileTypes.ImageFiles) != 0;
                            bool includeAudio = (fileTypes & FileTypes.AudioFiles) != 0;
                            files.AddRange(currentFolder.EnumerateFiles("*", enumerationOptions)
                                .Where(f => AllowedExtensions
                                .Any(x => PauseIfOnHold(f.FullName).EndsWith(x, StringComparison.OrdinalIgnoreCase))));

                            foreach ( DirectoryInfo subFolder in currentFolder.EnumerateDirectories("*", enumerationOptions)
                                .Where(d => !ExcludeDir.Any(x => d.FullName.Equals(x, StringComparison.OrdinalIgnoreCase))) )
                                subFolders.Enqueue(PauseIfOnHold(subFolder));
                            List<string> folders = new ();
                            foreach ( DirectoryInfo folder in subFolders )
                                folders.Add(folder.FullName);
                            DoFileSearch(folders.ToArray(), options, Tier + 1);
                        }
                        catch ( DirectoryNotFoundException ) { }
                        catch ( Exception e )
                        {
                            //lock ( eventLog )
                            //{
                            Debug.WriteLine/*eventLog.WriteEntry*/($"Failed to enumerate '{currentFolder.FullName}' because of: {e}; Tier={Tier}");
                            //}
                        }

                        List<FileProperty> f = new();
                        foreach(FileInfo fi  in files )
                            f.Add(new FileProperty(fi));
                        lock ( fileInfoList )
                        {
                            fileInfoList.AddRange(f);
                        }
                    }
                }
            });
        }

        private void DoDuplicateCompare(ParallelOptions options)
        {
            Dictionary<string, DuplicateFile> duplicateDict = new();
            ParallelOptions optionCpy = new (){ MaxDegreeOfParallelism = maxParallel,  CancellationToken = cts.Token };
            Parallel.For(0, fileInfoList.Count, optionCpy, i =>
            {
                PauseIfOnHold();
                FileProperty entry = fileInfoList[i];
                if ( entry.IsAudio && entry.waveControl != null ) // ToDo: Add code to temporarily convert audio to WAV type
                {
                    float accuracy = 0;
                    for ( int n = i + 1 ; n < fileInfoList.Count ; n++ )
                    {
                        PauseIfOnHold();
                        FileProperty compItem = fileInfoList[n];
                        if ( !compItem.IsAudio || compItem.waveControl == null ) // ToDo: Add code to temporarily convert audio to WAV type
                            continue;
                        accuracy = entry.waveControl.Sound.Compare(compItem.waveControl.Sound);
                        if ( accuracy >= minimumAccuracy )
                        {
                            lock ( duplicateDict )
                            {
                                bool foundBase = duplicateDict.TryGetValue(entry.Path, out DuplicateFile existingBase);
                                bool foundComp = duplicateDict.TryGetValue(compItem.Path, out DuplicateFile existingComp);
                                if ( foundBase && foundComp )
                                {
                                    //this happens with 4+ identical items:
                                    //first, 2+ duplicate groups are found independently, they are merged in this branch
                                    if ( existingBase!.GroupId != existingComp!.GroupId )
                                    {
                                        Guid groupID = existingComp!.GroupId;
                                        foreach ( DuplicateFile dup in duplicateDict.Values.Where(c =>
                                            c.GroupId == groupID) )
                                            dup.GroupId = existingBase.GroupId;
                                    }
                                }
                                else if ( foundBase )
                                {
                                    duplicateDict.TryAdd(compItem.Path,
                                        new DuplicateFile(compItem, accuracy, existingBase!.GroupId));
                                }
                                else if ( foundComp )
                                {
                                    duplicateDict.TryAdd(entry.Path,
                                        new DuplicateFile(entry, accuracy, existingComp!.GroupId));
                                }
                                else
                                {
                                    var groupId = Guid.NewGuid();
                                    duplicateDict.TryAdd(compItem.Path, new DuplicateFile(compItem, accuracy, groupId));
                                    duplicateDict.TryAdd(entry.Path, new DuplicateFile(entry, accuracy, groupId));
                                }
                            }
                        }
                    }
                }
            });
            Duplicates = new HashSet<DuplicateFile>(duplicateDict.Values);
            WaveSound.CleanUpFiles();
        }

        public List<FileProperty> GetFileList()
        {
            lock ( lockUntilComplete )
            {
                lock ( fileInfoList )
                {
                    List<FileProperty> f = new(fileInfoList);
                    return f; // return a copy so-as to avoid multithread race issues.
                }
            }
        }

        public HashSet<DuplicateFile> GetDuplicateList()
        {
            lock ( lockUntilComplete )
            {
                lock ( Duplicates )
                {
                    HashSet<DuplicateFile> f = new(Duplicates);
                    return f; // return a copy so-as to avoid multithread race issues.
                }
            }
        }

        private void ProccessFile(string filename)
        {

        }
        public void StopSearch() => cts.Cancel();
        public void PauseSearch() => OnHold = !OnHold;

        // //////////////////////////////////////////////////////////////////////////////////////////////
        // Private data members
        private string[] IncludeDir;
        private string[] ExcludeDir;
        private CancellationTokenSource cts = new ();
        private ParallelOptions parallelOptions;
        private EnumerationOptions enumerationOptions;
        //private EventLog eventLog;
        private int QtyFilesProcessed = 0;
        private bool OnHold = false;
        private string lockUntilComplete = "David-Maisonave";
        private readonly int MAX_PARALLEL = Environment.ProcessorCount -1;
        private int maxParallel = 0;

    }
}
