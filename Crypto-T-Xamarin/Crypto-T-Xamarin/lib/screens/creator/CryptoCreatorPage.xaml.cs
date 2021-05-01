using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crypto_T_Xamarin.lib.api;
using Crypto_T_Xamarin.lib.models.crypto;
using Crypto_T_Xamarin.lib.screens.locationPicker;
using Octane.Xamarin.Forms.VideoPlayer;
using Plugin.Media;
using Plugin.Media.Abstractions;
using RedCorners.Forms.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
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

        private Position? eventPosition = null;

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

            var e = assetToEdit.suggestedEvent;
            if (e != null)
            {
                eventPosition = new Position(Convert.ToDouble(e.Value.latitude), Convert.ToDouble(e.Value.longitude));
                EventNoteEntry.Text = e.Value.note;
            }
            
            Title = RL.L("Edit");
            
            CreateMenuButtons();
            
            UpdateMediaUI();
            
            stopAnimation();
        }
        
        public CryptoCreatorPage()
        {
            InitializeComponent();
            Title = RL.L("New");
            CreateMenuButtons();
            
            UpdateMediaUI();
            
            stopAnimation();
        }

        private void CreateMenuButtons()
        {
            if (_assetToEdit != null)
            {
                var deleteButton = new ToolbarItem
                {
                    Command = new Command(() =>
                    {
                        startAnimation();
                        
                        Session.Shared.deleteRemoteAsset(_assetToEdit.Value, error =>
                        {
                            stopAnimation();
                            
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
                    Text = RL.L("Delete"),
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
                        
                        if (eventPosition != null)
                        {
                            asset.suggestedEvent = new CryptoEvent
                            {
                                latitude = eventPosition.Value.Latitude.ToString(CultureInfo.InvariantCulture),
                                longitude = eventPosition.Value.Longitude.ToString(CultureInfo.InvariantCulture),
                                note = EventNoteEntry.Text
                            };
                        } else {
                            asset.suggestedEvent = null;
                        }
                        
                        startAnimation();
                        
                        Session.Shared.updateRemoteAsset(asset, iconUri, videoUri, error =>
                        {
                            stopAnimation();
                            
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
                Text = RL.L("Save"),
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
            Device.BeginInvokeOnMainThread (() => {
                if (iconUri != null)
                {
                    CryptoImage.Source =
                        iconUri.Scheme == "file"
                            ? ImageSource.FromFile(iconUri.AbsolutePath)
                            : new UriImageSource
                            {
                                Uri = iconUri,
                                CachingEnabled = true,
                                CacheValidity = new TimeSpan(0, 0, 5, 0)
                            };
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
            
                if (eventPosition != null)
                {
                    EventLatitudeLabel.Text = eventPosition.Value.Latitude.ToString(CultureInfo.InvariantCulture);
                    EventLongitudeLabel.Text = eventPosition.Value.Longitude.ToString(CultureInfo.InvariantCulture);
                
                    EventLatitudeLabel.IsVisible = true;
                    EventLongitudeLabel.IsVisible = true;
                    EventNoteEntry.IsVisible = true;
                    DeleteEventButton.IsVisible = true;
                }
                else
                {
                    EventLatitudeLabel.IsVisible = false;
                    EventLongitudeLabel.IsVisible = false;
                    EventNoteEntry.IsVisible = false;
                    DeleteEventButton.IsVisible = false;
                }
            });
        }

        private async void SelectImage_OnClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            var file = await CrossMedia.Current.PickPhotoAsync();
            if (file != null)
            {
                iconUri = new Uri(file.Path);
                UpdateMediaUI();
            }
        }
        
        private void DeleteImage_OnClicked(object sender, EventArgs e)
        {
            iconUri = null;
            UpdateMediaUI();
        }
        
        private async void SelectVideo_OnClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            var file = await CrossMedia.Current.PickVideoAsync();
            if (file != null)
            {
                videoUri = new Uri(file.Path);
                UpdateMediaUI();
            }
        }
        
        private void DeleteVideo_OnClicked(object sender, EventArgs e)
        {
            videoUri = null;
            UpdateMediaUI();
        }
        
        private void PickLocation_OnClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new LocationPickerPage(position =>
            {
                eventPosition = position;
                UpdateMediaUI();
            })));
        }
            
        private void DeleteEvent_OnClicked(object sender, EventArgs e)
        {
            eventPosition = null;
            EventNoteEntry.Text = "";
            UpdateMediaUI();
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            CryptoVideo.Pause();
            CryptoVideo.IsVisible = false;
        }

        protected override bool OnBackButtonPressed()
        {
            return ActivityIndicatorView.IsRunning;
        }

        private void startAnimation()
        {
            Device.BeginInvokeOnMainThread (() => {
                NavigationPage.SetHasBackButton(this, false);
                ActivityIndicatorView.IsRunning = true;
                Content.IsEnabled = false;
            });
        }
        
        private void stopAnimation()
        {
            Device.BeginInvokeOnMainThread (() => {
                NavigationPage.SetHasBackButton(this, true);
                ActivityIndicatorView.IsRunning = false;
                Content.IsEnabled = true;
            });
        }
    }
}