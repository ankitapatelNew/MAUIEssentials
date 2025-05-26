using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Bumptech.Glide.Request.Target;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using CustomImage = MAUIEssentials.AppCode.Controls.Image;


namespace MAUIEssentials.Platforms.Android.Handlers
{
    public class CommanImageHandler : ImageHandler
    {
        protected override ImageView CreatePlatformView()
        {
            var imageView = new ImageView(Context);
            imageView.SetScaleType(ImageView.ScaleType.CenterInside); // Corrected line
            return imageView;
        }

        protected override void ConnectHandler(ImageView platformView)
        {
            base.ConnectHandler(platformView);
            SetImage();
            SetTintColor();
        }

        protected override void DisconnectHandler(ImageView platformView)
        {
            // Cancel any ongoing Glide operations
            CancelGlide();

            // Clear the PlatformView to avoid accessing a disposed object
            if (platformView != null)
            {
                platformView.SetImageDrawable(null); // Clear the image
                platformView.ClearColorFilter(); // Clear the color filter
            }

            base.DisconnectHandler(platformView);
        }

        private void CancelGlide()
        {
            try
            {
                if (PlatformView != null)
                {
                    Glide.With(PlatformView.Context).Clear(PlatformView);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CancelGlide: {ex.Message}");
            }
        }

        private void SetImage()
        {
            try
            {
                if (PlatformView == null || VirtualView == null)
                    return;

                var element = VirtualView as CustomImage;
                if (element == null)
                    return;

                if (element.Source is UriImageSource uriImage)
                {
                    if (uriImage.Uri == null)
                    {
                        SetPlaceholder(true);
                        return;
                    }

                    var requestBuilder = Glide.With(PlatformView.Context)
                        .Load(uriImage.Uri.OriginalString)
                        .SetDiskCacheStrategy(DiskCacheStrategy.All);

                    if (element.Aspect == Aspect.AspectFill)
                    {
                        requestBuilder.CenterCrop();
                    }
                    else if (element.Aspect == Aspect.AspectFit)
                    {
                        requestBuilder.FitCenter();
                    }

                    if (element.Placeholder is FileImageSource placeholder && !string.IsNullOrEmpty(placeholder.File))
                    {
                        var resId = PlatformView.Context.GetDrawableId(placeholder.File);
                        requestBuilder.Placeholder(resId);
                    }

                    if (element.ErrorPlaceholder is FileImageSource errorPlaceholder && !string.IsNullOrEmpty(errorPlaceholder.File))
                    {
                        var resId = PlatformView.Context.GetDrawableId(errorPlaceholder.File);
                        requestBuilder.Error(resId);
                    }

                    requestBuilder.Listener(new GlideRequestListener(this, element));
                    requestBuilder.Into(PlatformView);
                }
                else if (element.Source is StreamImageSource streamImage)
                {
                    LoadStreamImage(streamImage);
                }
                else
                {
                    SetPlaceholder();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetImage: {ex.Message}");
            }
        }

        private async void LoadStreamImage(StreamImageSource streamImage)
        {
            try
            {
                if (PlatformView == null || VirtualView == null)
                    return;

                var stream = await streamImage.Stream(CancellationToken.None);
                if (stream == null)
                {
                    SetPlaceholder(true);
                    return;
                }

                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    imageBytes = ms.ToArray();
                }

                var requestOptions = new RequestOptions()
                    .SetDiskCacheStrategy(DiskCacheStrategy.None)
                    .SkipMemoryCache(true);

                var requestBuilder = Glide.With(PlatformView.Context)
                    .Load(imageBytes)
                    .Apply(requestOptions);

                if (VirtualView.Aspect == Aspect.AspectFill)
                {
                    requestBuilder.CenterCrop();
                }
                else if (VirtualView.Aspect == Aspect.AspectFit)
                {
                    requestBuilder.FitCenter();
                }

                requestBuilder.Listener(new GlideRequestListener(this, VirtualView as CustomImage));
                requestBuilder.Into(PlatformView);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadStreamImage: {ex.Message}");
            }
        }

        private void SetTintColor()
        {
            try
            {
                if (PlatformView == null || VirtualView == null)
                    return;

                var element = VirtualView as CustomImage;
                if (element == null)
                    return;

                if (!element.TintColor.Equals(Colors.Transparent))
                {
                    PlatformView.SetColorFilter(element.TintColor.ToPlatform(), PorterDuff.Mode.SrcAtop);
                }
                else
                {
                    PlatformView.ClearColorFilter();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetTintColor: {ex.Message}");
            }
        }

        public void SetPlaceholder(bool isError = false)
        {
            try
            {
                if (PlatformView == null || VirtualView == null)
                    return;

                var element = VirtualView as CustomImage;
                if (element == null)
                    return;

                ImageSource imageSource = isError ? element.ErrorPlaceholder : element.Placeholder;

                if (imageSource is FileImageSource fileImage && !string.IsNullOrEmpty(fileImage.File))
                {
                    var resId = PlatformView.Context.GetDrawableId(fileImage.File);
                    PlatformView.SetImageResource(resId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetPlaceholder: {ex.Message}");
            }
        }
    }

    public class GlideRequestListener : Java.Lang.Object, IRequestListener
    {
        private readonly CommanImageHandler _handler;
        private readonly CustomImage _element;

        public GlideRequestListener(CommanImageHandler handler, CustomImage element)
        {
            _handler = handler;
            _element = element;
        }

        public bool OnLoadFailed(GlideException p0, Java.Lang.Object p1, ITarget p2, bool p3)
        {
            _handler.SetPlaceholder(true);
            return false;
        }

        public bool OnResourceReady(Java.Lang.Object p0, Java.Lang.Object p1, ITarget p2, DataSource p3, bool p4)
        {
            try
            {
                if (!_element.TintColor.Equals(Colors.Transparent))
                {
                    if (p0 is BitmapDrawable bitmapDrawable)
                    {
                        bitmapDrawable.SetColorFilter(_element.TintColor.ToPlatform(), PorterDuff.Mode.SrcAtop);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnResourceReady: {ex.Message}");
            }
            return false;
        }
    }
}
