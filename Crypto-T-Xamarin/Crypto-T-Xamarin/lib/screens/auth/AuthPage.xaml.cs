using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto_T_Xamarin.lib.api;
using Crypto_T_Xamarin.lib.screens.home;
using RedCorners.Forms.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Crypto_T_Xamarin.lib.screens.auth
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthPage : ContentPage
    {

        private bool _onAppearCalledOnce = false;
        
        private string email => EmailEntry.Text;
        private string password => PasswordEntry.Text;

        public AuthPage()
        {
            InitializeComponent();

            SomethingWentWrongLabel.IsVisible = false;
            EmailEntry.Text = "";
            PasswordEntry.Text = "";
            
            Title = RL.L("Authorization");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!_onAppearCalledOnce)
            {
                _onAppearCalledOnce = true;
            }
            else
            {
                return;
            }
            
            if (Session.Shared.isInitialized())
            {
                Navigation.PushModalAsync(new NavigationPage(new HomePage()));
            }
            else
            {
                var restoredAuthData = Session.Shared.restore(error =>
                {
                    if (error != null) {
                        Device.BeginInvokeOnMainThread (() => {
                            SomethingWentWrongLabel.IsVisible = false;
                        });
                    } else {
                        Device.BeginInvokeOnMainThread (() => {
                            SomethingWentWrongLabel.IsVisible = false;
                            Navigation.PushModalAsync(new NavigationPage(new HomePage()));
                        });
                    }
                    return error;
                });

                if (restoredAuthData != null)
                {
                    EmailEntry.Text = restoredAuthData.Value.email;
                    PasswordEntry.Text = restoredAuthData.Value.password;
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            EmailEntry.Text = "";
            PasswordEntry.Text = "";
        }

        private void SignIn_OnClicked(object sender, EventArgs e)
        {
            SomethingWentWrongLabel.IsVisible = false;
            
            if (ValidateInput())
            {
                Session.Shared.signInEmail(email, password, error =>
                {
                    if (error == null)
                    {
                        Device.BeginInvokeOnMainThread (() => {
                            Navigation.PushModalAsync(new NavigationPage(new HomePage()));
                        });
                    }
                    else
                    {
                        Console.WriteLine(error.Message);
                        Device.BeginInvokeOnMainThread (() => {
                            SomethingWentWrongLabel.IsVisible = true;
                        });
                    }
                    return error;
                });
            }
            else
            {
                SomethingWentWrongLabel.IsVisible = true;
            }
        }

        private void SignUp_OnClicked(object sender, EventArgs e)
        {
            SomethingWentWrongLabel.IsVisible = false;
            
            if (ValidateInput())
            {
                Session.Shared.signUpEmail(email, password, error =>
                {
                    if (error == null)
                    {
                        Device.BeginInvokeOnMainThread (() => {
                            Navigation.PushModalAsync(new NavigationPage(new HomePage()));
                        });
                    }
                    else
                    {
                        Console.WriteLine(error.Message);
                        Device.BeginInvokeOnMainThread (() => {
                            SomethingWentWrongLabel.IsVisible = true;
                        });
                    }
                    return error;
                });
            }
            else
            {
                SomethingWentWrongLabel.IsVisible = true;
            }
        }

        private bool ValidateInput()
        {
            if (email.Length > 2 && password.Length > 5)
            {
                return true;
            }
            return false;
        }
    }
}