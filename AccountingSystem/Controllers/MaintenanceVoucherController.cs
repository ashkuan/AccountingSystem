using AccountingSystem.Models;

using System;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Globalization;

using System.Linq;
using System.Text;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Services.Description;

namespace AccountingSystem.Controllers

{
    public class MaintenanceVoucherController : Controller

    {
        DBmanager dbmanager;
        public MaintenanceVoucherController()
        {
            dbmanager = new DBmanager();
        }
        // GET: MaintenanceVoucher
        //會計傳票CRUD
        public ActionResult Voucher()
        {
            //實做物件
            DBmanager dbmanager = new DBmanager();
            //實現GetVouchers方法
            List<Voucher> vouchers = dbmanager.GetVouchersWithUserName();
            ViewBag.Vouchers = vouchers;
            return View();
        }

        public ActionResult CreateVoucher()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateVoucher(Voucher voucher)
        {
            DBmanager dbmanager = new DBmanager();
            try
            {
                dbmanager.NewVoucher(voucher);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("voucher");
        }
        public ActionResult EditVoucher(long Voucher_ID)
        {
            DBmanager dbmanager = new DBmanager();
            Voucher voucher = dbmanager.GetVoucherById(Voucher_ID);
            List<VoucherDetail> voucherDetails = dbmanager.GetVoucherDetails(Voucher_ID);
            if (voucherDetails != null && voucherDetails.Count > 0)
            {
                ViewBag.voucherDetails = voucherDetails;
            }
            return View(voucher);
        }

        [HttpPost]
        public ActionResult EditVoucher(Voucher Voucher)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.UpdateVoucher(Voucher);
            return RedirectToAction("voucher");
        }

        public ActionResult DeleteVoucher(long Voucher_ID)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.DeleteVoucherById(Voucher_ID);
            dbmanager.DeleteVoucherDetail(Voucher_ID);
            return RedirectToAction("Voucher");
        }

        public ActionResult VoucherDetail()
        {
            //實做物件
            DBmanager dbmanager = new DBmanager();
            VoucherDetail voucherDetails = dbmanager.GetVDetail();
            ViewBag.VoucherDetails = voucherDetails;
            return View(voucherDetails);
        }


        public ActionResult CreateVoucherDetail(long? Voucher_ID=null)
        {
            if (Voucher_ID != null) {
                ViewBag.Voucher_ID = Voucher_ID;
                Session["Voucher_ID"] = Voucher_ID;
            }

            //初始化一條明細數據(前端新增需要)
            ViewBag.InitialDetail = new VoucherDetail();
            return View();
        }

        [HttpPost]
        public ActionResult CreateVoucherDetail(List<VoucherDetail> voucherDetails)
        {
            var Voucher_ID = (long)Session["Voucher_ID"];
            ViewBag.Voucher_ID = Voucher_ID;

            bool hasErrors = false;
            List<string> errorMessagesList=new List<string>();
            //存錯誤的行號
            List<int> errorRows=new List<int>();

            for (int i= 0;i < voucherDetails.Count;i++)
            {
                var detail = voucherDetails[i];

                //定義參數
                var parameters = new Dictionary<string, object>
                {
                    {"Voucher_ID",Voucher_ID},
                    {"VDetail_Sn",detail.VDetail_Sn },
                    {"Subject_Name",detail.Subject_Name},
                    {"Subject_DrCr",detail.Subject_DrCr},
                    {"DrCr_Amount",detail.DrCr_Amount},
                    {"Dept_Name",detail.Dept_Name},
                    {"Product_Name",detail.Product_Name},
                    {"Voucher_Note",detail.Voucher_Note},
                };
                try
                {
                    //呼叫預存程序
                    dbmanager.ExecuteStoredProcedure(this, "InsertVoucherDetail", parameters);
                }
                catch (Exception ex)
                {
                    errorMessagesList.Add($"第{i+1}行錯誤：<br/>{ex.Message}<br/>");
                    errorRows.Add(i+1);
                    detail.HasError = true;
                    hasErrors = true;
                }
            }


            if (hasErrors)
            {
                ViewBag.ErrorMessage = string.Join("\n", errorMessagesList);
                return View(voucherDetails);
            }
            else
            {
                return RedirectToAction("EditVoucher", new { Voucher_ID = Voucher_ID });
            }
 
        }

        public ActionResult EditVoucherDetail(long Voucher_ID, byte VDetail_Sn)
        {
            DBmanager dbmanager = new DBmanager();
            VoucherDetail voucherDetail = dbmanager.GetVDetailById(Voucher_ID, VDetail_Sn);
            return View(voucherDetail);
        }

        [HttpPost]
        public ActionResult EditVoucherDetail(VoucherDetail voucherDetail)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.UpdateVoucherDetail(voucherDetail);
            return RedirectToAction("EditVoucher", new { Voucher_ID = voucherDetail.Voucher_ID, VDetail_Sn = voucherDetail.VDetail_Sn });
        }

        public ActionResult DeleteVoucherDetail(long Voucher_ID, byte VDetail_Sn)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.DeleteVoucherDetailBySn(Voucher_ID, VDetail_Sn);
            List<VoucherDetail> voucherDetails = dbmanager.GetVoucherDetails(Voucher_ID);
            if (voucherDetails != null && voucherDetails.Count > 0)
            {
                ViewBag.voucherDetails = voucherDetails;
            }
            return RedirectToAction("EditVoucher", new { Voucher_ID });
        }

        public ActionResult GetSubjects(string key,string searchField)
        {
            DBmanager dbmanager = new DBmanager();
            var subjects = dbmanager.GetAccountingSubjects(key, searchField);
            var result=subjects
            .Select(x =>new {
               label = x.Subject_ID + " " + x.Subject_Name,//組合ID和名稱
               value = x.Subject_Name
            })
            .ToArray();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepts(string key, string searchField)
        {
            DBmanager dbmanager = new DBmanager();
            var departments = dbmanager.GetDept(key, searchField);
            var result=departments
            .Select(x =>
            new {
                label = x.Dept_ID+" "+x.Dept_Name,
                value = x.Dept_Name
            })
            .ToArray();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProducts(string key, string searchField)
        {
            DBmanager dbmanager = new DBmanager();
            var products = dbmanager.GetProducts(key, searchField);
            var result = products
                .Select(x => new {
                    label = x.Product_ID + " " + x.Product_Name, // 組合ID和名稱
                    value = x.Product_Name
                })
                .ToArray();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }

}