using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto_T_Xamarin.lib.api;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.home.tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void SignOut_OnClicked(object sender, EventArgs e)
        {
            Session.Shared.destroy();
            Navigation.PopModalAsync();
        }
    }
}