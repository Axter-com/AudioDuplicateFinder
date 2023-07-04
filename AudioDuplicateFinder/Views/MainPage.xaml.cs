/*
//      Copyright © 2023 David Maisonave
//      GPLv3 License
*/

using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.Specialized;
using CtrlListbox = System.Windows.Controls.ListBox;
using System.Data.SQLite;
using SQLiteExtensions;

using AudioDuplicateFinder.ViewModels;
using AudioDuplicateFinder.Properties;
using AudioDuplicateFinder.FileUtils;
using AudioDuplicateFinder.SQL;

using Microsoft.Win32;
using System.Linq;
using System.IO;
using System.Diagnostics;
using AudioDuplicateFinder.Services;
using AudioDuplicateFinder.Core.Models;
using System.Collections.Generic;
using AudioDuplicateFinder.Core.Services;
using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AudioDuplicateFinder.Views;

public class MyUserState
{
    public double Maximum { get; set; }
    public string JobStatus { get; set; }
    public string LeftStatusText { get; set; }
    public string CenterStatusText { get; set; }
}
public partial class MainPage : Page
{
    public static MainPage mainPage = null;
    private StringCollection IncludeDirLastState = null;
    private StringCollection ExcludeDirLastState = null;
    private FileSearch filesearch;
    private BackgroundWorker lastSender = null;
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        mainPage = this;
        if ( Settings.Default.IncludeDir != null )
            foreach ( object includedir in Settings.Default.IncludeDir)
                lstVw_IncludeDir.Items.Add(includedir);
        if ( Settings.Default.ExcludeDir != null )
            foreach ( object excludedir in Settings.Default.ExcludeDir )
                lstVw_ExcludeDir.Items.Add(excludedir);
        if ( lstVw_IncludeDir.Items.Count > 0 )
            leftStatusText.Text = "Press 'Start Scan'";
        else
            leftStatusText.Text = "Add paths to 'Include Dir' list.";
    }

    public void closeMethod()
    {
        SaveLastState(lstVw_IncludeDir, ref IncludeDirLastState);
        SaveLastState(lstVw_ExcludeDir, ref ExcludeDirLastState);
        Settings.Default.IncludeDir = IncludeDirLastState;
        Settings.Default.ExcludeDir = ExcludeDirLastState;

        Settings.Default.Save();
    }

    private static CtrlListbox SaveLastState(CtrlListbox listBox, ref StringCollection LastState)
    {
        LastState = new StringCollection();
        foreach ( object dir in listBox.Items )
            LastState.Add(dir.ToString());
        return listBox;
    }

    private bool IsFolderInList(CtrlListbox listBox, bool ReplaceItem, string SelectedPath, string PreviousValue)
    {
        SelectedPath = SelectedPath + "\\";

        // Check for duplicate
        if ( listBox.Items.Contains(SelectedPath) )
        {
            if ( !ReplaceItem )
                MessageBox.Show("Selected directory already exist in list.", "Directory Already Exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return true;
        }

        foreach ( object f in listBox.Items )
        {
            if ( ReplaceItem && f.ToString().Equals(PreviousValue, StringComparison.InvariantCultureIgnoreCase) )
                continue;
            string folder = f + "\\";
            // Check if existing directory is root of selected path
            if ( SelectedPath.StartsWith(folder.ToString(), StringComparison.InvariantCultureIgnoreCase) )
            {
                MessageBox.Show($"Can not add selected directory, because another directory in the list is a root of the selected directory.\nTo add '{SelectedPath}', first remove path '{folder}'.", "Directory In Collection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            // Check if root of another directory
            if ( folder.ToString().StartsWith(SelectedPath, StringComparison.InvariantCultureIgnoreCase) )
            {
                MessageBox.Show($"Can not add selected directory, because it's a root of another directory in the list.\nTo add '{SelectedPath}', first remove path '{folder}'.", "Directory Is Root of Another", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
        }
        return false;
    }
    private void AddDirectory(CtrlListbox listBox, bool ReplaceItem = false)
    {
        System.Windows.Forms.FolderBrowserDialog dlg = new();
        if ( ReplaceItem )
        {
            if ( listBox.SelectedIndex < 0 )
                return;
            dlg.SelectedPath = listBox.SelectedValue.ToString();
        }
        if ( dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK )
        {
            // Check if new item is already in list
            if(!IsFolderInList(listBox, ReplaceItem, dlg.SelectedPath, ReplaceItem ? listBox.SelectedValue.ToString() : "") )
            {
                if ( ReplaceItem )
                {
                    int idx = listBox.SelectedIndex;
                    listBox.Items.Remove(listBox.SelectedValue);
                    listBox.Items.Insert(idx, dlg.SelectedPath);
                }
                else
                    listBox.Items.Add(dlg.SelectedPath);
            }
        }
    }

    private void RemoveSelectedItem(CtrlListbox listBox)
    {
        if ( listBox.SelectedIndex > -1 )
            listBox.Items.Remove(listBox.SelectedValue);
    }

    private void RemoveSelectedItem(System.Windows.Controls.ListView lstVw)
    {
        if ( lstVw.SelectedIndex > -1 )
            lstVw.Items.Remove(lstVw.SelectedValue);
    }

    private void AddIncludeDir_Click(object sender, System.Windows.RoutedEventArgs e) => AddDirectory(SaveLastState(lstVw_IncludeDir, ref IncludeDirLastState));
    private void RemoveIncludeDir_Click(object sender, System.Windows.RoutedEventArgs e) => RemoveSelectedItem(SaveLastState(lstVw_IncludeDir, ref IncludeDirLastState));
    private void EditIncludeDir_Click(object sender, System.Windows.RoutedEventArgs e) => AddDirectory(SaveLastState(lstVw_IncludeDir, ref IncludeDirLastState), true);
    public void IncludeDir_MouseDoubleClick(object sender, MouseButtonEventArgs e) => AddDirectory(SaveLastState(lstVw_IncludeDir, ref IncludeDirLastState), true);

    private void AddExcludeDir_Click(object sender, System.Windows.RoutedEventArgs e) => AddDirectory(SaveLastState(lstVw_ExcludeDir, ref ExcludeDirLastState));
    private void RemoveExcludeDir_Click(object sender, System.Windows.RoutedEventArgs e) => RemoveSelectedItem(SaveLastState(lstVw_ExcludeDir, ref ExcludeDirLastState));
    private void EditExcludeDir_Click(object sender, System.Windows.RoutedEventArgs e) => AddDirectory(SaveLastState(lstVw_ExcludeDir, ref ExcludeDirLastState), true);
    public void ExcludeDir_MouseDoubleClick(object sender, MouseButtonEventArgs e) => AddDirectory(SaveLastState(lstVw_ExcludeDir, ref ExcludeDirLastState), true);

    private void ResetIncludeDir_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        SaveLastState(lstVw_IncludeDir, ref IncludeDirLastState).Items.Clear();
        if ( Settings.Default.IncludeDir != null )
            foreach ( object includedir in Settings.Default.IncludeDir )
                lstVw_IncludeDir.Items.Add(includedir);
    }
    private void ClearAll_IncludeDir_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        SaveLastState(lstVw_IncludeDir, ref IncludeDirLastState).Items.Clear();
    }

    private void UndoIncludeDir_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if ( IncludeDirLastState != null )
        {
            StringCollection IncludeDirLastState_temp = new ();
            foreach ( object includedir in lstVw_IncludeDir.Items )
                IncludeDirLastState_temp.Add(includedir.ToString());
            lstVw_IncludeDir.Items.Clear();
            foreach ( object includedir in IncludeDirLastState )
                lstVw_IncludeDir.Items.Add(includedir);
            IncludeDirLastState = IncludeDirLastState_temp;
        }
    }

    private void ClearAll_ExcludeDir_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        SaveLastState(lstVw_ExcludeDir, ref ExcludeDirLastState).Items.Clear();
    }

    private void UndoExcludeDir_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if ( ExcludeDirLastState != null )
        {
            StringCollection ExcludeDirLastState_temp = new ();
            foreach ( object Excludedir in lstVw_ExcludeDir.Items )
                ExcludeDirLastState_temp.Add(Excludedir.ToString());
            lstVw_ExcludeDir.Items.Clear();
            foreach ( object Excludedir in ExcludeDirLastState )
                lstVw_ExcludeDir.Items.Add(Excludedir);
            ExcludeDirLastState = ExcludeDirLastState_temp;
        }
    }

    private void ResetExcludeDir_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        SaveLastState(lstVw_ExcludeDir, ref ExcludeDirLastState).Items.Clear();
        if ( Settings.Default.IncludeDir != null )
            foreach ( object excludedir in Settings.Default.ExcludeDir )
                lstVw_ExcludeDir.Items.Add(excludedir);
    }

    private void PauseScan_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if ( filesearch != null )
            filesearch.PauseSearch();
    }

    private void StopScan_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if ( filesearch != null )
            filesearch.StopSearch();
    }
    void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        MyUserState state = e.UserState as MyUserState;
        if ( state != null )
        {
            if ( state.Maximum >  0 )
                pbStatus.Maximum = state.Maximum;
            else
                pbStatus.Maximum++;
            if ( state.JobStatus != null )
            {
                leftStatusText.Text = state.LeftStatusText;
                centerStatusText.Text = state.CenterStatusText;
                if( state.JobStatus.Equals("Complete") )
                    ShellViewModel.shellViewModel.OnMenuViewsDuplicateGroups();
            }
        }
        lock ( pbStatus )
        {
            if ( e.ProgressPercentage == -1 )
            {
                pbStatus.Value++;
                if ( pbStatus.Value > pbStatus.Maximum )
                    pbStatus.Maximum = pbStatus.Maximum * 2;
            }
            else if ( e.ProgressPercentage > -1 )
                pbStatus.Value = e.ProgressPercentage;
        }
    }
    private void StartScan_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        BackgroundWorker worker = new  (){ WorkerReportsProgress = true };
        worker.DoWork += StartScanTask;
        worker.ProgressChanged += worker_ProgressChanged;

        worker.RunWorkerAsync();
    }
    private void StartScanTask(object sender, DoWorkEventArgs e)
    {
        lastSender = sender as BackgroundWorker;
        lastSender.ReportProgress(0, new MyUserState { Maximum = 100, JobStatus = "StartScanTask", LeftStatusText = "", CenterStatusText = "" });
        if ( filesearch != null )
            filesearch.StopSearch();
        SaveLastState(lstVw_IncludeDir, ref IncludeDirLastState);
        SaveLastState(lstVw_ExcludeDir, ref ExcludeDirLastState);
#if DEBUG
        int MaxParallel = -1; // Easier for debugging purposes
#else
        int MaxParallel = -1;
#endif
        filesearch = new(IncludeDirLastState, ExcludeDirLastState, lastSender, MaxParallel);
        // filesearch.fileTypes = FileSearch.FileTypes.AudioFiles;
        filesearch.StartSearch();
        List<FileProperty> fileInfos = filesearch.GetFileList();
        HashSet<DuplicateFile> DuplicateFiles = filesearch.GetDuplicateList();

        // This SQLite method is faster and can easily use "insert or replace"
        //        string createTableCmd = "CREATE TABLE \"FileProperties\" (\r\n\t\"ParentDir\"\tTEXT NOT NULL,\r\n\t\"Name\"\tTEXT NOT NULL,\r\n\t\"Ext\"\tTEXT NOT NULL,\r\n\t\"MediaType\"\tINTEGER NOT NULL DEFAULT 0,\r\n\t\"Size\"\tINTEGER NOT NULL DEFAULT 0,\r\n\t\"Duration\"\tINTEGER NOT NULL DEFAULT 0,\r\n\t\"CheckSum\"\tINTEGER NOT NULL DEFAULT 0,\r\n\t\"FingerPrint\"\tBLOB,\r\n\tPRIMARY KEY(\"Name\",\"ParentDir\")\r\n);";
        //        using ( SQLiteConnection? conn = SqliteExt.CreateConnection(".\\Database\\mfdf.db", createTableCmd) )
        //        {
        //            SQLiteCommand sql_cmd = conn.CreateCommand();
        //#if DEBUG
        //            sql_cmd.DeleteFrom("FileProperties");
        //#endif

        //            foreach ( FileInfo file in fileInfos )
        //                sql_cmd.Execute($"INSERT OR REPLACE into FileProperties (ParentDir, Name, Ext, Size) values (\"{file.Name}\", \"{file.DirectoryName}\", \"{file.Extension}\", {file.Length})");
        //            //sql_cmd.qu
        //        }

        // Too slow. About 30% slower
        //FilePropertiesContext filePropertiesContext = new ();
        //using ( FilePropertiesContext db = new() )
        //    foreach ( FileInfo file in fileInfos )
        //    {
        //        db.Add(new FileProperty()
        //        {
        //            Name = file.Name,
        //            ParentDir = file.DirectoryName,
        //            Ext = file.Extension,
        //            Size = file.Length
        //        });
        //    }
        List < MediaFileInfo > mediaFiles = new ();
        foreach ( DuplicateFile file in DuplicateFiles )
            mediaFiles.Add(new MediaFileInfo()
            {
                GUID = file.GroupId,
                Size = file.fileInfo.Length,
                Name = file.fileInfo.Name,
                Ext = file.fileInfo.Extension,
                DirectoryName = file.fileInfo.DirectoryName,
                Duration = file.Duration.TotalSeconds,
                IsReadOnly = file.fileInfo.IsReadOnly
            });
        MediaFileDataService.SetMediaFileInfo(mediaFiles);
        Debug.WriteLine($"*****************Completed scan with {fileInfos.Count} items found.");
        lastSender.ReportProgress(-1, new MyUserState { Maximum = -1, JobStatus = "Complete", LeftStatusText = "San Complete", CenterStatusText = $"{fileInfos.Count} items found" });
    }
}
