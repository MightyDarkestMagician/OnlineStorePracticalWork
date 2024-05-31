using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
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

            // Set the database initializer
            Database.SetInitializer(new ApplicationDbInitializer());

            // Initialize the database
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var context = new ApplicationDbContext())
            {
                ApplicationDbInitializer.InitializeIdentityForEF(context);
            }
        }
    }
}
