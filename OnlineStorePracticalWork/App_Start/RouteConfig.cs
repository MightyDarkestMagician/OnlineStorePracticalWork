using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineStorePracticalWork
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "UserProfile",
                url: "Account/Profile",
                defaults: new { controller = "Account", action = "Profile" }
            );

            routes.MapRoute(
                name: "AccountLogOff",
                url: "Account/LogOff",
                defaults: new { controller = "Account", action = "LogOff" }
            );

            routes.MapRoute(
                name: "Cart",
                url: "Cart/{action}/{id}",
                defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "CartCheckout",
                url: "Cart/Checkout",
                defaults: new { controller = "Cart", action = "Checkout" }
            );

            routes.MapRoute(
                name: "OrderCheckout",
                url: "Order/Checkout",
                defaults: new { controller = "Order", action = "Checkout" }
            );

            routes.MapRoute(
                name: "CompleteRegistration",
                url: "Order/CompleteRegistration/{orderId}",
                defaults: new { controller = "Order", action = "CompleteRegistration", orderId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "OrderConfirmation",
                url: "Order/OrderConfirmation/{id}",
                defaults: new { controller = "Order", action = "OrderConfirmation", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ManageProducts",
                url: "Products/Manage",
                defaults: new { controller = "Products", action = "Manage" }
            );

            routes.MapRoute(
                name: "Categories",
                url: "Product/Categories",
                defaults: new { controller = "Product", action = "Categories" }
            );

            routes.MapRoute(
                name: "Category",
                url: "Product/Category/{name}",
                defaults: new { controller = "Product", action = "Category", name = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ManageAccount",
                url: "Account/Manage",
                defaults: new { controller = "Account", action = "Profile" }
            );
        }
    }
}
