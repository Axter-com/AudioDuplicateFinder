/*
//      Copyright © 2023 David Maisonave
//      GPLv3 License
*/
using System;
using System.Globalization;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Controls;

namespace AudioDuplicateFinder.FileUtils
{
    public class DuplicateFile : FileProperty
    {
        public DuplicateFile() { }
        public DuplicateFile(FileProperty fp, float difference, Guid groupID)
        {
            fileInfo = fp.fileInfo;
            Duration = fp.Duration;
            CheckSum = fp.CheckSum;
            FingerPrint = fp.FingerPrint;

            GroupId = groupID;
            FileNameLength = Path.Length;
            DirNameLength = ParentDir.Length;
            DateCreated = fp.fileInfo.CreationTimeUtc;
            DateModified = fp.fileInfo.LastWriteTimeUtc;
            DateAccess = fp.fileInfo.LastAccessTimeUtc;
            Similarity = (1f - difference) * 100; // ToDo-Chk: Check if this math is needed.
            FileStillExists = fp.fileInfo.Exists;
            // Size = fp.fileInfo.Exists ? fp.fileInfo.Length : -1;
            if ( fp.IsImage )
                Format = fp.fileInfo.Extension[1..];

            MediaFile file = new (fp);
            if ( IsVideo && file.mediaInfo?.Streams?.Length > 0 )
            {
                Duration = file.mediaInfo.Duration;
                /*
					Stream selection rules:
					See: https://ffmpeg.org/ffmpeg.html#Automatic-stream-selection
					In the absence of any map options[...] It will select that stream based upon the following criteria:
					for video, it is the stream with the highest resolution,
					for audio, it is the stream with the most channels,
					In the case where several streams of the same type rate equally, the stream with the lowest index is chosen.
				*/
                int[] selVideo = { -1, 0 };
                int[] selAudio = { -1, 0 };
                for ( int i = file.mediaInfo.Streams.Length - 1 ; i >= 0 ; i-- )
                {
                    if ( file.mediaInfo.Streams[i].CodecType?.Equals("video", StringComparison.OrdinalIgnoreCase) == true &&
                        file.mediaInfo.Streams[i].Width * file.mediaInfo.Streams[i].Height >= selVideo[1] )
                    {
                        selVideo[0] = i;
                        selVideo[1] = file.mediaInfo.Streams[i].Width * file.mediaInfo.Streams[i].Height;
                    }
                    else if ( file.mediaInfo.Streams[i].CodecType?.Equals("audio", StringComparison.OrdinalIgnoreCase) == true &&
                             file.mediaInfo.Streams[i].Channels >= selAudio[1] )
                    {
                        selAudio[0] = i;
                        selAudio[1] = file.mediaInfo.Streams[i].Channels;
                    }
                }

                if ( selVideo[0] >= 0 )
                {
                    int i = selVideo[0];
                    Format = file.mediaInfo.Streams[i].CodecName ?? "<Unknown>";
                    Fps = file.mediaInfo.Streams[i].FrameRate;
                    BitRateKbs = Math.Round((decimal) file.mediaInfo.Streams[i].BitRate / 1000);
                    FrameSize = file.mediaInfo.Streams[i].Width + "x" + file.mediaInfo.Streams[i].Height;
                    FrameSizeInt = file.mediaInfo.Streams[i].Width + file.mediaInfo.Streams[i].Height;
                }
                if ( selAudio[0] >= 0 )
                {
                    int i = selAudio[0];
                    AudioFormat = file.mediaInfo.Streams[i].CodecName ?? "<Unknown>";
                    AudioChannel = file.mediaInfo.Streams[i].ChannelLayout ?? "<Unknown>";
                    AudioSampleRate = file.mediaInfo.Streams[i].SampleRate;
                }
            }
            else if (IsImage)
            {
                //We have only one stream if its an image
                if ( file.mediaInfo?.Streams?.Length > 0 )
                {
                    FrameSize = file.mediaInfo.Streams[0].Width + "x" + file.mediaInfo.Streams[0].Height;
                    FrameSizeInt = file.mediaInfo.Streams[0].Width + file.mediaInfo.Streams[0].Height;
                }
            }
        }
        public string ByteSize => BytesToString(Size);

        // Json members
        [JsonInclude]
        public Guid GroupId { get; set; } // Set needs to be public to allow merging groups together
        [JsonInclude]
        public DateTime DateCreated { get; private set; }
        [JsonInclude]
        public DateTime DateModified { get; private set; }
        [JsonInclude]
        public DateTime DateAccess { get; private set; }
        [JsonInclude]
        public float Similarity { get; set; } // ToDo-Chk: Check if this should be changed to double; See if set can be private
        [JsonInclude]
        public string FrameSize { get; private set; }
        [JsonInclude]
        public int FrameSizeInt { get; private set; }
        [JsonInclude]
        public string Format { get; private set; }
        [JsonInclude]
        public string AudioFormat { get; private set; }
        [JsonInclude]
        public string AudioChannel { get; private set; }
        [JsonInclude]
        public int AudioSampleRate { get; private set; }
        [JsonInclude]
        public decimal BitRateKbs { get; private set; }
        [JsonInclude]
        public float Fps { get; private set; }
        [JsonInclude]
        public int FileNameLength { get; private set; }
        [JsonInclude]
        public int DirNameLength { get; private set; }
        [JsonInclude]
        public bool FileStillExists { get; private set; }

        // Member value set if best or longest within a group of duplicates
        [JsonInclude]
        public bool IsBestDuration { get; set; }
        [JsonInclude]
        public bool IsBestFrameSize { get; set; }
        [JsonInclude]
        public bool IsBestBitRateKbs { get; set; }
        [JsonInclude]
        public bool IsBestFps { get; set; }
        [JsonInclude]
        public bool IsBestAudioSampleRate { get; set; }
        [JsonInclude]
        public bool IsLongestFileNameLength { get; set; }
        [JsonInclude]
        public bool IsLongestDirNameLength { get; set; }
        [JsonInclude]
        public bool IsBestSize { get; set; } // ToDo-Chk: Check how best size is determine

        static readonly string[] suf = { " B", " KB", " MB", " GB", " TB", " PB", " EB" };
        public static string BytesToString(long byteCount)
        {
            if ( byteCount == 0 )
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString(CultureInfo.InvariantCulture) + suf[place];
        }
    }
}