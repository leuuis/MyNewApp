
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MyNewApp.Utils
{
    public static class RsaKeyUtils
    {
        public static RsaSecurityKey LoadPublicKey(string PublicKeyPath)
        {
            var publicKeyText = File.ReadAllText(PublicKeyPath);
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyText.ToCharArray());

            return new RsaSecurityKey(rsa);
        }
    }
}