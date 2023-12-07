using AccountingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Controllers
{

    public class MaintenanceDepartmentController : Controller
    {
        // GET: MaintenanceDepartment
        //部門別CRUD

        public ActionResult Department()
        {
            DBmanager dbmanager = new DBmanager();
            List<Department> departments = dbmanager.GetDept();
            ViewBag.Departments = departments;
            return View();
        }

        public ActionResult CreateDept()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDept(Department department)
        {
            DBmanager dbmanager = new DBmanager();
            try
            {
                dbmanager.NewDept(department);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("Department");
        }

        public ActionResult EditDept(string Dept_ID)
        {
            DBmanager dbmanager = new DBmanager();
            Department department = dbmanager.GetDeptById(Dept_ID);
            return View(department);
        }

        [HttpPost]
        public ActionResult EditDept(Department department)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.UpdateDept(department);
            return RedirectToAction("Department");
        }

        public ActionResult DeleteDept(string Dept_ID)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.DeleteDeptById(Dept_ID);
            return RedirectToAction("Department");
        }

    }
}