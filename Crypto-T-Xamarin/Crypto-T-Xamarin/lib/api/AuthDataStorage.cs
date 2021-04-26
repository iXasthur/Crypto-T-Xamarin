using Crypto_T_Xamarin.lib.models.auth;

namespace Crypto_T_Xamarin.lib.api
{
    public class AuthDataStorage
    {
        // private val preferences = SharedPreferencesAssistant.encrypted
        
        public static void Save(AuthData authData) {
            // with(preferences.edit()) {
            //     putString(Constants.SharedPreferences.Encrypted.EMAIL_KEY, authData.email)
            //     putString(Constants.SharedPreferences.Encrypted.PASSWORD_KEY, authData.password)
            //     commit()
            // }
        }
        
        public static void Delete()
        {
            // with(preferences.edit()) {
            //     remove(Constants.SharedPreferences.Encrypted.EMAIL_KEY)
            //     remove(Constants.SharedPreferences.Encrypted.PASSWORD_KEY)
            //     commit()
            // }
        }
        
        public static AuthData? Restore() {
            // val email = preferences.getString(Constants.SharedPreferences.Encrypted.EMAIL_KEY, null)
            // val password = preferences.getString(Constants.SharedPreferences.Encrypted.PASSWORD_KEY, null)
            // if (email != null && password != null) {
            //     return AuthData(email, password)
            // }
            return null;
        }
    }
}