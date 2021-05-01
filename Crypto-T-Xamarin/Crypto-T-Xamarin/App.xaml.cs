using System;
using Crypto_T_Xamarin.lib.api;
using Crypto_T_Xamarin.lib.screens.auth;
using Crypto_T_Xamarin.lib.screens.home;
using RedCorners.Forms.Localization;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Crypto_T_Xamarin
{
    public partial class App : Application
    {
        public App()
        {
            RL.Load(typeof(App), "Crypto_T_Xamarin.", ".trans.json");
            
            InitializeComponent();
            
            RL.SetLanguage(Preferences.Get("lang", "En"));

            UpdateMainPage();
        }

        public void UpdateMainPage()
        {
            if (Session.Shared.isInitialized())
            {
                MainPage = new NavigationPage(new HomePage());
            }
            else
            {
                MainPage = new NavigationPage(new AuthPage());
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}