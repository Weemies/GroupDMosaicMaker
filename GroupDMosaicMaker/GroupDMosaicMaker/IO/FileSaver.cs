using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GroupDMosaicMaker.IO
{
    public class FileSaver
    {
        private FileSavePicker savePicker;

        public FileSaver()
        {
            this.initializeComponents();
        }

        public async Task<StorageFile> saveFile()
        {
            var file = await this.savePicker.PickSaveFileAsync();
            return file;
        }

        private void initializeComponents()
        {
            this.savePicker = new FileSavePicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            this.savePicker.FileTypeChoices.Add("PNG files", new List<string> { ".png" });
        }
    }
}
