using AccountingSystem.Models;

using System;

using System.Collections.Generic;

using System.Data.SqlClient;

using System.Globalization;

using System.Linq;

using System.Web;

using System.Web.Mvc;

using System.Web.Optimization;
using System.Web.Services.Description;

namespace AccountingSystem.Controllers

{
    public class MaintenanceVoucherController : Controller

    {
        public EditVoucherViewModel LoadEditVoucher(string Voucher_ID)
        {
            DBmanager dbmanager = new DBmanager();

            // 1. 數據庫查詢
            Voucher voucher = dbmanager.GetVoucherById(Voucher_ID);
            List<VoucherDetail> details = dbmanager.GetVoucherDetails(Voucher_ID);

            // 2. 建立ViewModel對象
            EditVoucherViewModel model = new EditVoucherViewModel();

            // 3. 赋值 
            model.Voucher = voucher;
            model.VoucherDetails = details;

            // 4. 传递ViewModel对象
            return model;

        }


        // GET: MaintenanceVoucher
        //會計傳票CRUD
        public ActionResult Voucher()
        {
            //實做物件
            DBmanager dbmanager = new DBmanager();
            //實現GetVouchers方法
            List<Voucher> vouchers = dbmanager.GetVouchers();
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
            return RedirectToAction("Voucher");
        }

        public ActionResult EditVoucher(string Voucher_ID)
        {
            DBmanager dbmanager = new DBmanager();
            var voucherViewModel = dbmanager.GetVoucherById(Voucher_ID);
            List<VoucherDetail> details = dbmanager.GetVoucherDetailById();
            var model = LoadEditVoucher(Voucher_ID);
            return View("EditVoucher", model);
        }

        [HttpPost]
        public ActionResult EditVoucher(Voucher voucher, List<VoucherDetail> voucherDetails)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.UpdateVoucher(voucher, voucherDetails);
            return RedirectToAction("EditVoucher");
        }

        public ActionResult VoucherDetail(string Voucher_ID)
        {
            //實做物件
            DBmanager dbmanager = new DBmanager();
            //實現GetVouchers方法
            List<VoucherDetail> voucherDetails = dbmanager.GetVoucherDetails(Voucher_ID);
            ViewBag.VoucherDetails = voucherDetails;
            return View();
        }
        public ActionResult EditVoucherDetail(string Voucher_ID)
        {
            //實做物件
            DBmanager dbmanager = new DBmanager();
            List<VoucherDetail> voucherDetails = dbmanager.GetVoucherDetails(Voucher_ID);
            return View(voucherDetails);
        }

        [HttpPost]
        public ActionResult EditVoucherDetail(VoucherDetail detail)
        {
            try
            {
                //實做物件
                DBmanager dbmanager = new DBmanager();
                dbmanager.UpdateVoucherDetail(detail);
                return RedirectToAction("VoucherDetail");

            }
            catch (Exception ex)
            {
                
                return Json(new { success = false, message = "更新失败：" + ex.Message });
            }
        }



        public ActionResult DeleteVoucher(string Voucher_ID)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.DeleteVoucherById(Voucher_ID);
            dbmanager.DeleteVoucherDetail(Voucher_ID);
            return RedirectToAction("Voucher");
        }

        public ActionResult DeleteVoucherDetail(string Voucher_ID, byte VDetail_Sn)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.DeleteVoucherDetailBySn(Voucher_ID, VDetail_Sn);
            return RedirectToAction("EditVoucher");
        }

    }

}