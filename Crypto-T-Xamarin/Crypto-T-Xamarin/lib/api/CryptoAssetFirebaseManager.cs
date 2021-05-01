using System;
using System.Collections.Generic;
using Crypto_T_Xamarin.lib.models.crypto;
using Crypto_T_Xamarin.lib.utils;
using Plugin.CloudFirestore;
using Plugin.FirebaseStorage;

namespace Crypto_T_Xamarin.lib.api
{
    public class CryptoAssetFirebaseManager
    {
        private IFirestore db = CrossCloudFirestore.Current.Instance;
        
        private IStorage storage = CrossFirebaseStorage.Current.Instance;
        
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
        
        private void getStorageDownloadURL(string path, Func<Uri?, Exception?, Uri?> completion)
        {
            var storageRef = storage.RootReference;
            var fileRef = storageRef.Child(path);
            fileRef
                .GetDownloadUrlAsync()
                .ContinueWith(task =>
                {
                    completion(task.Result, task.Exception);
                    return task.Result;
                });
        }
        
        private void deleteFile(CloudFileData file, Func<Exception?, Exception?> completion) 
        {
            var storageRef = storage.RootReference;
            var fileRef = storageRef.Child(file.path);
            
            fileRef
                .DeleteAsync()
                .ContinueWith(task =>
                {
                    completion(task.Exception);
                    return task.Exception;
                });
            
        }
        
        private void uploadFile(IStorageReference fileRef, string filePath, MetadataChange metadata, Func<CloudFileData?, Exception?, CloudFileData?> completion)
        {
            fileRef
                .PutFileAsync(filePath, metadata)
                .ContinueWith(task =>
                {
                    if (task.Exception == null)
                    {
                        getStorageDownloadURL(fileRef.Path, (uri, error) =>
                        {
                            if (error == null)
                            {
                                if (uri != null)
                                {
                                    Console.WriteLine("Uploaded file " + fileRef.Path);
                                    completion(
                                        new CloudFileData
                                        {
                                            path = fileRef.Path,
                                            downloadURL = uri.AbsoluteUri
                                        }, null
                                    );
                                }
                                else
                                {
                                    completion(null, new Exception("Both uri and error in uploadFile are null"));
                                }
                            }
                            else
                            {
                                completion(null, error);
                            }

                            return uri;
                        });
                    }
                    else
                    {
                        completion(null, task.Exception);
                    }

                    return task.Exception;
                });
        }
        
        private void uploadImage(Uri imageUri, Func<CloudFileData?, Exception?, CloudFileData?> completion) {
            var storageRef = storage.RootReference;
            var path = Constants.Api.Firebase.imagesFolderName + "/" + Guid.NewGuid() +"-xamarin-image";
            var imageRef = storageRef.Child(path);
            var metadata = new MetadataChange();
            uploadFile(imageRef, imageUri.AbsolutePath, metadata, (fileData, error) =>
            {
                completion(fileData, error);
                return fileData;
            });
        }
        
        private void uploadVideo(Uri videoUri, Func<CloudFileData?, Exception?, CloudFileData?> completion)
        {
            var storageRef = storage.RootReference;
            var path = Constants.Api.Firebase.videosFolderName + "/" + Guid.NewGuid() +"-xamarin-video";
            var videoRef = storageRef.Child(path);
            var metadata = new MetadataChange();
            uploadFile(videoRef, videoUri.AbsolutePath, metadata, (fileData, error) =>
            {
                completion(fileData, error);
                return fileData;
            });
        }
        
        private async void uploadAsset(CryptoAsset asset, Func<Exception?, Exception?> completion)
        {
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
                    if (asset.id == null) // true for old assets from Native iOS and Android
                    {
                        asset.id = document.Id;
                    }
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