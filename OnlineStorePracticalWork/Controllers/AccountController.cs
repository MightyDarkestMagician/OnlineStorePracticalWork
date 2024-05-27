using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlineStorePracticalWork.Models;
using System.Diagnostics;
using System;

namespace OnlineStorePracticalWork.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private AppSignInManager _signInManager;
        private AppUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(AppUserManager userManager, AppSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public AppSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<AppSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Debug.WriteLine($"Attempting to log in user: {model.Email}");

            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                Debug.WriteLine($"User not found: {model.Email}");
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            // Проверка, что пароль для пользователя совпадает
            var passwordMatch = UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.Password);
            Debug.WriteLine($"Password match result for user {model.Email}: {passwordMatch}");

            if (user.LockoutEnabled && user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc > DateTime.UtcNow)
            {
                Debug.WriteLine($"User {model.Email} is locked out.");
                ModelState.AddModelError("", "This account has been locked out.");
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                Debug.WriteLine($"User {model.Email} email is not confirmed.");
                ModelState.AddModelError("", "You must have a confirmed email to log on.");
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    Debug.WriteLine($"Login successful for user: {model.Email}");
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    Debug.WriteLine($"User locked out: {model.Email}");
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    Debug.WriteLine($"User requires verification: {model.Email}");
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    Debug.WriteLine($"Invalid login attempt for user: {model.Email}");
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Role = new SelectList(new[] { "Buyer", "Seller" });
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName, Email = model.Email, EmailConfirmed = true };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.Role);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            ViewBag.Role = new SelectList(new[] { "Buyer", "Seller" });
            return View(model);
        }

        [HttpGet]
        [Route("Account/Profile")]
        public ActionResult Profile()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            var model = new ManageUserViewModel
            {
                Email = user.Email,
                UserName = user.UserName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(ManageUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = UserManager.FindById(userId);
                user.Email = model.Email;
                user.UserName = model.UserName;
                var result = UserManager.Update(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
