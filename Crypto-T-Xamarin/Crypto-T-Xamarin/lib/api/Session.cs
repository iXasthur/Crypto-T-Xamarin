using System;
using System.Collections.Generic;
using Crypto_T_Xamarin.lib.models.auth;
using Crypto_T_Xamarin.lib.models.crypto;

namespace Crypto_T_Xamarin.lib.api
{
    public class Session
    {

        private AuthData? authData = null;
        private CryptoDashboard? dashboard = null;
        private bool initialized = false;
        private CryptoAssetFirebaseManager cryptoAssetManager = new CryptoAssetFirebaseManager();

        public CryptoAsset? SelectedAsset = null;

        public bool IsInitialized()
        {
            return initialized;
        }

        public List<CryptoAsset> GetLocalAssets()
        {
            return dashboard?.assets;
        }
        
        public CryptoAsset? AetLocalAsset(string id)
        {
            return GetLocalAssets().Find(asset => asset.id == id);
        }
        
        
        private void DeleteLocalAsset(CryptoAsset asset)
        {
            int? index = GetLocalAssets()?.FindIndex(asset0 =>
                asset0.id == asset.id
            );
            
            if (index != null && index != -1) {
                dashboard?.assets?.RemoveAt(index.Value);
                if (SelectedAsset?.id == asset.id)
                {
                    SelectedAsset = null;
                }
            }
        }
        
        public void DeleteRemoteAsset(CryptoAsset asset, Func<Exception> completion) {
            // cryptoAssetManager.deleteRemoteAsset(asset) { error ->
            //     if (error != null) {
            //         println(error)
            //         completion(error)
            //     } else {
            //         this.deleteLocalAsset(asset)
            //         completion(null)
            //     }
            // }
        }
        //
        // private fun updateLocalAsset(asset: CryptoAsset) {
        //     val index = getLocalAssets()?.indexOfFirst { asset0 ->
        //         asset0.id == asset.id
        //     }
        //     if (index != null && index != -1) {
        //         dashboard?.assets?.set(index, asset)
        //         if (selectedAsset?.id == asset.id) {
        //             selectedAsset = asset
        //         }
        //     } else {
        //         dashboard?.assets?.add(asset)
        //     }
        // }
        //
        // fun updateRemoteAsset(
        //     asset: CryptoAsset,
        //     iconUri: Uri?,
        //     videoUri: Uri?,
        //     completion: (Exception?) -> Unit
        // ) {
        //     cryptoAssetManager.updateRemoteAsset(asset, iconUri, videoUri) { updatedAsset, error ->
        //         if (error != null) {
        //             println(error)
        //             completion(error)
        //         } else if (updatedAsset != null) {
        //             this.updateLocalAsset(updatedAsset)
        //             completion(null)
        //         } else {
        //             completion(Exception("Invalid updateRemoteAsset form CryptoAssetFirebaseManager closure return"))
        //         }
        //     }
        // }
        //
        // fun syncDashboard(onCompleted: () -> Unit) {
        //     cryptoAssetManager.getRemoteAssets { assets, error ->
        //         if (error != null) {
        //             println(error)
        //             this.dashboard?.assets = ArrayList()
        //             this.selectedAsset = null
        //         } else if (assets != null) {
        //             this.dashboard?.assets = assets
        //             if (selectedAsset != null) {
        //                 this.selectedAsset = getLocalAsset(selectedAsset!!.id)
        //             }
        //         } else {
        //             println("Didn't receive assets and error")
        //             this.dashboard?.assets = ArrayList()
        //             this.selectedAsset = null
        //         }
        //         onCompleted()
        //     }
        // }
        //
        // private fun initialize(authData: AuthData, onCompleted: () -> Unit) {
        //     this.authData = authData
        //
        //     AuthDataStorage.save(authData)
        //
        //     dashboard = CryptoDashboard()
        //
        //     syncDashboard {
        //         this.initialized = true
        //         onCompleted()
        //     }
        //
        // }
        //
        // fun destroy() {
        //     initialized = false
        //     
        //     try {
        //         FirebaseAuth.getInstance().signOut()
        //     } catch (error: Throwable) {
        //         println(error)
        //     }
        //
        //     AuthDataStorage.delete()
        //     selectedAsset = null
        //     authData = null
        //     dashboard = null
        // }
        //
        // fun restore(completion: (Exception?) -> Unit): AuthData? {
        //     val authData = AuthDataStorage.restore()
        //     if (authData != null) {
        //         signInEmail(authData.email, authData.password) { error ->
        //             this.handleFirebaseAuthResponse(authData, error, completion)
        //         }
        //         return authData
        //     } else {
        //         completion(Exception("Unable to restore session"))
        //         return null
        //     }
        // }
        //
        // fun signUpEmail(email: String, password: String, completion: (Exception?) -> Unit) {
        //     val authData = AuthData(email, password)
        //     FirebaseAuth.getInstance()
        //         .createUserWithEmailAndPassword(email, password)
        //         .addOnSuccessListener { _ ->
        //             this.handleFirebaseAuthResponse(authData, null, completion)
        //         }
        //         .addOnFailureListener { e ->
        //             this.handleFirebaseAuthResponse(authData, e, completion)
        //         }
        // }
        //
        // fun signInEmail(email: String, password: String, completion: (Exception?) -> Unit) {
        //     val authData = AuthData(email, password)
        //     FirebaseAuth.getInstance()
        //         .signInWithEmailAndPassword(email, password)
        //         .addOnSuccessListener { _ ->
        //             this.handleFirebaseAuthResponse(authData, null, completion)
        //         }
        //         .addOnFailureListener { e ->
        //             this.handleFirebaseAuthResponse(authData, e, completion)
        //         }
        // }
        //
        // private fun handleFirebaseAuthResponse(authData: AuthData, error: Exception?, completion: (Exception?) -> Unit) {
        //     if (error != null) {
        //         completion(error)
        //         return
        //     }
        //
        //     initialize(authData) {
        //         if (this.initialized) {
        //             completion(null)
        //         } else {
        //             completion(Exception("Unable to initialize session"))
        //         }
        //     }
        // }
    }
}