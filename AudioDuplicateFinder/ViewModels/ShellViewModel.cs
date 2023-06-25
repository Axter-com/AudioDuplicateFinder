using System.Windows;
using System.Windows.Input;

using AudioDuplicateFinder.Contracts.Services;
using AudioDuplicateFinder.Properties;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AudioDuplicateFinder.ViewModels;

// You can show pages in different ways (update main view, navigate, right pane, new windows or dialog)
// using the NavigationService, RightPaneService and WindowManagerService.
// Read more about MenuBar project type here:
// https://github.com/microsoft/TemplateStudio/blob/main/docs/WPF/projectTypes/menubar.md
public class ShellViewModel : ObservableObject
{
    public static ShellViewModel shellViewModel { get; private set ; } = null;
    private readonly INavigationService _navigationService;
    private readonly IRightPaneService _rightPaneService;

    private RelayCommand _goBackCommand;
    private ICommand _menuFileSettingsCommand;
    private ICommand _menuViewsWebViewCommand;
    private ICommand _menuViewsDuplicateListCommand;
    private ICommand _menuViewsContentGridCommand;
    private ICommand _menuViewsListDetailsCommand;
    private ICommand _menuViewsDuplicateGroupsCommand;
    private ICommand _menuViewsMainCommand;
    private ICommand _menuFileExitCommand;
    private ICommand _loadedCommand;
    private ICommand _unloadedCommand;

    public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(OnGoBack, CanGoBack));

    public ICommand MenuFileSettingsCommand => _menuFileSettingsCommand ?? (_menuFileSettingsCommand = new RelayCommand(OnMenuFileSettings));

    public ICommand MenuFileExitCommand => _menuFileExitCommand ?? (_menuFileExitCommand = new RelayCommand(OnMenuFileExit));

    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(OnUnloaded));

    public ShellViewModel(INavigationService navigationService, IRightPaneService rightPaneService)
    {
        _navigationService = navigationService;
        _rightPaneService = rightPaneService;
        shellViewModel = this;
    }

    private void OnLoaded()
    {
        _navigationService.Navigated += OnNavigated;
    }

    private void OnUnloaded()
    {
        _rightPaneService.CleanUp();
        _navigationService.Navigated -= OnNavigated;
    }

    private bool CanGoBack()
        => _navigationService.CanGoBack;

    private void OnGoBack()
        => _navigationService.GoBack();

    private void OnNavigated(object sender, string viewModelName)
    {
        GoBackCommand.NotifyCanExecuteChanged();
    }

    private void OnMenuFileExit()
        => System.Windows.Application.Current.Shutdown();

    public ICommand MenuViewsMainCommand => _menuViewsMainCommand ?? (_menuViewsMainCommand = new RelayCommand(OnMenuViewsMain));

    public void OnMenuViewsMain()
        => _navigationService.NavigateTo(typeof(MainViewModel).FullName, null, true);

    public ICommand MenuViewsDuplicateGroupsCommand => _menuViewsDuplicateGroupsCommand ?? (_menuViewsDuplicateGroupsCommand = new RelayCommand(OnMenuViewsDuplicateGroups));

    public void OnMenuViewsDuplicateGroups()
        => _navigationService.NavigateTo(typeof(DuplicateGroupsViewModel).FullName, null, true);

    public ICommand MenuViewsListDetailsCommand => _menuViewsListDetailsCommand ?? (_menuViewsListDetailsCommand = new RelayCommand(OnMenuViewsListDetails));

    public void OnMenuViewsListDetails()
        => _navigationService.NavigateTo(typeof(ListDetailsViewModel).FullName, null, true);

    public ICommand MenuViewsContentGridCommand => _menuViewsContentGridCommand ?? (_menuViewsContentGridCommand = new RelayCommand(OnMenuViewsContentGrid));

    public void OnMenuViewsContentGrid()
        => _navigationService.NavigateTo(typeof(ContentGridViewModel).FullName, null, true);

    public ICommand MenuViewsDuplicateListCommand => _menuViewsDuplicateListCommand ?? (_menuViewsDuplicateListCommand = new RelayCommand(OnMenuViewsDuplicateList));

    public void OnMenuViewsDuplicateList()
        => _navigationService.NavigateTo(typeof(DuplicateListViewModel).FullName, null, true);

    public ICommand MenuViewsWebViewCommand => _menuViewsWebViewCommand ?? (_menuViewsWebViewCommand = new RelayCommand(OnMenuViewsWebView));

    public void OnMenuViewsWebView()
        => _navigationService.NavigateTo(typeof(WebViewViewModel).FullName, null, true);

    public void OnMenuFileSettings()
        => _rightPaneService.OpenInRightPane(typeof(SettingsViewModel).FullName);
}
