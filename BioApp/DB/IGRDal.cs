

using BioApp.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BioApp.DB
{
    public interface IGRDal
    {
        Task<bool> IsCandidateVerified(VerifyModel v);
        string FetchCandName(string CandidateNo);
        Task<List<VerifyViewModel>> FetchVerifiedRecords();
        Task<bool> SaveVerify(VerifyModel v);
        Task<bool> CreateDataBase();
        //void UpdateUploadedRecords(List<Templates> t);
        Task<bool> UpdateUploadedRecords(List<TemplatesUploadResultModel> t);
        //bool SaveDataToTemp(List<TemplatesModel> t);
        Task<bool> SaveDataToTemp(List<TemplatesModel> t);
        Task<bool> SaveDataToOperatorTemp(List<OnlineOperatorListApiModel> s);
        Task<bool> SaveDataToTotalUploadSummaryTable(List<TotalSummryApiModel> s);
        Task<bool> SaveDataToPersonalInfo(List<PersonalInfoModel> p);
        Task<bool> SaveTemplate(TemplatesModel m);
        //bool SaveDataToStaff(List<StaffModel> s);
        Task<bool> SaveDataToStaff(List<StaffModel> s);
        DataTable FetchTemplatesForUpload(string schnum, int batch);
        List<string> FetchTemplatesForFingerCount(string Schnum);
        IDataReader FetchTemplate(string schnum = "");
        DataTable FetchPersonalInfo(string CandidateNo, bool Verify);
        DataTable FetchPersonalInfo();
        //int FetchPersonalInfoCount(string Schnum);
        //DataTable FetchRecords(string schnum, string CandidateName, bool WithSearch);
        Task<DataTable> FetchRecords(string schnum, string CandidateName, bool WithSearch);
        Task<int> FetchPersonalInfoCount(string Schnum);
        Task<int> FetchPersonalInfoCount();
        Task<DataTable> FetchPersonalInfo(string Schnum, string CandidateName, bool WithSearch);
        DataTable FetchRecords(string schnum);
        DataTable FetchTemplatesToVerify(string schnum);
        DataTable FetchTemplates(string Schnum);
        //int FetchTemplatesCount(string Schnum);
        Task<int> FetchTemplatesCount(string Schnum);
        Task<int> FetchTemplatesCount();
        Task<int> FetchUploadedCount(string Schnum);
        Task<int> FetchUploadedCount();
        Task<List<OnlineOperatorListApiModel>> FetchOperatorsInfo(string operatorId = "");
        Task<List<TotalSummryApiModel>> FetchTotalSummaryValue(string operatorId = "");
        Task<int> FetchPendingUploadCount();


        Task<bool> DeleteTemplate(string SchoolNo, string CandidateNo);


        //DataTable FetchFinInfo();
        Task<DataTable> FetchFinInfo(string schnum = "");
        Task<bool> SaveDataToFin(FinModel m);
        Task<bool> SaveDataToBank(List<BankSaveModel> s);
        Task<bool> SaveDataToState(List<StateModel> s);
        Task<bool> SaveDataToSubject(List<SubjectModel> s);
        string FetchSubjectDescription(string subj_code);
        Task<List<SubjectViewModel>> FetchSubjects();
        Task<List<StateModel>> FetchStates();
        Task<StaffModel> FetchStaffRecordForActivation(string per_no);
        Task<DataTable> FetchBanks();
        Task<List<SubjectModel>> FetchSubjectsToVerify();
        //List<SubjectModel> FetchSubjectsToVerify();
        Task<int> CountTotalRecords(string schnum);

        Task<PersonalInfoViewModel> FetchPersonalInfoToPreview(string CandidateName);
        Task<DataTable> FetchRecordsForStat(string schnum);
    }
}
