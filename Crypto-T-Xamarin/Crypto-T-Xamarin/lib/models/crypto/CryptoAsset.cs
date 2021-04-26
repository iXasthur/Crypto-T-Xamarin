namespace Crypto_T_Xamarin.lib.models.crypto
{
    public struct CryptoAsset
    {
        public string id;
        public string name;
        public string code;
        public string description;
        public CloudFileData? iconFileData;
        public CloudFileData? videoFileData;
        public CryptoEvent? suggestedEvent;
    }
}