using System;
using System.Collections.Generic;

namespace AccountingSystem.Models
{
    public class Voucher
    {
        public long Voucher_ID { get; set; }
        public DateTime Voucher_Date { get; set; }
        public string Voucher_Type { get; set; }
        public byte Lister_ID { get; set; }
        public byte Voucher_State { get; set; }
        public byte Checker_ID { get; set; }
        public byte Auditor_ID { get; set; }
        public byte Approver_ID { get; set; }
        public string Lister_Name { get; set; }
        public string Checker_Name { get; set; }
        public string Auditor_Name { get; set; }
        public string Approver_Name { get; set; }
    }
}