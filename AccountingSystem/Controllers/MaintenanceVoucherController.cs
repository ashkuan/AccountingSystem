using AccountingSystem.Models;

using System;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Globalization;

using System.Linq;

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


        public ActionResult CreateVoucherDetail()
        {
            return View();
        }

        

            private VoucherDetail GetVoucherDetailModel()
            {
                // 从Session或者其他地方获取模型
                VoucherDetail model = new VoucherDetail();
                return model;
            }

        [HttpPost]
        public ActionResult CreateVoucherDetail(VoucherDetail voucherDetail)
        {
            //定義參數
            var parameters = new Dictionary<string, object>
      {
          {"Voucher_ID",voucherDetail.Voucher_ID },
          {"VDetail_Sn",voucherDetail.VDetail_Sn },
          {"Subject_ID",voucherDetail.Subject_ID},
          {"Subject_DrCr",voucherDetail.Subject_DrCr},
          {"DrCr_Amount",voucherDetail.DrCr_Amount},
          {"Dept_ID",voucherDetail.Dept_ID},
          {"Product_ID",voucherDetail.Product_ID},
          {"Voucher_Note",voucherDetail.Voucher_Note},
      };
            try
            {
                //呼叫預存程序
                dbmanager.ExecuteStoredProcedure(this, "InsertVoucherDetail", parameters);
            }
            catch (Exception ex)
            {
                //捕捉異常，將錯誤訊息加入 ModelState
                ModelState.AddModelError("", ex.Message);

                //將錯誤訊息存進ViewBag
                ViewBag.ErrorMessage = ex.Message;

                //返回錯誤View
                return View(voucherDetail);
            }
            //檢查ModelState
            if (!ModelState.IsValid)
            {
                //定義字符串列表 用來儲存所有的錯誤訊息
                var errorMessages = new List<string>();

                //遍歷每個模型錯誤
                foreach (var state in ViewData.ModelState.Values)
                {
                    //獲取錯誤信息
                    foreach (var error in state.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }

                //把錯誤訊息存到ViewBag
                ViewBag.ErrorMessage = string.Join("<br>", errorMessages);

                //返回原填寫頁面
                return View("CreateVoucherDetail", voucherDetail);
            }
            return RedirectToAction("EditVoucher", new { Voucher_ID = voucherDetail.Voucher_ID, VDetail_Sn = voucherDetail.VDetail_Sn });
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
    }

}