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
        private Uri _selectedImage;
        private Uri _editedImage;

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

        public Uri selectedImage
        {
            get => _selectedImage;
            set
            {
                if (Equals(value, _selectedImage)) return;
                _selectedImage = value;
                OnPropertyChanged();
            }
        }

        public Uri editedImage
        {
            get => _editedImage;
            set
            {
                if (Equals(value, _editedImage)) return;
                _editedImage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor
        public MainPageViewModel() { }


        #endregion

        public async void loadImage()
        {
            var file = await ImageDirectoryChooserGenerator.ChooseImageFileDialog();
            this.selectedImage = new Uri(file.Path);

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
