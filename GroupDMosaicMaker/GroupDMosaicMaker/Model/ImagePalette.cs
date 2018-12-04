using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Windows.Storage;
using MoreLinq;

namespace GroupDMosaicMaker.Model
{
    public class ImagePalette
    {

        #region Data Members

        public ICollection<MosaicImage> imageCollection;

        #endregion

        #region Constructors

        public ImagePalette()
        {
            this.imageCollection = new List<MosaicImage>();
        }

        #endregion

        #region Methods


        public void AddImage(StorageFile file)
        {
            MosaicImage newImage = new MosaicImage(file);
            this.imageCollection.Add(newImage);
        }
        public MosaicImage CalculateBestImageMatch(Color targetColor)
        {
            var closestValue = int.MaxValue;
            MosaicImage closestImage = null;
            foreach (var image in this.imageCollection)
            {
                if (image.colorDiff(image.AverageColor, targetColor) <= closestValue)
                {
                    closestValue = image.colorDiff(image.AverageColor, targetColor);
                    closestImage = image;
                }
            }
            return closestImage;
        }

        #endregion

    }
}
