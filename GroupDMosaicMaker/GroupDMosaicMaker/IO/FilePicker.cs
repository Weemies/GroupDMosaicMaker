using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GroupDMosaicMaker.IO
{
    public class FilePicker
    {
        private FileOpenPicker filePicker;

        public FilePicker()
        {
            this.initializeComponents();
        }

        public async Task<StorageFile> selectFile()
        {
            var file = await this.filePicker.PickSingleFileAsync();

            return file;
        }

        private void initializeComponents()
        {
            this.filePicker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            this.filePicker.FileTypeFilter.Add(".jpg");
            this.filePicker.FileTypeFilter.Add(".png");
            this.filePicker.FileTypeFilter.Add(".bmp");
        }
    }
}

