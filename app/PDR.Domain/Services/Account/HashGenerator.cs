using System.Security.Cryptography;
using System.Text;

namespace PDR.Domain.Services.Account
{
    public class HashGenerator
    {
        //public string ToPasswordHash(string password)
        //{
        //    var sha1Provider = new SHA1CryptoServiceProvider();
        //    var hashString = new StringBuilder();

        //    var hash = sha1Provider.ComputeHash(Encoding.UTF8.GetBytes(password));
            
        //    foreach (var b in hash)
        //    {
        //        hashString.Append(b.ToString("X2").ToLower());
        //    }

        //    return hashString.ToString();
        //}
    }
}