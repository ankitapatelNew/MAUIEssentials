namespace MAUIEssentials.Platforms.Android.Renderers
{
    public class CustomCaptchaImageRenderer : Microsoft.Maui.Controls.Compatibility.Platform.Android.FastRenderers.ImageRenderer
    {
        public CustomCaptchaImageRenderer(Context context) : base(context)
        {
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (e.OldElement != null || e.NewElement == null)
                {
                    return;
                }

                Control.SetScaleType(ImageView.ScaleType.CenterInside);
                SetPlaceholderCaptchaImage();
                SetImageCaptchaImage();
                SetTintColorCaptchaImage();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);

                switch (e.PropertyName)
                {
                    case nameof(Image.IsLoading):
                    case nameof(Image.IsVisible):
                    case nameof(Image.Source):
                    case nameof(CustomImage.TintColor):
                        SetImageCaptchaImage();
                        SetTintColorCaptchaImage();
                        break;
                    case nameof(CustomImage.Placeholder):
                        SetPlaceholderCaptchaImage();
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override async void Dispose(bool disposing)
        {
            try
            {
                CancelGlideCaptchaImage();
                base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private async void CancelGlideCaptchaImage()
        {
            try
            {
                if (Control != null && Control.Handle == IntPtr.Zero)
                {
                    return;
                }

                Glide.Get(global::Android.App.Application.Context).ClearMemory();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        private async void SetImageCaptchaImage()
        {
            try
            {
                var element = Element as CustomImage;
                var context = Platform.AppContext;

                if (element.Source is UriImageSource uriImage)
                {
                    if (uriImage.Uri == null)
                    {
                        SetPlaceholderCaptchaImage(true);
                        return;
                    }

                    var builder = Glide.With(context)
                        .Load(uriImage.Uri.OriginalString)
                        .SetDiskCacheStrategy(DiskCacheStrategy.All);

                    if (element.Aspect == Aspect.AspectFill)
                    {
                        builder.CenterCrop();
                    }
                    else if (element.Aspect == Aspect.AspectFit)
                    {
                        builder.FitCenter();
                    }

                    if (element.Placeholder is FileImageSource placeImage
                        && !string.IsNullOrEmpty(placeImage.File) && !File.Exists(placeImage.File))
                    {
                        var fileName = placeImage.File.Replace('-', '_').Replace(".png", "").Replace(".jpg", "");
                        var resId = context.GetDrawableId(fileName);
                        builder.Placeholder(resId);
                    }

                    if (element.ErrorPlaceholder is FileImageSource errorImage
                        && !string.IsNullOrEmpty(errorImage.File) && !File.Exists(errorImage.File))
                    {

                        var fileName = errorImage.File.Replace('-', '_').Replace(".png", "").Replace(".jpg", "");
                        var resId = context.GetDrawableId(fileName);
                        builder.Error(resId);
                    }

                    builder.Listener(new CaptchaRequestListener(this, element));
                    builder.Into(Control);
                }
                else if (element.Source is StreamImageSource streamImage)
                {
                    // Handle StreamImageSource
                    LoadStreamImageCaptchaImage(streamImage, context);
                }
                else
                {
                    SetPlaceholderCaptchaImage();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private async void LoadStreamImageCaptchaImage(StreamImageSource streamImage, Context context)
        {
            try
            {
                var stream = await streamImage.Stream(CancellationToken.None);
                if (stream == null)
                {
                    SetPlaceholderCaptchaImage(true);
                    return;
                }

                // Convert Stream to ByteArray
                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    imageBytes = ms.ToArray();
                }

                float density = context.Resources.DisplayMetrics.Density;
                int adjustedWidth = (int)(Element.WidthRequest * density);
                int adjustedHeight = (int)(Element.HeightRequest * density);

                var requestOptions = new RequestOptions()
                    .Override(adjustedWidth, adjustedHeight)
                    .SetDiskCacheStrategy(DiskCacheStrategy.None)
                    .SkipMemoryCache(true);

                var builder = Glide.With(context)
                    .Load(imageBytes)
                    .Apply(requestOptions)
                    .FitCenter();

                if (Element.Aspect == Aspect.AspectFill)
                {
                    builder.CenterCrop();
                }
                else if (Element.Aspect == Aspect.AspectFit)
                {
                    builder.FitCenter();
                }

                builder.Listener(new CaptchaRequestListener(this, Element as CustomImage));
                builder.Into(Control);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stream Image Resousce :{ex.StackTrace}");
            }
        }

        public async void SetPlaceholderCaptchaImage(bool isError = false)
        {
            try
            {
                var element = Element as CustomImage;
                var context = Platform.AppContext;

                ImageSource imageSource = isError ? element.ErrorPlaceholder : element.Placeholder;

                if (imageSource == null)
                {
                    return;
                }

                if (imageSource is FileImageSource fileImage && !string.IsNullOrEmpty(fileImage.File))
                {
                    if (File.Exists(fileImage.File))
                    {
                        Java.IO.File file = new Java.IO.File(fileImage.File);

                        global::Android.Net.Uri uri;
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            uri = FileProvider.GetUriForFile(context, context.PackageName + ".fileprovider", file);
                        }
                        else
                        {
                            uri = global::Android.Net.Uri.FromFile(file);
                        }

                        Control.SetImageURI(uri);
                    }
                    else
                    {
                        var fileName = fileImage.File.Replace('-', '_').Replace(".png", "").Replace(".jpg", "");
                        var resId = context.Resources.GetIdentifier(fileName, "drawable", context.PackageName);

                        Control.SetImageResource(resId);
                    }
                }
                else if (imageSource is StreamImageSource streamImage && streamImage.Stream != null)
                {
                    var stream = await ((IStreamImageSource)streamImage).GetStreamAsync().ConfigureAwait(false);
                    var bitmap = await BitmapFactory.DecodeStreamAsync(stream);
                    Control.SetImageBitmap(bitmap);

                }
                else if (imageSource is UriImageSource uriImage && uriImage.Uri != null)
                {
                    using var httpClient = new HttpClient();
                    var imageBytes = await httpClient.GetByteArrayAsync(uriImage.Uri).ConfigureAwait(false);
                    var bitmap = await BitmapFactory.DecodeByteArrayAsync(imageBytes, 0, imageBytes.Length);
                    Control.SetImageBitmap(bitmap);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public async void SetTintColorCaptchaImage()
        {
            try
            {
                var element = Element as CustomImage;

                if (element.TintColor.Equals(Colors.Transparent))
                {
                    Control.SetColorFilter(element.TintColor, PorterDuff.Mode.SrcAtop);
                }
                else
                {
                    Control.ClearColorFilter();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }

    public class CaptchaRequestListener : Java.Lang.Object, IRequestListener
    {
        readonly CustomImage _element;
        readonly CustomCaptchaImageRenderer _renderer;

        public CaptchaRequestListener(CustomCaptchaImageRenderer renderer, CustomImage element)
        {
            try
            {
                _renderer = renderer;
                _element = element;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public bool OnLoadFailed(GlideException p0, Java.Lang.Object p1, ITarget p2, bool p3)
        {
            _renderer.SetPlaceholderCaptchaImage(true);
            return false;
        }

        public bool OnResourceReady(Java.Lang.Object p0, Java.Lang.Object p1, ITarget p2, DataSource p3, bool p4)
        {
            try
            {
                if (_element.TintColor.Equals(Colors.Transparent))
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                    {
                        (p0 as BitmapDrawable).SetColorFilter(new BlendModeColorFilter(_element.TintColor, global::Android.Graphics.BlendMode.SrcAtop));
                    }
                    else
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        (p0 as BitmapDrawable).SetColorFilter(_element.TintColor, PorterDuff.Mode.SrcAtop);
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return false;
        }
    }
}