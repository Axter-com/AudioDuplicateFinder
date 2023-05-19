using System.Windows.Controls;

namespace AudioDuplicateFinder.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string key);

    Page GetPage(string key);
}
