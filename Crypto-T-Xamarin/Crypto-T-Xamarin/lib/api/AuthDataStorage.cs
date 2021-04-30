using Crypto_T_Xamarin.lib.models.auth;
using Xamarin.Essentials;

namespace Crypto_T_Xamarin.lib.api
{
    public class AuthDataStorage
    {
        // private val preferences = SharedPreferencesAssistant.encrypted
        
        public static void Save(AuthData authData) {
            Preferences.Set("email", authData.email);
            Preferences.Set("password", authData.password);
        }
        
        public static void Delete()
        {
            Preferences.Remove("email");
            Preferences.Remove("password");
        }
        
        public static AuthData? Restore()
        {
            var email = Preferences.Get("email", null);
            var password = Preferences.Get("password", null);
            if (email != null && password != null)
            {
                return new AuthData {email = email, password = password};
            }
            return null;
        }
    }
}