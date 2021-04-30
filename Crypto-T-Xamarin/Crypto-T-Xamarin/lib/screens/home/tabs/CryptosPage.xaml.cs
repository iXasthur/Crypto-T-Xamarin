using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto_T_Xamarin.lib.api;
using Crypto_T_Xamarin.lib.utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.home.tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CryptosPage : ContentPage
    {
        
        private List<string> _cryptoNames = new List<string>();
        
        private TableView _tableView = new TableView { Intent = TableIntent.Form };
        
        public CryptosPage()
        {
            InitializeComponent();

            Content = new StackLayout
            {
                Children =
                {
                    _tableView
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            updateTableView();
        }

        private void updateTableView()
        {
            var cells = new List<ImageCell>();
            
            var assets = Session.Shared.getLocalAssets();
            assets?.ForEach(asset =>
            {
                var imageUri = Constants.DefaultIconURI;
                
                if (asset.iconFileData?.downloadURL != null)
                {
                    imageUri = new Uri(asset.iconFileData.Value.downloadURL);
                }
                
                cells.Add(new ImageCell
                {
                    // Some differences with loading images in initial release.
                    ImageSource = ImageSource.FromUri(imageUri),
                    Text = asset.name,
                    Detail = asset.code
                });
            });

            _tableView.Root = new TableRoot
            {
                new TableSection
                {
                    cells
                }
            };
            
        }

        private void CryptosListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}