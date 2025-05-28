using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyNewApp.Models;
using MyNewApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MyNewApp.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly RsaSecurityKey _privateKey;

        public JwtTokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;

            var privateKeyPem = File.ReadAllText(_jwtSettings.PrivateKeyPath);
            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem.ToCharArray());
            _privateKey = new RsaSecurityKey(rsa);
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                NotBefore = now,
                Expires = now.AddMinutes(_jwtSettings.TokenLifetimeMinutes),
                SigningCredentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}
