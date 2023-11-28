namespace AccountingSystem.Models
{
    public class VoucherDetail
    {
        public string Voucher_ID { get; set; }
        public byte VDetail_Sn { get; set; }
        public string Subject_ID { get; set; }
        public string Subject_DrCr { get; set; }
        public decimal DrCr_Amount { get; set; }
        public string Dept_ID { get; set; }
        public byte Product_ID { get; set; }
        public string Voucher_Note { get; set; }
    }
}