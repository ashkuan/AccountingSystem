using System;
using System.Collections.Generic;

namespace AccountingSystem.Models
{
    public class Voucher
    {
        public string Voucher_ID { get; set; }
        public DateTime Voucher_Date { get; set; }
        public string Voucher_Type { get; set; }
        public byte User_ID { get; set; }
        public byte Voucher_State { get; set; }
        public byte User_ID1 { get; set; }
        public byte User_ID2 { get; set; }
        public byte User_ID3 { get; set; }
    }
}