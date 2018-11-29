using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using GroupDMosaicMaker.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GroupDMosaicMaker
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        #region Data members

        private double dpiX;
        private double dpiY;
        private WriteableBitmap modifiedImage;
        public MainPageViewModel viewModel;

        public Byte[] imageBytes;
        public uint height;
        public uint width;
        public int gridSize;

        public BitmapImage copyImage;

        public StorageFile source;
        #endregion

        #region Constructors

        public MainPage()
        {
            this.InitializeComponent();

            this.modifiedImage = null;
            this.dpiX = 0;
            this.dpiY = 0;
            this.viewModel = new MainPageViewModel();
            this.DataContext = viewModel;
            this.height = 0;
            this.width = 0;
            this.gridSize = 0;


            this.InitializeComponent();
        }

        #endregion


        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            this.saveWritableBitmap();
        }

        private async void saveWritableBitmap()
        {
            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            fileSavePicker.FileTypeChoices.Add("PNG files", new List<string> { ".png" });
            var savefile = await fileSavePicker.PickSaveFileAsync();

            if (savefile != null)
            {
                var stream = await savefile.OpenAsync(FileAccessMode.ReadWrite);
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                var pixelStream = this.modifiedImage.PixelBuffer.AsStream();
                var pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                    (uint)this.modifiedImage.PixelWidth,
                    (uint)this.modifiedImage.PixelHeight, this.dpiX, this.dpiY, pixels);
                await encoder.FlushAsync();

                stream.Dispose();
            }
        }

        private async void loadButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var sourceImageFile = await this.selectSourceImageFile();
            this.source = sourceImageFile;
            var copyBitmapImage = await this.MakeACopyOfTheFileToWorkOn(sourceImageFile);
            this.copyImage = copyBitmapImage;
            this.SourceImage.Source = copyBitmapImage;

            using (var fileStream = await sourceImageFile.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var transform = new BitmapTransform
                {
                    ScaledWidth = Convert.ToUInt32(copyBitmapImage.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(copyBitmapImage.PixelHeight)
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
                this.imageBytes = sourcePixels;
                this.height = decoder.PixelHeight;
                this.width = decoder.PixelWidth;

                this.modifiedImage = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                using (var writeStream = this.modifiedImage.PixelBuffer.AsStream())
                {
                    await writeStream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                    this.ModifiedImage.Source = this.modifiedImage;
                }
            }
        }

        private async void MosaicRefresh()
        {
            var copyBitmapImage = await this.MakeACopyOfTheFileToWorkOn(source);
            using (var fileStream = await this.source.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var transform = new BitmapTransform
                {
                    ScaledWidth = Convert.ToUInt32(copyBitmapImage.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(copyBitmapImage.PixelHeight)
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
                this.imageBytes = sourcePixels;
                this.height = decoder.PixelHeight;
                this.width = decoder.PixelWidth;


                //TODO fix unknown loop issue
                //Surrond with loop to cover entire image.
                for (int i = 0; i < this.height; i += this.gridSize)
                {
                    for (int j = 0; j < this.width; j += this.gridSize)
                    {
                        if (i + this.gridSize < height && j + this.gridSize < width)
                        {
                            this.giveImageAverageColor(sourcePixels, i, j, (uint) (i + this.gridSize), (uint)(j + this.gridSize), decoder.PixelWidth, decoder.PixelHeight);
                        }
                    }
                }
                



                this.modifiedImage = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                using (var writeStream = this.modifiedImage.PixelBuffer.AsStream())
                {
                    await writeStream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                    this.ModifiedImage.Source = this.modifiedImage;
                }
            }
        }

        private void giveImageAverageColor(byte[] sourcePixels, int xstart, int ystart, uint imageHeight, uint imageWidth, uint fullWidth, uint fullHeight)
        {
            List<Color> pixels = new List<Color>();
            for (var i = xstart; i < imageHeight; i++)
            {
                for (var j = ystart; j < imageWidth; j++)
                {
                    var pixelColor = this.getPixelBgra8(sourcePixels, i, j, fullWidth, fullHeight);
                    pixels.Add(pixelColor);
                }
            }
            var reds = pixels.Select(x => x.R).ToList();
            var averageR = reds.Average(x => x);
            var green = pixels.Select(x => x.G).ToList();
            var averageG = green.Average(x => x);
            var blue = pixels.Select(x => x.B).ToList();
            var averageB = blue.Average(x => x);
            var aveColor = Color.FromArgb(0, Convert.ToByte(averageR), Convert.ToByte(averageG), Convert.ToByte(averageB));

            for (var i = xstart; i < imageHeight; i++)
            {
                for (var j = ystart; j < imageWidth; j++)
                {
                    this.setPixelBgra8(sourcePixels, i, j, aveColor, fullWidth, fullHeight);
                }
            }
        }

        private void DrawGrid(byte[] sourcePixels, uint imageWidth, uint imageHeight, int gridSize)
        {
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j++)
                {
                    if (i % gridSize == 0 || j % gridSize == 0)
                    {
                        var pixelColor = this.getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                        var gridColor =  Color.FromArgb(0, 255, pixelColor.G, pixelColor.B);
                        this.setPixelBgra8(sourcePixels, i, j, gridColor, imageWidth, imageHeight);
                    }
                }
            }
        }

        private async Task GridRefreshAsync(int grid)
        {
            var gridBitmapImage = await this.MakeACopyOfTheFileToWorkOn(source);
            using (var fileStream = await source.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var transform = new BitmapTransform {
                    ScaledWidth = Convert.ToUInt32(gridBitmapImage.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(gridBitmapImage.PixelHeight)
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
                this.DrawGrid(sourcePixels, decoder.PixelWidth, decoder.PixelHeight, grid);
                this.modifiedImage = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                using (var writeStream = this.modifiedImage.PixelBuffer.AsStream())
                {
                    await writeStream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                    this.SourceImage.Source = this.modifiedImage;
                }

                this.MosaicRefresh();
            }
        }

        private async Task<StorageFile> selectSourceImageFile()
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            var file = await openPicker.PickSingleFileAsync();

            return file;
        }

        private async Task<BitmapImage> MakeACopyOfTheFileToWorkOn(StorageFile imageFile)
        {
            IRandomAccessStream inputstream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputstream);
            return newImage;
        }

        private Color getPixelBgra8(byte[] pixels, int x, int y, uint width, uint height)
        {
            var offset = (x * (int)width + y) * 4;
            var r = pixels[offset + 2];
            var g = pixels[offset + 1];
            var b = pixels[offset + 0];
            return Color.FromArgb(0, r, g, b);
        }

        private void setPixelBgra8(byte[] pixels, int x, int y, Color color, uint width, uint height)
        {
            var offset = (x * (int)width + y) * 4;
            pixels[offset + 2] = color.R;
            pixels[offset + 1] = color.G;
            pixels[offset + 0] = color.B;
        }

        private void DrawGrid_Click(object sender, RoutedEventArgs e)
        {
            int size;
            int.TryParse(this.GridBox.Text.ToString(), out size);
            this.gridSize = size;
            this.GridRefreshAsync(size);
            }
        }
    }