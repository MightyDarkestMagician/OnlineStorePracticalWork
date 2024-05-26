using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineStorePracticalWork.Models;

namespace OnlineStorePracticalWork
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Add this code to create an admin user
            CreateAdminUser();
        }

        private void CreateAdminUser()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Check if the Admin role exists, if not, create it
            if (!RoleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole("Admin");
                var roleResult = RoleManager.Create(role);
                if (!roleResult.Succeeded)
                {
                    // Log the errors
                    Console.WriteLine("Role creation failed: " + string.Join(", ", roleResult.Errors));
                }
            }

            // Check if the admin user exists, if not, create it
            var adminUser = UserManager.FindByEmail("admin@example.com");
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                var userResult = UserManager.Create(adminUser, "Admin@123456");
                if (userResult.Succeeded)
                {
                    var addToRoleResult = UserManager.AddToRole(adminUser.Id, "Admin");
                    if (!addToRoleResult.Succeeded)
                    {
                        // Log the errors
                        Console.WriteLine("Add to role failed: " + string.Join(", ", addToRoleResult.Errors));
                    }
                }
                else
                {
                    // Log the errors
                    Console.WriteLine("User creation failed: " + string.Join(", ", userResult.Errors));
                }
            }
            else
            {
                // Log that user already exists
                Console.WriteLine("Admin user already exists");
            }
        }
    }
}
