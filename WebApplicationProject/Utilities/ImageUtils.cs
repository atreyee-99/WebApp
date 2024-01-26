/*using System.Drawing;
using WebApplicationProject.Models;
using System.Drawing.Imaging;
using System.IO;
using Aspose.Imaging;
using Image = Aspose.Imaging.Image;

namespace WebApplicationProject.Utilities
{
    public static class ImageUtils
    {
        public static byte[] RotateImage(byte[] imageData, int degrees)
        {
            using (MemoryStream ms = new MemoryStream(imageData))
            using (Image originalImage = Image.FromStream(ms))
            using (Bitmap rotatedImage = RotateImage(originalImage, degrees))
            using (MemoryStream rotatedMs = new MemoryStream())
            {
                rotatedImage.Save(rotatedMs, ImageFormat.Jpeg); // Save as JPEG, adjust as needed
                return rotatedMs.ToArray();
            }
        }
        private static Bitmap RotateImage(Image image, float angle)
        {
            Bitmap rotatedImage = new Bitmap(image.Width, image.Height);
            rotatedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(rotatedImage.Width / 2, rotatedImage.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-rotatedImage.Width / 2, -rotatedImage.Height / 2);
                g.DrawImage(image, new Aspose.Imaging.Point(0, 0));
            }

            return rotatedImage;
        }
    }
}
*/