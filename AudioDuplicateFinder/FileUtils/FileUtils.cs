using System;
using System.IO;
using System.Windows.Shapes;

using Path = System.IO.Path;

namespace AudioDuplicateFinder.FileUtils
{
    public class Files
    {
        private static string _AppTempFolder;
        private static string TempFolderSub = "AudioDuplicateFinder";
        public static string AppTempFolder
        {
            get 
            {
                if (_AppTempFolder == null)
                {
                    lock( TempFolderSub )
                    {
                        _AppTempFolder = $"{Path.GetTempPath()}{TempFolderSub}{Path.DirectorySeparatorChar}";
                        if (!Path.Exists(_AppTempFolder))
                            Directory.CreateDirectory(_AppTempFolder);
                        //else if ( CleanPreviousSessionTempFiles )
                        //    DeleteDirectoryFiles(_AppTempFolder );
                    }
                }
                return _AppTempFolder;
            }
            private set => _AppTempFolder = value;
        }
        public static bool CleanPreviousSessionTempFiles { get; } = true;
        public static void DeleteDirectoryFiles(string dirPath)
        {
            // To be safe, only delete sub sub sub directories
            if ( dirPath.Length > 4 && dirPath.Count(t => t == Path.DirectorySeparatorChar) > 3 && Directory.Exists(dirPath) )
            {
                Directory.Delete(dirPath, true);
                Directory.CreateDirectory(dirPath);
            }
        }
    }
}