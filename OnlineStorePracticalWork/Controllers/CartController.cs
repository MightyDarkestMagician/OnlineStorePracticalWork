using OnlineStorePracticalWork.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;

namespace OnlineStorePracticalWork.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cart
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var cartItems = db.CartItems.Where(c => c.UserId == userId).Include(c => c.Product).ToList();
            return View(cartItems);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int productId, int quantity)
        {
            var userId = User.Identity.GetUserId();
            var cartItem = db.CartItems.SingleOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    UserId = userId,
                    Quantity = quantity,
                    DateCreated = DateTime.Now
                };
                db.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Cart/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromCart(int id)
        {
            var cartItem = db.CartItems.Find(id);
            db.CartItems.Remove(cartItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Cart/Checkout
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
                return RedirectToAction("CompleteRegistration", "Order", new { orderId = order.ID });
            }

            return View(order);
        }

        [HttpGet]
        public JsonResult CheckStock(int productId, int quantity)
        {
            var product = db.Products.Find(productId);
            if (product == null || product.Stock < quantity)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
