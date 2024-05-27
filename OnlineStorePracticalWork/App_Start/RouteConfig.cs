using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

            // Маршруты для Account
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

            // Маршрут для корзины
            routes.MapRoute(
                name: "Cart",
                url: "Cart/Index",
                defaults: new { controller = "Cart", action = "Index" }
            );

            // Маршрут для управления товарами
            routes.MapRoute(
                name: "ManageProducts",
                url: "Products/Manage",
                defaults: new { controller = "Products", action = "Manage" }
            );

            // Маршруты для категорий товаров
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

            // Добавляем маршрут для выхода из аккаунта
            routes.MapRoute(
                name: "LogOff",
                url: "Account/LogOff",
                defaults: new { controller = "Account", action = "LogOff" }
            );

            routes.MapRoute(
                name: "Profile",
                url: "Account/Profile",
                defaults: new { controller = "Account", action = "Profile" }
            );
        }
    }


}
