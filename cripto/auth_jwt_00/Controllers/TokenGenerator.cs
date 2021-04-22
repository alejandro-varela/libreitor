using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace auth_jwt_00.Controllers
{
    public class TokenGenerator
    {
        public static object GenerateTokenJwt(string username)
        {
            var secretKey           = MiConfig.SecretKey;
            var audienceToken       = MiConfig.AudienceToken;
            var issuerToken         = MiConfig.IssuerToken;
            var expireTime          = MiConfig.ExpireTime;

            var securityKey         = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials  = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) });

            // create token to the user
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);
            return jwtTokenString;
        }
    }

}