using System.Security.Cryptography;
using System.Text;

namespace UserManagerApp.Helpers
{
    public class MD5Helper
    {
        public static string GetMD5Hash(string text)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(text));

            return Convert.ToBase64String(hash);
        }
    }
}
