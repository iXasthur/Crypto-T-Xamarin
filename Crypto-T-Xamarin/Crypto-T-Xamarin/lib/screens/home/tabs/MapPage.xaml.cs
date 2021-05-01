using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto_T_Xamarin.lib.api;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.home.tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            GoogleMap.Pins.Clear();
            
            Session.Shared.getLocalAssets()?.ForEach(asset =>
            {
                var e = asset.suggestedEvent;
                if (e != null)
                {
                    var pin = new Pin();
                    pin.Position = new Position(
                        Convert.ToDouble(e.Value.latitude),
                        Convert.ToDouble(e.Value.longitude)
                        );
                    pin.Label = asset.name;
                    pin.Address = e.Value.note;
                    GoogleMap.Pins.Add(pin);
                }
            });
        }
    }
}