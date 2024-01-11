using AccountingSystem.Models;
using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
// 引用這個命名空間來使用 ReportViewer
using Microsoft.Reporting.WebForms;
using System.Net;
using System.Net.Http.Headers;
using System.Web.UI.WebControls;

namespace AccountingSystem.Controllers
{
    public class FinancialReportsController : Controller
    {
        #region 日記帳
        [HttpGet]
        public ActionResult SearchVoucherDetails()
        {
            DBmanager dBmanager = new DBmanager();
            List<VoucherReportViewModel> voucherReportViewModel = dBmanager.GetVouchersDetailsReports();
            // 初始化视图模型或从数据库获取数据
            return View(voucherReportViewModel);
        }

        [HttpPost]
        public ActionResult SearchVoucherDetails(DateTime? startDate, DateTime? endDate)
        {
            DBmanager dBmanager = new DBmanager();
            var voucherDetailList = new List<VoucherReportViewModel>();
            ViewBag.selectedStartDate = startDate;
            ViewBag.selectedEndDate = endDate;
            if (startDate.HasValue && endDate.HasValue)
            {
                //獲取數據
                voucherDetailList = dBmanager.GetVouchersDetailsInRange(startDate.Value, endDate.Value);
                //將數據傳遞給View
            }

            //如果沒有搜尋參數，只顯示空表單
            return View(voucherDetailList);
        }


        public ActionResult GenerateVoucherReport(DateTime? startDate,DateTime? endDate)
        {
            if(!startDate.HasValue|| !endDate.HasValue)
            {
                //如果沒有提供日期，返回錯誤訊息
                TempData["ErrorMessage"] = "必須提供起始日期和截止日期!";
                return RedirectToAction("SearchVoucherDetails");
            }
            DBmanager dBmanager = new DBmanager();
            //獲取指定日期範圍內的傳票明細數據
            var reportDataList = dBmanager.GetVouchersDetailsInRange(startDate.Value, endDate.Value);

            //設定RDLC報表的路徑
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Journal.rdlc");

            //建立報表的資料來源
            ReportDataSource reportDataSource1 = new ReportDataSource("Journal", reportDataList);  
            localReport.DataSources.Add(reportDataSource1);

            //設定報表參數
            ReportParameter[] reportParameters = new ReportParameter[]
            {
                new ReportParameter("startDate",startDate?.ToShortDateString()),
                new ReportParameter("endDate",endDate?.ToShortDateString()),
                new ReportParameter("reportDate",DateTime.Now.ToShortDateString()),
            };

            localReport.SetParameters(reportParameters);

            //渲染報表為PDF格式
            //這些變數用來存渲染過程中生成的報表屬性
            string mimeType, encoding, fileNameExtension;           

            //這些變數都用來存渲染過程中可能出現的警告和其他信息。
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            

            //調用Render方法渲染報表，將報表轉換成指定格式，並將生成的字節Array存在renderedBytes中
            renderedBytes = localReport.Render(
                "PDF",
                null, //報表設備信息設定，傳null為默認值。
                out mimeType, //out參數在方法內部被賦值 =>文件的MIME類型
                out encoding, //文件的編碼信息
                out fileNameExtension, //生成的文件擴展名
                out streams,//存儲報表的流信息
                out warnings);//存儲渲染過程中的警示訊息


            //用File方法創建一個FileResult對象，該對象將PDF文件用HTTP響應形式發給客戶端 第三個參數是文件名稱(當前日期+時間確保文件唯一性)
            //返回報表給用戶
            return File(renderedBytes, mimeType, $"JournalReport_{DateTime.Now.ToString("yyyyMMddHHmmss")}.{fileNameExtension}");
        }
        #endregion

        #region 總分類帳
        [HttpGet]
        public ActionResult SearchVDinLedger()
        {
            DBmanager dBmanager = new DBmanager();
            List<VoucherReportViewModel> voucherReportViewModel = dBmanager.GetVDinLedgerReports();
            return View(voucherReportViewModel);
        }

        [HttpPost]
        public ActionResult SearchVDinLedger(DateTime? startDate, DateTime? endDate, string selectedSubjects, string selectedDepts)
        {
            if (!startDate.HasValue || !endDate.HasValue || string.IsNullOrEmpty(selectedSubjects) || string.IsNullOrEmpty(selectedDepts))
            {
                //如果沒有提供日期，返回錯誤訊息
                TempData["ErrorMessage"] = "必須提供查詢條件!";
                return RedirectToAction("SearchVDinLedger");
            }
            DBmanager dBmanager = new DBmanager();
            var LedgerDetail = dBmanager.GetVDinLedger(startDate, endDate, selectedSubjects, selectedDepts);
            ViewBag.SelectedStartDate = startDate?.ToShortDateString();
            ViewBag.SelectedEndDate=endDate?.ToShortDateString();
            ViewBag.SelectedSubjects = selectedSubjects;
            ViewBag.SelectedDepts = selectedDepts;
            return View(LedgerDetail);
        }

        public ActionResult GenerateLedgerReport(DateTime? startDate, DateTime? endDate, string selectedSubjects, string selectedDepts)
        {
             if (!startDate.HasValue || !endDate.HasValue || string.IsNullOrEmpty(selectedSubjects) || string.IsNullOrEmpty(selectedDepts))
            {
                //如果沒有提供日期，返回錯誤訊息
                TempData["ErrorMessage"] = "必須提供查詢條件!";
                return RedirectToAction("SearchVDinLedger");
            }
            
            DBmanager dBmanager = new DBmanager();
            var voucherReportViewModel = dBmanager.GetVDinLedger(startDate, endDate, selectedSubjects, selectedDepts);
            
            //獲取部門和科目參數
            string deptParam=string.Join(", ",selectedDepts);
            string subjectParam=string.Join(", ",selectedSubjects);
            
            //設置RDLC報表的路徑
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Ledger.rdlc");

            //建立報表的資料來源
            ReportDataSource reportDataSource = new ReportDataSource("Journal", voucherReportViewModel);
            localReport.DataSources.Add(reportDataSource);

            //設置報表參數 string.Empty可以避免null值時拋出異常
            ReportParameter[] reportParameters = new ReportParameter[]
            {
            new ReportParameter("startDate", startDate?.ToShortDateString()),
            new ReportParameter("endDate", endDate?.ToShortDateString()),
            new ReportParameter("reportDate", DateTime.Now.ToShortDateString()),
            new ReportParameter("Dept", deptParam),
            new ReportParameter("Subject", subjectParam),
            };
            localReport.SetParameters(reportParameters);

            //渲染報表為Excel格式
            string mimeType, encoding, fileNameExtension;
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(
                "Excel",
                null, //報表設備訊息設定，傳null為默認值
                out mimeType, //文件的MIME類型
                out encoding, //文件的編碼訊息
                out fileNameExtension, //生成的文件擴展名
                out streams, //存儲報表的流訊息
                out warnings); //存儲渲染過程中的警告訊息

            //調用生成PDF的方法，這個方法將返回一個FileResult
            return File(renderedBytes, mimeType, $"LedgerReport_{DateTime.Now.ToString("yyyyMMddHHmmss")}.{fileNameExtension}");
        }       
        #endregion
    }
}