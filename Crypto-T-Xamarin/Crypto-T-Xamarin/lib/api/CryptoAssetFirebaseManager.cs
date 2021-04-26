using System;
using System.Collections;
using System.Collections.Generic;
using Crypto_T_Xamarin.lib.models.crypto;
using Crypto_T_Xamarin.lib.utils;
using Plugin.CloudFirestore;

namespace Crypto_T_Xamarin.lib.api
{
    public class CryptoAssetFirebaseManager
    {
        private IFirestore db = CrossCloudFirestore.Current.Instance;
        
        // private val storage = FirebaseStorage.getInstance()
        //
        // fun deleteRemoteAsset(asset: CryptoAsset, completion: (Exception?) -> Unit) {
        //
        //     val iconFile = asset.iconFileData
        //     if (iconFile != null) {
        //         deleteFile(iconFile) { error ->
        //             if (error != null) {
        //                 println(error)
        //             } else {
        //                 println("Deleted file with path $iconFile.path")
        //             }
        //         }
        //     }
        //
        //     val videoFile = asset.videoFileData
        //     if (videoFile != null) {
        //         deleteFile(videoFile) { error ->
        //             if (error != null) {
        //                 println(error)
        //             } else {
        //                 println("Deleted file with path $videoFile.path")
        //             }
        //         }
        //     }
        //
        //     val document = db.collection(Constants.Api.Firebase.assetsCollectionName).document(asset.id)
        //     document
        //         .delete()
        //         .addOnSuccessListener {
        //             completion(null)
        //         }
        //         .addOnFailureListener { e ->
        //             completion(e)
        //         }
        // }
        //
        // private fun getStorageDownloadURL(path: String, completion: (Uri?, Exception?) -> Unit) {
        //     val storageRef = storage.reference
        //     val fileRef = storageRef.child(path)
        //     fileRef
        //         .downloadUrl
        //         .addOnSuccessListener { uri ->
        //             completion(uri, null)
        //         }
        //         .addOnFailureListener { e ->
        //             completion(null, e)
        //         }
        // }
        //
        // private fun deleteFile(file: CloudFileData, completion: (Exception?) -> Unit) {
        //     val storageRef = this.storage.reference
        //     val fileRef = storageRef.child(file.path)
        //
        //     fileRef
        //         .delete()
        //         .addOnSuccessListener {
        //             completion(null)
        //         }
        //         .addOnFailureListener { e ->
        //             completion(e)
        //         }
        // }
        //
        // private fun uploadFile(fileRef: StorageReference, stream: InputStream, metadata: StorageMetadata, completion: (CloudFileData?, Exception?) -> Unit) {
        //     fileRef
        //         .putStream(stream, metadata)
        //         .addOnSuccessListener { _ ->
        //             this.getStorageDownloadURL(fileRef.path) { uri, error ->
        //                 when {
        //                     error != null -> {
        //                         completion(null, error)
        //                     }
        //                     uri != null -> {
        //                         println("Uploaded file $fileRef.path")
        //                         completion(CloudFileData(fileRef.path, uri.toString()), null)
        //                     }
        //                     else -> {
        //                         completion(null, Exception("Both uri and error in uploadFile are null"))
        //                     }
        //                 }
        //             }
        //         }
        //         .addOnFailureListener { e ->
        //             completion(null, e)
        //         }
        // }
        //
        // private fun uploadImage(imageUri: Uri, completion: (CloudFileData?, Exception?) -> Unit) {
        //     try {
        //         val inputStream: InputStream? = App.applicationContext().contentResolver.openInputStream(imageUri)
        //         if (inputStream != null) {
        //             val storageRef = this.storage.reference
        //             val path = "${Constants.Api.Firebase.imagesFolderName}/${UUID.randomUUID()}-android-image"
        //             val videoRef = storageRef.child(path)
        //             val metadata = StorageMetadata()
        //             this.uploadFile(videoRef, inputStream, metadata) { fileData, error ->
        //                 completion(fileData, error)
        //             }
        //         } else {
        //             completion(null, Exception("Unable to open stream from Uri"))
        //         }
        //     } catch (error: Throwable) {
        //         completion(null, Exception("Unable to process image Uri"))
        //     }
        // }
        //
        // private fun uploadVideo(videoUri: Uri, completion: (CloudFileData?, Exception?) -> Unit) {
        //     try {
        //         val inputStream: InputStream? = App.applicationContext().contentResolver.openInputStream(videoUri)
        //         if (inputStream != null) {
        //             val storageRef = this.storage.reference
        //             val path = "${Constants.Api.Firebase.videosFolderName}/${UUID.randomUUID()}-android-video"
        //             val videoRef = storageRef.child(path)
        //             val metadata = StorageMetadata()
        //             this.uploadFile(videoRef, inputStream, metadata) { fileData, error ->
        //                 completion(fileData, error)
        //             }
        //         } else {
        //             completion(null, Exception("Unable to open stream from Uri"))
        //         }
        //     } catch (error: Throwable) {
        //         completion(null, Exception("Unable to process video Uri"))
        //     }
        // }
        //
        // private fun uploadAsset(asset: CryptoAsset, completion: (Exception?) -> Unit) {
        //     try {
        //         val map = GsonConverter.toMap(asset)?.toMutableMap()
        //         map?.remove("id")
        //
        //         if (map != null) {
        //             val document = db.collection(Constants.Api.Firebase.assetsCollectionName).document(asset.id)
        //             document
        //                 .set(map)
        //                 .addOnSuccessListener {
        //                     println("Document successfully written!")
        //                     completion(null)
        //                 }
        //                 .addOnFailureListener { e ->
        //                     println("Error writing document: $e")
        //                     completion(e)
        //                 }
        //         } else {
        //             completion(Exception("Unable to create json object"))
        //         }
        //     } catch (_: Throwable) {
        //         completion(Exception("Unable to encode crypto asset"))
        //     }
        // }
        //
        // private fun updateRemoteAssetRec(asset: CryptoAsset, iconUri: Uri?, videoUri: Uri?, completion: (CryptoAsset?, Exception?) -> Unit) {
        //
        //     // Upload files 1 by 1 with every call
        //     // Priority:
        //     // 1 - Video
        //     // 2 - Image
        //     // 3 - Asset
        //
        //     if (asset.videoFileData?.downloadURL != videoUri?.toString()) {
        //         var updatedAsset = asset
        //
        //         // Delete previous file in background
        //         val videoFileData = asset.videoFileData
        //         if (videoFileData != null) {
        //             updatedAsset.videoFileData = null
        //             deleteFile(videoFileData) { error ->
        //                 if (error != null) {
        //                     println(error)
        //                 } else {
        //                     println("Deleted icon with path $videoFileData.path")
        //                 }
        //             }
        //         }
        //
        //         // Upload with recursive call in completion
        //         if (videoUri != null) {
        //             uploadVideo(videoUri) { fileData, error ->
        //                 if (error != null) {
        //                     // Error uploading video so we ignore it
        //                     println(error)
        //                     this.updateRemoteAssetRec(updatedAsset, iconUri, null, completion)
        //                 } else if (fileData != null) {
        //                     // Successful video upload
        //                     updatedAsset.videoFileData = fileData
        //
        //                     val downloadUri = Uri.parse(fileData.downloadURL)
        //                     this.updateRemoteAssetRec(updatedAsset, iconUri, downloadUri, completion)
        //                 }
        //             }
        //         } else {
        //             this.updateRemoteAssetRec(updatedAsset, iconUri, videoUri, completion)
        //         }
        //
        //         return
        //     }
        //
        //
        //     if (asset.iconFileData?.downloadURL != iconUri?.toString()) {
        //         var updatedAsset = asset
        //
        //         // Delete previous file in background
        //         val iconFileData = asset.iconFileData
        //         if (iconFileData != null) {
        //             updatedAsset.iconFileData = null
        //             deleteFile(iconFileData) { error ->
        //                 if (error != null) {
        //                     println(error)
        //                 } else {
        //                     println("Deleted icon with path $iconFileData.path")
        //                 }
        //             }
        //         }
        //
        //         // Upload with recursive call in completion
        //         if (iconUri != null) {
        //             uploadImage(iconUri) { fileData, error ->
        //                 if (error != null) {
        //                     // Error uploading video so we ignore it
        //                     println(error)
        //                     this.updateRemoteAssetRec(updatedAsset, null, videoUri, completion)
        //                 } else if (fileData != null) {
        //                     // Successful video upload
        //                     updatedAsset.iconFileData = fileData
        //
        //                     val downloadUri = Uri.parse(fileData.downloadURL)
        //                     this.updateRemoteAssetRec(updatedAsset, downloadUri, videoUri, completion)
        //                 }
        //             }
        //         } else {
        //             this.updateRemoteAssetRec(updatedAsset, iconUri, videoUri, completion)
        //         }
        //
        //         return
        //     }
        //
        //     uploadAsset(asset) { error ->
        //         if (error != null) {
        //             completion(null, error)
        //         } else {
        //             completion(asset, null)
        //         }
        //     }
        //
        // }
        //
        // fun updateRemoteAsset(asset: CryptoAsset, iconUri: Uri?, videoUri: Uri?, completion: (CryptoAsset?, Exception?) -> Unit) {
        //     updateRemoteAssetRec(asset, iconUri, videoUri) { updatedAsset, error ->
        //         completion(updatedAsset, error)
        //     }
        // }
        //
        public async void getRemoteAssets(Func<List<CryptoAsset>, Exception, List<CryptoAsset>> completion)
        {
            var value = await db.Collection(Constants.Api.Firebase.assetsCollectionName)
                .GetAsync();
            if (!value.IsEmpty)
            {
                var assets = new List<CryptoAsset>();
                
                foreach (var document in value.Documents)
                {
                    var asset = document.ToObject<CryptoAsset>();
                    assets.Add(asset);
                }

                completion(assets, null);
            }
            else
            {
                completion(null, new Exception("getRemoteAssets error"));
            }
        }
    }
}