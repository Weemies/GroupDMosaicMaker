using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml.Controls;
using GroupDMosaicMaker.Model;

namespace GroupDMosaicMaker
{
    public static class ImageParser
    {
        public static async Task<List<MosaicImage>> ParseDirectoryToImageList(StorageFolder folder)
        {
            var imageList = new List<MosaicImage>();
            DirectoryInfo dir = new DirectoryInfo(folder.Path);

            foreach (FileInfo file in dir.GetFiles())

            {

                try

                {

                    List<string> fileTypeFilter = new List<string>();
                    fileTypeFilter.Add(".jpg");
                    fileTypeFilter.Add(".png");
                    QueryOptions queryOptions = new QueryOptions(Windows.Storage.Search.CommonFileQuery.OrderByName, fileTypeFilter);
                    StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(queryOptions);
                    var files = await queryResult.GetFilesAsync();
                    foreach (StorageFile x in files)
                    {
                        imageList.Add(new MosaicImage(new Uri((String.Format("ms-appdata:///local/{0}", x.Name)))));
                    }

                }

                catch
                {

                    Console.WriteLine("This is not an image file");

                }

            }

            return imageList;
        }
    }
}


