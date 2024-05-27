using System;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OnlineStorePracticalWork.Models;

namespace OnlineStorePracticalWork.Models
{
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<AppRoleManager>();

            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin@123456";
            const string adminRoleName = "Admin";

            // Создание роли администратора
            if (!roleManager.RoleExists(adminRoleName))
            {
                var role = new IdentityRole { Name = adminRoleName };
                roleManager.Create(role);
            }

            // Создание пользователя администратора
            var adminUser = userManager.FindByEmail(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = userManager.Create(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    userManager.SetLockoutEnabled(adminUser.Id, false);
                }
            }

            // Назначение роли администратора пользователю
            var rolesForUser = userManager.GetRoles(adminUser.Id);
            if (!rolesForUser.Contains(adminRoleName))
            {
                userManager.AddToRole(adminUser.Id, adminRoleName);
            }

            // Создание других ролей
            var roles = new[] { "Buyer", "Seller" };
            foreach (var role in roles)
            {
                if (!roleManager.RoleExists(role))
                {
                    roleManager.Create(new IdentityRole(role));
                }
            }
        }
    }
}
