using System.Collections.Generic;

namespace Crypto_T_Xamarin.lib.models.crypto
{
    public struct CryptoDashboard
    {
        public List<CryptoAsset> assets;

        public void setAssets(List<CryptoAsset> assets)
        {
            this.assets = assets;
        }
    }
}