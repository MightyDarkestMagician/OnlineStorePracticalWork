using Microsoft.AspNet.Identity;
using OnlineStorePracticalWork.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OnlineStorePracticalWork.Controllers
{
    [Authorize(Roles = "Admin,Seller")]
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Seller).ToList();
            return View(products);
        }

        // GET: Product/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Include(p => p.Seller).FirstOrDefault(p => p.ID == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [AllowAnonymous]
        public ActionResult Categories()
        {
            var categories = db.Products.Select(p => p.Category).Distinct().ToList();
            return View(categories);
        }

        // GET: Product/Create
        [Authorize(Roles = "Admin,Seller")]
        public ActionResult Create()
        {
            ViewBag.Categories = new SelectList(new List<string> { "Электроника", "Одежда", "Книги" });
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Price,Stock,Category")] Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    imageFile.SaveAs(path);
                    product.ImagePath = "~/Images/" + fileName;
                }

                var userId = User.Identity.GetUserId();
                var seller = db.Users.Find(userId);
                product.Seller = seller;

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(new List<string> { "Электроника", "Одежда", "Книги" });
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "Admin,Seller")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin,Seller")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Price,Stock,Category,ImagePath")] Product product, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(upload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    upload.SaveAs(path);
                    product.ImagePath = "~/Images/" + fileName;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Product/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                var path = Server.MapPath(product.ImagePath);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Product/Category/{name}
        [AllowAnonymous]
        public ActionResult Category(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.CategoryName = name;
            var products = db.Products.Include(p => p.Seller).Where(p => p.Category == name).ToList();

            if (!products.Any())
            {
                ViewBag.Message = "Нет товаров в данной категории.";
            }

            return View(products);
        }

    }
}
