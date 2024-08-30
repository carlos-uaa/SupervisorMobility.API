using DocumentFormat.OpenXml.Drawing.Charts;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SupervisorMobility.API.DataAccess.Services.ExportationServices
{
    public class ExportationImgService
    {
        public ExportationImgService() { }


        public Image ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        public Image ResizeImageMaintainingAspectRatio(Image image, int size, bool isWidth)
        {
            int width, height;

            if (isWidth)
            {
                // Calculate the new height to maintain the aspect ratio
                width = size;
                height = (int)((float)size / image.Width * image.Height);
            }
            else
            {
                // Calculate the new width to maintain the aspect ratio
                height = size;
                width = (int)((float)size / image.Height * image.Width);
            }

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public (int, int) GetResizeMagnitudesMaintainingAspectRatio(int imageW, int imageH, int size, bool isWidth)
        {
            int width, height;

            if (isWidth)
            {
                // Calculate the new height to maintain the aspect ratio
                width = size;
                height = (int)((float)size / imageW * imageH);
            }
            else
            {
                // Calculate the new width to maintain the aspect ratio
                height = size;
                width = (int)((float)size / imageH * imageW);
            }

            return (height, width);
        }


        public int WidthToPixels(double width)
        {
            return (int)((width - 1) * 7.0 + 12 - 5);
        }
        public int HeightToPixels(double height)
        {
            return (int)(height * 96.0 / 72);
        }
        public double PixelsToWidth(int pixels)
        {
            return (pixels - 12 + 5) / 7.0 + 1;
        }
        public double PixelsToHeight(int pixels)
        {
            return pixels * 72 / 96.0;
        }
    }
}
