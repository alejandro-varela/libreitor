using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            var claims = new[] {

                new Claim(JwtRegisteredClaimNames.Sub, "some_id"), // http://tools.ietf.org/html/rfc7519#section-4
                new Claim("granny", "cookie"),
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);

            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(
                key, 
                algorithm
            );

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience,
                claims,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                signingCredentials
            );

            var tokenJson = new JwtSecurityTokenHandler()
                .WriteToken(token)
            ;

            //return RedirectToAction("Index");
            return Ok(new { access_token = tokenJson });
        }

        public IActionResult Decode(string part)
        {
            string decoded = Encoding.UTF8.GetString( Convert.FromBase64String(part));
            return Ok(decoded);
        }
    }
}
