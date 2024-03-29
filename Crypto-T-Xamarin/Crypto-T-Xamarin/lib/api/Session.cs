﻿using System;
using System.Collections.Generic;
using Crypto_T_Xamarin.lib.models.auth;
using Crypto_T_Xamarin.lib.models.crypto;
using Plugin.FirebaseAuth;

namespace Crypto_T_Xamarin.lib.api
{
    public class Session
    {
        public static Session Shared = new Session();
        
        private Session() { }

        private AuthData? authData = null;
        private List<CryptoAsset>? assets = null;
        
        private bool initialized = false;

        private CryptoAssetFirebaseManager cryptoAssetManager = new CryptoAssetFirebaseManager();

        public bool isInitialized()
        {
            return initialized;
        }
        
        public List<CryptoAsset>? getLocalAssets()
        {
            return assets;
        }
        
        public CryptoAsset? getLocalAsset(string id)
        {
            var index = getLocalAssets()?.FindIndex(asset0 => asset0.id == id);
            if (index != null && index != -1)
            {
                return assets![index.Value];
            }
            else
            {
                return null;
            }
        }
        
        private void deleteLocalAsset(CryptoAsset asset)
        {
            assets?.Remove(asset);
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
            var index = getLocalAssets()?.FindIndex(asset0 => asset0.id == asset.id);
            if (index != null && index != -1)
            {
                assets![index.Value] = asset;
            } else {
                assets!.Add(asset);
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
                    this.assets = new List<CryptoAsset>();
                } else if (assets != null)
                {
                    this.assets = assets;
                } else
                {
                    Console.WriteLine("Didn't receive assets and error");
                    this.assets = new List<CryptoAsset>();
                }

                onCompleted();

                return assets;
            });
        }

        private void initialize(AuthData authData, Action onCompleted)
        {
            this.authData = authData;

            AuthDataStorage.Save(authData);

            assets = new List<CryptoAsset>();

            syncDashboard(() =>
            {
                initialized = true;
                onCompleted();
            });
        }
        
        public void destroy()
        {
            initialized = false;
            
            try
            {
                CrossFirebaseAuth.Current.Instance.SignOut();
            } catch (Exception error) {
                Console.WriteLine(error);
            }

            AuthDataStorage.Delete();
            authData = null;
            assets = null;
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
        
        public void signUpEmail(String email, string password, Func<Exception?, Exception?> completion)
        {
            CrossFirebaseAuth.Current.Instance.CreateUserWithEmailAndPasswordAsync(email, password)
                .ContinueWith(task =>
                {
                    var authData = new AuthData {email = email, password = password};
                    handleFirebaseAuthResponse(authData, task.Exception, completion);
                });
        }
        
        public void signInEmail(String email, string password, Func<Exception?, Exception?> completion)
        {
            CrossFirebaseAuth.Current.Instance.SignInWithEmailAndPasswordAsync(email, password)
                .ContinueWith(task =>
                {
                    var authData = new AuthData {email = email, password = password};
                    handleFirebaseAuthResponse(authData, task.Exception, completion);
                });
        }
        
        private void handleFirebaseAuthResponse(AuthData authData, Exception? error, Func<Exception?, Exception?> completion) {
            if (error != null)
            {
                completion(error);
                return;
            }

            initialize(authData, () => {
                if (initialized)
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