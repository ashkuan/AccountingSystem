using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Controllers
{
    public class StatementsController : Controller
    {
        public ActionResult Statements()
        {
            return View();
        }
    }
}