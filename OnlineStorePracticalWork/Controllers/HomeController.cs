using System.Linq;
using System.Web.Mvc;
using OnlineStorePracticalWork.Models;

namespace OnlineStorePracticalWork.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var products = db.Products.ToList();
            return View(products);
        }

        public ActionResult About()
        {
            ViewBag.Message = "LazySale";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Ваша страница контактов.";
            return View();
        }

        // Метод для отображения списка категорий
        public ActionResult Categories()
        {
            var categories = db.Products.Select(p => p.Category).Distinct().ToList();
            return View(categories);
        }

        public ActionResult Category(string name)
        {
            var products = db.Products.Where(p => p.Category == name).ToList();
            return View(products);
        }

        public ActionResult Details(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
    }
}
