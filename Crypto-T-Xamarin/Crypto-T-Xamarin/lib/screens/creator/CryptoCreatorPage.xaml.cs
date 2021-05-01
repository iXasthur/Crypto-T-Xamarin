using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto_T_Xamarin.lib.api;
using Crypto_T_Xamarin.lib.models.crypto;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.creator
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CryptoCreatorPage : ContentPage
    {

        private string name => NameEntry.Text;
        private string code => CodeEntry.Text;
        private string description => DescriptionEntry.Text;
        
        public CryptoCreatorPage()
        {
            InitializeComponent();

            Title = "Creator";
            
            var cancelButton = new ToolbarItem {
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
            
            var saveButton = new ToolbarItem {
                Command = new Command(() =>
                {
                    if (ValidateInputs())
                    {
                        var asset = new CryptoAsset
                        {
                            id = Guid.NewGuid().ToString(),
                            name = name,
                            code = code,
                            description = description,
                            iconFileData = null,
                            suggestedEvent = null,
                            videoFileData = null
                        };
                        
                        Session.Shared.updateRemoteAsset(asset, null, null, error =>
                        {
                            if (error == null)
                            {
                                Device.BeginInvokeOnMainThread (() =>
                                {
                                    Navigation.PopModalAsync();
                                });
                            }

                            return error;
                        });
                    }
                    else
                    {
                        Console.WriteLine("ValidateInputs false");
                    }
                }),
                Text = "Save",
                Priority = 0
            };
            ToolbarItems.Add(saveButton);
        }

        private bool ValidateInputs()
        {
            if (name.Length > 2 && code.Length > 2)
            {
                return true;
            }
            return false;
        }
    }
}