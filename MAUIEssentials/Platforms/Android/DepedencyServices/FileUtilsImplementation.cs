using Android.Graphics;
using FirebaseEssentials.Shared;
using MAUIEssentials.DependencyServices;

namespace MAUIEssentials.DepedencyServices
{
    public class FileUtilsImplementation : DisposableBase, IFileUtils
    {
        public async Task<Size> GetFileSize(byte[] fileData)
        {
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            await BitmapFactory.DecodeByteArrayAsync(fileData, 0, fileData.Length, options);
            return new Size(Convert.ToDouble(options.OutWidth), Convert.ToDouble(options.OutHeight));
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height, string extension = "")
        {
            // Load the bitmap 
            BitmapFactory.Options options = new BitmapFactory.Options();
            Bitmap? originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);

            if (originalImage == null)
                throw new InvalidOperationException("Failed to decode image data.");

            float newHeight, newWidth;
            var originalHeight = originalImage.Height;
            var originalWidth = originalImage.Width;

            if (originalHeight > originalWidth)
            {
                newHeight = height;
                float ratio = originalHeight / height;
                newWidth = originalWidth / ratio;
            }
            else
            {
                newWidth = width;
                float ratio = originalWidth / width;
                newHeight = originalHeight / ratio;
            }

            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, true);
            originalImage.Recycle();

            using MemoryStream ms = new MemoryStream();
            bool compressed;

            if (extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
#pragma warning disable CS8604 // Possible null reference argument.
                compressed = resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 95, ms);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            else
            {
#pragma warning disable CS8604 // Possible null reference argument.
                compressed = resizedImage.Compress(Bitmap.CompressFormat.Png, 95, ms);
#pragma warning restore CS8604 // Possible null reference argument.
            }

            resizedImage.Recycle();

            if (!compressed)
                throw new InvalidOperationException("Image compression failed.");

            return ms.ToArray();
        }
    }
}
