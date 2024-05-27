using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using OnlineStorePracticalWork.Models;
using System;

namespace OnlineStorePracticalWork
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Настройки контекста конфигурации базы данных
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppSignInManager>(AppSignInManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

            // Включение использования файла cookie для хранения информации о вошедшем пользователе
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, AppUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            // Включение использования внешнего файла cookie для временного хранения информации о пользователе
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Включение двухфакторной аутентификации
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }
    }
}
