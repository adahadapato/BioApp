using BioApp.ApiInfrastructure;
using BioApp.DB;
using BioApp.Models;
using BioApp.Network;
using BioApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Position;
using ToastNotifications.Lifetime;
using ToastNotifications.Core;
//using ToastNotifications.Messages;

namespace BioApp.Activities
{
    public class RemoteDataClass:IDisposable
    {
        //private readonly ToastViewModel _vm;// = new ToastViewModel();
        private NetworkClass networkClass = new NetworkClass();
        public RemoteDataClass()
        {
            //_vm = new ToastViewModel();
           // networkClass = new NetworkClass();
        }

       /* void ShowMessage(Action<string, MessageOptions> action, string name)
        {
            MessageOptions opts = new MessageOptions
            {
                //CloseClickAction = CloseAction,
                FreezeOnMouseEnter = false,
                UnfreezeOnMouseLeave = true,
                ShowCloseButton = true
            };
           // _lastMessage = $"{_count} {name}";
            //action(_lastMessage, opts);
           // bClearLast.IsEnabled = true;
        }*/


        public async Task<bool> FetchCentreForVerification(string schnum)
        {
            var Json = await NetworkClass.FetchTemplateBySchool("0010055");
            if(!string.IsNullOrWhiteSpace(Json))
            {
                var result = JsonLayer.DecodeJsonToModel<TemplateToVerifyApiModel>(Json);
                if(result!=null)
                {
                    var k = result;
                    return true;
                }
            }
           
            return true;
        }

        public async Task<bool> AddNewOpertor(List<NewCafeOperatorModel> model)
        {
            if (!await networkClass.ping(HttpClientInstance.PingJideSVRAddress))
                return false;

            var op = new NewCafeOperatorApiModel
            {
                CafeOperators= model
            };
            var Json = JsonLayer.EncodeModelToJson<NewCafeOperatorApiModel>(op);
            var result = await NetworkClass.AddNewOperator(Json);
            int correct = 0;
            int wrong =0;
            string correctMSG = "";
            string wrongMSG = "";
            if (!string.IsNullOrWhiteSpace(result))
            {
                var ans = JsonLayer.DecodeJsonToModel<List<NewCafeOperatorResultApiModel>>(result);
                foreach(var t in ans)
                {
                    if (t.responseStatus == "1")
                    {
                        correct++;
                        correctMSG=t.responseComment;//, "Add Operator", MessageBoxButton.OK, MessageBoxImage.Information);
                    }else
                    {
                        wrong++;
                        wrongMSG=t.responseComment;//, "Add Operator", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                if (correct>0)
                {
                   SafeGuiWpf.ShowSuccess(correctMSG +" for "+ correct+" Operators");//, "Add Operator", MessageBoxButton.OK, MessageBoxImage.Information);
                   
                }
                
                if(wrong>0)
                {
                    SafeGuiWpf.ShowWarning(wrongMSG + " for " + wrong + " Operators");//, "Add Operator", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                return true;
            }
            return false;
        }

        public async Task<bool> SetOpertorStatus(SetOperatorStatusModel model)
        {
            try
            {
                if (!await networkClass.ping(HttpClientInstance.PingJideSVRAddress))
                    return false;

                var Json = JsonLayer.EncodeModelToJson<SetOperatorStatusModel>(model);
                var result = await NetworkClass.SetOperatorStatus(Json);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var ans = JsonLayer.DecodeJsonToModel<SetOperatorStatusResultModel>(result);
                    if (ans.responseStatus == "1")
                    {
                        SafeGuiWpf.ShowSuccess(ans.responseComment);//, "Operator status", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    SafeGuiWpf.ShowError(ans.responseComment);
                }
            }
            catch(Exception ex)
            {
                SafeGuiWpf.ShowError(ex.Message);
            }
            return false;
        }


        public async Task<List<TotalSummryApiModel>> FetchTotalUploadSummary()
        {
            try
            {
                if (!await networkClass.ping(HttpClientInstance.PingAliSVRAddress))
                    return null;

                var result = await NetworkClass.FetchTotalSummary();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var ans = JsonLayer.DecodeJsonToModel<List<TotalSummryApiModel>>(result);
                    using (WriteDataClass wd = new WriteDataClass())
                    {
                        var save = await wd.SaveTotalUploadSummaryToDatabase(ans);
                        if (save)
                        {
                            using (FetchDataClass fd = new FetchDataClass())
                            {
                                var model = await fd.FetchTotalUploadSummary("");
                                return model;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SafeGuiWpf.ShowError(ex.Message);
            }
            return null;
        }
        public async Task<List<OnlineOperatorListApiModel>> FetchOnlineOperators()
        {
            try
            {
                if (!await networkClass.ping(HttpClientInstance.PingJideSVRAddress))
                    return null;

                //var Json = JsonLayer.EncodeModelToJson<OnlineOperatorListApiModel>(model);
                var result = await NetworkClass.FetchOnlineOperators();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var ans = JsonLayer.DecodeJsonToModel<List<OnlineOperatorListApiModel>>(result);
                    using (WriteDataClass wd = new WriteDataClass())
                    {
                        var save = await wd.SaveOperatorsToDatabase(ans);
                        if(save)
                        {
                            using (FetchDataClass fd = new FetchDataClass())
                            {
                                var model = await fd.FetchOperators("");
                                return model;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //SafeGuiWpf.ShowError(ex.Message);
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public async Task<bool> UploadTemplates(List<TemplatesUploadTempModel> templates)
        {
            int tries = 0;
            bool ping = await networkClass.ping(HttpClientInstance.PingAliSVRAddress);
            if (!ping)
                return false;

            var cafe = CafeOperatorClass.GetId;
            TemplatesUploadModel model = new TemplatesUploadModel
            {
                cafe = CafeOperatorClass.GetId,
                templates = templates
            };
            using (new WaitCursor())
            {
                string Json = JsonLayer.EncodeModelToJson<TemplatesUploadModel>(model);//.CreateTemplateUploadJson(model);
                SafeGuiWpf.ShowInformation("Please wait while Templates upload ,,,");
                string result = await networkClass.UploadTemplate(Json);
                if (string.IsNullOrWhiteSpace(result))
                {
                    SafeGuiWpf.ShowError("Error: Templates upload failed ,,,");
                    return false;
                }
                
                var  tmp = JsonLayer.DecodeJsonToModel<List<TemplatesUploadResultModel>>(result);

                var DistinctsTemplatesResult = tmp.GroupBy(s => s.regNo)
                                                .Select(t => t.First()).ToList();
                
                var failed = (from r in DistinctsTemplatesResult.AsEnumerable()
                                where (r.status == "0")
                                select r).ToList();

                var success = (from r in DistinctsTemplatesResult.AsEnumerable()
                              where (r.status == "1")
                              select r).ToList();

                SafeGuiWpf.ShowSuccess(success.Count.ToString()+"Candidates Templates uploaded successfully\n"+
                                failed.Count.ToString()+" Candidates Templates failed to upload");

                bool retVal;
                using (WriteDataClass wd = new WriteDataClass())
                {
                    retVal = await wd.UpdateUploadedRecords(success);
                    
                }

                var date = DateTime.Now;
                string tempDate = string.Format("{0:yyyy/MM/dd}", date);
                var confirmation = new List<CandidateConfirmationModel>();

                foreach (var t in tmp)
                {
                    confirmation.Add(new CandidateConfirmationModel
                    {
                        RegNo = t.regNo,
                        Status = t.status,
                        schNum = t.schNum,
                        ValueDate = tempDate
                    });
                }

                TemplateUploadConfirmationToJideModel summary = new TemplateUploadConfirmationToJideModel
                {
                    cafeid=CafeOperatorClass.GetId,
                    Candidates= confirmation
                };

                string summaryJson = JsonLayer.EncodeModelToJson<TemplateUploadConfirmationToJideModel>(summary);
                
                while(tries <=4)
                {
                    var summaryResult = await NetworkClass.PostUploadSummary(summaryJson);

                    var summaryResults = JsonLayer.DecodeJsonToModel<SummaryUploadResultModel>(summaryResult);
                    if (summaryResults.responseStatus == "1")
                    {
                        tries = 5;
                        break;
                    }
                    tries++;
                }
                return retVal;  
            }
               
        }

        public async Task<string> AuthenticateOperator(string id)
        {
            if (!await networkClass.ping(HttpClientInstance.PingJideSVRAddress))
                return null;

            string Json = await networkClass.AuthenticateOfficer(id);
            return Json;
        }

        public async Task<bool> UploadTemplates(List<TemplatesUploadTempModel> templatesData, int batchSize)
        {
            bool retVal = false; ;
            int tries = 0;
            var confirmation = new List<CandidateConfirmationModel>();
           
            
            
            var DistinctsTemplates = templatesData.GroupBy(s => s.regNo)
                .Select(t => t.First()).ToList();

            SafeGuiWpf.ShowInformation(DistinctsTemplates.Count.ToString() + " Batche(s) ready for upload");
            int BatchNo = 0;
            foreach (var batch in Extensionsn.Batch<TemplatesUploadTempModel>(DistinctsTemplates, batchSize))
            {
                ++BatchNo;
                var TemplatesToBeUploaded = (from w1 in templatesData
                                             where batch.Any(w2 => w1.regNo == w2.regNo)
                                                 select w1).ToList();


                
                TemplatesUploadModel model = new TemplatesUploadModel
                {
                     cafe = CafeOperatorClass.GetId,
                     templates = TemplatesToBeUploaded
                };


                string Json = JsonLayer.EncodeModelToJson<TemplatesUploadModel>(model);//.CreateTemplateUploadJson(model);

                if (!await networkClass.ping(HttpClientInstance.PingAliSVRAddress))
                    continue;

                SafeGuiWpf.ShowInformation("Uploading templates from " + BatchNo);
                string result = await networkClass.UploadTemplate(Json);
                if (string.IsNullOrWhiteSpace(result))
                {
                    SafeGuiWpf.ShowError("Upload failed for " + BatchNo);
                    continue;
                }
                    

                var tmp = JsonLayer.DecodeJsonToModel<List<TemplatesUploadResultModel>>(result);

                if (tmp.Count == 0)
                    continue;

                var date = DateTime.Now;
                string tempDate = string.Format("{0:yyyy/MM/dd}", date);   // 05/06/2018

                var DistinctsTemplatesResult = tmp.GroupBy(s => s.regNo)
                    .Select(t => t.First()).ToList();

                if (DistinctsTemplatesResult.Count == 0)
                    continue;


                using (WriteDataClass wd = new WriteDataClass())
                {
                    retVal = await wd.UpdateUploadedRecords(DistinctsTemplatesResult);
                }

                

                foreach (var t in tmp)
                {
                    confirmation.Add(new CandidateConfirmationModel
                    {
                        RegNo = t.regNo,
                        Status = t.status,
                        schNum =t.schNum,
                        ValueDate = tempDate
                    });
                }
            }

            if(retVal)
            {
                TemplateUploadConfirmationToJideModel summary = new TemplateUploadConfirmationToJideModel
                {
                    cafeid = CafeOperatorClass.GetId,
                    Candidates = confirmation
                };

                string summaryJson = JsonLayer.EncodeModelToJson<TemplateUploadConfirmationToJideModel>(summary);
                
                while (tries <= 4)
                {
                    var summaryResult = await NetworkClass.PostUploadSummary(summaryJson);
                    if(!string.IsNullOrWhiteSpace(summaryResult))
                    {
                        var summaryResults = JsonLayer.DecodeJsonToModel<SummaryUploadResultModel>(summaryResult);
                        if (summaryResults.responseStatus == "1")
                        {
                            tries = 5;
                            break;
                        }
                    }
                    tries++;
                }
            }
           
            return retVal;
        }


        public async Task<UpdateBankDetailsResult>  UpdateBankDetails(UpdateBankDetailsModel model)
        {
            return await Task.Run(async () =>
            {
                var Json = JsonLayer.CreateOperatorBankDetailsJson(model);
                if (!await networkClass.ping(HttpClientInstance.PingJideSVRAddress))
                    return null;

                string result = await networkClass.UpdateOperatorBankDetails(Json);
                var resp = JsonLayer.CreateOperatorBankDetailsResultJson(result);
                return resp;
            });
           
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //_vm.OnUnloaded();
                    ((IDisposable)networkClass).Dispose();
                   
                }
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RemoteDataClass() {
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
