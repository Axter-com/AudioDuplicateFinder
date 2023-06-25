/*
//      Copyright © 2023 David Maisonave
//      GPLv3 License
*/
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Controls;
using System.Windows.Shapes;

using ProtoBuf;
using ProtoBuf.Meta;


namespace AudioDuplicateFinder.FileUtils
{
    [ProtoContract]
    [DebuggerDisplay("{" + nameof(Path) + ",nq}")]
    public class MediaFile : FileProperty
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        public MediaFile() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.
        public MediaFile(string file) : this(new FileInfo(file)) { }
        public MediaFile(FileInfo fi) : this(new FileProperty()
        {
            fileInfo = fi
        }){ }
        public MediaFile(FileProperty file)
        {
            fileInfo = file.fileInfo;
            Duration = file.Duration;
            CheckSum = file.CheckSum;
            FingerPrint = file.FingerPrint;
            grayBytes = new Dictionary<double, byte[]?>();
        }
        //[ProtoMember(1)]
        //internal string _Path;
        //[ProtoMember(2)]
        //public string Folder;
        [ProtoMember(3)]
        public Dictionary<double, byte[]?> grayBytes;
        [ProtoMember(4)]
        public MediaInfo? mediaInfo;
        //[ProtoMember(5)]
        //public DateTime DateCreated;
        //[ProtoMember(6)]
        //public DateTime DateModified;
        //[ProtoMember(7)]
        //public long FileSize;

        [ProtoIgnore]
        internal bool invalid = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetGrayBytesIndex(float position) => mediaInfo!.Duration.TotalSeconds * position;

        public override bool Equals(object? obj) =>
            obj is MediaFile entry &&
            Path.Equals(entry.Path, CoreUtils.IsWindows ?
                StringComparison.OrdinalIgnoreCase :
                StringComparison.Ordinal);

        public override int GetHashCode() => CoreUtils.IsWindows ?
            StringComparer.OrdinalIgnoreCase.GetHashCode(Path) :
            HashCode.Combine(Path);

    }
}