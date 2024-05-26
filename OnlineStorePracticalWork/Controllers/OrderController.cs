using OnlineStorePracticalWork.Models;
using System.Collections.Generic;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace OnlineStorePracticalWork.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Order/Cart
        public ActionResult Cart()
        {
            var cart = Session["Cart"] as List<OrderDetail> ?? new List<OrderDetail>();
            return View(cart);
        }

        // POST: Order/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(OrderDetail orderDetail)
        {
            var product = db.Products.Find(orderDetail.ProductId);
            if (product == null)
            {
                return HttpNotFound();
            }

            var cart = Session["Cart"] as List<OrderDetail> ?? new List<OrderDetail>();

            var existingItem = cart.FirstOrDefault(x => x.ProductId == orderDetail.ProductId);
            if (existingItem != null)
            {
                if (existingItem.Quantity + orderDetail.Quantity > product.Stock)
                {
                    ModelState.AddModelError("", "Недостаточно товара на складе.");
                    return View(orderDetail);
                }
                existingItem.Quantity += orderDetail.Quantity;
            }
            else
            {
                if (orderDetail.Quantity > product.Stock)
                {
                    ModelState.AddModelError("", "Недостаточно товара на складе.");
                    return View(orderDetail);
                }
                cart.Add(orderDetail);
            }

            Session["Cart"] = cart;
            return RedirectToAction("Cart");
        }

        // GET: Order/Checkout
        public ActionResult Checkout()
        {
            return View();
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(Order order)
        {
            if (ModelState.IsValid)
            {
                var cart = Session["Cart"] as List<OrderDetail>;
                if (cart == null || !cart.Any())
                {
                    ModelState.AddModelError("", "Ваша корзина пуста.");
                    return View(order);
                }

                order.OrderDate = DateTime.Now;
                order.OrderDetails = cart;
                db.Orders.Add(order);
                db.SaveChanges();

                // Обновление количества товаров на складе
                foreach (var item in cart)
                {
                    var product = db.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        product.Stock -= item.Quantity;
                    }
                }
                db.SaveChanges();

                Session["Cart"] = null;
                return RedirectToAction("OrderConfirmation", new { id = order.Id });
            }

            return View(order);
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

        // GET: Order/ManageOrders
        [Authorize(Roles = "Admin")]
        public ActionResult ManageOrders()
        {
            return View(db.Orders.ToList());
        }

        // GET: Order/OrderDetails/5
        [Authorize(Roles = "Admin")]
        public ActionResult OrderDetails(int? id)
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
