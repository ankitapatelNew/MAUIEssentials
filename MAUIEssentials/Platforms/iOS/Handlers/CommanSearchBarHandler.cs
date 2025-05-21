namespace MAUIEssentials.Platforms.iOS.Handlers
{
    public class CommanSearchBarHandler : SearchBarHandler
    {
        private UIImage? _searchImage;
        private UIImage? _closeImage;
        private const float IconSize = 20f;

        protected override void ConnectHandler(MauiSearchBar platformView)
        {
            try
            {
                base.ConnectHandler(platformView);

                platformView.SearchButtonClicked += OnSearchButtonClicked;

                UpdateSearchBar();
                UpdateIcons();

                if (VirtualView is AppCode.Controls.SearchBar searchBar)
                {
                    searchBar.PropertyChanged += OnSearchBarPropertyChanged;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override void DisconnectHandler(MauiSearchBar platformView)
        {
            try
            {
                platformView.SearchButtonClicked -= OnSearchButtonClicked;

                if (VirtualView is AppCode.Controls.SearchBar searchBar)
                {
                    searchBar.PropertyChanged -= OnSearchBarPropertyChanged;
                }

                base.DisconnectHandler(platformView);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void OnSearchButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                // Dismiss the keyboard when search button is pressed
                PlatformView?.ResignFirstResponder();

                // Execute the search command if one exists
                if (VirtualView is AppCode.Controls.SearchBar searchBar && searchBar.SearchCommand?.CanExecute(null) == true)
                {
                    searchBar.SearchCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void OnSearchBarPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == Microsoft.Maui.Controls.SearchBar.TextProperty.PropertyName)
                {
                    PlatformView.ShowsCancelButton = false;
                }
                else if (e.PropertyName == AppCode.Controls.SearchBar.SearchImageProperty.PropertyName ||
                        e.PropertyName == AppCode.Controls.SearchBar.CloseIconImageProperty.PropertyName ||
                        e.PropertyName == AppCode.Controls.SearchBar.TintColorProperty.PropertyName)
                {
                    UpdateIcons();
                }
                else if (e.PropertyName == Microsoft.Maui.Controls.SearchBar.PlaceholderProperty.PropertyName ||
                        e.PropertyName == Microsoft.Maui.Controls.SearchBar.FontFamilyProperty.PropertyName ||
                        e.PropertyName == Microsoft.Maui.Controls.SearchBar.FontAttributesProperty.PropertyName ||
                        e.PropertyName == Microsoft.Maui.Controls.SearchBar.FontSizeProperty.PropertyName)
                {
                    UpdateSearchBar();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void UpdateSearchBar()
        {
            try
            {
                if (VirtualView is not AppCode.Controls.SearchBar element || PlatformView == null)
                    return;

                var searchField = PlatformView.SearchTextField;
                searchField.Placeholder = element.Placeholder;
                searchField.Font = Utils.CustomFont(element.FontFamily, element.FontAttributes, (float)(element.FontSize - 1));

                PlatformView.SearchBarStyle = UISearchBarStyle.Minimal;
                PlatformView.SetShowsCancelButton(false, false);
                PlatformView.ShowsCancelButton = false;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void UpdateIcons()
        {
            try
            {
                if (VirtualView is not AppCode.Controls.SearchBar element || PlatformView == null)
                    return;

                // Resize and update search icon
                if (!string.IsNullOrEmpty(element.SearchIcon))
                {
                    _searchImage = ResizeImage(UIImage.FromFile(element.SearchIcon), IconSize, IconSize);
                }
                else
                {
                    _searchImage = ResizeImage(UIImage.FromFile("ic_search.png"), IconSize, IconSize);
                }

                // Resize and update close icon
                if (!string.IsNullOrEmpty(element.CloseIcon))
                {
                    _closeImage = ResizeImage(UIImage.FromFile(element.CloseIcon), IconSize, IconSize);
                }
                else
                {
                    _closeImage = ResizeImage(UIImage.FromFile("ic_Cross.png"), IconSize, IconSize);
                }

                // Handle tint color
                if (element.TintColor != null)
                {
                    _searchImage = _searchImage?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                    _closeImage = _closeImage?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                    PlatformView.TintColor = element.TintColor.ToPlatform();
                }

                // Apply icons
                PlatformView.SetImageforSearchBarIcon(_searchImage, UISearchBarIcon.Search, UIControlState.Normal);
                PlatformView.SetImageforSearchBarIcon(_closeImage, UISearchBarIcon.Clear, UIControlState.Normal);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private UIImage ResizeImage(UIImage sourceImage, float width, float height)
        {
            UIGraphics.BeginImageContextWithOptions(new CGSize(width, height), false, UIScreen.MainScreen.Scale);
            sourceImage.Draw(new CGRect(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }
    }
}
