using System;
using System.Collections.Generic;
using System.Linq;
using AccountingSystem.Models;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}