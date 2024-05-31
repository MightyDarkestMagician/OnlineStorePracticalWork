using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineStorePracticalWork.Models
{
    public class Order
    {
        public int ID { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Адрес")]
        public string Address { get; set; }

        public DateTime OrderDate { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>(); // Инициализация коллекции
    }
}
