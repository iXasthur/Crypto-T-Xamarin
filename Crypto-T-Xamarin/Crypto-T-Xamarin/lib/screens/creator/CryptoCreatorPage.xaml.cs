using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.creator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CryptoCreatorPage : ContentPage
    {
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
                    Console.WriteLine("Save");
                }),
                Text = "Save",
                Priority = 0
            };
            ToolbarItems.Add(saveButton);
        }
    }
}