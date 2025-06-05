using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentialsApp.DepedencyServices
{
    public class PageNavigationService : IPageNavigationService
    {
        public async Task NavigateToSplashPage()
        {
            try
            {
                Application.Current.MainPage = new AppShell();
                _ = NavigationServices.OpenShellPage("SplashPage");
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}
