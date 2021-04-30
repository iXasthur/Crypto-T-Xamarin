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
        
        public void deleteRemoteAsset(CryptoAsset asset, Func<Exception?, Exception?> completion)
        {

            var iconFile = asset.iconFileData;
            if (iconFile != null)
            {
                deleteFile(iconFile.Value, error =>
                {
                    if (error != null)
                    {
                        Console.WriteLine(error);
                    } else
                    {
                        Console.WriteLine("Deleted file with path " + iconFile.Value.path);
                    }
                    return error;
                });
            }

            var videoFile = asset.videoFileData;
            if (videoFile != null) {
                deleteFile(videoFile.Value, error =>
                {
                    if (error != null)
                    {
                        Console.WriteLine(error);
                    } else
                    {
                        Console.WriteLine("Deleted file with path " + videoFile.Value.path);
                    }
                    return error;
                });
            }

            var document = db.Collection(Constants.Api.Firebase.assetsCollectionName).Document(asset.id);
            document
                .DeleteAsync()
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Console.WriteLine("Document successfully deleted!");
                        completion(null);
                    }
                    else
                    {
                        Console.WriteLine("Error deleting document: " + task.Exception);
                        completion(task.Exception);
                    }
                });
        }
        
        // private void getStorageDownloadURL(path: String, completion: (Uri?, Exception?) -> Unit) {
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
        
        private void deleteFile(CloudFileData file, Func<Exception?, Exception?> completion) {
            completion(new Exception("deleteFile not implemented"));
            
            // val storageRef = this.storage.reference
            // val fileRef = storageRef.child(file.path)
            //
            // fileRef
            //     .delete()
            //     .addOnSuccessListener {
            //         completion(null)
            //     }
            //     .addOnFailureListener { e ->
            //         completion(e)
            //     }
        }
        
        // private void uploadFile(fileRef: StorageReference, stream: InputStream, metadata: StorageMetadata, completion: (CloudFileData?, Exception?) -> Unit) {
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
        
        private void uploadImage(Uri imageUri, Func<CloudFileData?, Exception?, CloudFileData?> completion) {
            completion(null, new Exception("uploadImage not implemented"));
            // try {
            //     val inputStream: InputStream? = App.applicationContext().contentResolver.openInputStream(imageUri)
            //     if (inputStream != null) {
            //         val storageRef = this.storage.reference
            //         val path = "${Constants.Api.Firebase.imagesFolderName}/${UUID.randomUUID()}-android-image"
            //         val videoRef = storageRef.child(path)
            //         val metadata = StorageMetadata()
            //         this.uploadFile(videoRef, inputStream, metadata) { fileData, error ->
            //             completion(fileData, error)
            //         }
            //     } else {
            //         completion(null, Exception("Unable to open stream from Uri"))
            //     }
            // } catch (error: Throwable) {
            //     completion(null, Exception("Unable to process image Uri"))
            // }
        }
        
        private void uploadVideo(Uri videoUri, Func<CloudFileData?, Exception?, CloudFileData?> completion)
        {
            completion(null, new Exception("uploadVideo not implemented"));
            // try {
            //     val inputStream: InputStream? = App.applicationContext().contentResolver.openInputStream(videoUri)
            //     if (inputStream != null) {
            //         val storageRef = this.storage.reference
            //         val path = "${Constants.Api.Firebase.videosFolderName}/${UUID.randomUUID()}-android-video"
            //         val videoRef = storageRef.child(path)
            //         val metadata = StorageMetadata()
            //         this.uploadFile(videoRef, inputStream, metadata) { fileData, error ->
            //             completion(fileData, error)
            //         }
            //     } else {
            //         completion(null, Exception("Unable to open stream from Uri"))
            //     }
            // } catch (error: Throwable) {
            //     completion(null, Exception("Unable to process video Uri"))
            // }
        }
        
        private async void uploadAsset(CryptoAsset asset, Func<Exception?, Exception?> completion) {
            var document = db.Collection(Constants.Api.Firebase.assetsCollectionName).Document(asset.id);
            await document
                .SetAsync(asset)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Console.WriteLine("Document successfully written!");
                        completion(null);
                    }
                    else
                    {
                        Console.WriteLine("Error writing document: " + task.Exception);
                        completion(task.Exception);
                    }
                });
        }

        private void updateRemoteAssetRec(CryptoAsset asset, Uri? iconUri, Uri? videoUri,
            Func<CryptoAsset?, Exception?, CryptoAsset?> completion)
        {

            // Upload files 1 by 1 with every call
            // Priority:
            // 1 - Video
            // 2 - Image
            // 3 - Asset

            if (asset.videoFileData?.downloadURL != videoUri?.ToString())
            {
                var updatedAsset = asset;

                // Delete previous file in background
                var videoFileData = asset.videoFileData;
                if (videoFileData != null)
                {
                    updatedAsset.videoFileData = null;
                    deleteFile(videoFileData.Value, error =>
                    {
                        if (error != null)
                        {
                            Console.WriteLine(error);
                        }
                        else
                        {
                            Console.WriteLine("Deleted video with path " + videoFileData.Value.path);
                        }

                        return error;
                    });
                }

                // Upload with recursive call in completion
                if (videoUri != null)
                {
                    uploadVideo(videoUri, (fileData, error) =>
                    {
                        if (error != null)
                        {
                            // Error uploading video so we ignore it
                            Console.WriteLine(error);
                            updateRemoteAssetRec(updatedAsset, iconUri, null, completion);
                        }
                        else if (fileData != null)
                        {
                            // Successful video upload
                            updatedAsset.videoFileData = fileData;

                            var downloadUri = new Uri(fileData.Value.downloadURL);
                            updateRemoteAssetRec(updatedAsset, iconUri, downloadUri, completion);
                        }

                        return fileData;
                    });
                }
                else
                {
                    updateRemoteAssetRec(updatedAsset, iconUri, videoUri, completion);
                }

                return;
            }


            if (asset.iconFileData?.downloadURL != iconUri?.ToString())
            {
                var updatedAsset = asset;

                // Delete previous file in background
                var iconFileData = asset.iconFileData;
                if (iconFileData != null)
                {
                    updatedAsset.iconFileData = null;
                    deleteFile(iconFileData.Value, error =>
                    {
                        if (error != null)
                        {
                            Console.WriteLine(error);
                        }
                        else
                        {
                            Console.WriteLine("Deleted icon with path " + iconFileData.Value.path);
                        }

                        return error;
                    });
                }

                // Upload with recursive call in completion
                if (iconUri != null)
                {
                    uploadImage(iconUri, (fileData, error) =>
                    {
                        if (error != null)
                        {
                            // Error uploading video so we ignore it
                            Console.WriteLine(error);
                            updateRemoteAssetRec(updatedAsset, null, videoUri, completion);
                        }
                        else if (fileData != null)
                        {
                            // Successful video upload
                            updatedAsset.iconFileData = fileData;

                            var downloadUri = new Uri(fileData.Value.downloadURL);
                            updateRemoteAssetRec(updatedAsset, downloadUri, videoUri, completion);
                        }
                        return fileData;
                    });
                }
                else
                {
                    updateRemoteAssetRec(updatedAsset, iconUri, videoUri, completion);
                }

                return;
            }

            uploadAsset(asset, error =>
            {
                if (error != null)
                {
                    completion(null, error);
                } else
                {
                    completion(asset, null);
                }
                return error;
            });
        }

        public void updateRemoteAsset(CryptoAsset asset, Uri? iconUri, Uri? videoUri, Func<CryptoAsset?, Exception?, CryptoAsset?> completion)
        {
            updateRemoteAssetRec(asset, iconUri, videoUri, (updatedAsset, error) =>
            {
                completion(updatedAsset, error);
                return updatedAsset;
            });
        }
        
        public async void getRemoteAssets(Func<List<CryptoAsset>?, Exception?, List<CryptoAsset>?> completion)
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