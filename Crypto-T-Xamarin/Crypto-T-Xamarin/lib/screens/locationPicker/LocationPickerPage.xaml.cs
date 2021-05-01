using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.locationPicker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationPickerPage : ContentPage
    {

        private Action<Position?> completion;
        private Position? _selectedPosition = null;
        
        public LocationPickerPage(Action<Position?> completion)
        {
            this.completion = completion;
            InitializeComponent();

            Title = "Pick Location";
            
            var cancelButton = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    Device.BeginInvokeOnMainThread (() =>
                    {
                        Navigation.PopModalAsync();
                    });
                }),
                Text = "Cancel",
                Priority = 0
            };
            ToolbarItems.Add(cancelButton);
            
            var pickButton = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    Device.BeginInvokeOnMainThread (() =>
                    {
                        _selectedPosition = GoogleMap.VisibleRegion.Center;
                        Navigation.PopModalAsync();
                    });
                }),
                Text = "Pick",
                Priority = 0
            };
            ToolbarItems.Add(pickButton);
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            completion(_selectedPosition);
        }
    }
}