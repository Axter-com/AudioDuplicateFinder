using System.Windows.Controls;

using MahApps.Metro.Controls;

namespace AudioDuplicateFinder.Contracts.Views;

public interface IShellWindow
{
    Frame GetNavigationFrame();

    void ShowWindow();

    void CloseWindow();

    Frame GetRightPaneFrame();

    SplitView GetSplitView();
}
