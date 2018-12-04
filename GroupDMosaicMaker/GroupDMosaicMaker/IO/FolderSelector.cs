using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GroupDMosaicMaker.IO
{
    public class FolderSelector
    {
        private FolderPicker picker;

        public FolderSelector()
        {
            this.initializeComponents();
        }

        public async Task<StorageFolder> LoadFolder()
        {
            var folder = await this.picker.PickSingleFolderAsync();
            return folder;
        }

        private void initializeComponents()
        {
            this.picker = new FolderPicker()
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            this.picker.FileTypeFilter.Add(".jpg");
            this.picker.FileTypeFilter.Add(".png");
            this.picker.FileTypeFilter.Add(".bmp");
        }
    }
}
