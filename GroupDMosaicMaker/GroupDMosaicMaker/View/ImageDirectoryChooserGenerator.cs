using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

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


    }
}
