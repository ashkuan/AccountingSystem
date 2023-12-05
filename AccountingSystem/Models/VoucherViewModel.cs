using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountingSystem.Models
{
    public class VoucherViewModel
    {
       public Voucher Voucher { get; set; }
       public List<VoucherDetail> Details { get; set; }
    }
}