﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.home
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }
        protected override bool OnBackButtonPressed()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IOnBackPressed>().CloseApp();
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }
    }
}

public interface IOnBackPressed
{
    void CloseApp();
}

