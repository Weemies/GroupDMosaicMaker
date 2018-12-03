using System;
using System.Collections.Generic;
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
            var closestImage = this.imageCollection.MinBy(n => colorDiff(n.AverageColor, targetColor)).Min(n => n);
            return closestImage;
        }


        private static int colorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                  + (c1.G - c2.G) * (c1.G - c2.G)
                                  + (c1.B - c2.B) * (c1.B - c2.B));
        }

        #endregion

    }
}
