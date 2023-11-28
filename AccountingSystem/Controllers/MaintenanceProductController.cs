using AccountingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Controllers
{

    public class MaintenanceProductController : Controller
    {
        // GET: MaintenanceProduct
        //產品別CRUD

        public ActionResult Product()
        {
            DBmanager dbmanager = new DBmanager();
            List<Product> products = dbmanager.GetProducts();
            ViewBag.Products = products;
            return View();
        }

        public ActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateProduct(Product product)
        {
            DBmanager dbmanager = new DBmanager();
            try
            {
                dbmanager.NewProduct(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("Product");
        }

        public ActionResult EditProduct(byte Product_ID)
        {
            DBmanager dbmanager = new DBmanager();
            Product product = dbmanager.GetProductById(Product_ID);
            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.UpdateProduct(product);
            return RedirectToAction("Product");
        }

        public ActionResult DeleteProduct(byte Product_ID)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.DeleteProductById(Product_ID);
            return RedirectToAction("Product");
        }



    }
}