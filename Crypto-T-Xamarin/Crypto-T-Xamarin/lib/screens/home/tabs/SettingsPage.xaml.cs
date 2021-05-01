using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto_T_Xamarin.lib.api;
using Crypto_T_Xamarin.lib.screens.auth;
using RedCorners.Forms.Localization;
using Xamarin.Essentials;
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
            Application.Current.MainPage = new NavigationPage(new AuthPage());
        }
        
        private void ThemeAuto_OnClicked(object sender, EventArgs e)
        {
            Application.Current.UserAppTheme = OSAppTheme.Unspecified;
        }
        
        private void ThemeLight_OnClicked(object sender, EventArgs e)
        {
            Application.Current.UserAppTheme = OSAppTheme.Light;
        }
        
        private void ThemeDark_OnClicked(object sender, EventArgs e)
        {
            Application.Current.UserAppTheme = OSAppTheme.Dark;
        }
        
        private void LanguageAuto_OnClicked(object sender, EventArgs e)
        {
            Preferences.Remove("lang");
            HomePage.SavedPageIndex = 2;
            RL.SetLanguage("En");
            Application.Current.MainPage = new NavigationPage(new HomePage());
        }
        
        private void LanguageEnglish_OnClicked(object sender, EventArgs e)
        {
            Preferences.Set("lang", "En");
            HomePage.SavedPageIndex = 2;
            RL.SetLanguage("En");
            Application.Current.MainPage = new NavigationPage(new HomePage());
        }
        
        private void LanguageRussian_OnClicked(object sender, EventArgs e)
        {
            Preferences.Set("lang", "Ru");
            HomePage.SavedPageIndex = 2;
            RL.SetLanguage("Ru");
            Application.Current.MainPage = new NavigationPage(new HomePage());
        }
    }
}