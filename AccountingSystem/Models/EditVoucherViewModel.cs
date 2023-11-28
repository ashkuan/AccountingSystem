using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Models
{
    public class EditVoucherViewModel
    {
        public Voucher Voucher { get; set; }
        public List<VoucherDetail> VoucherDetails { get; set; }
    }
}