using OnlineStorePracticalWork.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Net;
using System.Web;

namespace OnlineStorePracticalWork.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Order/Checkout
        public ActionResult Checkout()
        {
            var userId = User.Identity.GetUserId();
            var cartItems = db.CartItems.Where(c => c.UserId == userId).Include(c => c.Product).ToList();
            var order = new Order
            {
                OrderDetails = cartItems.Select(c => new OrderDetail
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Product = c.Product
                }).ToList()
            };
            return View(order);
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(Order order)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var cartItems = db.CartItems.Where(c => c.UserId == userId).Include(c => c.Product).ToList();
                if (cartItems == null || !cartItems.Any())
                {
                    ModelState.AddModelError("", "Ваша корзина пуста.");
                    return View(order);
                }

                order.UserId = userId;
                order.OrderDate = DateTime.Now;
                order.OrderDetails = cartItems.Select(c => new OrderDetail
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Product = c.Product
                }).ToList();
                db.Orders.Add(order);
                db.SaveChanges();

                // Обновление количества товаров на складе
                foreach (var item in cartItems)
                {
                    var product = db.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        product.Stock -= item.Quantity;
                    }
                }
                db.SaveChanges();

                // Очистка корзины
                db.CartItems.RemoveRange(cartItems);
                db.SaveChanges();

                // Возвращаем представление для завершения регистрации
                return RedirectToAction("CompleteRegistration", new { orderId = order.ID });
            }

            return View(order);
        }

        // GET: Order/CompleteRegistration
        public ActionResult CompleteRegistration(int? orderId)
        {
            if (orderId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.Orders.Find(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            var registrationModel = new CompleteRegistrationViewModel
            {
                OrderId = order.ID,
                Email = order.Email
            };

            return View(registrationModel);
        }

        // POST: Order/CompleteRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteRegistration(CompleteRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userManager = HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = userManager.Create(user, model.Password);

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Buyer");
                    return RedirectToAction("OrderConfirmation", new { id = model.OrderId });
                }
                AddErrors(result);
            }

            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        // GET: Order/OrderConfirmation/5
        public ActionResult OrderConfirmation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
    }
}
