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
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product (доступ только для админа)
        public ActionResult Index()
        {
            return View(db.Products.Include(p => p.Seller).ToList());
        }

        // GET: Product/Details/5 (доступ разрешен всем)
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

        // GET: Product/Categories (доступ разрешен всем)
        [AllowAnonymous]
        public ActionResult Categories()
        {
            var categories = new List<string> { "Electronics", "Clothing", "Books" }; // Используйте английские названия категорий для согласованности
            return View(categories);
        }

        // GET: Product/Create (доступ для админа и продавца)
        [Authorize(Roles = "Admin,Seller")]
        public ActionResult Create()
        {
            ViewBag.Categories = new SelectList(new List<string> { "Electronics", "Clothing", "Books" }); // Используйте английские названия категорий для согласованности
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

                product.SellerId = User.Identity.GetUserId();

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(new List<string> { "Electronics", "Clothing", "Books" }); // Используйте английские названия категорий для согласованности
            return View(product);
        }

        // GET: Product/Edit/5 (доступ для админа и продавца)
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

        // POST: Product/Edit/5 (доступ для админа и продавца)
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

        // GET: Product/Delete/5 (доступ только для админа)
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

        // POST: Product/Delete/5 (доступ только для админа)
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (product != null)
            {
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
            }
            return RedirectToAction("Index");
        }

        // GET: Product/Category/{name} (доступ разрешен всем)
        [AllowAnonymous]
        public ActionResult Category(string name)
        {
            ViewBag.CategoryName = name;
            var products = db.Products.Where(p => p.Category == name).Include(p => p.Seller).ToList();
            return View(products);
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
