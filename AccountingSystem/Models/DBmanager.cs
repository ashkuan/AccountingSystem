using System;
using System.Collections.Generic;
//要用SqlConnection
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Web.UI.WebControls;

namespace AccountingSystem.Models
{
    //連資料庫用
    public class DBmanager
    {
        private readonly string ConnStr = "Data Source=X-HLCIT01-20;Initial Catalog=AccountSystem;Integrated Security=True";
        //--會計科目--
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
                        Subject_Group1 = reader.GetString(reader.GetOrdinal("Subject_Group1")),
                        Subject_Group2 = reader.GetString(reader.GetOrdinal("Subject_Group2")),
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

        //新增會計科目
        public void CreateSubject(AccountingSubject accountingSubject)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand(
                @"INSERT INTO AccountingSubject(Subject_ID, Subject_Name,Subject_Group1,Subject_Group2)
                  VALUES(@Subject_ID, @Subject_Name,@Subject_Group1,@Subject_Group2) ");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_ID", accountingSubject.Subject_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_Name", accountingSubject.Subject_Name));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_Group1", accountingSubject.Subject_Group1));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_Group2", accountingSubject.Subject_Group2));
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
                        Subject_Group1 = reader.GetString(reader.GetOrdinal("Subject_Group1")),
                        Subject_Group2 = reader.GetString(reader.GetOrdinal("Subject_Group2")),
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
                SET Subject_Name = @Subject_Name, Subject_Group1 = @Subject_Group1, Subject_Group2 = @Subject_Group2 
                WHERE Subject_ID = @Subject_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_Name", accountingSubject.Subject_Name));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_Group1", accountingSubject.Subject_Group1));
            sqlCommand.Parameters.Add(new SqlParameter("@Subject_Group2", accountingSubject.Subject_Group2));
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

        //--產品別--
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


        //--部門別--
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
                @"INSERT INTO Department (Dept_ID, Dept_Name)
                VALUES (@Dept_ID, @Dept_Name)");
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

        //--傳票--
        //取得傳票資料表的所有資料
        public List<Voucher> GetVouchers()
        {
            List<Voucher> vouchers = new List<Voucher>();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Voucher ORDER BY Voucher_ID");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Voucher voucher = new Voucher
                    {
                        Voucher_ID = reader.GetString(reader.GetOrdinal("Voucher_ID")),
                        Voucher_Date = reader.GetDateTime(reader.GetOrdinal("Voucher_Date")),
                        Voucher_Type = reader.GetString(reader.GetOrdinal("Voucher_Type")),
                        User_ID = reader.GetByte(reader.GetOrdinal("User_ID")),
                        Voucher_State = reader.GetByte(reader.GetOrdinal("Voucher_State")),
                        User_ID1 = reader.GetByte(reader.GetOrdinal("User_ID1")),
                        User_ID2 = reader.GetByte(reader.GetOrdinal("User_ID2")),
                        User_ID3 = reader.GetByte(reader.GetOrdinal("User_ID3")),
                    };
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
                    @"INSERT INTO Voucher (Voucher_ID, Voucher_Date, Voucher_Type, User_ID, Voucher_State, User_ID1, User_ID2, User_ID3)
                     VALUES (@Voucher_ID, @Voucher_Date, @Voucher_Type, @User_ID, @Voucher_State, @User_ID1, @User_ID2, @User_ID3);");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", voucher.Voucher_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Date", voucher.Voucher_Date));
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Type", voucher.Voucher_Type));
            sqlCommand.Parameters.Add(new SqlParameter("@User_ID", voucher.User_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_State", voucher.Voucher_State));
            sqlCommand.Parameters.Add(new SqlParameter("@User_ID1", voucher.User_ID1));
            sqlCommand.Parameters.Add(new SqlParameter("@User_ID2", voucher.User_ID2));
            sqlCommand.Parameters.Add(new SqlParameter("@User_ID3", voucher.User_ID3));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }



        //新增傳票後取得全部傳票DB資料
        public Voucher GetVoucherById(string Voucher_ID)
        {
            var result = new EditVoucherViewModel();
            var voucher = new Voucher();
            var Details = new List<VoucherDetail>();
            result.VoucherDetails = Details;

            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            sqlConnection.Open();  // 開啟連線

            SqlCommand sqlCommand = new SqlCommand(
                @"SELECT * FROM Voucher V 
        JOIN VoucherDetail VD ON V.Voucher_ID = VD.Voucher_ID
        WHERE V.Voucher_ID = @Voucher_ID", sqlConnection);  // 設置連線
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", Voucher_ID));

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                if (voucher.Voucher_ID == null)  // 只需設定一次，以防止重複設定
                {
                    voucher.Voucher_ID = reader.GetString(reader.GetOrdinal("Voucher_ID"));
                    voucher.Voucher_Date = reader.GetDateTime(reader.GetOrdinal("Voucher_Date"));
                    voucher.Voucher_Type = reader.GetString(reader.GetOrdinal("Voucher_Type"));
                    voucher.User_ID = reader.GetByte(reader.GetOrdinal("User_ID"));
                    voucher.Voucher_State = reader.GetByte(reader.GetOrdinal("Voucher_State"));
                    voucher.User_ID1 = reader.GetByte(reader.GetOrdinal("User_ID1"));
                    voucher.User_ID2 = reader.GetByte(reader.GetOrdinal("User_ID2"));
                    voucher.User_ID3 = reader.GetByte(reader.GetOrdinal("User_ID3"));
                }

                var detail = new VoucherDetail
                {
                    Voucher_ID = reader.GetString(reader.GetOrdinal("Voucher_ID")),
                    VDetail_Sn = reader.GetByte(reader.GetOrdinal("VDetail_Sn")),
                    Subject_ID = reader.GetString(reader.GetOrdinal("Subject_ID")),
                    Subject_DrCr = reader.GetString(reader.GetOrdinal("Subject_DrCr")),
                    DrCr_Amount = reader.GetDecimal(reader.GetOrdinal("DrCr_Amount")),
                    Dept_ID = reader.GetString(reader.GetOrdinal("Dept_ID")),
                    Product_ID = reader.GetByte(reader.GetOrdinal("Product_ID")),
                    Voucher_Note = reader.GetString(reader.GetOrdinal("Voucher_Note"))
                };

                Details.Add(detail);
            }

            reader.Close();
            sqlConnection.Close();  // 關閉連線
            return voucher;
        }


        //編輯傳票
        public void UpdateVoucher(Voucher voucher, List<VoucherDetail> Details)
        {
            {
                List<VoucherDetail> details= new List<VoucherDetail>();
                //資料庫連線
                SqlConnection sqlConnection = new SqlConnection(ConnStr);
                sqlConnection.Open();
                
                    //更新傳票表
                    var updateVoucherSql = @"UPDATE Voucher 
                        SET Voucher_Date=@Voucher_Date, Voucher_Type=@Voucher_Type, User_ID=@User_ID,Voucher_State=@Voucher_State
                        WHERE Voucher_ID=@Voucher_ID";
                    SqlCommand sqlCommand = new SqlCommand(updateVoucherSql, sqlConnection);
                    sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", voucher.Voucher_ID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Date", voucher.Voucher_Date));
                    sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Type", voucher.Voucher_Type));
                    sqlCommand.Parameters.Add(new SqlParameter("@User_ID", voucher.User_ID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Voucher_State", voucher.Voucher_State));
                    sqlCommand.Parameters.Add(new SqlParameter("@User_ID1", voucher.User_ID1));
                    sqlCommand.Parameters.Add(new SqlParameter("@User_ID2", voucher.User_ID2));
                    sqlCommand.Parameters.Add(new SqlParameter("@User_ID3", voucher.User_ID3));
                    sqlCommand.ExecuteNonQuery();
                    
                    //循環更新傳票明細
                    foreach (var voucherDetail in details)
                    {
                        SqlConnection sqlConnection2= new SqlConnection(ConnStr);
                        sqlConnection2.Open();
                        string sqlSelect =
                        @"SELECT VD.*, V.* FROM VoucherDetail VD 
                            JOIN Voucher V ON VD.Voucher_ID = V.Voucher_ID
                            WHERE V.Voucher_ID =@Voucher_ID";
                        SqlCommand selectCmd = new SqlCommand(sqlSelect, sqlConnection2);
                        selectCmd.Parameters.Add(new SqlParameter("@Voucher_ID", voucherDetail.Voucher_ID));
                        var reader = selectCmd.ExecuteReader();
                        string sqlUpdate = @"
                            UPDATE VoucherDetail
                            SET VDetail_Sn = @VDetail_Sn,
                            Subject_ID = @Subject_ID,
                            Subject_DrCr=@Subject_DrCr, 
                            DrCr_Amount=@DrCr_Amount, 
                            Dept_ID = @Dept_ID,
                            Product_ID = @Product_ID, 
                            Voucher_Note=@Voucher_Note
                            WHERE VoucherDetail.Voucher_ID = @Voucher_ID 
                            AND VoucherDetail.VDetail_Sn = @VDetail_Sn";
                        SqlCommand updateCmd = new SqlCommand(sqlUpdate, sqlConnection);
                        updateCmd.Parameters.Add(new SqlParameter("@Voucher_ID", voucherDetail.Voucher_ID));
                        updateCmd.Parameters.Add(new SqlParameter("@VDetail_Sn", voucherDetail.VDetail_Sn));
                        updateCmd.Parameters.Add(new SqlParameter("@Subject_ID", voucherDetail.Subject_ID));
                        updateCmd.Parameters.Add(new SqlParameter("@Subject_DrCr", voucherDetail.Subject_DrCr));
                        updateCmd.Parameters.Add(new SqlParameter("@DrCr_Amount", voucherDetail.DrCr_Amount));
                        updateCmd.Parameters.Add(new SqlParameter("@Dept_ID", voucherDetail.Dept_ID));
                        updateCmd.Parameters.Add(new SqlParameter("@Product_ID", voucherDetail.Product_ID));
                        updateCmd.Parameters.Add(new SqlParameter("@Voucher_Note", voucherDetail.Voucher_Note));
                        
                        reader.Close();
                      
                        updateCmd.ExecuteNonQuery();

                    }
                    sqlCommand.Parameters.Clear();
                    sqlConnection.Close();
            }
        }

        //刪除傳票
        public void DeleteVoucherById(string Voucher_ID)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM Voucher WHERE Voucher_ID=@Voucher_ID");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("Voucher_ID", Voucher_ID));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        //--傳票明細--
        //取得傳票明細資料表所有資料
        public List<VoucherDetail> GetVoucherDetails(string Voucher_ID)
        {
            List<VoucherDetail> voucherDetails = new List<VoucherDetail>();
            string sql = @" SELECT * FROM VoucherDetail VD 
                            JOIN Voucher V ON VD.Voucher_ID = V.Voucher_ID
                            WHERE V.Voucher_ID =@Voucher_ID";
            using (SqlConnection conn =new SqlConnection(ConnStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Voucher_ID", Voucher_ID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VoucherDetail voucherDetail = new VoucherDetail() {
                            Voucher_ID = reader.GetString(reader.GetOrdinal("Voucher_ID")),
                            VDetail_Sn = reader.GetByte(reader.GetOrdinal("VDetail_Sn")),
                            Subject_ID = reader.GetString(reader.GetOrdinal("Subject_ID")),
                            Subject_DrCr = reader.GetString(reader.GetOrdinal("Subject_DrCr")),
                            DrCr_Amount = reader.GetDecimal(reader.GetOrdinal("DrCr_Amount")),
                            Dept_ID = reader.GetString(reader.GetOrdinal("Dept_ID")),
                            Product_ID = reader.GetByte(reader.GetOrdinal("Product_ID")),
                            Voucher_Note = reader.GetString(reader.GetOrdinal("Voucher_Note")),
                        };
                            voucherDetails.Add(voucherDetail);
                        }
                        reader.Close();
                    }

                    }
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

        //新增傳票明細後取得全部部門DB資料
        public List<VoucherDetail> GetVoucherDetailById() 
        {
            List<VoucherDetail> voucherDetails = new List<VoucherDetail>();
            VoucherDetail voucherDetail = new VoucherDetail();
            SqlConnection sqlConnection = new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM VoucherDetail");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows) 
            { 
                while (reader.Read())
                {
                    voucherDetail = new VoucherDetail
                    {
                        Voucher_ID = reader.GetString(reader.GetOrdinal("Voucher_ID")),
                        VDetail_Sn = reader.GetByte(reader.GetOrdinal("VDetail_Sn")),
                        Subject_ID = reader.GetString(reader.GetOrdinal("Subject_ID")),
                        Subject_DrCr = reader.GetString(reader.GetOrdinal("Subject_DrCr")),
                        DrCr_Amount = reader.GetDecimal(reader.GetOrdinal("DrCr_Amount")),
                        Dept_ID = reader.GetString(reader.GetOrdinal("Dept_ID")),
                        Product_ID = reader.GetByte(reader.GetOrdinal("Product_ID")),
                        Voucher_Note = reader.GetString(reader.GetOrdinal("Voucher_Note")),
                    };
                    voucherDetails.Add(voucherDetail);
                }
            }
            else
            {
                voucherDetail.Subject_ID = "未找到該筆資料";
            }
            sqlConnection.Close();
            return voucherDetails;
        }

        //編輯傳票明細
        public void UpdateVoucherDetail(VoucherDetail detail)

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
                    sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", detail.Voucher_ID));
                    sqlCommand.Parameters.Add(new SqlParameter("@VDetail_Sn", detail.VDetail_Sn));
                    sqlCommand.Parameters.Add(new SqlParameter("@Subject_ID", detail.Subject_ID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Subject_DrCr", detail.Subject_DrCr));
                    sqlCommand.Parameters.Add(new SqlParameter("@DrCr_Amount", detail.DrCr_Amount));
                    sqlCommand.Parameters.Add(new SqlParameter("@Dept_ID", detail.Dept_ID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Product_ID", detail.Product_ID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Voucher_Note", detail.Voucher_Note));
                    sqlCommand.ExecuteNonQuery();
                }
              
            }
        }

        public void DeleteVoucherDetail(String Voucher_ID)
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
        public void DeleteVoucherDetailBySn(String Voucher_ID, Byte VDetail_Sn)
        {
            SqlConnection sqlConnection=new SqlConnection(ConnStr);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM VoucherDetail WHERE Voucher_ID=@Voucher_ID AND VDetail_Sn=@VDetail_Sn");
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Parameters.Add(new SqlParameter("@Voucher_ID", Voucher_ID));
            sqlCommand.Parameters.Add(new SqlParameter("@VDetail_Sn", VDetail_Sn));
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
}