using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
            var userManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

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

            // Список продавцов
            var sellers = new[]
            {
                new { UserName = "Seller_Real01", Email = "seller01@gmail.com", Password = "Seller01!" },
                new { UserName = "Seller_Real02", Email = "seller02@gmail.com", Password = "Seller02!" },
                new { UserName = "Seller_Real03", Email = "seller03@gmail.com", Password = "Seller03!" }
            };

            // Создание продавцов и добавление их в роль
            foreach (var seller in sellers)
            {
                CreateUser(userManager, seller.UserName, seller.Email, seller.Password, "Seller");
            }

            // Список покупателей
            var buyers = new[]
            {
                new { UserName = "Buyer_Real01", Email = "buyer01@gmail.com", Password = "Buyer01!" },
                new { UserName = "Buyer_Real02", Email = "buyer02@gmail.com", Password = "Buyer02!" },
                new { UserName = "Buyer_Real03", Email = "buyer03@gmail.com", Password = "Buyer03!" }
            };

            // Создание покупателей и добавление их в роль
            foreach (var buyer in buyers)
            {
                CreateUser(userManager, buyer.UserName, buyer.Email, buyer.Password, "Buyer");
            }
        }

        private static void CreateUser(UserManager<AppUser> userManager, string userName, string email, string password, string role)
        {
            var user = userManager.FindByEmail(email);
            if (user == null)
            {
                user = new AppUser { UserName = userName, Email = email, EmailConfirmed = true };
                var result = userManager.Create(user, password);
                if (result.Succeeded)
                {
                    userManager.SetLockoutEnabled(user.Id, false);
                    userManager.AddToRole(user.Id, role);
                }
            }
        }
    }
}
