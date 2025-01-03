using System;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Windows;
using BioApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using BioApp.RegistryHelper;
using BioApp.Tools;

namespace BioApp.DB.Dal
{
    public class SqlDataAccessLayer:IGRDal
    {
        public readonly string CONNECTION_STRING;
        public SQLiteConnection dbConection;
        private readonly string DBFileName = AppPathClass.FetcAppPath() + @"\" + "BioAppDB.db";
        
        string APP_PATH = AppPathClass.FetcAppPath();
        bool DelSuccess = false;
        private IRegistryToken regToken;
        /// <summary>
        /// Class cunstructore
        /// </summary>
        public SqlDataAccessLayer()
        {
            CONNECTION_STRING = "Data Source = " +  DBFileName+ ";Version=3;";
            dbConection = new SQLiteConnection(CONNECTION_STRING);
            regToken = new RegistryToken();
        }

        #region DataBase & Creation

        /// <summary>
        /// This method creates the DataBase, it deletes it if it exists.
        /// </summary>
        public async Task<bool> CreateDataBase()
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(DBFileName))
                        File.Delete(DBFileName);

                    SQLiteConnection.CreateFile(DBFileName);
                    DelSuccess = true;

                    CreateTables("Templates");
                    CreateTables("TVerify");
                    CreateTables("Temp");
                    CreateTables("PersonalInfo");
                    CreateTables("Subject");
                    CreateTables("Fin");
                    CreateTables("Staff");
                    CreateTables("Bank");
                    CreateTables("State");
                    CreateTables("OperatorTemp");
                    CreateTables("summaryTable");
                    CreateTables("schoolSummaryTable");
                    return true;
                }
                catch (Exception e)
                {
                    //  MsBox.Show(e.Message, "Create DB", MessageBoxButton.OK, MessageBoxImage.Error);
                    System.Windows.Forms.MessageBox.Show(e.Message);
                    DelSuccess = false;
                }
                return false;
            });
           
        }
        /// <summary>
        /// Creates the tables to run the App.
        /// </summary>
        /// <param name="TableName"></param>
        private void CreateTables(string TableName)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();

            string strCommand = "";
            try
            {
                switch (TableName)
                {
                    case "Bank":
                        strCommand = @"CREATE TABLE  IF NOT EXISTS [Bank] (
                          [bank_code] NVARCHAR(3) NOT NULL PRIMARY KEY,
                          [bank_name] NVARCHAR(60) NOT NULL
                          )";
                        break;
                    case "summaryTable":
                        strCommand = @"CREATE TABLE  IF NOT EXISTS [summaryTable] (
                          [operatorid] NVARCHAR(13) NOT NULL PRIMARY KEY,
                          [cafe] NVARCHAR(60) NOT NULL,
                          [total] INTEGER NOT NULL
                          )";
                        break;//schoolSummaryTable
                    case "schoolSummaryTable":
                        strCommand = @"CREATE TABLE  IF NOT EXISTS [schoolSummaryTable] (
                          [schoolNo] NVARCHAR(7) NOT NULL PRIMARY KEY,
                          [schoolName] NVARCHAR(100) NOT NULL,
                          [operatorid] NVARCHAR(13) NOT NULL,
                          [total] INTEGER NOT NULL
                          )";
                        break;
                    case "Staff":
                        strCommand = @"CREATE TABLE  IF NOT EXISTS [Staff] (
                          [per_no] NVARCHAR(4) NOT NULL PRIMARY KEY,
                          [name] NVARCHAR(45) NOT NULL
                          )";
                        break;
                    case "OperatorTemp":
                        strCommand = @"CREATE TABLE  IF NOT EXISTS [OperatorTemp] (
                          [operatorId] NVARCHAR(13) NOT NULL PRIMARY KEY,
                          [cafeName] NVARCHAR(100) NOT NULL,
                          [stateName] NVARCHAR(15) NULL,
                          [email] NVARCHAR(80) NULL,
                          [phone] NVARCHAR(20) NULL
                          )";
                        break;
                    case "State":
                        strCommand = @"CREATE TABLE  IF NOT EXISTS [State] (
                          [code] NVARCHAR(3) NOT NULL PRIMARY KEY,
                          [state] NVARCHAR(20) NOT NULL
                          )";
                        break;
                    case "Templates":
                        strCommand = @"CREATE TABLE IF NOT EXISTS [Templates] (
                          [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                          [finger] BLOB NOT NULL,
                          [template] BLOB NOT NULL,
                          [quality] INTEGER NOT  NULL,
                          [reg_no] NVARCHAR(10) NOT NULL,
                          [schnum] NVARCHAR(7) NOT NULL,
                          [status] NVARCHAR(1) NOT NULL
                          )";
                        break;
                    case "TVerify":
                        strCommand = @"CREATE TABLE IF NOT EXISTS [TVerify] (
                          [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                          [schnum] NVARCHAR(7) NOT NULL,
                          [reg_no] NVARCHAR(10) NOT NULL,
                          [cand_name] NVARCHAR(45) NOT NULL,
                          [code] NVARCHAR(4) NOT NULL,
                          [subj_code] NVARCHAR(3) NOT NULL,
                          [subject] NVARCHAR(60) NOT NULL,
                          [paper] NVARCHAR(60) NOT NULL,
                          [Verified] NVARCHAR(1) NOT NULL,
                          [status] NVARCHAR(1) NOT NULL,
                          [remark] NVARCHAR(40) NOT NULL,
                          [verifyDate] NVARCHAR(12) NOT NULL
                          )";
                        break;
                    case "Subject":
                        strCommand = @"CREATE TABLE IF NOT EXISTS [Subject] (
                          [Code] NVARCHAR(4) NOT NULL PRIMARY KEY,
                          [subj_code] NVARCHAR(3) NOT NULL,
                          [Subject] NVARCHAR(3) NOT  NULL,
                          [Paper] NVARCHAR(25) NOT NULL,
                          [Descript] NVARCHAR(25) NOT NULL,
                          [Date] NVARCHAR(10) NOT NULL,
                          [StartTime] NVARCHAR(7) NOT NULL,
                          [StopTime] NVARCHAR(7) NOT NULL
                          )";
                        break;
                    case "Fin":
                        strCommand = @"CREATE TABLE IF NOT EXISTS [Fin] (
                          [SchoolNo] NVARCHAR(7) NOT NULL PRIMARY KEY,
                          [SchoolName] NVARCHAR(104) NOT  NULL,
                          [Captured] INT NOT NULL,
                          [Balance] INT NOT NULL
                          )";
                        break;
                    case "Temp":
                        strCommand = @"CREATE TABLE IF NOT EXISTS [Temp] (
                          ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                          [template] BLOB  NULL,
                          [quality] INTEGER NULL,
                          [reg_no] NVARCHAR(10) NOT NULL,
                          [schnum] NVARCHAR(7) NOT NULL
                          )";
                        break;
                    case "PersonalInfo":
                        strCommand = @"CREATE TABLE IF NOT EXISTS [PersonalInfo] (
                          [schnum] NVARCHAR(7) NOT NULL,
                          [reg_no] NVARCHAR(10) NOT NULL PRIMARY KEY,
                          [cand_name] NVARCHAR(45) NOT NULL,
                          [Sex] NVARCHAR(1) NOT NULL,
                          [HasPassport] BOOLEAN NOT NULL,
                          [Passport] BLOB NOT NULL,
                          [Subj1] NVARCHAR(3) NULL,
                          [Subj2] NVARCHAR(3) NULL,
                          [Subj3] NVARCHAR(3) NULL,
                          [Subj4] NVARCHAR(3) NULL,
                          [Subj5] NVARCHAR(3) NULL,
                          [Subj6] NVARCHAR(3) NULL,
                          [Subj7] NVARCHAR(3) NULL,
                          [Subj8] NVARCHAR(3) NULL,
                          [Subj9] NVARCHAR(3) NULL
                          
                          )";
                        break;
                }
                using (SQLiteCommand oleCommand = new SQLiteCommand(strCommand, dbConection))
                {
                    oleCommand.ExecuteNonQuery();
                }
            }
            catch( Exception e)
            {
               System.Windows.MessageBox.Show(e.Message, "Create Tables", MessageBoxButton.OK, MessageBoxImage.Error);
                DelSuccess = false;
            }
            finally
            {
                dbConection.Close();
                SQLiteConnection.ClearAllPools();
            }
        }
        #endregion

        #region Delete Records

        private void TruncateTable(string TableName)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            try
            {
                string strCommand = "DELETE  FROM " + TableName;
                SQLiteCommand oleCommand = new SQLiteCommand(strCommand, dbConection);
                oleCommand.ExecuteNonQuery();
                //return true;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Truncate Table");
            }
            finally
            {
                dbConection.Close();
                SQLiteConnection.ClearAllPools();
            }
            //return false;
        }
        public async Task<bool> DeleteTemplate(string SchoolNo, string CandidateNo)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (dbConection.State == ConnectionState.Closed)
                        dbConection.Open();

                    string strCommand = "DELETE FROM templates WHERE schnum=@SchoolNo AND reg_no=@CandidateNo";
                    SQLiteCommand oleCommand = new SQLiteCommand(strCommand, dbConection);
                    oleCommand.Parameters.Add(new SQLiteParameter("@SchoolNo", SchoolNo));
                    oleCommand.Parameters.Add(new SQLiteParameter("@CandidateNo", CandidateNo));
                    oleCommand.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Delete Templates");
                }
                finally
                {
                    dbConection.Close();
                    SQLiteConnection.ClearAllPools();
                }
                return false;
            });
        }

        private void DeletePersonalInfo(string SchoolNo)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();

            string strCommand = "DELETE FROM PersonalInfo WHERE schnum=@SchoolNo";
            SQLiteCommand oleCommand = new SQLiteCommand(strCommand, dbConection);
            oleCommand.Parameters.Add(new SQLiteParameter("@SchoolNo", SchoolNo));
            oleCommand.ExecuteNonQuery();
            
        }

        private void DeleteTempTemplates(string SchoolNo)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();

            string strCommand = "DELETE FROM Temp WHERE schnum=@SchoolNo";
            SQLiteCommand oleCommand = new SQLiteCommand(strCommand, dbConection);
            oleCommand.Parameters.Add(new SQLiteParameter("@SchoolNo", SchoolNo));
            oleCommand.ExecuteNonQuery();
            
        }
        #endregion

        #region Save Records

        public async Task<bool> UpdateUploadedRecords(List<TemplatesUploadResultModel> t)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task.Run(() =>
            {
                try
                {
                    using (var cmd = new SQLiteCommand(dbConection))
                    {
                        using (var transaction = dbConection.BeginTransaction())
                        {
                            foreach (var tm in t)
                            {
                                string sql = "UPDATE Templates SET status=@status WHERE reg_no=@RegNo";
                                cmd.CommandText = sql;
                                cmd.Parameters.Add(new SQLiteParameter("@RegNo", tm.regNo));
                                cmd.Parameters.Add(new SQLiteParameter("@status", "1"));
                                cmd.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                    }
                    return true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Update Records");
                }
                finally
                {
                    dbConection.Close();
                    SQLiteConnection.ClearAllPools();
                }
                return false;
            });
        }

        /// <summary>
        /// Inserts Candidates Finger print info into Temporary table
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        public async Task<bool> SaveDataToTemp(List<TemplatesModel> t)
        {
            
            return await Task.Run(() =>
            {
                DelSuccess = false;
                if (t != null)
                {
                    List<TemplatesModel> TemplateNotInDatabase = new List<TemplatesModel>();
                    DataTable ExistingTemplates = FetchTempTemplates(SchoolDetailsClass.SchoolNo);//get data of school already saved.

                    if (ExistingTemplates.Rows.Count > 0)
                    {
                        var ExistingTemplatesList = (from r in ExistingTemplates.AsEnumerable()
                                                     select new
                                                     {
                                                         schnum = r.Field<string>("schnum"),
                                                         reg_no = r.Field<string>("reg_no"),
                                                         quality = r.Field<long>("quality"),
                                                         template = r.Field<byte[]>("template"),
                                                     }).ToList();

                        //Fetch saved records from Temp Templates table and compare with the newly selected file
                        //if duplicates are found, remove them leaving only the new records
                        TemplateNotInDatabase = (from w1 in t
                                                     where !ExistingTemplatesList.Any(w2 => w1.reg_no == w2.reg_no)
                                                     select w1).ToList();
                    }else
                    {
                        TemplateNotInDatabase = t;
                    }
                       

                    


                   
                    
                    //if records exist continue
                    if (TemplateNotInDatabase.Count == 0)
                        return false;
                    
                    try
                    {
                        if (dbConection.State == ConnectionState.Closed)
                            dbConection.Open();

                        using (var cmd = new SQLiteCommand(dbConection))
                        {
                            using (var transaction = dbConection.BeginTransaction())
                            {
                                foreach (var tm in TemplateNotInDatabase)
                                {
                                    string sql = "INSERT INTO Temp(schnum, reg_no, quality, template) " +
                                        "VALUES(@schnum, @reg_no, @quality, @template)";
                                    cmd.CommandText = sql;
                                    cmd.Parameters.Add(new SQLiteParameter("@schnum", tm.schnum));
                                    cmd.Parameters.Add(new SQLiteParameter("@reg_no", tm.reg_no));
                                    cmd.Parameters.Add(new SQLiteParameter("@quality", tm.quality));
                                    cmd.Parameters.Add(new SQLiteParameter("@template", tm.template));
                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Save Data to Temp", MessageBoxButton.OK, MessageBoxImage.Error);
                        DelSuccess = false;
                    }
                }
                return DelSuccess;
            });
        }

        public async Task<bool> SaveDataToSubject(List<SubjectModel> s)
        {
            return await Task.Run(() =>
            {
                DelSuccess = false;
                if (s != null)
                {
                    try
                    {
                        if (dbConection.State == ConnectionState.Closed)
                            dbConection.Open();


                        using (var cmd = new SQLiteCommand(dbConection))
                        {
                            using (var transaction = dbConection.BeginTransaction())
                            {
                                foreach (var tm in s)
                                {
                                    string sql = "INSERT INTO Subject(code, subj_code, subject, paper, Descript, Date, StartTime, StopTime) " +
                                        "VALUES(@code, @subj_code, @subject, @paper, @Descript, @Date, @StartTime, @StopTime)";
                                    cmd.CommandText = sql;
                                    cmd.Parameters.Add(new SQLiteParameter("@code", tm.Code));
                                    cmd.Parameters.Add(new SQLiteParameter("@subj_code", tm.subj_code));
                                    cmd.Parameters.Add(new SQLiteParameter("@subject", tm.Subject));
                                    cmd.Parameters.Add(new SQLiteParameter("@paper", tm.Paper));
                                    cmd.Parameters.Add(new SQLiteParameter("@Descript", tm.Descript));
                                    cmd.Parameters.Add(new SQLiteParameter("@Date", tm.Date));
                                    cmd.Parameters.Add(new SQLiteParameter("@StartTime", tm.StartTime));
                                    cmd.Parameters.Add(new SQLiteParameter("@StopTime", tm.StopTime));
                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                        }
                        DelSuccess = true;
                    }
                    catch (Exception e)
                    {
                        SafeGuiWpf.ShowError("Save Data to Subject\n" + e.Message);//,, MessageBoxButton.OK, MessageBoxImage.Error);
                        DelSuccess = false;
                    }
                }
                return DelSuccess;
            });
           
        }


        public async Task<bool> SaveDataToStaff(List<StaffModel> s)
        {
            return await Task.Run(() =>
            {
                DelSuccess = false;
                if (s != null)
                {

                    try
                    {
                        if (dbConection.State == ConnectionState.Closed)
                            dbConection.Open();



                        using (var cmd = new SQLiteCommand(dbConection))
                        {
                            using (var transaction = dbConection.BeginTransaction())
                            {
                                foreach (var tm in s)
                                {
                                    string sql = "INSERT INTO Staff(per_no, name) " +
                                        "VALUES(@per_no, @name)";
                                    cmd.CommandText = sql;
                                    cmd.Parameters.Add(new SQLiteParameter("@per_no", tm.per_no));
                                    cmd.Parameters.Add(new SQLiteParameter("@name", tm.name));

                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                        }
                        DelSuccess = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Save Data to Staff", MessageBoxButton.OK, MessageBoxImage.Error);
                        DelSuccess = false;
                    }
                }
                return DelSuccess;
            });
           
        }


        public async Task<bool> SaveDataToBank(List<BankSaveModel> s)
        {
            return await Task.Run(() =>
            {
                DelSuccess = false;
                if (s != null)
                {
                    try
                    {
                        if (dbConection.State == ConnectionState.Closed)
                            dbConection.Open();

                        using (var cmd = new SQLiteCommand(dbConection))
                        {
                            using (var transaction = dbConection.BeginTransaction())
                            {
                                foreach (var tm in s)
                                {
                                    string sql = "INSERT INTO Bank(bank_code, bank_name) " +
                                        "VALUES(@bank_code, @bank_name)";
                                    cmd.CommandText = sql;
                                    cmd.Parameters.Add(new SQLiteParameter("@bank_code", tm.bank_code));
                                    cmd.Parameters.Add(new SQLiteParameter("@bank_name", tm.bank_name));

                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                        }
                        DelSuccess = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Save Data to Bank", MessageBoxButton.OK, MessageBoxImage.Error);
                        DelSuccess = false;
                    }
                }
                return DelSuccess;
            });

        }

        public async Task<bool> SaveDataToTotalUploadSummaryTable(List<TotalSummryApiModel> s)
        {
            return await Task.Run(() =>
            {
                DelSuccess = false;
                if (s != null)
                {
                    try
                    {
                        if (dbConection.State == ConnectionState.Closed)
                            dbConection.Open();

                        using (var cmd = new SQLiteCommand(dbConection))
                        {
                            using (var transaction = dbConection.BeginTransaction())
                            {
                                foreach (var tm in s)
                                {
                                    string sql = "INSERT OR REPLACE INTO summaryTable(operatorId, cafe, total) " +
                                        "VALUES(@operatorId, @cafe, @total)";
                                    cmd.CommandText = sql;
                                    cmd.Parameters.Add(new SQLiteParameter("@operatorId", tm.operatorId));
                                    cmd.Parameters.Add(new SQLiteParameter("@cafe", tm.cafe));
                                    cmd.Parameters.Add(new SQLiteParameter("@total", tm.total));
                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                        }
                        DelSuccess = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Save Data to Bank", MessageBoxButton.OK, MessageBoxImage.Error);
                        DelSuccess = false;
                    }
                }
                return DelSuccess;
            });

        }

        public async Task<bool> SaveDataToSchoolSummaryTable(List<SchoolSummrySaveModel> s)
        {
            return await Task.Run(() =>
            {
                DelSuccess = false;
                if (s != null)
                {
                    try
                    {
                        if (dbConection.State == ConnectionState.Closed)
                            dbConection.Open();

                        using (var cmd = new SQLiteCommand(dbConection))
                        {
                            using (var transaction = dbConection.BeginTransaction())
                            {
                                foreach (var tm in s)
                                {
                                    string sql = "INSERT OR REPLACE INTO schoolSummaryTable(schoolNo, operatorid, schoolName, total) " +
                                        "VALUES(@schoolNo, @operatorId, @schoolName, @total)";
                                    cmd.CommandText = sql;
                                    cmd.Parameters.Add(new SQLiteParameter("@schoolNo", tm.schoolNo));
                                    cmd.Parameters.Add(new SQLiteParameter("@operatorId", tm.operatorId));
                                    cmd.Parameters.Add(new SQLiteParameter("@schoolName", tm.schoolName));
                                    cmd.Parameters.Add(new SQLiteParameter("@total", tm.total));
                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                        }
                        DelSuccess = true;
                    }
                    catch (Exception e)
                    {
                        SafeGuiWpf.ShowError("Save Data to School Summary\n"+e.Message);//, , MessageBoxButton.OK, MessageBoxImage.Error);
                        DelSuccess = false;
                    }
                }
                return DelSuccess;
            });

        }
        public async Task<bool> SaveDataToOperatorTemp(List<OnlineOperatorListApiModel> s)
        {
            return await Task.Run(() =>
            {
                DelSuccess = false;
                if (s != null)
                {
                    try
                    {
                        if (dbConection.State == ConnectionState.Closed)
                            dbConection.Open();

                        using (var cmd = new SQLiteCommand(dbConection))
                        {
                            using (var transaction = dbConection.BeginTransaction())
                            {
                                foreach (var tm in s)
                                {
                                    string sql = "INSERT OR IGNORE INTO OperatorTemp(operatorId, cafeName, stateName, email, phone) " +
                                        "VALUES(@operatorId, @cafeName,@stateName, @email, @phone)";
                                    cmd.CommandText = sql;
                                    cmd.Parameters.Add(new SQLiteParameter("@operatorId", tm.operatorId));
                                    cmd.Parameters.Add(new SQLiteParameter("@cafeName", tm.cafeName));
                                    cmd.Parameters.Add(new SQLiteParameter("@stateName", ""));
                                    cmd.Parameters.Add(new SQLiteParameter("@email", ""));
                                    cmd.Parameters.Add(new SQLiteParameter("@phone", ""));
                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                        }
                        DelSuccess = true;
                    }
                    catch (Exception e)
                    {
                        SafeGuiWpf.ShowError(e.Message);//, "Save Data to Operator", MessageBoxButton.OK, MessageBoxImage.Error);
                        DelSuccess = false;
                    }
                }
                return DelSuccess;
            });

        }

        public async Task<bool> SaveDataToState(List<StateModel> s)
        {
            return await Task.Run(() =>
            {
                DelSuccess = false;
                if (s != null)
                {

                    try
                    {
                        if (dbConection.State == ConnectionState.Closed)
                            dbConection.Open();



                        using (var cmd = new SQLiteCommand(dbConection))
                        {
                            using (var transaction = dbConection.BeginTransaction())
                            {
                                foreach (var tm in s)
                                {
                                    string sql = "INSERT INTO State(code, state) " +
                                        "VALUES(@code, @state)";
                                    cmd.CommandText = sql;
                                    cmd.Parameters.Add(new SQLiteParameter("@code", tm.code));
                                    cmd.Parameters.Add(new SQLiteParameter("@state", tm.state));

                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                        }
                        DelSuccess = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Save Data to State", MessageBoxButton.OK, MessageBoxImage.Error);
                        DelSuccess = false;
                    }
                }
                return DelSuccess;
            });

        }

        /* public DataTable ToDataTable<T>(List<T> items)
         {
             DataTable dataTable = new DataTable(typeof(T).Name);
             //Get all the properties
             PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
             foreach (PropertyInfo prop in Props)
             {
                 //Setting column names as Property names
                 dataTable.Columns.Add(prop.Name);
             }

             foreach (T item in items)
             {
                 var values = new object[Props.Length];
                 for (int i = 0; i < Props.Length; i++)
                 {
                     //inserting property values to datatable rows
                     values[i] = Props[i].GetValue(item, null);
                 }

                 dataTable.Rows.Add(values);
             }

             //put a breakpoint here and check datatable
             return dataTable;

         }*/

        /// <summary>
        /// Saves Centre Json Data to Personal Info Table
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        public async Task<bool> SaveDataToPersonalInfo(List<PersonalInfoModel> p)
        {
            return await Task.Run(() =>
            {
                DelSuccess = false;
                if (p != null && p.Count > 0)//if model is valid, continue
                {
                   
                    try
                    {

                       // DataTable PersonalInfo = FetchPersonalInfoEx(SchoolNo);//get data of school already saved.


                      /*  var PersonalInfoList = (from r in PersonalInfo.AsEnumerable()
                                                select new
                                                {
                                                    schnum = r.Field<string>("schnum"),
                                                    reg_no = r.Field<string>("reg_no"),
                                                    cand_name = r.Field<string>("cand_name"),
                                                    sex = r.Field<string>("sex"),
                                                    HasPassport=r.Field<bool>("HasPassport"),
                                                    passport = r.Field<byte[]>("passport"),

                                                    subj1 = r.Field<string>("subj1"),
                                                    subj2 = r.Field<string>("subj2"),
                                                    subj3 = r.Field<string>("subj3"),
                                                    subj4 = r.Field<string>("subj4"),
                                                    subj5 = r.Field<string>("subj5"),
                                                    subj6 = r.Field<string>("subj6"),
                                                    subj7 = r.Field<string>("subj7"),
                                                    subj8 = r.Field<string>("subj8"),
                                                    subj9 = r.Field<string>("subj9"),
                                                }).ToList();*/


                        //Fetch saved records from Temp Templates table and compare with the newly selected file
                        //if duplicates are found, remove them leaving only the new records
                       /* var PersonalInfoNotInDatabase = (from w1 in p
                                                         where !PersonalInfoList.Any(w2 => w1.reg_no == w2.reg_no)
                                                         select w1).ToList();*/


                        //continue to update only if records are in the new datatable.
                       /* if (PersonalInfoNotInDatabase.Count == 0)
                            return false;*/

                        if (dbConection.State == ConnectionState.Closed)//if connection is closed, open it.
                            dbConection.Open();


                        using (var cmd = new SQLiteCommand(dbConection))
                        {
                            using (var transaction = dbConection.BeginTransaction())
                            {
                                foreach (var tm in p)
                                {
                                    string sql = "INSERT OR REPLACE INTO PersonalInfo(schnum, reg_no, cand_name, sex, HasPassport, passport, " +
                                                 "subj1, subj2, subj3, subj4, subj5, subj6, subj7, subj8, subj9) " +
                                        "VALUES(@schnum, @reg_no, @cand_name, @sex, @HasPassport, @passport, " +
                                        "@subj1, @subj2, @subj3, @subj4, @subj5, @subj6, @subj7, @subj8, @subj9)";
                                    cmd.CommandText = sql;
                                    cmd.Parameters.Add(new SQLiteParameter("@schnum", tm.schnum));
                                    cmd.Parameters.Add(new SQLiteParameter("@reg_no", tm.reg_no));
                                    cmd.Parameters.Add(new SQLiteParameter("@cand_name", tm.cand_name));
                                    cmd.Parameters.Add(new SQLiteParameter("@sex", tm.sex));
                                    cmd.Parameters.Add(new SQLiteParameter("@HasPassport", tm.HasPassport));
                                    cmd.Parameters.Add(new SQLiteParameter("@passport", tm.passport));


                                    cmd.Parameters.Add(new SQLiteParameter("@subj1", tm.subj1));
                                    cmd.Parameters.Add(new SQLiteParameter("@subj2", tm.subj2));
                                    cmd.Parameters.Add(new SQLiteParameter("@subj3", tm.subj3));
                                    cmd.Parameters.Add(new SQLiteParameter("@subj4", tm.subj4));
                                    cmd.Parameters.Add(new SQLiteParameter("@subj5", tm.subj5));
                                    cmd.Parameters.Add(new SQLiteParameter("@subj6", tm.subj6));
                                    cmd.Parameters.Add(new SQLiteParameter("@subj7", tm.subj7));
                                    cmd.Parameters.Add(new SQLiteParameter("@subj8", tm.subj8));
                                    cmd.Parameters.Add(new SQLiteParameter("@subj9", tm.subj9));
                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            }
                            DelSuccess = true;
                        }
                        
                    }
                    catch (Exception e)
                    {
                       SafeGuiWpf.ShowError(e.Message);
                        DelSuccess = false;
                    }
                }
                return DelSuccess;
            });
            
        }

        public async Task<bool> IsCandidateVerified(VerifyModel v)
        {
            try
            {
                if (dbConection.State == ConnectionState.Closed)
                    dbConection.Open();

               return await Task.Run(() =>
                {
                    string sql = "SELECT reg_no FROM tverify WHERE reg_no=@CandidateNo AND code=@Code";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@CandidateNo", v.reg_no));
                        cmd.Parameters.Add(new SQLiteParameter("@Code", v.code));
                        object result = cmd.ExecuteScalar();
                        return result != null;
                    }
                });
               

            }
            catch (Exception e)
            {
                SafeGuiWpf.ShowError(e.Message);
            }
            finally
            {
                dbConection.Close();
                SQLiteConnection.ClearAllPools();
            }
            return true;
        }

        /// <summary>
        /// SAve verified candidate staus
        /// </summary>
        /// <param name="v"></param>
        public async Task<bool> SaveVerify(VerifyModel v)
        {
            try
            {
                if (dbConection.State == ConnectionState.Closed)
                    dbConection.Open();
                return await Task.Run(() =>
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(dbConection))
                    {
                        using (var transaction = dbConection.BeginTransaction())
                        {
                            cmd.CommandText = "INSERT INTO tverify(schnum, reg_no, cand_name, code, subj_code, subject, paper, Verified, status, remark, verifyDate)" +
                                    " VALUES (@schnum, @reg_no, @cand_name, @code, @subj_code, @subject, @paper, @verified, @status, @remark, @verifyDate)";

                            cmd.Parameters.Add(new SQLiteParameter("@schnum", v.schnum));
                            cmd.Parameters.Add(new SQLiteParameter("@reg_no", v.reg_no));
                            cmd.Parameters.Add(new SQLiteParameter("@cand_name", v.cand_name));
                            cmd.Parameters.Add(new SQLiteParameter("@code", v.code));
                            cmd.Parameters.Add(new SQLiteParameter("@subj_code", v.subj_code));
                            cmd.Parameters.Add(new SQLiteParameter("@subject", v.subject));
                            cmd.Parameters.Add(new SQLiteParameter("@paper", v.paper));
                            cmd.Parameters.Add(new SQLiteParameter("@Verified", v.Verified));
                            cmd.Parameters.Add(new SQLiteParameter("@status", v.status));
                            cmd.Parameters.Add(new SQLiteParameter("@remark", v.remark));
                            cmd.Parameters.Add(new SQLiteParameter("@verifyDate", v.verifyDate));
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            return true;
                        }

                    }
                });
                
               

            }
            catch(Exception e)
            {
                SafeGuiWpf.ShowError(e.Message);
            }
            finally
            {
                dbConection.Close();
                SQLiteConnection.ClearAllPools();
            }
            return false;
        }

        /// <summary>
        /// Save individual none duplicate templates and finger images
        /// </summary>
        /// <param name="m"></param>
        public async Task<bool> SaveTemplate(TemplatesModel m)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (dbConection.State == ConnectionState.Closed)
                        dbConection.Open();


                    using (dbConection)
                    {

                        string strCommand = "INSERT INTO templates(schnum, reg_no, finger, template, quality, status) " +
                                            "VALUES (@schnum, @reg_no, @finger, @template, @quality, @status)";
                        SQLiteCommand oleCommand = new SQLiteCommand(strCommand, dbConection);

                        // oleCommand.Parameters.Add(new SqlCeParameter("@cafe", m.cafe));
                        oleCommand.Parameters.Add(new SQLiteParameter("@finger", m.finger));
                        oleCommand.Parameters.Add(new SQLiteParameter("@template", m.template));
                        oleCommand.Parameters.Add(new SQLiteParameter("@quality", m.quality));
                        oleCommand.Parameters.Add(new SQLiteParameter("@reg_no", m.reg_no));
                        oleCommand.Parameters.Add(new SQLiteParameter("@schnum", m.schnum));
                        oleCommand.Parameters.Add(new SQLiteParameter("@status", "0"));

                        oleCommand.ExecuteNonQuery();

                    }
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Save Template");
                }
                finally
                {
                    dbConection.Close();
                    SQLiteConnection.ClearAllPools();
                }
                return false;
            });
           
        }

        private bool IsSchoolExist(string schnum)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            var dt = new DataTable();
            string sql = "SELECT * FROM fin WHERE SchoolNo=@schnum";
            using (SQLiteDataAdapter da = new SQLiteDataAdapter())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@schnum", schnum));
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                    
            }
            return (dt.Rows.Count > 0) ? true : false;
        }

        public async Task<bool> SaveDataToFin(FinModel m)
        {
            return await Task.Run(() =>
            {
                if (IsSchoolExist(m.SchoolNo) || string.IsNullOrWhiteSpace(m.SchoolNo))
                    return false;
                try
                {
                    if (dbConection.State == ConnectionState.Closed)
                        dbConection.Open();

                    using (var cmd = new SQLiteCommand(dbConection))
                    {
                        using (var transaction = dbConection.BeginTransaction())
                        {
                            string strCommand = "INSERT OR REPLACE INTO Fin(SchoolNo, SchoolName, Captured, Balance)" +
                                     " VALUES (@SchoolNo, @SchoolName, @Captured, @Balance)";
                            SQLiteCommand oleCommand = new SQLiteCommand(strCommand, dbConection);

                            oleCommand.Parameters.Add(new SQLiteParameter("@SchoolNo", m.SchoolNo));
                            oleCommand.Parameters.Add(new SQLiteParameter("@SchoolName", m.SchoolName));
                            oleCommand.Parameters.Add(new SQLiteParameter("@Captured", m.Captured));
                            oleCommand.Parameters.Add(new SQLiteParameter("@Balance", m.Balance));

                            oleCommand.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    SafeGuiWpf.ShowError(e.Message);
                }
                return false;
            });
            
        }
        #endregion

        #region Fetch Records Region

        public DataTable FetchTemplatesToVerify(string schnum)
        {
            DataTable Templates = FetchTemplates(schnum);
            DataTable TempTemplates = FetchTempTemplates(schnum);
            DataTable TemplatesToVerify = new DataTable();
            try
            {
                if(Templates.Rows.Count>0 && TempTemplates.Rows.Count>0)
                {
                    TemplatesToVerify = (from r in Templates.AsEnumerable()
                                                   where TempTemplates.AsEnumerable().Any(r2 => r["schnum"].ToString().Trim() == r2["schnum"].ToString().Trim())
                                                   select r).CopyToDataTable();
                    return TemplatesToVerify;
                }else if(Templates.Rows.Count>0 && TempTemplates.Rows.Count==0)
                {
                    return Templates;
                }else if(Templates.Rows.Count == 0 && TempTemplates.Rows.Count > 0)
                {
                    return TempTemplates;
                }

               
            }
            catch (Exception e)
            {
              MessageBox.Show(e.Message, "Fetch Templates to Verify", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return Templates;
        }

        public DataTable FetchRecords(string schnum)
        {
           return FetchPersonalInfo(schnum);
        }

        public async Task<int> CountTotalRecords(string schnum)
        {
            return await Task.Run(() =>
            {
                DataTable PersonalInfo = FetchPersonalInfo(schnum);
                DataTable TempTemplates = FetchTempTemplates(schnum);
                if (TempTemplates.Rows.Count > 0)
                {
                    DataTable UnMatchedRecords = (from r in PersonalInfo.AsEnumerable()
                                                  where !TempTemplates.AsEnumerable().Any(r2 => r["reg_no"].ToString().Trim() == r2["reg_no"].ToString().Trim())
                                                  select r).CopyToDataTable();
                    return UnMatchedRecords.Rows.Count;
                }

                return PersonalInfo.Rows.Count;
            });
           
        }


        public async Task<DataTable> FetchRecordsForStat(string schnum)
        {
            return await Task.Run(() =>
            {
                DataTable PersonalInfo = FetchPersonalInfo(schnum);
                DataTable TempTemplates = FetchTempTemplates(schnum);
                if (TempTemplates.Rows.Count > 0)
                {
                    DataTable UnMatchedRecords = (from r in PersonalInfo.AsEnumerable()
                                                  where !TempTemplates.AsEnumerable().Any(r2 => r["reg_no"].ToString().Trim() == r2["reg_no"].ToString().Trim())
                                                  select r).CopyToDataTable();
                    return UnMatchedRecords;
                }

                return PersonalInfo;
            });
           
        }



       /* public async Task<List<PersonalInfoModel>> FetchRecordsToEnroll(string schnum, string Search)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task<List<PersonalInfoModel>>.Run(() =>
            {
                string sql = string.IsNullOrWhiteSpace(Search) ? "SELECT * FROM PersonalInfo WHERE SchoolNo=@schnum ORDER BY reg_no"
                                                               : "SELECT * FROM PersonalInfo WHERE SchoolNo = @schnum AND(reg_no LIKE @Search||'%' OR surname LIKE @Search||'%' OR firstname LIKE @Search||'%' OR middlename LIKE @Search||'%'";


            });
        }*/

        /// <summary>
        /// Fetch Data to Display on Enrollment Page
        /// Only Candidates without Templates Info are Displayed
        /// </summary>
        /// <param name="schnum"></param>
        /// <returns></returns>
        public async Task<DataTable> FetchRecords(string schnum, string CandidateName, bool WithSearch)
        {
            DataTable TPersonalInfo = new DataTable();
            if (string.IsNullOrWhiteSpace(CandidateName))
                TPersonalInfo = FetchPersonalInfo(schnum);
            else
                TPersonalInfo = await FetchPersonalInfo(schnum, CandidateName, WithSearch);

            DataTable Templates = FetchTemplates(schnum);
            DataTable TempTemplates = FetchTempTemplates(schnum);

            if (TPersonalInfo.Rows.Count == 0)
                return null;

            try
            {
                //Get the difference of two datatables
                DataTable UnMatchedRecords = (from r in TPersonalInfo.AsEnumerable()
                                              where !Templates.AsEnumerable().Any(r2 => r.Field<string>("reg_no") == r2.Field<string>("reg_no"))
                                              select r).CopyToDataTable() as DataTable;
                 var NoTemplates = new DataTable();

               /* var DistinctTemplates = TempTemplates.AsEnumerable().Select(s =>
                                   new { reg_no = s.Field<string>("reg_no") }).Distinct().ToList();*/



                var rows = (from x in UnMatchedRecords.AsEnumerable()
                                   where !TempTemplates.AsEnumerable().Any(x2 => x.Field<string>("reg_no") == x2.Field<string>("reg_no"))
                                   select x);

                if(rows.Any())
                {
                    NoTemplates = rows.CopyToDataTable();
                }
                return NoTemplates;
            }
            catch (Exception e)
            {
                SafeGuiWpf.ShowError(e.Message);// + " " + e.StackTrace, "Fetch Template", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

      

        public async Task<DataTable> FetchFinInfo(string schnum="")
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task.Run(() =>
            {
                var dt = new DataTable();
                string sql =string.IsNullOrWhiteSpace(schnum)? "SELECT * FROM fin ORDER BY SchoolNo"
                                                                : "SELECT * FROM fin WHERE SchoolNo LIKE @schnum||'%' OR  SchoolName LIKE @schnum||'%' ORDER BY SchoolNo";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@schnum", schnum));
                        da.SelectCommand = cmd;
                        da.Fill(dt);
                    }
                        
                }
                return dt;
            });
           
        }

        public async Task<List<OnlineOperatorListApiModel>> FetchOperatorsInfo(string operatorId = "")
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task.Run(() =>
            {
                var lst = new List<OnlineOperatorListApiModel>();
                string sql = string.IsNullOrWhiteSpace(operatorId) ? "SELECT * FROM OperatorTemp ORDER BY operatorId"
                                                                : "SELECT * FROM OperatorTemp WHERE operatorId LIKE @operatorId||'%' OR  cafeName LIKE @operatorId||'%' ORDER BY operatorId";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    SQLiteDataReader dr;
                    cmd.Parameters.Add(new SQLiteParameter("@operatorId", operatorId));
                    dr = cmd.ExecuteReader();
                    if(dr.HasRows)
                    {
                        while(dr.Read())
                        {
                            lst.Add(new OnlineOperatorListApiModel
                            {
                                operatorId = dr["operatorId"].ToString(),
                                cafeName=dr["cafeName"].ToString(),
                            });
                        }
                    }
                }
                return lst;
            });

        }

        public async Task<List<TotalSummryApiModel>> FetchTotalSummaryValue(string operatorId = "")
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task.Run(() =>
            {
                var lst = new List<TotalSummryApiModel>();
                string sql = string.IsNullOrWhiteSpace(operatorId) ? "SELECT * FROM summaryTable ORDER BY operatorid"
                                                                : "SELECT * FROM summaryTable WHERE operatorid LIKE @operatorid||'%' OR  cafe LIKE @operatorid||'%' ORDER BY operatorId";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    SQLiteDataReader dr;
                    cmd.Parameters.Add(new SQLiteParameter("@operatorId", operatorId));
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            lst.Add(new TotalSummryApiModel
                            {
                                operatorId = dr["operatorid"].ToString(),
                                cafe = dr["cafe"].ToString(),
                                total= Convert.ToInt64(dr["total"])
                            });
                        }
                    }
                }
                return lst;
            });
        }


        public Task<DataTable> FetchBanks()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return Task.Run(() =>
            {
                var dt = new DataTable();
                string sql = "SELECT * FROM bank ORDER BY bank_code";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, dbConection))
                {
                    da.Fill(dt);
                }
                return dt;
            });
        }

        public async Task<List<VerifyViewModel>> FetchVerifiedRecords()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task<List<VerifyViewModel>>.Run(() =>
            {
                var model = new List<VerifyViewModel>();
                try
                {
                    string sql = "SELECT * FROM TVerify ORDER BY subj_code, schnum";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        SQLiteDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            int SN = 0;
                            while (dr.Read())
                            {
                                ++SN;
                                model.Add(new VerifyViewModel
                                {
                                    S_No=SN,
                                    schnum = dr["schnum"].ToString(),
                                    reg_no = dr["reg_no"].ToString(),
                                    cand_name = dr["cand_name"].ToString(),
                                    subject = dr["subject"].ToString(),
                                    paper = dr["paper"].ToString(),
                                    Verified = dr["Verified"].ToString(),
                                    code=dr["code"].ToString(),
                                    subj_code=dr["subj_code"].ToString(),
                                    remark=dr["remark"].ToString()
                                });
                            }
                            return model;
                        }
                    }

                }
                catch (Exception e)
                {
                    SafeGuiWpf.ShowError(e.Message);
                }
                finally
                {
                    dbConection.Close();
                    SQLiteConnection.ClearAllPools();
                }
                return null;
            });

        }
        public async Task<List<SubjectModel>> FetchSubjectsToVerify()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();

           return await Task<List<SubjectModel>>.Run(() =>
            {
                var subj = new List<SubjectModel>();
                try
                {
                    string VerifyDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string sql = "SELECT * FROM Subject WHERE date=@VerifyDate";
                    //string sql = "SELECT * FROM Subject";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@VerifyDate", VerifyDate));
                        SQLiteDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                           while(dr.Read())
                            {
                                subj.Add(new SubjectModel
                                {
                                    Code = dr["code"].ToString(),
                                    subj_code = dr["subj_code"].ToString(),
                                    Subject = dr["subject"].ToString(),
                                    Paper = dr["paper"].ToString(),
                                    Descript = string.Format("{0} [{1}]", dr["descript"].ToString().Trim(), dr["paper"].ToString().Trim())
                                });
                            }
                            return subj;
                        }
                    }

                    
                }
                catch (Exception e)
                {
                    SafeGuiWpf.ShowError(e.Message);
                }
                finally
                {
                    dbConection.Close();
                    SQLiteConnection.ClearAllPools();
                }
                return null;
            });
           
        }
        public string FetchSubjectDescription(string subj_code)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();

            string sql = "SELECT Descript FROM Subject WHERE subj_code=@subj_code";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
            {
                cmd.Parameters.Add(new SQLiteParameter("@subj_code", subj_code));
               return cmd.ExecuteScalar().ToString();
            }
            //return string.Empty;
        }


        public async Task<List<SubjectViewModel>> FetchSubjects()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task.Run(() =>
            {
                var subjList = new List<SubjectViewModel>();
                string sql = "SELECT DISTINCT subj_code,Descript FROM Subject";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    SQLiteDataReader dr;
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            subjList.Add(new SubjectViewModel
                            {
                                //Code = dr["code"].ToString(),
                                subj_code = dr["subj_code"].ToString(),
                                Descript = dr["Descript"].ToString().Trim(),
                                //Subject = dr["Subject"].ToString()
                            });
                        }
                    }

                }
                return subjList;
            });
           
        }

        public async Task<List<StateModel>> FetchStates()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task.Run(() =>
            {
                var stateList = new List<StateModel>();
                string sql = "SELECT code,state FROM State";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    SQLiteDataReader dr;
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            stateList.Add(new StateModel
                            {
                                //Code = dr["code"].ToString(),
                                code = dr["code"].ToString(),
                                state = dr["state"].ToString().Trim(),
                                //Subject = dr["Subject"].ToString()
                            });
                        }
                    }

                }
                return stateList;
            });

        }

        public DataTable FetchPersonalInfo(string Schnum)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            var dt = new DataTable();
            string sql = "SELECT * FROM PersonalInfo WHERE schnum=@Schnum";
            using (SQLiteDataAdapter da = new SQLiteDataAdapter())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                   
            }
            return dt;
        }

        //PersonalInfoViewModel

        public async Task<PersonalInfoViewModel> FetchPersonalInfoToPreview(string CandidateName)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task<PersonalInfoViewModel>.Run(() =>
            {
                try
                {
                    string sql = (!string.IsNullOrWhiteSpace(CandidateName)) ? "SELECT * FROM PersonalInfo WHERE  reg_no = @CandidateName"
                            : "SELECT * FROM PersonalInfo";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@CandidateName", CandidateName));
                        SQLiteDataReader dr = cmd.ExecuteReader();
                        if(dr.HasRows)
                        {
                            while(dr.Read())
                            {
                                return new PersonalInfoViewModel
                                {
                                    schnum=dr["schnum"].ToString(),
                                    cand_name=dr["cand_name"].ToString(),
                                    sex=dr["sex"].ToString(),
                                    passport=((byte[])dr["passport"]),
                                    subj1=dr["subj1"].ToString(),
                                    subj2=dr["subj2"].ToString(),
                                    subj3=dr["subj3"].ToString(),
                                    subj4=dr["subj4"].ToString(),
                                    subj5=dr["subj5"].ToString(),
                                    subj6=dr["subj6"].ToString(),
                                    subj7=dr["subj7"].ToString(),
                                    subj8=dr["subj8"].ToString(),
                                    subj9=dr["subj9"].ToString(),
                                    reg_no=dr["reg_no"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    SafeGuiWpf.ShowError(e.Message);
                }
                finally
                {
                    dbConection.Close();
                    SQLiteConnection.ClearAllPools();
                }
                return null;
            });

        }


        public async Task<DataTable> FetchPersonalInfo(string Schnum, string CandidateName, bool WithSearch)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task.Run(() =>
            {
                var dt = new DataTable();
                try
                {
                    string sql = (WithSearch) ? "SELECT * FROM PersonalInfo WHERE  reg_no LIKE @CandidateName||'%' OR cand_name LIKE @CandidateName||'%' AND schnum=@Schnum"
                            : "SELECT * FROM PersonalInfo WHERE schnum=@Schnum";

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                        {
                            cmd.Parameters.Add(new SQLiteParameter("@CandidateName", CandidateName));
                            cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                            da.SelectCommand = cmd;
                            da.Fill(dt);
                        }

                    }
                    return dt;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Fetch PersonalInfo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return null;
            });
           
        }

        public async Task<int> FetchPersonalInfoCount(string Schnum)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();

            return await Task.Run(() =>
            {
                var dt = new DataTable();
                string sql = "SELECT * FROM Personalinfo WHERE schnum=@Schnum";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                        da.SelectCommand = cmd;
                        da.Fill(dt);
                    }
                }

                DataTable TempTemplates = FetchTempTemplates(Schnum);
                // var TempCount = TempTemplates.AsEnumerable().Select(s =>
                //                    new { reg_no = s.Field<string>("reg_no"), }).Distinct().ToList();

                if (dt.Rows.Count == TempTemplates.AsEnumerable().Select(s =>
                                     new { reg_no = s.Field<string>("reg_no") }).Distinct().ToList().Count)
                    return 0;

                DataTable UnMatchedRecords = (from r in dt.AsEnumerable()
                                              where !TempTemplates.AsEnumerable().Any(r2 => r["reg_no"].ToString().Trim() == r2["reg_no"].ToString().Trim())
                                              select r).CopyToDataTable();

                var distinctIds = UnMatchedRecords.AsEnumerable().Select(s =>
                                      new { reg_no = s.Field<string>("reg_no"), }).Distinct().ToList();
                int NumberOfRecords = distinctIds.Count;

                return NumberOfRecords;
            });
           
        }

        public async Task<int> FetchPersonalInfoCount()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();

            return await Task.Run(() =>
            {
                var dt = new DataTable();
                string sql = "SELECT * FROM Personalinfo ORDER BY schnum";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        //cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                        da.SelectCommand = cmd;
                        da.Fill(dt);
                    }
                }



                return dt.Rows.Count;
            });

        }

        public async Task<int> FetchTemplatesCount(string Schnum)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task.Run(() =>
            {
                var dt = new DataTable();
                string sql = "SELECT * FROM Templates WHERE schnum=@Schnum";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                        da.SelectCommand = cmd;
                        da.Fill(dt);
                    }
                }
                var distinctIds = dt.AsEnumerable().Select(s =>
                                      new { reg_no = s.Field<string>("reg_no"), }).Distinct().ToList();
                int NumberOfRecords = distinctIds.Count;

                return NumberOfRecords;
            });

           
        }



        public async Task<int> FetchTemplatesCount()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task.Run(() =>
            {
                var dt = new DataTable();
                string sql = "SELECT * FROM Templates ORDER BY schnum";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        //cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                        da.SelectCommand = cmd;
                        da.Fill(dt);
                    }
                }
                var distinctIds = dt.AsEnumerable().Select(s =>
                                      new { reg_no = s.Field<string>("reg_no"), }).Distinct().ToList();
                int NumberOfRecords = distinctIds.Count;

                return NumberOfRecords;
            });


        }


        public List<string> FetchTemplatesForFingerCount(string Schnum)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            var TemplatesList = new List<string>();
            var dt = new DataTable();
            string sql = "SELECT reg_no FROM Templates WHERE schnum=@Schnum";
            //using (SQLiteDataAdapter da = new SQLiteDataAdapter())
            //{
            using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
            {
                cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                SQLiteDataReader dr;
                dr = cmd.ExecuteReader();
                if(dr.HasRows)
                {
                    while (dr.Read())
                        TemplatesList.Add(dr["reg_no"].ToString());
                }
            }

            //}
            return TemplatesList;
        }

        public DataTable FetchTemplates(string Schnum)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            var dt = new DataTable();
            string sql = "SELECT * FROM Templates WHERE schnum=@Schnum";
            using (SQLiteDataAdapter da = new SQLiteDataAdapter())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                    
            }
            return dt;
        }

       /* public DataTable FetchTemplatesForUpload(int schnum, int batch)
        {
            try
            {

                IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                DataTable PersonalInfo = dl.PersonalInfo();
                DataTable Templates =  dl.Templates();

                var distinctIds = PersonalInfo.AsEnumerable().Select(p =>
                              new { serialNumber = p.Field<string>("ser_no"), }).Distinct().ToList();
                int NumberOfPersonalInfoRecords = distinctIds.Count;

                var distinctTIds = Templates.AsEnumerable().Select(t =>
                              new { serialNumber = t.Field<string>("ser_no"), }).Distinct().ToList();
                int NumberOfTemplatesRecords = distinctTIds.Count;

                

                var pageSize = 100; // set your page size, which is number of records per page
                var page = 1; // set current page number, must be >= 1
                var Mod = NumberOfPersonalInfoRecords % pageSize;
                var Iteration = NumberOfPersonalInfoRecords / pageSize + Mod;

                /* if (canPage) // do what you wish if you can page no further
                     return;*/
               /* for (int i = 0; i < Iteration; i++)
                {
                    var skip = pageSize * (page - 1);
                    var canPage = skip < NumberOfPersonalInfoRecords;

                    var PersonalInfoExt = PersonalInfo.Rows.Cast<System.Data.DataRow>()
                             .Skip(skip)
                             .Take(pageSize)
                             .CopyToDataTable();

                    DataTable TemplatesExt = new DataTable();

                    var matched = from table1 in PersonalInfoExt.AsEnumerable()
                                  join table2 in Templates.AsEnumerable() on table1.Field<string>("ser_no") equals table2.Field<string>("ser_no")
                                  where table1.Field<string>("ser_no") == table2.Field<string>("ser_no")
                                  select table2;
                    if (matched.Count() > 0)
                    {
                        TemplatesExt = matched.CopyToDataTable();
                    }

                    page++;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export");
            }
        }*/

        /// <summary>
        /// This method fetches the templates neded for upload
        /// </summary>
        /// <param name="schnum"></param>
        /// schnum is supplied to fetch templates of a school to upload
        /// <param name="batch"></param>
        /// batch is supplied to select a number of templates to upload at a 
        /// given time.
        /// <returns></returns>
        public DataTable FetchTemplatesForUpload(string schnum, int batch)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();

            var dt = new DataTable();
            string sql = string.IsNullOrWhiteSpace(schnum)? "SELECT * FROM Templates WHERE status='0' "
                : "SELECT * FROM Templates WHERE status='0' AND schnum=@schnum";
           
           
            using (SQLiteDataAdapter da = new SQLiteDataAdapter())// filling the datatable dt with 
            {                                                                   //records.
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@schnum", schnum));
                    da.SelectCommand = cmd;
                    da.Fill(dt);

                    if (batch > 0 && dt.Rows.Count > 0)
                    {
                        var DistinctTemplates = dt.DefaultView.ToTable(true, "reg_no");
                        var NumberOfTemplatesfoRecords = DistinctTemplates.Rows.Count;

                        var pageSize = batch; // set your page size, which is number of records per page
                        var page = 1; // set current page number, must be >= 1
                        var Mod = NumberOfTemplatesfoRecords % pageSize;
                        //var Iteration = NumberOfPersonalInfoRecords / pageSize + Mod;

                        var skip = pageSize * (page - 1);
                        // var canPage = skip < NumberOfTemplatesfoRecords;

                        var TemplatesExt = DistinctTemplates.Rows.Cast<System.Data.DataRow>()
                                 .Skip(skip)
                                 .Take(pageSize)
                                 .CopyToDataTable();

                        var matched = from table1 in TemplatesExt.AsEnumerable()
                                      join table2 in dt.AsEnumerable() on table1.Field<string>("reg_no") equals table2.Field<string>("reg_no")
                                      where table1.Field<string>("reg_no") == table2.Field<string>("reg_no")
                                      select table2;
                        if (matched.Count() > 0)
                        {
                            return matched.CopyToDataTable();
                        }
                    }
                }
                return dt;
            }
        }

        private DataTable FetchDistinctTemplatesForUpload()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            DataTable dt = new DataTable();
            try
            {
                string sql = "SELECT DISTINCT * FROM Templates WHERE status='0' ";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, dbConection))
                {
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Fetch Distinct templates", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return dt;
        }
        public Task<int> FetchUploadedCount(string Schnum)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return Task.Run(() =>
            {
                var dt = new DataTable();
                string sql = "SELECT * FROM Templates WHERE schnum=@Schnum AND status='1'";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                        da.SelectCommand = cmd;
                        da.Fill(dt);
                    }

                }
                var distinctIds = dt.AsEnumerable().Select(s =>
                                      new { reg_no = s.Field<string>("reg_no"), }).Distinct().ToList();
                int NumberOfRecords = distinctIds.Count;

                return NumberOfRecords;
            });
          
        }

        public Task<int> FetchUploadedCount()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return Task.Run(() =>
            {
                var dt = new DataTable();
                string sql = "SELECT * FROM Templates WHERE status='1' ORDER BY schnum";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        //cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                        da.SelectCommand = cmd;
                        da.Fill(dt);
                    }

                }
                var distinctIds = dt.AsEnumerable().Select(s =>
                                      new { reg_no = s.Field<string>("reg_no"), }).Distinct().ToList();
                int NumberOfRecords = distinctIds.Count;

                return NumberOfRecords;
            });
        }

        public Task<int> FetchPendingUploadCount()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return Task.Run(() =>
            {
                var dt = new DataTable();
                string sql = "SELECT * FROM Templates WHERE status ='0' ORDER BY schnum";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        //cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                        da.SelectCommand = cmd;
                        da.Fill(dt);
                    }

                }
                var distinctIds = dt.AsEnumerable().Select(s =>
                                      new { reg_no = s.Field<string>("reg_no"), }).Distinct().ToList();
                int NumberOfRecords = distinctIds.Count;

                return NumberOfRecords;
            });
        }

        public DataTable FetchTempTemplates(string Schnum)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            var dt = new DataTable();
            string sql = "SELECT * FROM Temp WHERE schnum=@Schnum";
            using (SQLiteDataAdapter da = new SQLiteDataAdapter())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@Schnum", Schnum));
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public async Task<StaffModel> FetchStaffRecordForActivation(string per_no)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            return await Task.Run(() =>
            {
                try
                {
                    var model = new StaffModel();
                    string sql = "SELECT * FROM staff WHERE per_no=@per_no";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                    {
                        SQLiteDataReader dr;
                        cmd.Parameters.Add(new SQLiteParameter("@per_no", per_no));
                        dr = cmd.ExecuteReader();
                        //if(dr.HasRows)
                        // {
                        while (dr.Read())
                        {
                            model.per_no = dr["per_no"].ToString();
                            model.name = dr["name"].ToString();
                        }
                        // }
                        return model;
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                return null;
            });
           
          
        }
        public DataTable FetchPersonalInfo(string CandidateNo, bool Verify)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            var dt = new DataTable();
            string sql = "SELECT * FROM PersonalInfo WHERE reg_no=@CandidateNo";
            using (SQLiteDataAdapter da = new SQLiteDataAdapter())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@CandidateNo", CandidateNo));
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public string FetchCandName(string CandidateNo)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            var dt = new DataTable();
            string sql = "SELECT cand_name FROM PersonalInfo WHERE reg_no=@CandidateNo";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
            {
                cmd.Parameters.Add(new SQLiteParameter("@CandidateNo", CandidateNo));
                return cmd.ExecuteScalar().ToString();
            }
            //return string.Empty;
        }

        public DataTable FetchPersonalInfo()
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            var dt = new DataTable();
            string sql = "SELECT * FROM PersonalInfo";
            using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, dbConection))
            {
                da.Fill(dt);
            }
            return dt;
        }

        private DataTable FetchPersonalInfoEx(string schnum)
        {
            if (dbConection.State == ConnectionState.Closed)
                dbConection.Open();
            var dt = new DataTable();
            string sql = "SELECT * FROM PersonalInfo WHERE schnum=@schnum";
            using (SQLiteDataAdapter da = new SQLiteDataAdapter())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConection))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@schnum", schnum));
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public IDataReader FetchTemplate(string schnum="")
        {
            try
            {
                if (dbConection.State == ConnectionState.Closed)
                    dbConection.Open();
                string strCommand = string.IsNullOrWhiteSpace(schnum)? "SELECT * FROM templates"
                    : "SELECT * FROM templates WHERE schnum=@schnum";

               

                SQLiteCommand cmd = new SQLiteCommand(strCommand, dbConection);
                cmd.Parameters.Add(new SQLiteParameter("@schnum", schnum));
                return cmd.ExecuteReader();

            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Fetch Template", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        #endregion
    }
}
