using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.Specialized;
using CtrlListbox = System.Windows.Controls.ListBox;

using AudioDuplicateFinder.ViewModels;
using AudioDuplicateFinder.Properties;

using Microsoft.Win32;
using System.Linq;

namespace AudioDuplicateFinder.Views;

public partial class MainPage : Page
{
    public static MainPage mainPage = null;
    private StringCollection IncludeDirLastState = null;
    private StringCollection ExcludeDirLastState = null;
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

        foreach ( object flder in listBox.Items )
        {
            if ( flder.ToString().Equals(PreviousValue, StringComparison.InvariantCultureIgnoreCase) )
                continue;
            string folder = flder + "\\";
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

    }

    private void StopScan_Click(object sender, System.Windows.RoutedEventArgs e)
    {

    }
    private void StartScan_Click(object sender, System.Windows.RoutedEventArgs e)
    {

    }
}
