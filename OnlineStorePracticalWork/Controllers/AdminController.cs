using OnlineStorePracticalWork.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace OnlineStorePracticalWork.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Orders
        public ActionResult Orders()
        {
            var orders = db.Orders.Include(o => o.OrderDetails).ToList();
            return View(orders);
        }

        // GET: Admin/OrderDetails/5
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.ID == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
    }
}
