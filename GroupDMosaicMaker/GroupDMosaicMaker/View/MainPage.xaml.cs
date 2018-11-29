using Windows.UI.Xaml.Controls;
using GroupDMosaicMaker.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GroupDMosaicMaker
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel viewModel;
        public MainPage()
        {
            this.viewModel = new MainPageViewModel();
            this.DataContext = viewModel;
            this.InitializeComponent();
        }
      

        
    }
}
