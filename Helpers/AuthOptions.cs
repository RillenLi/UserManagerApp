using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserManagerApp.Helpers
{
    public class AuthOptions
    {
        public const string ISSUER = "UserManagerServer";
        public const string AUDIENCE = "UserManagerClient";
        private const string KEY = "usermanager_security_key_many_sign_aND_SYMBOLS_AND_SUPER_SECRET_2024@!!!";
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }
    }
}
