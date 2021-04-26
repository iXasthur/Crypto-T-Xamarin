﻿using System;
using Crypto_T_Xamarin.lib.api;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Crypto_T_Xamarin
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            new CryptoAssetFirebaseManager().getRemoteAssets((a, b) =>
            {
                foreach (var cryptoAsset in a)
                {
                    Console.WriteLine(a.Count);
                }

                return a;
            });
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            new CryptoAssetFirebaseManager().getRemoteAssets((a, b) =>
            {
                foreach (var cryptoAsset in a)
                {
                    Console.WriteLine(a.Count);
                }

                return a;
            });
        }
    }
}