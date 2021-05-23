using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IndentityExample.Data;
using Microsoft.AspNetCore.Identity;
using NETCore.MailKit.Core;

namespace IndentityExample.Controllers
{
    public class HomeController : Controller
    {
        UserManager<IdentityUser> _userManager;
        SignInManager<IdentityUser> _signInManager;
        IEmailService _emailService;

        public HomeController(
            UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService
        )
        {
            _userManager   = userManager;
            _signInManager = signInManager;
            _emailService  = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                // sign in aca...
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser
            {
                UserName = username,
                Email = "",
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                //// SIN EMAIL
                //// sign in aca...
                //var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
                //if (signInResult.Succeeded)
                //{
                //    return RedirectToAction("Index");
                //}

                // CON EMAIL
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                
                var link = Url.Action(
                    action    : nameof(VerifyEmail),
                    controller: "Home",
                    values    : new { userId = user.Id, code = code },
                    protocol  : Request.Scheme,
                    host      : Request.Host.ToString()
                );
                
                var linkHtml = $"<a href=\"{ link }\">Siga este link</a>";

                await _emailService.SendAsync(
                    mailTo : "test@otracosa.com",
                    subject: "email verify",
                    message: linkHtml,
                    isHtml : true
                );

                return RedirectToAction(nameof(EmailVerification));
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return View();
            }

            return BadRequest();
        }

        public IActionResult EmailVerification() => View();

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
