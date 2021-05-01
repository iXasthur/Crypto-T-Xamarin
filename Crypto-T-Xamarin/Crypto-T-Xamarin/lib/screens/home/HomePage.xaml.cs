using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto_T_Xamarin.lib.screens.creator;
using RedCorners.Forms.Localization;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.home
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : TabbedPage
    {
        public static int SavedPageIndex = 0;
        
        public HomePage()
        {
            InitializeComponent();

            Title = "Crypto-T";

            TabbedPager.CurrentPage = TabbedPager.Children[SavedPageIndex];
            SavedPageIndex = 0;
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

