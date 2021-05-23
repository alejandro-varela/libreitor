using Basics.CustomPolicyProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basics.Controllers
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

        [Authorize(Policy = "Claim.DoB")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        [Authorize(Policy = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }

        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return View("Secret");
        }

        [SecurityLevel(10)]
        public IActionResult SecretHigherLevel()
        {
            return View("Secret");
        }

        [AllowAnonymous] // esto es para que anden los "Filters"
        public async Task<IActionResult> Authenticate()
        {
            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "Bob@rmail.com"),
                new Claim("Grandma.Says", "Very nice boi."),
                new Claim(ClaimTypes.DateOfBirth, "1980-05-07T15:00:09+03:00"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(DynamicPolicies.SecurityLevel, "7"),
            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bob K Foo"),
                new Claim("DrivingLicense", "A+"),
            };

            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            var licenceIdentity = new ClaimsIdentity(licenseClaims, "Government");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenceIdentity });

            await HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoStuff([FromServices] IAuthorizationService authorizationService)
        {
            var builder     = new AuthorizationPolicyBuilder("BlaBla");
            var customPolicy= builder.RequireClaim("Hello").Build();
            var authResult  = await authorizationService.AuthorizeAsync(User, customPolicy);

            if (authResult.Succeeded)
            {
                return View("Index");
            }

            return View("Index");
        }
    }
}
