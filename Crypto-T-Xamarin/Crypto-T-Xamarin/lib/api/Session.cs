using System;
using System.Collections.Generic;
using Crypto_T_Xamarin.lib.models.auth;
using Crypto_T_Xamarin.lib.models.crypto;
using Crypto_T_Xamarin.lib.utils;
using Firebase.Auth;
using Firebase.Auth.Providers;

namespace Crypto_T_Xamarin.lib.api
{
    public class Session
    {
        private FirebaseAuthClient authClient = new FirebaseAuthClient(new FirebaseAuthConfig {ApiKey = Secrets.FirebaseWebApiKey});
        
        private AuthData? authData = null;
        private CryptoDashboard? dashboard = null;
        
        private bool initialized = false;

        private CryptoAssetFirebaseManager cryptoAssetManager = new CryptoAssetFirebaseManager();
        
        CryptoAsset? selectedAsset = null;

        public bool isInitialized()
        {
            return initialized;
        }
        
        public List<CryptoAsset>? getLocalAssets()
        {
            return dashboard?.assets;
        }
        
        public CryptoAsset? getLocalAsset(string id)
        {
            return getLocalAssets()?.Find(asset => asset.id == id);
        }
        
        private void deleteLocalAsset(CryptoAsset asset)
        {
            var removed = dashboard?.assets.Remove(asset);
            if (removed != null && removed.Value && selectedAsset?.id == asset.id)
            {
                selectedAsset = null;
            }
        }
        
        public void deleteRemoteAsset(CryptoAsset asset, Func<Exception?, Exception?> completion)
        {
            cryptoAssetManager.deleteRemoteAsset(asset, error =>
            {
                if (error != null)
                {
                    Console.WriteLine(error);
                    completion(error);
                } else
                {
                    deleteLocalAsset(asset);
                    completion(null);
                }
                return error;
            });
        }
        
        private void updateLocalAsset(CryptoAsset asset)
        {
            if (dashboard == null) return;
            
            var index = getLocalAssets()?.IndexOf(asset);
            if (index != null && index != -1)
            {
                dashboard.Value.assets[index.Value] = asset;
                if (selectedAsset?.id == asset.id)
                {
                    selectedAsset = asset;
                }
            } else {
                dashboard?.assets.Add(asset);
            }
        }
        
        public void updateRemoteAsset(CryptoAsset asset, Uri? iconUri, Uri? videoUri, Func<Exception?, Exception?> completion)
        {
            cryptoAssetManager.updateRemoteAsset(asset, iconUri, videoUri, (updatedAsset, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine(error);
                    completion(error);
                } else if (updatedAsset != null)
                {
                    updateLocalAsset(updatedAsset.Value);
                    completion(null);
                } else
                {
                    completion(
                        new Exception("Invalid updateRemoteAsset form CryptoAssetFirebaseManager closure return"));
                }
                return updatedAsset;
            });
        }
        
        public void syncDashboard(Action onCompleted)
        {
            cryptoAssetManager.getRemoteAssets((assets, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine(error);
                    dashboard?.setAssets(new List<CryptoAsset>());
                    selectedAsset = null;
                } else if (assets != null)
                {
                    dashboard?.setAssets(assets);
                    if (selectedAsset != null)
                    {
                        selectedAsset = getLocalAsset(selectedAsset.Value.id);
                    }
                } else
                {
                    Console.WriteLine("Didn't receive assets and error");
                    dashboard?.setAssets(new List<CryptoAsset>());
                    selectedAsset = null;
                }

                onCompleted();

                return assets;
            });
        }

        private void initialize(AuthData authData, Action onCompleted)
        {
            this.authData = authData;

            AuthDataStorage.Save(authData);

            dashboard = new CryptoDashboard();

            syncDashboard(() =>
            {
                initialized = true;
                onCompleted();
            });
        }
        
        public async void destroyAsync()
        {
            initialized = false;
            
            try
            {
                await authClient!.SignOutAsync();
            } catch (Exception error) {
                Console.WriteLine(error);
            }

            AuthDataStorage.Delete();
            selectedAsset = null;
            authData = null;
            dashboard = null;
        }
        
        public AuthData? restore(Func<Exception?, Exception?> completion)
        {
            var authData = AuthDataStorage.Restore();
            if (authData != null)
            {
                signInEmail(authData.Value.email, authData.Value.password, error =>
                {
                    handleFirebaseAuthResponse(authData.Value, error, completion);
                    return error;
                });
                return authData;
            } else
            {
                completion(new Exception("Unable to restore session"));
                return null;
            }
        }
        
        public void signUpEmail(String email, string password, Func<Exception?, Exception?> completion) {
            authClient.CreateUserWithEmailAndPasswordAsync(email, password)
                .ContinueWith(task =>
                {
                    var authData = new AuthData {email = email, password = password};
                    if (task.IsCompleted)
                    {
                        handleFirebaseAuthResponse(authData, null, completion);
                    }
                    else
                    {
                        handleFirebaseAuthResponse(authData, task.Exception, completion);
                    }
                });
        }
        
        public void signInEmail(String email, string password, Func<Exception?, Exception?> completion)
        {
            authClient.SignInWithEmailAndPasswordAsync(email, password)
                .ContinueWith(task =>
                {
                    var authData = new AuthData {email = email, password = password};
                    if (task.IsCompleted)
                    {
                        handleFirebaseAuthResponse(authData, null, completion);
                    }
                    else
                    {
                        handleFirebaseAuthResponse(authData, task.Exception, completion);
                    }
                });
        }
        
        private void handleFirebaseAuthResponse(AuthData authData, Exception? error, Func<Exception?, Exception?> completion) {
            if (error != null)
            {
                completion(error);
                return;
            }

            initialize(authData, () => {
                if (this.initialized)
                {
                    completion(null);
                } else
                {
                    completion(new Exception("Unable to initialize session"));
                }
            });
        }
    }
}