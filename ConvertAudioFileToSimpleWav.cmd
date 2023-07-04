@ECHO OFF
setlocal ENABLEDELAYEDEXPANSION
:: This script converts audio files to a simple basic wav format.
:: This allows a comparison tool (https://github.com/David-Maisonave/SoundComparer2) to be able to 
:: compare audio files to see if they're duplicates fairly accurately even if the original files are in different formats or file types.
:: Requirements:
::				ffmpeg.exe

:: Change the following path to point to ffmpeg.exe if ffmpeg.exe is not in the environmental path
set FFMPEG="ffmpeg.exe"
:: Change the following folder name to the desired target folder to send the converted files
set WorkingPath=Conversions

:: Ussing ffmpeg, convert audio file to *.wav type file with AudioCodec=PCM, Stream=1, Channel=Mono, Quality=Lossless, SampleRate=8k, BitDepth=8
set PRE_ARG=-y -i
set POST_ARG=-vn -c:a pcm_u8 -ar 8000 -rematrix_maxval 1.0 -ac 1 -map 0:a:0? -sn -map_chapters 0 -map_metadata 0 -f wav -threads 0

:: Example: ffmpeg.exe -y -i Z:\TestFiles\MediaTestFiles\Audio\Grp#35_Original.mp3 -vn -c:a pcm_u8 -ar 8000 -rematrix_maxval 1.0 -ac 1 -map 0:a:0? -sn -map_chapters 0 -map_metadata 0 -f wav -threads 0 C:\Users\admin3\AppData\Local\Temp\AudioDuplicateFinder\ConvertedFilesFolder\Grp#35_Original.mp3.wav
:: ffmpeg.exe binary with expected results:
::			E:\Repos\David-Maisonave\MediaFileDuplicateFinder\Test_Data\ProjectsToIncorporateIntoMFDF\Axiom_FFmpegGUI\Axiom.FFmpeg\ffmpeg\bin
::			
::			
::			

:: Failed conversions in AudioDuplicateFinder
::			Grp#30_FileTypeTest.ogg__928066a1-539e-4dfe-9f17-18bf74a668f4.delete_me.fft
::			Grp#31_FileTypeTest.mp3__33ca9915-2bc8-48a7-a1bd-cb1b4913c4a4.delete_me.fft.wav
::			
::			P?? "C:\\Users\\admin3\\AppData\\Local\\Temp\\AudioDuplicateFinder\\ConvertedFilesFolder\\Grp#30_FileTypeTest.amr__1c7cbcb8-3cc5-4497-9936-b8fd1292dbc2.delete_me.fft.wav"
::			C?? "C:\\Users\\admin3\\AppData\\Local\\Temp\\AudioDuplicateFinder\\ConvertedFilesFolder\\Grp#30_FileTypeTest.ogg__928066a1-539e-4dfe-9f17-18bf74a668f4.delete_me.fft.wav"
::			
::			
::			
::			

:: Optionally uncomment the following line and set a source path
:: cd /D E:\Repos\David-Maisonave\MediaFileDuplicateFinder\Test_Data\MediaTestFiles\Audio

mkdir %WorkingPath%
for /f "delims=" %%Q in ('dir *.* /b /a-d-h-s') do (
	%FFMPEG% %PRE_ARG% "%%Q" %POST_ARG% "%WorkingPath%\%%~nQ_ConvertFrom%%~xQ_PCM_8k_8_mono.wav"
)











