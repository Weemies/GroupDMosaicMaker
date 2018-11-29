using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;

namespace GroupDMosaicMaker.View
{
   public static class ImageDirectoryChooserGenerator
    {

        public static async Task<StorageFolder> ChooseImageFolderDialog()
        {
            var directoryPicker = new FolderPicker();
            directoryPicker.FileTypeFilter.Add(".png");
            directoryPicker.FileTypeFilter.Add(".jpeg");
            directoryPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

            var directoryResult = await directoryPicker.PickSingleFolderAsync();
            if (directoryResult != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", directoryResult);
                
            }

            return directoryResult;
        }

        public static async Task<StorageFile> ChooseImageFileDialog()
        {
         
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            return file;

        }

        private static async void SaveFileDialog(byte[] data )
        {
          

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            savePicker.FileTypeChoices.Add("jpeg", new List<string>() { ".jpeg" });
            savePicker.FileTypeChoices.Add("gif", new List<string>() { ".gif" });
            savePicker.FileTypeChoices.Add("png", new List<string>() { ".png" });
            savePicker.SuggestedFileName = "New Document";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                await FileIO.WriteBytesAsync(file, data);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
               
            }
            
        }


    }
}
