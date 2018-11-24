using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace GroupDMosaicMaker.Model
{
    public class MosaicImage 
    {
        public Uri Uri{ get; set; }

        public MosaicImage(Uri uri)
        {
            this.Uri = uri ?? throw new ArgumentNullException();
        }
    }
}
