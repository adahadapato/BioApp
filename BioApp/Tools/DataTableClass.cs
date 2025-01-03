

namespace BioApp
{
    using System;
    using System.Data;
    using BioApp.DB;
    using System.Data.SqlTypes;
    using Models;
    using System.Collections.Generic;

    public class DataTableClass
    {

        public static DataTable CreateSubjectsDataTable(List<SubjectModel> Subjects)
        {
            DataTable table = new DataTable("dt");

            DataColumn colcode = new DataColumn("Code", typeof(string));
            table.Columns.Add(colcode);
            DataColumn colsubj_code = new DataColumn("subj_code", typeof(string));
            table.Columns.Add(colsubj_code);
            DataColumn colSubject = new DataColumn("Subject", typeof(string));
            table.Columns.Add(colSubject);
            DataColumn colPaper = new DataColumn("Paper", typeof(string));
            table.Columns.Add(colPaper);
            DataColumn colDescript = new DataColumn("Descript", typeof(string));
            table.Columns.Add(colDescript);
            DataColumn colDate = new DataColumn("Date", typeof(string));
            table.Columns.Add(colDate);
            DataColumn colStartTime = new DataColumn("StartTime", typeof(string));
            table.Columns.Add(colStartTime);
            DataColumn colStopTime = new DataColumn("StopTime", typeof(string));
            table.Columns.Add(colStopTime);

            foreach (var s in Subjects)
            {
                DataRow drow = table.NewRow();
                drow["Code"] = s.Code.ToString();
                drow["subj_code"] = s.subj_code.ToString();
                drow["Subject"] = s.Subject.ToString();
                drow["Paper"] = s.Paper.ToString();
                drow["Descript"] = s.Descript.ToString();
                drow["Date"] = s.Date.ToString();
                drow["StartTime"] = s.StartTime.ToString();
                drow["StopTime"] = s.StopTime.ToString();

                table.Rows.Add(drow);
            }
            table.AcceptChanges();
            return table;
        }

        public static DataTable CreateStaffDataTable(List<StaffModel> staff)
        {
            DataTable table = new DataTable("dt");

            DataColumn colper_no = new DataColumn("per_no", typeof(string));
            table.Columns.Add(colper_no);
            DataColumn colname = new DataColumn("name", typeof(string));
            table.Columns.Add(colname);
           

            foreach (var s in staff)
            {
                DataRow drow = table.NewRow();
                drow["per_no"] = s.per_no.ToString();
                drow["name"] = s.name.ToString().Trim();
               
                table.Rows.Add(drow);
            }
            table.AcceptChanges();
            return table;
        }
        public static DataTable CreateTemplateDataTable(List<TemplatesModel> t)
        {
            
             DataTable table = new DataTable("dt");

            DataColumn colschnum = new DataColumn("schnum", typeof(string));
            table.Columns.Add(colschnum);
            DataColumn colreg_no = new DataColumn("reg_no", typeof(string));
            table.Columns.Add(colreg_no);
            DataColumn colTemplate = new DataColumn("Template", typeof(SqlBinary));
             table.Columns.Add(colTemplate);
             DataColumn colQuality = new DataColumn("Quality", typeof(int));
             table.Columns.Add(colQuality);

            foreach (var Template in t)
            {
                DataRow drow = table.NewRow();
                drow["schnum"] = Template.schnum.ToString();
                drow["reg_no"] = Template.reg_no.ToString();
                drow["Template"] = (byte[])Template.template;
                drow["Quality"] = Convert.ToInt32(Template.quality);
                
                table.Rows.Add(drow);
            }
            table.AcceptChanges();
            return table;
        }

        public static DataTable CreatePersonalInfoTable(List<PersonalInfoModel> p)
        {

            DataTable table = new DataTable("dt");
            

            DataColumn colschnum = new DataColumn("schnum", typeof(string));
            table.Columns.Add(colschnum);
            DataColumn colreg_no = new DataColumn("reg_no", typeof(string));
            table.Columns.Add(colreg_no);
            DataColumn colcand_name = new DataColumn("cand_name", typeof(string));
            table.Columns.Add(colcand_name);
            DataColumn colsex = new DataColumn("sex", typeof(string));
            table.Columns.Add(colsex);
            DataColumn colpassport = new DataColumn("Passport", typeof(SqlBinary));
            table.Columns.Add(colpassport);

            //
            DataColumn colsubj1 = new DataColumn("subj1", typeof(string));
            table.Columns.Add(colsubj1);
            DataColumn colsubj2 = new DataColumn("subj2", typeof(string));
            table.Columns.Add(colsubj2);
            DataColumn colsubj3 = new DataColumn("subj3", typeof(string));
            table.Columns.Add(colsubj3);
            DataColumn colsubj4 = new DataColumn("subj4", typeof(string));
            table.Columns.Add(colsubj4);
            DataColumn colsubj5 = new DataColumn("subj5", typeof(string));
            table.Columns.Add(colsubj5);
            DataColumn colsubj6 = new DataColumn("subj6", typeof(string));
            table.Columns.Add(colsubj6);
            DataColumn colsubj7 = new DataColumn("subj7", typeof(string));
            table.Columns.Add(colsubj7);
            DataColumn colsubj8 = new DataColumn("subj8", typeof(string));
            table.Columns.Add(colsubj8);
            DataColumn colsubj9 = new DataColumn("subj9", typeof(string));
            table.Columns.Add(colsubj9);
            DataColumn colsch_name = new DataColumn("sch_name", typeof(string));
            table.Columns.Add(colsch_name);

            foreach (var Personalinfo in p)
            {
                DataRow drow = table.NewRow();
                drow["schnum"] = Personalinfo.schnum.ToString();
                drow["reg_no"] = Personalinfo.reg_no.ToString();
                drow["cand_name"] = Personalinfo.cand_name.ToString();
                drow["sex"] = Personalinfo.sex.ToString();
                drow["passport"] = (byte[])Personalinfo.passport;
                drow["subj1"] = Personalinfo.subj1.ToString();
                drow["subj2"] = Personalinfo.subj2.ToString();
                drow["subj3"] = Personalinfo.subj3.ToString();
                drow["subj4"] = Personalinfo.subj4.ToString();
                drow["subj5"] = Personalinfo.subj5.ToString();
                drow["subj6"] = Personalinfo.subj6.ToString();
                drow["subj7"] = Personalinfo.subj7.ToString();
                drow["subj8"] = Personalinfo.subj8.ToString();
                drow["subj9"] = Personalinfo.subj9.ToString();
                drow["sch_name"] = "Test Secondaey School, Minna";
                // Personalinfo.sch_name.ToString();
                table.Rows.Add(drow);
            }
            table.AcceptChanges();
            return table;
        }
        /* public static DataTable CreateDataTable(string SchoolNo, int k)
         {
             /* DataTable table = new DataTable("dt");
              DataColumn colid = new DataColumn("id", typeof(int));
              table.Columns.Add(colid);

              DataColumn colTemplate = new DataColumn("Template");//, typeof(Byte[]));
              colTemplate.DataType =Type.GetType("System.byte[]");
              table.Columns.Add(colTemplate);

              DataColumn colQuality = new DataColumn("Quality", typeof(int));
              table.Columns.Add(colQuality);

              DataColumn colreg_no = new DataColumn("reg_no", typeof(string));
              table.Columns.Add(colreg_no);

              DataColumn colschnum = new DataColumn("schnum", typeof(string));
              table.Columns.Add(colschnum);

              DataColumn colcafe = new DataColumn("cafe", typeof(string));
              table.Columns.Add(colcafe);


              IDataReader dr = FetchTemplateRecords();
              using (dr)
              {
                  while (dr.Read())
                  {
                      DataRow drow = table.NewRow();
                      drow["id"] = Convert.ToInt32(dr["id"].ToString());
                      drow["Template"] = (byte[])dr["template"];
                      drow["Quality"] = Convert.ToInt32(dr["Quality"].ToString());
                      drow["reg_no"] = dr["reg_no"].ToString();
                      drow["schnum"] = dr["schnum"].ToString();
                      drow["cafe"] = CafeOperatorClass.LoadOperatorId();

                      table.Rows.Add(drow);
                  }
                  table.AcceptChanges();
              }*/


        /*  DataTable table = new DataTable();
          IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
          table = dl.CreateDataTable(SchoolNo);
          return table;
      }*/

        private static IDataReader FetchTemplateRecords()
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            IDataReader dataReader = dl.FetchTemplate();

            return dataReader;
        }

        
    }
}
