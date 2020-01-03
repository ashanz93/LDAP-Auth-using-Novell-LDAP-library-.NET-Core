using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreLDAPAuth.LDAP;
using NetCoreLDAPAuth.LDAP.Services;
using NetCoreLDAPAuth.Views.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCoreLDAPAuth.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ILdapService _ldapService;
        private readonly ILogger _logger;

        public AccountController(
            ILdapService ldapService,
            ILogger<AccountController> logger)
        {
            this._ldapService = ldapService;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Signin(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ViewData["ReturnUrl"] = returnUrl;

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signin([FromForm]SignInViewModel model, string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;

            if (this.ModelState.IsValid)
            {
                try
                {
                    var user = _ldapService.Authenticate(model.UserName, model.Password);

                    if (user != null)
                    {
                        var userClaims = new List<Claim>
                        {
                            new Claim("displayName", user.DisplayName),
                            new Claim("userName", user.UserName)
                        };
                        var principal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, _ldapService.GetType().Name));
                        await HttpContext.SignInAsync("app", principal);
                        return View("HomePage");
                    }

                    // I added the exclamation mark to make it more dramatic
                    this.TempData["ErrorMessage"] = "The username and/or password are incorrect!";

                    return View("ErrorPage");
                }
                catch (Exception ex)
                {
                    this.TempData["ErrorMessage"] = ex.Message;

                    return View("ErrorPage");
                }
            }

            return View("Views/ErrorPage");
        }

        [HttpGet]
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync("app");
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            else
            {
                return this.RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
