using System;
using System.Collections.Generic;

namespace AccountingSystem.Models
{
    public class VoucherDetail
    {
        public long Voucher_ID { get; set; }
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
        public bool HasError {  get; set; }
    }
}