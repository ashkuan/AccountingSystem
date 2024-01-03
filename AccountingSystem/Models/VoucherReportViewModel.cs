using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Models
{
    //資料庫查數據傳送到RDLC報表時，把所有數據封裝到一個單一對象裡
    public class VoucherReportViewModel
    {
        //Voucher
        public long Voucher_ID { get; set; }
        public DateTime Voucher_Date { get; set; }
        public string Voucher_Type { get; set; }
        public byte Lister_ID { get; set; }
        public byte Voucher_State { get; set; }
        public byte Checker_ID { get; set; }
        public byte Auditor_ID { get; set; }
        public byte Approver_ID { get; set; }

        //VoucherDetail
        public byte VDetail_Sn { get; set; }
        public int Subject_ID { get; set; }
        public string Subject_DrCr { get; set; }
        public decimal DrCr_Amount { get; set; }
        public string Dept_ID { get; set; }
        public byte Product_ID { get; set; }
        public string Voucher_Note { get; set; }
        public string Subject_Name { get; set; }
        public string Dept_Name { get; set; }
        public string Product_Name { get; set; }

        //關聯數據
        public string Lister_Name { get; set; }
        public string Checker_Name { get; set; }
        public string Auditor_Name { get; set; }
        public string Approver_Name { get; set; }
    }
}