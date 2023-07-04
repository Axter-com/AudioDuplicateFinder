using System;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.FileProviders;

namespace AudioDuplicateFinder.AudioUtils
{
	/// <summary>
	/// Gets details of a WAV file
	/// </summary>
	public class WavFile
    {
        private WaveSound _Sound;
        public WaveSound Sound
        {// This is the WaveFile class variable that describes the internal structures of the .WAV
            get => _Sound ??= new(fileInfo, isValidWavFile);
            private set=> _Sound = value;
        }
        public string fileName { get;}
        public FileInfo fileInfo { get; }
        private bool isValidWavFile = true;
        public WavFile(FileInfo fi, bool IsValidWavFile = true)
        {
            fileInfo = fi;
            isValidWavFile =IsValidWavFile;
            fileName = fi.FullName;
            try
            {
                Sound.ReadWavFile();
            }
            catch ( Exception ex )
            {
                // ToDo: Change this to console output
                MessageBox.Show($"Error while reading the sound file {fileName}:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
