using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace OnlineStorePracticalWork.Models
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {

        public ApplicationDbContext()
                    : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Order> Orders { get; set; }
        public System.Data.Entity.DbSet<Product> Products { get; set; }
        public System.Data.Entity.DbSet<CartItem> CartItems { get; set; }
        public System.Data.Entity.DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
