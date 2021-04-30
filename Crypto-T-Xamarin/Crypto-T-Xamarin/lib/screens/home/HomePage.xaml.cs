using System;
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
    public partial class HomePage : TabbedPage
    {
        public HomePage()
        {
            InitializeComponent();

            Title = "Crypto-T";

            var newCrypto = new ToolbarItem {
                Command = new Command(() =>
                {
                    Console.WriteLine("Creator Call");
                }),
                Text = "New",
                Priority = 0
            } ;

            ToolbarItems.Add(newCrypto);
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

