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

        private CryptoAsset? _assetToEdit = null;

        private string name => NameEntry.Text;
        private string code => CodeEntry.Text;
        private string description => DescriptionEntry.Text;

        private Uri? iconUri = null;
        private Uri? videoUri = null;

        public CryptoCreatorPage(CryptoAsset assetToEdit)
        {
            _assetToEdit = assetToEdit;
            if (assetToEdit.iconFileData?.downloadURL != null)
            {
                iconUri = new Uri(assetToEdit.iconFileData.Value.downloadURL);
            }
            if (assetToEdit.videoFileData?.downloadURL != null)
            {
                videoUri = new Uri(assetToEdit.videoFileData.Value.downloadURL);
            }
            InitializeComponent();

            NameEntry.Text = _assetToEdit.Value.name;
            CodeEntry.Text = _assetToEdit.Value.code;
            DescriptionEntry.Text = _assetToEdit.Value.description;
            
            Title = "Edit Crypto";
            
            CreateMenuButtons();
            
            UpdateMediaUI();
        }
        
        public CryptoCreatorPage()
        {
            InitializeComponent();
            Title = "New Crypto";
            CreateMenuButtons();
            
            UpdateMediaUI();
        }

        private void CreateMenuButtons()
        {
            if (_assetToEdit != null)
            {
                var deleteButton = new ToolbarItem
                {
                    Command = new Command(() =>
                    {
                        Session.Shared.deleteRemoteAsset(_assetToEdit.Value, error =>
                        {
                            if (error == null)
                            {
                                Device.BeginInvokeOnMainThread (() =>
                                {
                                    Navigation.PopAsync();
                                });
                            }
                            return error;
                        });
                    }),
                    Text = "Delete",
                    Priority = 0
                };
                ToolbarItems.Add(deleteButton);
            }

            var saveButton = new ToolbarItem {
                Command = new Command(() =>
                {
                    if (ValidateInputs())
                    {
                        CryptoAsset asset;
                        if (_assetToEdit != null)
                        {
                            asset = new CryptoAsset
                            {
                                id = _assetToEdit.Value.id,
                                name = name,
                                code = code,
                                description = description,
                                iconFileData = _assetToEdit?.iconFileData,
                                suggestedEvent = _assetToEdit?.suggestedEvent,
                                videoFileData = _assetToEdit?.videoFileData
                            };
                        }
                        else
                        {
                            asset = new CryptoAsset
                            {
                                id = Guid.NewGuid().ToString(),
                                name = name,
                                code = code,
                                description = description,
                                iconFileData = null,
                                suggestedEvent = null,
                                videoFileData = null
                            };
                        }
                        
                        Session.Shared.updateRemoteAsset(asset, iconUri, videoUri, error =>
                        {
                            if (error == null)
                            {
                                Device.BeginInvokeOnMainThread (() =>
                                {
                                    Navigation.PopAsync();
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

        private void UpdateMediaUI()
        {
            if (iconUri != null)
            {
                CryptoImage.Source = iconUri;
                CryptoImage.IsVisible = true;
                DeleteImageButton.IsVisible = true;
            }
            else
            {
                CryptoImage.Source = null;
                CryptoImage.IsVisible = false;
                DeleteImageButton.IsVisible = false;
            }

            if (videoUri != null)
            {
                CryptoVideo.Source = videoUri;
                CryptoVideo.IsVisible = true;
                DeleteVideoButton.IsVisible = true;
            }
            else
            {
                CryptoVideo.Source = null;
                CryptoVideo.Pause();
                CryptoVideo.IsVisible = false;
                DeleteVideoButton.IsVisible = false;
            }
        }

        private void SelectImage_OnClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        
        private void DeleteImage_OnClicked(object sender, EventArgs e)
        {
            iconUri = null;
            UpdateMediaUI();
        }
        
        private void SelectVideo_OnClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        
        private void DeleteVideo_OnClicked(object sender, EventArgs e)
        {
            videoUri = null;
            UpdateMediaUI();
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            CryptoVideo.Pause();
            CryptoVideo.IsVisible = false;
        }
    }
}