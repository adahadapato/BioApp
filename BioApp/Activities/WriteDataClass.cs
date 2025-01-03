using BioApp.DB;
using BioApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioApp.Activities
{
    public class WriteDataClass:IDisposable
    {
        public async Task<bool> SaveVerify(VerifyModel v)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var result= await dl.SaveVerify(v);
            return result;
        }
        public async void SaveBankToDataBase(List<BankSaveModel> model)
        {
            
        }

        public async Task<bool> SaveTemplate(TemplatesModel t)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return await dl.SaveTemplate(t);
        }

        public async Task<bool> SaveOperatorsToDatabase(List<OnlineOperatorListApiModel> t)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return await dl.SaveDataToOperatorTemp(t);
        }

        public async Task<bool> SaveTotalUploadSummaryToDatabase(List<TotalSummryApiModel> t)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return await dl.SaveDataToTotalUploadSummaryTable(t);
        }
        public async Task<bool> CreateDataBase()
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
           return await dl.CreateDataBase();
        }
        public async Task<bool> SaveDataToFin(FinModel f)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return await dl.SaveDataToFin(f);
        }

        public async void SaveDataToTemp(List<TemplatesModel> t)
        {
            IGRDal d = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            await d.SaveDataToTemp(t);
        }
        public async void SaveDataToPersonalInfo(List<PersonalInfoModel> p)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            await dl.SaveDataToPersonalInfo(p);
        }
        public async Task<bool> UpdateUploadedRecords(List<TemplatesUploadResultModel> lst)
        {
           IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
           var result= await dl.UpdateUploadedRecords(lst);
           return result;
        }

        public async Task<bool> DeteTemplates(string schnum, string CandidateNo)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            var result = await dl.DeleteTemplate(schnum, CandidateNo);
            return result;
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
        // ~WriteDataClass() {
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
