using AccountingSystem.Controllers;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
//要用SqlConnection
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace AccountingSystem.Models
{
    //連資料庫用
    public class DBmanager
    {
        private readonly string ConnStr = "Data Source=X-HLCIT01-20;Initial Catalog=AccountSystem;Integrated Security=True";
        #region 會計科目
        //取得會計科目資料表的所有資料
        public List<AccountingSubject> GetAccountingSubjects()
        {
            List<AccountingSubject> accountingSubjects = new List<AccountingSubject>();
            //用連線字串連資料庫
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            //下SQL指令拿到資料
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM AccountingSubject ORDER BY Subject_ID");
            sqlCommand.Connection = sqlConnection;
            //Open(開啟)連線資料庫
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    AccountingSubject accountingSubject = new AccountingSubject
                    {
                        Subject_ID = reader.GetInt32(reader.GetOrdinal("Subject_ID")),
                        Subject_Name = reader.GetString(reader.GetOrdinal("Subject_Name")),
                        Subject_MainGroup = reader.GetString(reader.GetOrdinal("Subject_MainGroup")),
                        Subject_SubGroup = reader.GetString(reader.GetOrdinal("Subject_SubGroup")),
                    };
                    accountingSubjects.Add(accountingSubject);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空!");
            }
            sqlConnection.Close();
            return accountingSubjects;
        }

        //去判斷輸入的字串是不是數字
        private bool IsDigitsOnly(string str)
        {
            foreach(char c in str)
            {
                //如果輸入字不在'0'到'9'之間
                if(c<'0'||c>'9')
                    //輸入字不是數字 返回false
                    return false;
            }
            //字串中的所有字符都是數字，返回true
            return true;
        }
        //提供新增傳票明細自動填充功能用
        public List<AccountingSubject> GetAccountingSubjects(string key, string searchField)
        {
            List<AccountingSubject> accountingSubjects = new List<AccountingSubject>();
            //連線資料庫
            using (SqlConnection sqlConnection = new SqlConnection(ConnStr))
            {
                //建立命令
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    //設立基本的select語句
                    sqlCommand.CommandText = "SELECT * FROM AccountingSubject";

                    //根據搜尋欄位加入where條件 參數化查詢可以避免SQL Injection
                    if (searchField == "IDOrName")
                    {
                        //去判斷輸入的是數字還是非數字
                        if (IsDigitsOnly(key))
                        { 
                            //根據ID或名稱模糊搜尋
                            sqlCommand.CommandText += " WHERE Subject_ID LIKE @key";
                            sqlCommand.Parameters.AddWithValue("@key",key + "%");
                        }
                        else
                        {
                            //包含非數字的輸入字，假裝使用者就是輸入Name
                            sqlCommand.CommandText += " WHERE Subject_Name LIKE @key";
                            sqlCommand.Parameters.AddWithValue("@key", "%" + key + "%");
                        }
                        
                    }
               
                    sqlCommand.Connection = sqlConnection;
                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                AccountingSubject accountingSubject = new AccountingSubject
                                {
                                    Subject_ID = reader.GetInt32(reader.GetOrdinal("Subject_ID")),
                                    Subject_Name = reader.GetString(reader.GetOrdinal("Subject_Name")),
                                };
                                accountingSubjects.Add(accountingSubject);
                            }
                        }
                        else
                        {
                            Console.WriteLine("資料庫為空!"); 
                        }
                    }
                }
                sqlConnection.Close();
            }
            
            return accountingSubjects;
        }

        //新增會計科目
        public void CreateSubject(AccountingSubject accountingSubject)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                @"INSERT INTO AccountingSubject(Subject_ID, Subject_Name,Subject_MainGroup,Subject_SubGroup)
                  VALUES(@Subject_ID, @Subject_Name,@Subject_MainGroup,@Subject_SubGroup) ");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_ID", accountingSubject.Subject_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_Name", accountingSubject.Subject_Name));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_MainGroup", accountingSubject.Subject_MainGroup));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_SubGroup", accountingSubject.Subject_SubGroup));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        //新增會計科目後取得全部會計科目DB資料
        public AccountingSubject GetSubjectByID(int Subject_ID)
        {
            AccountingSubject accountingSubject = new AccountingSubject();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM AccountingSubject WHERE Subject_ID = @Subject_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_ID", Subject_ID));
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    accountingSubject = new AccountingSubject
                    {
                        Subject_ID = reader.GetInt32(reader.GetOrdinal("Subject_ID")),
                        Subject_Name = reader.GetString(reader.GetOrdinal("Subject_Name")),
                        Subject_MainGroup = reader.GetString(reader.GetOrdinal("Subject_MainGroup")),
                        Subject_SubGroup = reader.GetString(reader.GetOrdinal("Subject_SubGroup")),
                    };
                }
            }
            else
            {
                accountingSubject.Subject_Name = "未找到該筆資料";
            }
            sqlConnection.Close();
            return accountingSubject;
        }

        //編輯會計科目
        public void UpdateSubject(AccountingSubject accountingSubject)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                @"UPDATE accountingSubject 
                SET Subject_Name = @Subject_Name, Subject_MainGroup = @Subject_MainGroup, Subject_SubGroup = @Subject_SubGroup 
                WHERE Subject_ID = @Subject_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_Name", accountingSubject.Subject_Name));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_MainGroup", accountingSubject.Subject_MainGroup));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_SubGroup", accountingSubject.Subject_SubGroup));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_ID", accountingSubject.Subject_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        //刪除會計科目
        public void DeleteSubjectByID(int Subject_ID)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                @"DELETE FROM AccountingSubject WHERE Subject_ID = @Subject_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_ID", Subject_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        #endregion

        # region 產品別
        //取得產品資料表的所有資料
        public List<Product> GetProducts()
        {
            List<Product> products = new List<Product>();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Product ORDER BY Product_ID");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Product product = new Product
                    {
                        Product_ID = reader.GetByte(reader.GetOrdinal("Product_ID")),
                        Product_Name = reader.GetString(reader.GetOrdinal("Product_Name")),
                    };
                    products.Add(product);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空!");
            }
            sqlConnection.Close();
            return products;
        }

        //提供新增傳票明細自動填充功能用
        public List<Product> GetProducts(string key, string searchField)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection sqlConnection = new SqlConnection(ConnStr))
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.CommandText = "SELECT * FROM Product";

                    if (searchField == "IDOrName")
                    {
                        sqlCommand.CommandText += " WHERE Product_ID LIKE @key OR Product_Name LIKE @key";
                        sqlCommand.Parameters.AddWithValue("@key", "%" + key + "%");
                    }
                    else if (searchField == "ID")
                    {
                        sqlCommand.CommandText += " WHERE Product_ID = @key";
                        sqlCommand.Parameters.AddWithValue("@key", key);
                    }
                    else if (searchField == "Name")
                    {
                        sqlCommand.CommandText += " WHERE Product_Name LIKE @key";
                        sqlCommand.Parameters.AddWithValue("@key", "%" + key + "%");
                    }

                    sqlCommand.Connection = sqlConnection;
                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    Product_ID = reader.GetByte(reader.GetOrdinal("Product_ID")),
                                    Product_Name = reader.GetString(reader.GetOrdinal("Product_Name")),
                                };
                                products.Add(product);
                            }
                        }
                        else
                        {
                            Console.WriteLine("資料庫為空!"); // Database is empty!
                        }
                    }
                }
            }
            return products;
        }

        //新增產品
        public void NewProduct(Product product)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                @"INSERT INTO Product (Product_ID, Product_Name)
                VALUES (@Product_ID, @Product_Name)");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Product_ID", product.Product_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Product_Name", product.Product_Name));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        //新增產品後取得全部產品DB資料
        public Product GetProductById(int Product_ID)
        {
            Product product = new Product();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Product WHERE Product_ID=@Product_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Product_ID", Product_ID));
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    product = new Product()
                    {
                        Product_ID = reader.GetByte(reader.GetOrdinal("Product_ID")),
                        Product_Name = reader.GetString(reader.GetOrdinal("Product_Name")),
                    };
                }
            }
            else
            {
                product.Product_Name = "未找到該筆資料";
            }
            sqlConnection.Close();
            return product;
        }

        //編輯產品
        public void UpdateProduct(Product product)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                @"UPDATE Product SET Product_Name=@Product_Name WHERE Product_ID=@Product_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Product_Name", product.Product_Name));
            sqlCommand.Parameters.Add(new SqlParameter("@Product_ID", product.Product_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        //刪除產品
        public void DeleteProductById(int Product_ID)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM Product WHERE Product_ID=@Product_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("Product_ID", Product_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        #endregion

        private bool ContainesChinese(string input)
        {
            return Regex.IsMatch(input, "[u4e00-u9fff]");
        }

        //提供新增傳票明細自動填充功能用
        public List<Department> GetDept(string key, string searchField)
        {
            List<Department> departments = new List<Department>();
            //連線資料庫
            using (SqlConnection sqlConnection = new SqlConnection(ConnStr))
            {
                //建立命令
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    //設立基本的select語句
                    sqlCommand.CommandText = "SELECT * FROM Department";

                    //根據搜尋欄位加入where條件 參數化查詢可以避免SQL Injection
                    if (searchField == "IDOrName")
                    {
                        //去判斷輸入的是數字還是非數字
                        if (ContainesChinese(key))
                        {
                            //包含中文輸入字，假裝使用者就是輸入Name
                            sqlCommand.CommandText += " WHERE Dept_Name LIKE @key";
                            sqlCommand.Parameters.AddWithValue("@key", "%" + key + "%");
                        }
                        else
                        {
                            sqlCommand.CommandText += " WHERE Dept_ID LIKE @key";
                            sqlCommand.Parameters.AddWithValue("@key", key + "%");
                        }

                    }

                    sqlCommand.Connection = sqlConnection;
                    sqlConnection.Open();

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Department department = new Department
                                {
                                    Dept_ID = reader.GetString(reader.GetOrdinal("Dept_ID")),
                                    Dept_Name = reader.GetString(reader.GetOrdinal("Dept_Name")),
                                };
                                departments.Add(department);
                            }
                        }
                        else
                        {
                            Console.WriteLine("資料庫為空!");
                        }
                    }
                }
                sqlConnection.Close();
            }

            return departments;
        }

        #region 部門別
        //取得部門資料表的所有資料
        public List<Department> GetDept()
        {
            List<Department> departments = new List<Department>();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Department");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Department department = new Department
                    {
                        Dept_ID = reader.GetString(reader.GetOrdinal("Dept_ID")),
                        Dept_Name = reader.GetString(reader.GetOrdinal("Dept_Name")),
                        Company_ID = reader.GetByte(reader.GetOrdinal("Company_ID")),
                    };
                    departments.Add(department);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空!");
            }
            sqlConnection.Close();
            return departments;
        }

        //新增部門
        public void NewDept(Department department)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                @"INSERT INTO Department (Dept_ID, Dept_Name,Company_ID)
                VALUES (@Dept_ID, @Dept_Name,1)");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Dept_ID", department.Dept_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Dept_Name", department.Dept_Name));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        //新增部門後取得全部部門DB資料
        public Department GetDeptById(string Dept_ID)
        {
            Department department = new Department();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Department WHERE Dept_ID= @Dept_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Dept_ID", Dept_ID));
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    department = new Department()
                    {
                        Dept_ID = reader.GetString(reader.GetOrdinal("Dept_ID")),
                        Dept_Name = reader.GetString(reader.GetOrdinal("Dept_Name")),
                        Company_ID = reader.GetByte(reader.GetOrdinal("Company_ID")),
                    };
                }
            }
            else
            {
                department.Dept_Name = "未找到該筆資料";
            }
            sqlConnection.Close();
            return department;
        }

        //編輯部門
        public void UpdateDept(Department department)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                @"UPDATE Department SET Dept_Name=@Dept_Name, Company_ID=@Company_ID WHERE Dept_ID=@Dept_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Dept_Name", department.Dept_Name));
            sqlCommand.Parameters.Add(new SqlParameter("@Company_ID", department.Company_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Dept_ID", department.Dept_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        //刪除部門
        public void DeleteDeptById(string Dept_ID)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM Department WHERE Dept_ID=@Dept_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("Dept_ID", Dept_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        #endregion

        #region 傳票
        //取得傳票資料表的所有資料 傳票JOIN使用者-要顯示名字
        public List<Voucher> GetVouchersWithUserName()
        {
            List<Voucher> vouchers = new List<Voucher>();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                @"SELECT V.*, 
                 U.User_Name AS Lister_Name,
                 U1.User_Name AS Checker_Name,
                 U2.User_Name AS Auditor_Name,
                 U3.User_Name AS Approver_Name
                 FROM Voucher V
                 INNER JOIN [User] U ON V.Lister_ID = U.User_Id
                 INNER JOIN [User] U1 ON V.Checker_ID = U1.User_Id
                 INNER JOIN [User] U2 ON V.Auditor_ID = U2.User_Id
                 INNER JOIN [User] U3 ON V.Approver_ID = U3.User_Id
                 ORDER BY V.Voucher_ID DESC");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Voucher voucher = new Voucher();
                    voucher.Voucher_ID = reader.GetInt64(reader.GetOrdinal("Voucher_ID"));
                    voucher.Voucher_Date = reader.GetDateTime(reader.GetOrdinal("Voucher_Date"));
                    voucher.Voucher_Type = reader.GetString(reader.GetOrdinal("Voucher_Type"));
                    voucher.Lister_ID = reader.GetByte(reader.GetOrdinal("Lister_ID"));
                    voucher.Voucher_State = reader.GetByte(reader.GetOrdinal("Voucher_State"));
                    voucher.Checker_ID = reader.GetByte(reader.GetOrdinal("Checker_ID"));
                    voucher.Auditor_ID = reader.GetByte(reader.GetOrdinal("Auditor_ID"));
                    voucher.Approver_ID = reader.GetByte(reader.GetOrdinal("Approver_ID"));
                    voucher.Lister_Name = reader.GetString(reader.GetOrdinal("Lister_Name"));
                    voucher.Checker_Name = reader.GetString(reader.GetOrdinal("Checker_Name"));
                    voucher.Auditor_Name = reader.GetString(reader.GetOrdinal("Auditor_Name"));
                    voucher.Approver_Name = reader.GetString(reader.GetOrdinal("Approver_Name"));
                    vouchers.Add(voucher);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空!");
            }
            reader.Close();
            sqlConnection.Close();
            return vouchers;
        }

        //新增傳票
        public void NewVoucher(Voucher voucher)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                    @"INSERT INTO Voucher (Voucher_ID, Voucher_Date, Voucher_Type, Lister_ID, Voucher_State, Checker_ID, Auditor_ID, Approver_ID)
                     VALUES (@Voucher_ID, @Voucher_Date, @Voucher_Type, @Lister_ID, @Voucher_State, 4, 3, 2);");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", voucher.Voucher_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Date", voucher.Voucher_Date));
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Type", voucher.Voucher_Type));
            sqlCommand.Parameters.Add(new SqlParameter("@Lister_ID", voucher.Lister_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_State", voucher.Voucher_State));
            sqlCommand.Parameters.Add(new SqlParameter("4", voucher.Checker_ID));
            sqlCommand.Parameters.Add(new SqlParameter("3", voucher.Auditor_ID));
            sqlCommand.Parameters.Add(new SqlParameter("2", voucher.Approver_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        //新增傳票後取得全部傳票DB資料
        public Voucher GetVoucherById(long Voucher_ID)
        {
            var voucher = new Voucher();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                @"SELECT * FROM Voucher
                WHERE Voucher_ID = @Voucher_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", Voucher_ID));
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    voucher = new Voucher()
                    {
                        Voucher_ID = reader.GetInt64(reader.GetOrdinal("Voucher_ID")),
                        Voucher_Date = reader.GetDateTime(reader.GetOrdinal("Voucher_Date")),
                        Voucher_Type = reader.GetString(reader.GetOrdinal("Voucher_Type")),
                        Lister_ID = reader.GetByte(reader.GetOrdinal("Lister_ID")),
                        Voucher_State = reader.GetByte(reader.GetOrdinal("Voucher_State")),
                        Checker_ID = reader.GetByte(reader.GetOrdinal("Checker_ID")),
                        Auditor_ID = reader.GetByte(reader.GetOrdinal("Auditor_ID")),
                        Approver_ID = reader.GetByte(reader.GetOrdinal("Approver_ID")),
                    };
                }
            }
            else
            {
                voucher.Voucher_Type = "未找到該筆資料";
            }
            reader.Close();
            sqlConnection.Close();
            return voucher;
        }

        //編輯傳票
        public void UpdateVoucher(Voucher voucher)
        {
            {
                //資料庫連線
                SqlConnection sqlConnection = new SqlConnection(ConnStr);
                sqlConnection.Open();

                //更新傳票表
                var updateVoucherSql = @"UPDATE Voucher 
                        SET Voucher_Date=@Voucher_Date, Voucher_Type=@Voucher_Type, Lister_ID=@Lister_ID,Voucher_State=@Voucher_State,Checker_ID=4,Auditor_ID=3,Approver_ID=2
                        WHERE Voucher_ID=@Voucher_ID";
                SqlCommand sqlCommand = new SqlCommand(updateVoucherSql, sqlConnection);
                sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", voucher.Voucher_ID));
                sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Date", voucher.Voucher_Date));
                sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Type", voucher.Voucher_Type));
                sqlCommand.Parameters.Add(new SqlParameter("@Lister_ID", voucher.Lister_ID));
                sqlCommand.Parameters.Add(new SqlParameter("@Voucher_State", voucher.Voucher_State));
                sqlCommand.Parameters.Add(new SqlParameter("4", voucher.Checker_ID));
                sqlCommand.Parameters.Add(new SqlParameter("3", voucher.Auditor_ID));
                sqlCommand.Parameters.Add(new SqlParameter("2", voucher.Approver_ID));
                sqlCommand.ExecuteNonQuery();

                sqlCommand.Parameters.Clear();

                sqlConnection.Close();
            }
        }

        //刪除傳票
        public void DeleteVoucherById(long Voucher_ID)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM Voucher WHERE Voucher_ID=@Voucher_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("Voucher_ID", Voucher_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        #endregion

        public void ExecuteStoredProcedure(Controller controller, string procedureName, Dictionary<string, object> parameters)
        {
            // 資料庫連線
            SqlConnection sqlConnection = new SqlConnection(ConnStr);

            // 創建命令
            SqlCommand sqlCommand = new SqlCommand(procedureName, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            // 遍歷參數賦值
            foreach (var param in parameters)
            {
                sqlCommand.Parameters.AddWithValue(param.Key, param.Value);
            }

            try
            {
                // 開啟連線
                sqlConnection.Open();

                // 執行預存程序
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {  
                controller.ModelState.AddModelError("@ErroMsg", ex.Message);
                throw;
            }
            finally
            {
                // 關閉連線
                sqlConnection.Close();
            }
        }


        # region 傳票明細
        //取得傳票明細資料表所有資料
        public List<VoucherDetail> GetVoucherDetails(long Voucher_ID)
        {
            List<VoucherDetail> voucherDetails = new List<VoucherDetail>();
            try
            {
            string sql = @" SELECT VD.*,
            S.Subject_Name AS Subject_Name,
            D.Dept_Name AS Dept_Name,
            P.Product_Name AS Product_Name
            FROM VoucherDetail VD
            INNER JOIN [AccountingSubject] S ON VD.Subject_ID=S.Subject_ID
            INNER JOIN [Department] D ON VD.Dept_ID=D.Dept_ID
            INNER JOIN [Product] P ON VD.Product_ID=P.Product_ID
            WHERE VD.Voucher_ID = @Voucher_ID
            ORDER BY VD.VDetail_Sn";

                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Voucher_ID", Voucher_ID);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VoucherDetail voucherDetail = new VoucherDetail()
                                {
                                    Voucher_ID = reader.GetInt64(reader.GetOrdinal("Voucher_ID")),
                                    VDetail_Sn = reader.GetByte(reader.GetOrdinal("VDetail_Sn")),
                                    Subject_ID = reader.GetInt32(reader.GetOrdinal("Subject_ID")),
                                    Subject_DrCr = reader.GetString(reader.GetOrdinal("Subject_DrCr")),
                                    DrCr_Amount = reader.GetDecimal(reader.GetOrdinal("DrCr_Amount")),
                                    Dept_ID = reader.GetString(reader.GetOrdinal("Dept_ID")),
                                    Product_ID = reader.GetByte(reader.GetOrdinal("Product_ID")),
                                    Voucher_Note = reader.GetString(reader.GetOrdinal("Voucher_Note")),
                                    Subject_Name = reader.GetString(reader.GetOrdinal("Subject_Name")),
                                    Dept_Name = reader.GetString(reader.GetOrdinal("Dept_Name")),
                                    Product_Name = reader.GetString(reader.GetOrdinal("Product_Name"))
                                };
                                voucherDetails.Add(voucherDetail);
                            }
                            reader.Close();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<VoucherDetail>();
            }

            return voucherDetails;
        }

        //新增傳票明細
        public void NewVoucherDetail(VoucherDetail voucherDetail)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                 @"INSERT INTO VoucherDetail (Voucher_ID, VDetail_Sn, Subject_ID, Subject_DrCr, DrCr_Amount, Dept_ID, Product_ID, Voucher_Note)
                   VALUES (@Voucher_ID, @VDetail_Sn, @Subject_ID, @Subject_DrCr, @DrCr_Amount, @Dept_ID, @Product_ID, @Voucher_Note);");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", voucherDetail.Voucher_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@VDetail_Sn", voucherDetail.VDetail_Sn));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_ID", voucherDetail.Subject_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_DrCr", voucherDetail.Subject_DrCr));
            sqlCommand.Parameters.Add(new SqlParameter("@DrCr_Amount", voucherDetail.DrCr_Amount));
            sqlCommand.Parameters.Add(new SqlParameter("@Dept_ID", voucherDetail.Dept_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Product_ID", voucherDetail.Product_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Note", voucherDetail.Voucher_Note));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        public VoucherDetail GetVDetailById(long Voucher_ID, byte VDetail_Sn)
        {
            VoucherDetail voucherDetail = new VoucherDetail();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(@"SELECT * FROM VoucherDetail WHERE Voucher_ID=@Voucher_ID AND VDetail_Sn=@VDetail_Sn");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", Voucher_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@VDetail_Sn", VDetail_Sn));
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    voucherDetail = new VoucherDetail
                    {
                        Voucher_ID = reader.GetInt64(reader.GetOrdinal("Voucher_ID")),
                        VDetail_Sn = reader.GetByte(reader.GetOrdinal("VDetail_Sn")),
                        Subject_ID = reader.GetInt32(reader.GetOrdinal("Subject_ID")),
                        Subject_DrCr = reader.GetString(reader.GetOrdinal("Subject_DrCr")),
                        DrCr_Amount = reader.GetDecimal(reader.GetOrdinal("DrCr_Amount")),
                        Dept_ID = reader.GetString(reader.GetOrdinal("Dept_ID")),
                        Product_ID = reader.GetByte(reader.GetOrdinal("Product_ID")),
                        Voucher_Note = reader.GetString(reader.GetOrdinal("Voucher_Note")),
                    };
                }
            }
            else
            {
                voucherDetail.Subject_DrCr = "未找到該筆資料";
            }
            sqlConnection.Close();
            return voucherDetail;
        }

        public VoucherDetail GetVDetail()
        {
            VoucherDetail voucherDetail = new VoucherDetail();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(@"SELECT * FROM VoucherDetail");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    voucherDetail = new VoucherDetail
                    {
                        Voucher_ID = reader.GetInt64(reader.GetOrdinal("Voucher_ID")),
                        VDetail_Sn = reader.GetByte(reader.GetOrdinal("VDetail_Sn")),
                        Subject_ID = reader.GetInt32(reader.GetOrdinal("Subject_ID")),
                        Subject_DrCr = reader.GetString(reader.GetOrdinal("Subject_DrCr")),
                        DrCr_Amount = reader.GetDecimal(reader.GetOrdinal("DrCr_Amount")),
                        Dept_ID = reader.GetString(reader.GetOrdinal("Dept_ID")),
                        Product_ID = reader.GetByte(reader.GetOrdinal("Product_ID")),
                        Voucher_Note = reader.GetString(reader.GetOrdinal("Voucher_Note")),
                    };
                }
            }
            else
            {
                voucherDetail.Subject_DrCr = "未找到該筆資料";
            }
            sqlConnection.Close();
            return voucherDetail;
        }

        //編輯傳票明細
        public void UpdateVoucherDetail(VoucherDetail voucherDetail)

        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnStr))

            {
                sqlConnection.Open();

                // 先選擇相關的資料
                using (SqlCommand sqlCommand = new SqlCommand(
                @"UPDATE VoucherDetail
                SET
                Subject_ID = @Subject_ID,
                Subject_DrCr = @Subject_DrCr,
                DrCr_Amount = @DrCr_Amount,
                Dept_ID = @Dept_ID,
                Product_ID = @Product_ID,
                Voucher_Note = @Voucher_Note
                WHERE VoucherDetail.Voucher_ID = @Voucher_ID
                AND VoucherDetail.VDetail_Sn = @VDetail_Sn", sqlConnection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", voucherDetail.Voucher_ID));
                    sqlCommand.Parameters.Add(new SqlParameter("@VDetail_Sn", voucherDetail.VDetail_Sn));
                    sqlCommand.Parameters.Add(new SqlParameter("@Subject_ID", voucherDetail.Subject_ID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Subject_DrCr", voucherDetail.Subject_DrCr));
                    sqlCommand.Parameters.Add(new SqlParameter("@DrCr_Amount", voucherDetail.DrCr_Amount));
                    sqlCommand.Parameters.Add(new SqlParameter("@Dept_ID", voucherDetail.Dept_ID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Product_ID", voucherDetail.Product_ID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Note", voucherDetail.Voucher_Note));
                    sqlCommand.ExecuteNonQuery();
                }

            }
        }

        public void DeleteVoucherDetail(long Voucher_ID)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM VoucherDetail WHERE Voucher_ID=@Voucher_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", Voucher_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        //刪除傳票明細
        public void DeleteVoucherDetailBySn(long Voucher_ID, Byte VDetail_Sn)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM VoucherDetail WHERE Voucher_ID=@Voucher_ID AND VDetail_Sn=@VDetail_Sn");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", Voucher_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@VDetail_Sn", VDetail_Sn));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
        #endregion
    }
}