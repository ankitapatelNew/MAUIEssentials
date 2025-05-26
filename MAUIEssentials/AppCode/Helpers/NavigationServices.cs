using MAUIEssentials.AppCode.AlertViews;
using Mopups.Pages;
using Mopups.Services;

namespace MAUIEssentials.AppCode.Helpers
{
    public static class NavigationServices
    {
        public static Page MainPage => Application.Current.MainPage;

        public static INavigation Navigation
        {
            get
            {
                if (MainPage is NavigationPage navigationPage)
                {
                    if (navigationPage.CurrentPage is TabbedPage tabbedPage)
                    {
                        return tabbedPage.CurrentPage.Navigation;
                    }
                    else
                    {
                        return navigationPage.Navigation;
                    }
                }
                else
                {
                    return MainPage.Navigation;
                }
            }
        }

        public static async Task OpenShellPage(string page, Dictionary<string, object> parameters = null, bool animated = true, bool clearStack = false)
        {
            try
            {
                if (Shell.Current != null)
                {
                    string prefix = string.Empty;
                    if (clearStack)
                    {
                        prefix = "//";
                    }
                    if (parameters == null)
                    {
                        await Shell.Current.GoToAsync(prefix + page, false);
                    }
                    else
                    {
                        await Shell.Current.GoToAsync(prefix + page, false, parameters);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task PushAsyncPage(Page page)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.Navigation.PushAsync(page);
                }
                else
                {
                    if (MainPage.Navigation.NavigationStack.Count > 1)
                    {
                        await MainPage.Navigation.PushAsync(page);
                    }
                    else
                    {
                        await Navigation.PushAsync(page);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task ClosePage(bool animated = true)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.Navigation.PopAsync(animated);
                }
                else
                {
                    if (MainPage.Navigation.NavigationStack.Count > 1)
                    {
                        await MainPage.Navigation.PopAsync(animated);
                    }
                    else
                    {
                        await Navigation.PopAsync(animated);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task OpenModalPage(Page page, bool animated = true, bool isFromMain = false)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.Navigation.PushModalAsync(page, animated);
                }
                else
                {
                    if (MainPage.Navigation.NavigationStack.Count > 1)
                    {
                        await MainPage.Navigation.PushModalAsync(page, animated);
                    }
                    else
                    {
                        await Navigation.PushModalAsync(page, animated);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task CloseModalPage(bool animated = true, Dictionary<string, object> parameters = null)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.Navigation.PopModalAsync(animated);
                }
                else
                {
                    if (MainPage.Navigation.NavigationStack.Count > 1)
                    {
                        await MainPage.Navigation.PopModalAsync(animated);
                    }
                    else
                    {
                        await Navigation.PopModalAsync(animated);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task PopToRootAsync(bool animated = false)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.Navigation.PopToRootAsync(animated);
                }
                else
                {
                    if (MainPage.Navigation.NavigationStack.Count > 1)
                    {
                        await MainPage.Navigation.PopToRootAsync(animated);
                    }
                    else
                    {
                        await Navigation.PopToRootAsync(animated);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void SetBarTextColor(string color)
        {
            if (MainPage != null && MainPage is NavigationPage navigationPage)
            {
                navigationPage.BarTextColor = color.ToColor();
            }
        }

        public static async Task DisplayAlert(string message, string cancel)
        {
            await DisplayAlert(string.Empty, message, cancel);
        }

        public static async Task DisplayAlert(string message, string cancel, AlertConfig config)
        {
            await DisplayAlert(string.Empty, message, cancel, config);
        }

        public static async Task DisplayAlert(string title, string message, string cancel)
        {
            await MainPage.DisplayAlert(title, message, cancel);
        }

        public static async Task DisplayAlert(string title, string message, string cancel, AlertConfig config)
        {
            await Navigation.ShowAlert(title, message, cancel, config);
        }

        public static async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await MainPage.DisplayAlert(title, message, accept, cancel);
        }

        public static async Task<bool> DisplayAlert(string title, string message, string accept, string cancel, AlertConfig config)
        {
            return await Navigation.ShowAlert(title, message, config, accept, cancel);
        }

        public static async Task<string> DisplayActionSheet(string title, string cancel, ActionSheetConfig config, params string[] buttons)
        {
            return await Navigation.ShowActionSheet(title, cancel, config, buttons);
        }

        public static async Task OpenPopupPage(PopupPage page, bool animated = true)
        {
            try
            {
                await MopupService.Instance.PushAsync(page, animated);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task ClosePopupPage(bool animated = true)
        {
            try
            {
                await MopupService.Instance.PopAsync(animated);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task DisplaySnackbar(SnackbarConfig config)
        {
            await Navigation.ShowSnackbar(config);
        }

        public static async Task<bool> GoToPage(Type pageType)
        {
            try
            {
                var pages = Shell.Current.Navigation.NavigationStack.Where(p => p != null).ToList();
                int indexOfToPage = pages.FindIndex(x => x.GetType() == pageType);

                for (int i = pages.Count - 1; i > indexOfToPage; i--)
                {
                    _ = pages[i].Navigation.PopAsync(false);
                }

                if (indexOfToPage == -1)
                {
                    await OpenShellPage("MainPage", clearStack: true);
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.LogException();
                await OpenShellPage("MainPage", clearStack: true);
            }
            return false;
        }

        public static async Task<List<Page>> GetAllPagesInStack(bool isDelay = false)
        {
            var pages = new List<Page>();

            try
            {
                if (DeviceInfo.Platform == DevicePlatform.Android && isDelay)
                {
                    await Task.Delay(100);
                }

                if (MainPage is NavigationPage navigationPage)
                {
                    var navigationStack = navigationPage.Navigation.NavigationStack;
                    var modalStack = navigationPage.Navigation.ModalStack;
                    var popupStack = MopupService.Instance?.PopupStack;

                    AddPagesToList(navigationStack, pages);
                    AddPagesToList(modalStack, pages);

                    if (popupStack != null)
                    {
                        pages.AddRange(popupStack);
                    }
                }
                else if (Shell.Current != null)
                {
                    pages = Shell.Current.Navigation.NavigationStack.ToList();
                }
                else
                {
                    pages = Navigation.NavigationStack.Where(x => x != null).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return pages;
        }

        private static void AddPagesToList(IEnumerable<Page> source, List<Page> destination)
        {
            foreach (var item in source)
            {
                if (item is TabbedPage tabbedPage)
                {
                    destination.AddRange(tabbedPage.Children.SelectMany(GetPagesFromTabbedPage));
                }
                else
                {
                    destination.Add(item);
                }
            }
        }

        private static IEnumerable<Page> GetPagesFromTabbedPage(Page page)
        {
            if (page is NavigationPage navigation)
            {
                return navigation.Navigation.NavigationStack;
            }
            return new List<Page> { page };
        }
    }
}
