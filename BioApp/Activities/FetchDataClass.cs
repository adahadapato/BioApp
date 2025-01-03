using BioApp.DB;
using BioApp.Models;
using BioApp.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioApp.Activities
{
    public class FetchDataClass:IDisposable
    {
        public async Task<List<VerifyViewModel>> FetchVerifiedRecords()
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var result = await dl.FetchVerifiedRecords();
            return result;
        }
        public async Task<PersonalInfoViewModel> FetchPersonalInfoToPreview(string CandidateName)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var result = await dl.FetchPersonalInfoToPreview(CandidateName);
            return result;
        }

        public async Task<bool> IsCandidateVerified(VerifyModel v)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var result = await dl.IsCandidateVerified(v);
            return result;
        }
        public async Task<List<SubjectModel>> FetchSubjectsToVerify()
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var result = await dl.FetchSubjectsToVerify();
            return result;
        }

        public async Task<StaffModel> FetchStaffForActivation(string id)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var model = await dl.FetchStaffRecordForActivation(id);
            return model;
        }

        public async Task<List<StateModel>> FetchStates()
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var model = await dl.FetchStates();
            return model;
        }

        public async Task<List<OnlineOperatorListApiModel>> FetchOperators(string operatorid)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var model = await dl.FetchOperatorsInfo(operatorid);
            return model;
        }

        public async Task<List<TotalSummryApiModel>> FetchTotalUploadSummary(string operatorid)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var model = await dl.FetchTotalSummaryValue(operatorid);
            return model;
        }
        public async Task<List<TemplatesUploadTempModel>> FetchTemplatesToUpload(List<FinModel> model)
        {
            return await Task.Run(() =>
            {
                List<TemplatesUploadTempModel> t = new List<TemplatesUploadTempModel>();
                foreach (var m in model)
                {
                    var dt = FetchTemplatesFromDB(m.SchoolNo, 0);
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                t.Add(new TemplatesUploadTempModel
                                {
                                    schNum = dt.Rows[i]["schnum"].ToString(),
                                    regNo = dt.Rows[i]["reg_no"].ToString(),
                                    finger = GZipClassLayer.CompressByteToGzipBase64((byte[])dt.Rows[i]["finger"]),
                                    //finger = GZipClassLayer.CompressByteToGzipBase64(ProcessImageData.byteToString((byte[])dt.Rows[i]["finger"])),
                                    template = GZipClassLayer.CompressByteToGzipBase64((byte[])dt.Rows[i]["template"]),
                                    quality = (long)dt.Rows[i]["quality"]
                                });
                            }
                        }
                    }
                }
                return t;
            });
           
        }

        /* public async Task<List<TemplatesModel>> FetchTemplatesToUpload(List<FinModel> model, int batch)
         {
             return await Task.Run(() =>
             {
                 List<TemplatesModel> t = new List<TemplatesModel>();
                 foreach (var m in model)
                 {
                     var dt = FetchTemplatesFromDB(m.SchoolNo, 0);
                     if (dt != null)
                     {
                         if (dt.Rows.Count > 0)
                         {
                             for (int i = 0; i < dt.Rows.Count; i++)
                             {
                                 t.Add(new TemplatesModel
                                 {
                                     schnum = dt.Rows[i]["schnum"].ToString(),
                                     reg_no = dt.Rows[i]["reg_no"].ToString(),
                                     finger = (byte[])dt.Rows[i]["finger"],
                                     template = (byte[])dt.Rows[i]["template"],
                                     quality = (long)dt.Rows[i]["quality"]
                                 });
                             }
                         }
                     }
                 }
                 return t;
             });
         }*/


        public async Task<int> CountTotalRecords(string SchoolNo)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return await dl.CountTotalRecords(SchoolNo);
        }
        public async Task<List<FinModel>> FetchFinInfo(string schnum)
        {
            List<FinModel> items = new List<FinModel>();
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            DataTable dt = await dl.FetchFinInfo(schnum);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        int Bal = 0;
                        int Balance = await FetchTotalCount(dt.Rows[i]["SchoolNo"].ToString());
                        if (Balance > 0)//Dont add to list if there are no records present.
                        {
                            int Captured = await FetchCapturedCount(dt.Rows[i]["SchoolNo"].ToString());
                            if (Captured > 0)
                                Bal = Balance - Captured;
                            else
                                Bal = Balance;
                            items.Add(new FinModel()
                            {
                                SchoolNo = dt.Rows[i]["SchoolNo"].ToString(),
                                SchoolName = dt.Rows[i]["SchoolName"].ToString(),
                                Captured = Captured,
                                Balance = Bal,
                                Progress = await Progress(Balance, Captured)
                            });
                        }
                    }
                    catch(Exception) {    }
                }
            }
            return items;
        }


        public async Task<List<UploadStatModel>> FetchFinInfoEx(string Schnm)
        {
            List<UploadStatModel> model = new List<UploadStatModel>();
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            DataTable dt = await dl.FetchFinInfo(Schnm);
            if(dt!=null)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            int Pending = 0;
                            int Balance = await FetchTotalCount(dt.Rows[i]["SchoolNo"].ToString());
                            if (Balance > 0)
                            {
                                int Captured = await FetchCapturedCount(dt.Rows[i]["SchoolNo"].ToString());
                                int Uploaded = await FetchUploadedCount(dt.Rows[i]["SchoolNo"].ToString());

                                Pending = Captured - Uploaded;

                                model.Add(new UploadStatModel
                                {
                                    SchoolNo = dt.Rows[i]["SchoolNo"].ToString(),
                                    SchoolName = dt.Rows[i]["SchoolName"].ToString(),
                                    Captured = Captured,
                                    Uploaded = Uploaded,
                                    Pending = Pending,
                                    Total = Balance,
                                    Progress = await Progress(Captured, Uploaded)
                                });
                            }

                        }
                        catch { }
                    }
                }
            }
            return model;
        }

        public async Task<int> FetchTotalCount(string schnum="")
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var result=(string.IsNullOrWhiteSpace(schnum))? await dl.FetchPersonalInfoCount(): await dl.FetchPersonalInfoCount(schnum);
            return result;
        }

      
        public async Task<int> FetchCapturedCount(string schnum="")
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var result=(string.IsNullOrWhiteSpace(schnum))? await dl.FetchTemplatesCount() : await dl.FetchTemplatesCount(schnum);
            return result;
        }

        private Task<string> Progress(int Captured, int Uploaded)
        {
            //return Task.Delay(10000)
        //.ContinueWith(t => "Hello");
            return  Task.FromResult(ColorProcessLayer.CalculateColor(Uploaded, Captured));
        }
        public async Task<int> FetchUploadedCount(string schnum="")
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var result=(string.IsNullOrWhiteSpace(schnum))? await dl.FetchUploadedCount(): await dl.FetchUploadedCount(schnum);
            return result;
        }

        public async Task<int> FetchPendingUploadCount(string schnum = "")
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var result = (string.IsNullOrWhiteSpace(schnum)) ? await dl.FetchPendingUploadCount() : await dl.FetchUploadedCount(schnum);
            return result;
        }
        private DataTable FetchTemplatesFromDB(string schnum, int batch)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            DataTable dt = dl.FetchTemplatesForUpload(schnum, batch);
            return dt;
        }
        public async Task<List<BankSaveModel>> FetchBanks()
        {
            List<BankSaveModel> model = new List<BankSaveModel>();
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var dt = await dl.FetchBanks();
            if(dt!=null)
            {
                if(dt.Rows.Count>0)
                {
                    for(int i=0;i<dt.Rows.Count;i++)
                    {
                        model.Add(new BankSaveModel
                        {
                            bank_code = dt.Rows[i]["bank_code"].ToString(),
                            bank_name=dt.Rows[i]["bank_name"].ToString().Trim()
                        });
                    }
                   
                }
            }
            return model;
        }

        public List<string> FetchTemplatesForFingerCount(string schnum)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return dl.FetchTemplatesForFingerCount(schnum);
        }

        public async Task<List<PersonalInfoViewModel>> FetchRecordsForStat(string schnum)
        {
            List<PersonalInfoViewModel> items = new List<PersonalInfoViewModel>();
            
            try
            {
                List<string> Templates = FetchTemplatesForFingerCount(schnum);
                IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                DataTable dt =await dl.FetchRecordsForStat(schnum);
                if (dt.Rows.Count > 0)
                {
                    //schName = dt.Rows[0]["sch_name"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string reg_no = dt.Rows[i]["reg_no"].ToString();
                        string Sch = dt.Rows[i]["schnum"].ToString();

                        var fingers = Templates.Where(x => x == reg_no).ToList().Count;
                        
                        items.Add(new PersonalInfoViewModel()
                        {
                            schnum = dt.Rows[i]["schnum"].ToString(),
                            reg_no = dt.Rows[i]["reg_no"].ToString(),
                            cand_name = dt.Rows[i]["cand_name"].ToString(),
                            sex = dt.Rows[i]["Sex"].ToString(),
                            passport = (byte[])dt.Rows[i]["passport"],
                            subj1 = dt.Rows[i]["subj1"].ToString(),
                            subj2 = dt.Rows[i]["subj2"].ToString(),
                            subj3 = dt.Rows[i]["subj3"].ToString(),
                            subj4 = dt.Rows[i]["subj4"].ToString(),
                            subj5 = dt.Rows[i]["subj5"].ToString(),
                            subj6 = dt.Rows[i]["subj6"].ToString(),
                            subj7 = dt.Rows[i]["subj7"].ToString(),
                            subj8 = dt.Rows[i]["subj8"].ToString(),
                            subj9 = dt.Rows[i]["subj9"].ToString(),
                            Pict = SafeGuiWpf.SetImage((byte[])dt.Rows[i]["passport"]) ,
                            fingers = fingers.ToString(),
                            Progress = await Progress(4, fingers)
                        });
                    }
                }

            }
            catch (Exception)
            {
                // ShowMessageBox(e.Message, "Load data", "Error");
            }
            return items;
        }

        private int FetchFingerCountForCandidate(string reg_no, string schnum)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            DataTable dt = dl.FetchTemplates(schnum);

            var name = (from r in dt.AsEnumerable()
                        where (r.Field<string>("reg_no") == reg_no)
                        select r.Field<string>("reg_no")).ToList();

            return name.Count;
        }

        public async Task<List<PersonalInfoModel>> FetchRecordsForDisplay(string Schnum, string CandidateName, bool WithSearch)
        {
            var items = new List<PersonalInfoModel>();
            var subjList = await FetchSubjects();
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var dt = await dl.FetchRecords(Schnum, CandidateName, WithSearch);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            items.Add(new PersonalInfoModel()
                            {
                                schnum = dt.Rows[i]["schnum"].ToString(),
                                reg_no = dt.Rows[i]["reg_no"].ToString(),
                                cand_name = dt.Rows[i]["cand_name"].ToString(),
                                sex = dt.Rows[i]["Sex"].ToString(),
                                passport = (byte[])dt.Rows[i]["passport"],


                                subj1 = (string.IsNullOrWhiteSpace(dt.Rows[i]["subj1"].ToString())) ? "" : subjList.First(s => s.subj_code == dt.Rows[i]["subj1"].ToString()).Descript,
                                subj2 = (string.IsNullOrWhiteSpace(dt.Rows[i]["subj2"].ToString())) ? "" : subjList.First(s => s.subj_code == dt.Rows[i]["subj2"].ToString()).Descript,
                                subj3 = (string.IsNullOrWhiteSpace(dt.Rows[i]["subj3"].ToString())) ? "" : subjList.First(s => s.subj_code == dt.Rows[i]["subj3"].ToString()).Descript,
                                subj4 = (string.IsNullOrWhiteSpace(dt.Rows[i]["subj4"].ToString())) ? "" : subjList.First(s => s.subj_code == dt.Rows[i]["subj4"].ToString()).Descript,
                                subj5 = (string.IsNullOrWhiteSpace(dt.Rows[i]["subj5"].ToString())) ? "" : subjList.First(s => s.subj_code == dt.Rows[i]["subj5"].ToString()).Descript,
                                subj6 = (string.IsNullOrWhiteSpace(dt.Rows[i]["subj6"].ToString())) ? "" : subjList.First(s => s.subj_code == dt.Rows[i]["subj6"].ToString()).Descript,
                                subj7 = (string.IsNullOrWhiteSpace(dt.Rows[i]["subj7"].ToString())) ? "" : subjList.First(s => s.subj_code == dt.Rows[i]["subj7"].ToString()).Descript,
                                subj8 = (string.IsNullOrWhiteSpace(dt.Rows[i]["subj8"].ToString())) ? "" : subjList.First(s => s.subj_code == dt.Rows[i]["subj8"].ToString()).Descript,
                                subj9 = (string.IsNullOrWhiteSpace(dt.Rows[i]["subj9"].ToString())) ? "" : subjList.First(s => s.subj_code == dt.Rows[i]["subj9"].ToString()).Descript
                            });
                        }
                        catch
                        {
                            continue;
                        }
                       
                    }
                }

            }
            return items;
        }


        public async Task<List<SubjectViewModel>> FetchSubjects()
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return await dl.FetchSubjects();
        }
        string FetchSubjectDescription(string subj_code)
        {
            if (string.IsNullOrWhiteSpace(subj_code))
            {
                return string.Empty;
            }
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return dl.FetchSubjectDescription(subj_code);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FetchDataClass() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
