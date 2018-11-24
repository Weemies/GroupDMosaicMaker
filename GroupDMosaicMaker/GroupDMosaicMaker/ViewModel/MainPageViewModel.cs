using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GroupDMosaicMaker.Annotations;
using GroupDMosaicMaker.Model;
using GroupDMosaicMaker.View;

namespace GroupDMosaicMaker.ViewModel
{
   public class MainPageViewModel : INotifyPropertyChanged 
    {
        #region DataMembers
        private List<MosaicImage> _workingImages;


        #endregion

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        
        public List<MosaicImage> WorkingImages
        {
            get => _workingImages;
            set
            {
                if (Equals(value, _workingImages)) return;
                _workingImages = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region Constructor
       public MainPageViewModel() { }


        #endregion

        public async void loadImages()
        {
            var folder = await Task.Run(() => ImageDirectoryChooserGenerator.ChooseImageFolderDialog().Result);
            this.WorkingImages = await Task.Run(() => ImageParser.ParseDirectoryToImageList(folder).Result);
        }

        #region INotifyPropertyChangedImplementation




        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
