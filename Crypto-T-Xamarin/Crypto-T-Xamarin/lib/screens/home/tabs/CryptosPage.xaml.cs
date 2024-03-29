﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto_T_Xamarin.lib.api;
using Crypto_T_Xamarin.lib.screens.creator;
using Crypto_T_Xamarin.lib.screens.details;
using Crypto_T_Xamarin.lib.utils;
using RedCorners.Forms.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.home.tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CryptosPage : ContentPage
    {

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
            
            var newCrypto = new ToolbarItem {
                Command = new Command(() =>
                {
                    Device.BeginInvokeOnMainThread (() => {
                        Navigation.PushAsync(new CryptoCreatorPage());
                    });
                }),
                Text = RL.L("New"),
                Priority = 0
            } ;

            ToolbarItems.Add(newCrypto);
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

                var cell = new ImageCell
                {
                    ImageSource = 
                        imageUri.Scheme == "file"
                            ? ImageSource.FromFile(imageUri.AbsolutePath)
                            : new UriImageSource
                            {
                                Uri = imageUri,
                                CachingEnabled = true,
                                CacheValidity = new TimeSpan(0, 0, 5, 0)
                            },
                    Text = asset.name,
                    Detail = asset.code,
                    Command = new Command(() =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Navigation.PushAsync(new CryptoDetailsPage(asset));
                        });
                    })
                };

                cells.Add(cell);
            });

            _tableView.Root = new TableRoot
            {
                new TableSection
                {
                    cells
                }
            };
            
        }
    }
}