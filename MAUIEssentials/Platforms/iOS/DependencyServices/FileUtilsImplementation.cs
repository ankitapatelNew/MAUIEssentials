using CoreGraphics;
using FirebaseEssentials.Shared;
using Foundation;
using UIKit;

namespace MAUIEssentials.DependencyServices
{
    public class FileUtilsImplementation : DisposableBase, IFileUtils
    {
        private UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            return new UIImage(NSData.FromArray(data));
        }

        public async Task<Size> GetFileSize(byte[] fileData)
        {
            UIImage image = ImageFromByteArray(fileData);
            return new Size(Convert.ToDouble(image.Size.Width), Convert.ToDouble(image.Size.Height));
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height, string extension = "")
        {
            UIImage originalImage = ImageFromByteArray(imageData);

            var originalHeight = originalImage.Size.Height;
            var originalWidth = originalImage.Size.Width;

            nfloat newHeight = 0;
            nfloat newWidth = 0;

            if (originalHeight > originalWidth)
            {
                newHeight = height;
                nfloat ratio = originalHeight / height;
                newWidth = originalWidth / ratio;
            }
            else
            {
                newWidth = width;
                nfloat ratio = originalWidth / width;
                newHeight = originalHeight / ratio;
            }

            width = (float)newWidth;
            height = (float)newHeight;

            UIGraphics.BeginImageContext(new CGSize(width, height));
            originalImage.Draw(new CGRect(0, 0, width, height));

            var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            byte[] bytesImagen;

            if (extension == ".jpg" || extension == ".jpeg")
            {
                bytesImagen = resizedImage.AsJPEG().ToArray();
            }
            else
            {
                bytesImagen = resizedImage.AsPNG().ToArray();
            }

            resizedImage.Dispose();
            return bytesImagen;
        }
    }
}
