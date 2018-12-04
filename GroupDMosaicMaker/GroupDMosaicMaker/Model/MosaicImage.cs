using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupDMosaicMaker.Model
{
    public class MosaicImage
    {
        public StorageFile File { get; set; }

        public BitmapImage Image { get; set; }

        public uint Height { get; set; }

        public uint Width { get; set; }

        public byte[] Pixels { get; set; }
        public Color AverageColor { get; set; }

        private double dpiX;
        private double dpiY;


        public MosaicImage(StorageFile file)
        {
            this.File = file ?? throw new ArgumentNullException();
            this.Image = new BitmapImage();
            this.dpiX = 0;
            this.dpiY = 0;
            this.extractPixelData();
        }

        private async void extractPixelData()
        {
            using (var fileStream = await this.File.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var transform = new BitmapTransform
                {
                    ScaledWidth = Convert.ToUInt32(this.Image.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(this.Image.PixelHeight)
                };

                this.dpiX = decoder.DpiX;
                this.dpiY = decoder.DpiY;

                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage
                );

                var sourcePixels = pixelData.DetachPixelData();
                this.Height = decoder.PixelHeight;
                this.Width = decoder.PixelWidth;
                this.calcAverageColor(sourcePixels, decoder.PixelWidth, decoder.PixelHeight);
                this.Pixels = sourcePixels;
            }
        }

        private void calcAverageColor(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            var pixels = new List<Color>();
            for (var i = 0; i < imageWidth; i++)
            {
                for (var j = 0; j < imageHeight; j++)
                {
                    if (i < imageWidth && j < imageHeight)
                    {
                        var pixelColor = this.getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);

                        pixels.Add(pixelColor);
                    }
                }
            }

            var reds = pixels.Select(x => x.R).ToList();
            var averageR = reds.Average(x => x);
            var green = pixels.Select(x => x.G).ToList();
            var averageG = green.Average(x => x);
            var blue = pixels.Select(x => x.B).ToList();
            var averageB = blue.Average(x => x);
            var aveColor = Color.FromArgb(0, Convert.ToByte(averageR), Convert.ToByte(averageG), Convert.ToByte(averageB));
            this.AverageColor = aveColor;
        }

        private Color getPixelBgra8(byte[] pixels, int x, int y, uint width, uint height)
        {
            var offset = (x * (int)width + y) * 4;
            var r = pixels[offset + 2];
            var g = pixels[offset + 1];
            var b = pixels[offset + 0];
            return Color.FromArgb(0, r, g, b);
        }


        public int colorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                  + (c1.G - c2.G) * (c1.G - c2.G)
                                  + (c1.B - c2.B) * (c1.B - c2.B));
        }
    }
}
