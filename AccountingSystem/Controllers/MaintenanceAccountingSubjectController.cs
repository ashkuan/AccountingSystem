using AccountingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Controllers
{
    public class MaintenanceAccountingSubjectController : Controller
    {
        // GET: MaintenanceAccountingSubject
        //會計科目CRUD
        public ActionResult AccountingSubject()
        {
            DBmanager dbmanager = new DBmanager();
            List<AccountingSubject> accountingSubjects = dbmanager.GetAccountingSubjects();
            ViewBag.accountingSubjects = accountingSubjects;
            return View();
        }
      
        public ActionResult SubjectIndex()
        {
            DBmanager dbmanager = new DBmanager();
            var subjects = dbmanager.GetAccountingSubjects();
            return View(subjects);
        }

        public ActionResult CreateSubject()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateSubject(AccountingSubject accountingSubject)
        {
            DBmanager dbmanager = new DBmanager();
            try
            {
                dbmanager.CreateSubject(accountingSubject);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("AccountingSubject");
        }
        public ActionResult EditSubject(int Subject_ID)
        {
            DBmanager dbmanager = new DBmanager();
            AccountingSubject accountingSubjects = dbmanager.GetSubjectByID(Subject_ID);
            return View(accountingSubjects);
        }

        [HttpPost]
        public ActionResult EditSubject(AccountingSubject accountingSubject)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.UpdateSubject(accountingSubject);
            return RedirectToAction("AccountingSubject");
        }

        public ActionResult DeleteSubject(int Subject_ID)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.DeleteSubjectByID(Subject_ID);
            return RedirectToAction("AccountingSubject");
        }
    }
}