using System;                                   // Подключение пространства имен для основных системных типов
using System.Collections.Generic;               // Подключение пространства имен для работы с коллекциями
using System.Linq;                              // Подключение пространства имен для LINQ-запросов
using System.Web;                               // Подключение пространства имен для классов веб-приложений
using System.Data.Entity;                       // Подключение пространства имен для работы с Entity Framework

namespace OnlineStorePracticalWork.Models
{
    // Определение класса ApplicationDbInitializer, который наследует от DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        // Переопределение метода Seed для заполнения базы данных начальными данными
        protected override void Seed(ApplicationDbContext context)
        {
            // Добавление данных для тестирования в таблицу Products
            context.Products.Add(new Product { Name = "Продукт1", Description = "Описание1", Price = 100, Stock = 10 });
            context.Products.Add(new Product { Name = "Продукт2", Description = "Описание2", Price = 200, Stock = 20 });

            // Вызов базового метода Seed
            base.Seed(context);
        }
    }
}
