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
        public ActionResult EditVoucher(string Voucher_ID)
        {
            DBmanager dbmanager = new DBmanager();
            Voucher voucher = dbmanager.GetVoucherById(Voucher_ID);
            List<VoucherDetail> voucherDetails = dbmanager.GetVoucherDetails(Voucher_ID);
            if(voucherDetails!=null && voucherDetails.Count > 0)
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

        public ActionResult DeleteVoucher(string Voucher_ID)
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


        public ActionResult CreateVoucherDetail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateVoucherDetail(VoucherDetail voucherDetails)
        {
            DBmanager dbmanager = new DBmanager();
            try
            {
                dbmanager.NewVoucherDetail(voucherDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("VoucherDetail");
        }
        public ActionResult EditVoucherDetail(string Voucher_ID, byte VDetail_Sn)
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

        public ActionResult DeleteVoucherDetail(string Voucher_ID, byte VDetail_Sn)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.DeleteVoucherDetailBySn(Voucher_ID, VDetail_Sn);
            return RedirectToAction("EditVoucher");
        }
    }

}