using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Controllers
{
    public class LogoutController : Controller
    {
        public ActionResult Logout()
        {
            return View();
        }
    }
}