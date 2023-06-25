/*
//      Copyright © 2023 David Maisonave
//      GPLv3 License
*/
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace AudioDuplicateFinder.FileUtils
{    
    public class FileExtensions
    {
        // //////////////////////////////////////////////////////////////////////////////////////////////
        // Extensions
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
        public static readonly string[] MediaExtensions = VideoExtensions.Concat(ImageExtensions).Concat(AudioExtensions).ToArray();
        public static readonly string[] OfficeExtensions = DocExtensions.Concat(SpreadSheetExtensions).Concat(PresentationExtensions).ToArray();
        public static readonly string[] OfficeAndTextExtensions = TextExtensions.Concat(DocExtensions).Concat(SpreadSheetExtensions).Concat(PresentationExtensions).ToArray();
        public static readonly string[] AllExtensions = VideoExtensions.Concat(ImageExtensions).Concat(AudioExtensions).Concat(TextExtensions).Concat(DocExtensions).Concat(SpreadSheetExtensions).Concat(PresentationExtensions).ToArray();
    }
}