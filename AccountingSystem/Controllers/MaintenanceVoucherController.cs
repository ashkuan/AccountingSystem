using AccountingSystem.Models;
using Microsoft.Reporting.WebForms;
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
            //生成條碼
            string barcode = GenerateBarcode(voucher.Voucher_ID);

            //將條碼保存到傳票訊息裡
            voucher.Barcode = barcode;

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
            //先獲取原有的傳票訊息
            Voucher existingVoucher= dbmanager.GetVoucherById(Voucher.Voucher_ID);
            
            //如果原有的傳票訊息中已有條碼，則保留原有的條碼
            if(!string.IsNullOrEmpty(existingVoucher.Barcode))
            {
                Voucher.Barcode = existingVoucher.Barcode;
            }
            else
            {
                //如果原有的傳票訊息沒有條碼，就生成新的條碼
                string barcode = GenerateBarcode(Voucher.Voucher_ID);
                Voucher.Barcode = barcode;
            }

            dbmanager.UpdateVoucher(Voucher);
            return RedirectToAction("voucher");
        }

        //生成條碼
        public string GenerateBarcode(long Voucher_ID)
        {
            string barcode = "V" + Voucher_ID.ToString();
            return barcode;
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

        //添加匯出RDLC(PDF)報表
        public ActionResult ExportToPDF(long Voucher_ID)
        {
            //根據Voucher_ID獲取相應的數據
            DBmanager dbmanager = new DBmanager();
            Voucher voucher = dbmanager.GetVoucherById(Voucher_ID);
            //創建ReportViewer
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            //加載RDLC文件
            reportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Voucher.rdlc");

            //傳遞數據到報表
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("VoucherReportViewModel", new List<VoucherReportViewModel>()));

            //定義匯出參數
            string mimeType,encoding, fileNameExtension;
            string[] streams;
            Warning[] warnings;
            byte[] renderedBytes;

            //設置匯出格式為PDF
            string deviceInfo = 
                "<DeviceInfo>" +
                " <OutputFormat>PDF</OutputFormat>" +
                "<PageWidth>8.5in</PageWidth>" +
                "<PageHeight>11in</PageHeight>" +
                "<MarginTop>0.5in</MarginTop>" +
                "<MarginLeft>0.5in</MarginLeft>" +
                "<MarginRight>0.5in</MarginRight>" +
                "<MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";

            //執行報表生成
            renderedBytes = reportViewer.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            //返回PDF文件
            return File(renderedBytes, mimeType);
        }
    }

}