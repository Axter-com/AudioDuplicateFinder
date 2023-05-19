using System.Windows.Controls;

using AudioDuplicateFinder.Contracts.Views;
using AudioDuplicateFinder.ViewModels;

using MahApps.Metro.Controls;

namespace AudioDuplicateFinder.Views;

public partial class ShellDialogWindow : MetroWindow, IShellDialogWindow
{
    public ShellDialogWindow(ShellDialogViewModel viewModel)
    {
        InitializeComponent();
        viewModel.SetResult = OnSetResult;
        DataContext = viewModel;
    }

    public Frame GetDialogFrame()
        => dialogFrame;

    private void OnSetResult(bool? result)
    {
        DialogResult = result;
        Close();
    }
}
