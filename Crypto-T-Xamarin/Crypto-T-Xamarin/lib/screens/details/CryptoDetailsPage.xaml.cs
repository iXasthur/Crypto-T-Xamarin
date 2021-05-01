using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto_T_Xamarin.lib.api;
using Crypto_T_Xamarin.lib.models.crypto;
using Crypto_T_Xamarin.lib.screens.creator;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.details
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CryptoDetailsPage : ContentPage
    {

        private string _assetId;
        
        public CryptoDetailsPage(CryptoAsset asset)
        {
            InitializeComponent();
            
            _assetId = asset.id;
            Title = "Details";
            
            UpdateDetailsUi(asset);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var asset = Session.Shared.getLocalAsset(_assetId);
            if (asset != null)
            {
                UpdateDetailsUi(asset.Value);
            }
            else
            {
                Navigation.PopAsync();
            }
        }

        private void UpdateDetailsUi(CryptoAsset asset)
        {
            CryptoNameLabel.Text = asset.name;
            CryptoCodeLabel.Text = asset.code;
            CryptoDescriptionLabel.Text = asset.description;
            
            ToolbarItems.Clear();
            
            var editCrypto = new ToolbarItem {
                Command = new Command(() =>
                {
                    Device.BeginInvokeOnMainThread (() => {
                        Navigation.PushAsync(new CryptoCreatorPage(asset));
                    });
                }),
                Text = "Edit",
                Priority = 0
            } ;

            ToolbarItems.Add(editCrypto);
        }
    }
}