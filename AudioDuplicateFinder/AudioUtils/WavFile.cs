using System;
using System.IO;

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
            get => _Sound ??= new(fileName);
            private set=> _Sound = value;
        }
        public string fileName { get;}
        public WavFile(string filename)
        {
            fileName = filename;
            try
            {
                Sound.ReadWavFile();
            }
            catch ( Exception ex )
            {
                // ToDo: Change this to console output
                MessageBox.Show($"Error while reading the sound file {filename}:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
   }
}
