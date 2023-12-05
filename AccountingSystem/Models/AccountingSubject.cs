namespace AccountingSystem.Models
{
    //放會計科目資料表的資料用
    public class AccountingSubject
    {
        public int Subject_ID {get; set;}
        public string Subject_Name { get; set; }
        public string Subject_MainGroup { get; set; }
        public string Subject_SubGroup { get; set; }
    }
}