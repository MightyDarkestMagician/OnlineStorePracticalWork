using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using OnlineStorePracticalWork.Models;

namespace OnlineStorePracticalWork.Models
{
    public class Product
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Количество на складе")]
        public int Stock { get; set; }

        [Required]
        [Display(Name = "Категория")]
        public string Category { get; set; }

        public string SellerId { get; set; }
        public virtual AppUser Seller { get; set; }

        public string ImagePath { get; set; } // Используем это свойство для хранения пути к изображению
    }
}
