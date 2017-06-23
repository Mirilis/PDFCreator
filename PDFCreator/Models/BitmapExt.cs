using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PDFCreator.Models
{
    public static class BitmapExt
    {
        public static Bitmap ResizeBitmap(this Bitmap original, int boxHeight, int boxWidth)
        {
            Bitmap resizedImage;
            //calculate the ratio
            double dbl = (double)original.Width / (double)original.Height;

            //set height of image to boxHeight and check if resulting width is less than boxWidth, 
            //else set width of image to boxWidth and calculate new height
            if ((int)((double)boxHeight * dbl) <= boxWidth)
            {
                resizedImage = new Bitmap(original, (int)((double)boxHeight * dbl), boxHeight);
            }
            else
            {
                resizedImage = new Bitmap(original, boxWidth, (int)((double)boxWidth / dbl));
            }
            return resizedImage;
        }

        public static Bitmap CropAtRect(this Bitmap imgToResize, Size destinationSize)
        {
            var originalWidth = imgToResize.Width;
            var originalHeight = imgToResize.Height;

            //how many units are there to make the original length
            var hRatio = (float)originalHeight / destinationSize.Height;
            var wRatio = (float)originalWidth / destinationSize.Width;

            //get the shorter side
            var ratio = Math.Min(hRatio, wRatio);

            var hScale = Convert.ToInt32(destinationSize.Height * ratio);
            var wScale = Convert.ToInt32(destinationSize.Width * ratio);

            //start cropping from the center
            var startX = (originalWidth - wScale) / 2;
            var startY = (originalHeight - hScale) / 2;

            //crop the image from the specified location and size
            var sourceRectangle = new Rectangle(startX, startY, wScale, hScale);

            //the future size of the image
            var bitmap = new Bitmap(destinationSize.Width, destinationSize.Height);

            //fill-in the whole bitmap
            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            //generate the new image
            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            return bitmap;

        }

    }
}