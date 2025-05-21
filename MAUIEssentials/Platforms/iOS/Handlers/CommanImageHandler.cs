namespace MAUIEssentials.Platforms.iOS.Handlers
{
    public class CommanImageHandler : ViewHandler<CustomImage, UIImageView>
    {
        private static readonly NSCache _imageCache = new NSCache();
        private NSUrlSession _urlSession;
        private NSUrlCache _urlCache;
        private CancellationTokenSource _setImageCts;
        private NSObject _memoryWarningObserver;

        public static IPropertyMapper<CustomImage, CommanImageHandler> Mapper =
        new PropertyMapper<CustomImage, CommanImageHandler>(ViewMapper)
        {
            [nameof(CustomImage.Source)] = MapSource,
            [nameof(CustomImage.TintColor)] = MapTintColor,
            [nameof(CustomImage.Placeholder)] = MapPlaceholder,
            [nameof(CustomImage.Aspect)] = MapAspect
        };

        public CommanImageHandler() : base(Mapper)
        {
            InitializeUrlCache();
        }

        private void InitializeUrlCache()
        {
            try
            {
                // Configure URL cache with reasonable defaults
                _urlCache = new NSUrlCache(
                    memoryCapacity: 20 * 1024 * 1024, // 20MB memory cache
                    diskCapacity: 50 * 1024 * 1024,   // 50MB disk cache
                    diskPath: null);                  // default path

                var configuration = NSUrlSessionConfiguration.DefaultSessionConfiguration;
                configuration.URLCache = _urlCache;
                configuration.RequestCachePolicy = NSUrlRequestCachePolicy.UseProtocolCachePolicy;

                _urlSession = NSUrlSession.FromConfiguration(configuration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing URL cache: {ex.Message}");
                ex.LogException();
            }
        }

        protected override UIImageView CreatePlatformView()
        {
            var imageView = new UIImageView
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                ClipsToBounds = true,
            };

            return imageView;
        }

        protected override void ConnectHandler(UIImageView platformView)
        {
            base.ConnectHandler(platformView);

            platformView.TranslatesAutoresizingMaskIntoConstraints = false;

            if (VirtualView.Width > 0 && VirtualView.Height > 0)
            {
                platformView.WidthAnchor.ConstraintEqualTo((nfloat)VirtualView.Width).Active = true;
                platformView.HeightAnchor.ConstraintEqualTo((nfloat)VirtualView.Height).Active = true;
            }

            // Register for memory warnings
            _memoryWarningObserver = NSNotificationCenter.DefaultCenter.AddObserver(
                UIApplication.DidReceiveMemoryWarningNotification,
                HandleMemoryWarning);

            SetPlaceholder();
            SetImage();
            SetTintColor();
            SetAspect();
        }

        protected override void DisconnectHandler(UIImageView platformView)
        {
            // Clear cache when memory is low
            _setImageCts?.Cancel();
            _setImageCts?.Dispose();
            _setImageCts = null;

            if (_memoryWarningObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_memoryWarningObserver);
                _memoryWarningObserver = null;
            }

            _urlSession?.InvalidateAndCancel();

            if (platformView != null)
            {
                platformView.Image = null;
            }

            base.DisconnectHandler(platformView);
        }

        private void HandleMemoryWarning(NSNotification notification)
        {
            _imageCache.RemoveAllObjects();
            _urlCache?.RemoveAllCachedResponses();
        }

        public static void MapSource(CommanImageHandler handler, CustomImage image)
        {
            handler.SetImage();
        }

        public static void MapTintColor(CommanImageHandler handler, CustomImage image)
        {
            handler.SetTintColor();
        }

        public static void MapPlaceholder(CommanImageHandler handler, CustomImage image)
        {
            handler.SetPlaceholder();
        }

        public static void MapAspect(CommanImageHandler handler, CustomImage image)
        {
            handler.SetAspect();
        }

        private void SetAspect()
        {
            if (VirtualView == null || PlatformView == null)
                return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (PlatformView == null)
                    return;

                try
                {
                    switch (VirtualView.Aspect)
                    {
                        case Aspect.Fill:
                            PlatformView.ContentMode = UIViewContentMode.ScaleToFill;
                            break;
                        case Aspect.AspectFill:
                            PlatformView.ContentMode = UIViewContentMode.ScaleAspectFill;
                            PlatformView.ClipsToBounds = true;
                            break;
                        case Aspect.AspectFit:
                            PlatformView.ContentMode = UIViewContentMode.ScaleAspectFit;
                            break;
                        default:
                            PlatformView.ContentMode = UIViewContentMode.ScaleToFill;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting aspect: {ex.Message}");
                }
            });
        }

        private async void SetImage()
        {
            _setImageCts?.Cancel();
            _setImageCts?.Dispose();
            _setImageCts = new CancellationTokenSource();
            var token = _setImageCts.Token;

            try
            {
                if (VirtualView == null || PlatformView == null)
                    return;

                UIImage? uiImage = null;

                switch (VirtualView.Source)
                {
                    case UriImageSource uriImage when uriImage.Uri != null:
                        uiImage = await LoadImageWithRetry(uriImage, token);
                        break;

                    case FileImageSource fileImage when !string.IsNullOrEmpty(fileImage.File):
                        uiImage = await LoadLocalImage(fileImage.File, token);
                        break;
                }

                if (uiImage != null)
                {
                    PlatformView.Image = uiImage;
                    SetTintColor();
                }
                else
                {
                    Console.WriteLine("No image available, setting placeholder");
                    SetPlaceholder(true);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Ignore cancellation");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
                SetPlaceholder(true);
            }
        }

        private async Task<UIImage> LoadImageWithRetry(IUriImageSource uriImage, CancellationToken cancellationToken, int maxRetries = 2)
        {
            if (VirtualView == null || uriImage?.Uri == null)
                return null;

            var cacheKey = uriImage.Uri.ToString();
            if (_imageCache.ObjectForKey(new NSString(cacheKey)) is UIImage cachedImage)
            {
                return cachedImage;
            }

            for (int i = 0; i < maxRetries; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var url = new NSUrl(uriImage.Uri.ToString());
                    var request = new NSUrlRequest(url);

                    var tcs = new TaskCompletionSource<(NSData Data, NSUrlResponse Response, NSError Error)>();

                    var task = _urlSession.CreateDataTask(request, (data, response, error) =>
                    {
                        tcs.TrySetResult((data, response, error));
                    });

                    task.Resume();
                    var (data, response, error) = await tcs.Task.WaitAsync(cancellationToken);

                    if (error == null && data != null)
                    {
                        var downloadedImage = UIImage.LoadFromData(data);
                        if (downloadedImage != null)
                        {
                            // Cache the image in memory
                            _imageCache.SetObjectforKey(downloadedImage, new NSString(cacheKey));
                            return downloadedImage;
                        }
                    }

                    var imageSourceService = this.Services?.GetRequiredService<IImageSourceServiceProvider>()
                                        .GetRequiredImageSourceService(uriImage);
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    cts.CancelAfter(TimeSpan.FromSeconds(30));

                    var result = await imageSourceService.GetImageAsync(uriImage, cancellationToken: cts.Token);

                    if (result?.Value is UIImage fallbackImage)
                    {
                        _imageCache.SetObjectforKey(fallbackImage, new NSString(cacheKey));
                        return fallbackImage;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Retry {i + 1} failed: {ex.Message}");
                    if (i == maxRetries - 1)
                        throw;

                    await Task.Delay(1000 * (i + 1), cancellationToken);
                }
            }
            return null;
        }

        private async Task<UIImage> LoadLocalImage(string imagePath, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var imageName = Path.GetFileNameWithoutExtension(imagePath);
                var uiImage = UIImage.FromBundle(imageName);

                if (uiImage != null)
                {
                    Console.WriteLine($"Loaded image from bundle: {imageName}");
                    return uiImage;
                }

                uiImage = UIImage.FromFile(imagePath);
                if (uiImage != null)
                {
                    Console.WriteLine($"Loaded image from full path: {imagePath}");
                    return uiImage;
                }

                var assembly = GetType().Assembly;
                var resourcePath = imagePath.Replace("/", ".").Replace("\\", ".");
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith(resourcePath, StringComparison.OrdinalIgnoreCase));

                if (resourceName != null)
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            var data = NSData.FromStream(stream);
                            return UIImage.LoadFromData(data);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadLocalImage for {imagePath}: {ex.Message}");
                return null;
            }
        }

        private async void SetPlaceholder(bool isError = false)
        {
            try
            {
                if (PlatformView?.Image == null || isError)
                {
                    if (VirtualView?.Placeholder is FileImageSource file && !string.IsNullOrEmpty(file.File))
                    {
                        var placeholderImage = UIImage.FromBundle(Path.GetFileNameWithoutExtension(file.File)) ??
                                               UIImage.FromFile(file.File);

                        if (placeholderImage != null)
                            PlatformView.Image = placeholderImage;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading placeholder: {ex.Message}");
            }
        }

        private void SetTintColor()
        {
            if (VirtualView == null || PlatformView == null)
                return;

            MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (VirtualView.TintColor != Colors.Transparent)
                        {
                            PlatformView.Image = PlatformView.Image?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                            PlatformView.TintColor = VirtualView.TintColor.ToPlatform();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error setting tint color: {ex.Message}");
                    }
                }
            );
        }
    }
}
