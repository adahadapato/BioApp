


using BioApp.ApiInfrastructure;
using BioApp.Models;
using BioApp.Tools;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BioApp.Network
{
    public class NetworkClass:IDisposable
    {
        
        public  async Task<bool> ping(string address)
        {
            return await Task.Run(async () =>
            {
                //_vm.ShowInformation("Contacting the remote Server ... ");
                try
                {
                    //string address = "www.google.com";
                    System.Net.NetworkInformation.Ping pingSender
                        = new System.Net.NetworkInformation.Ping();
                    System.Net.NetworkInformation.PingReply reply
                        = pingSender.Send(address);
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        return true;
                    }
                    else
                    {
                        SafeGuiWpf.ShowError("The Server is not reacheable at this time\n" +
                                            "Please try again when you have internet connection");//, "Server", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        await Task.Delay(3000);
                        return false;
                    }
                }
                catch
                {
                    SafeGuiWpf.ShowError("The Server is not reacheable at this time\n" +
                                            "Please try again when you have internet connection");//, "Server", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    await Task.Delay(3000);
                    return false;
                }
            });
        }

        public  async Task<string> AuthenticateOfficer(string id)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();//Clear all headers
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));//set the return type to JSON
                        string query = string.Format("{0}GetOperator/{1}", HttpClientInstance.Address, id);
                        string resp = "";
                        var task = client.GetAsync(query)
                               .ContinueWith((taskwithresponse) =>
                               {
                                   var response = taskwithresponse.Result;
                                   var jsonString = response.Content.ReadAsStringAsync();
                                   jsonString.Wait();
                                   resp = jsonString.Result;
                               });
                        task.Wait();
                        return resp;
                    }
                }
                catch (Exception ex)
                {
                    SafeGuiWpf.ShowError(ex.Message);//, "Error",System.Windows.MessageBoxButton.OK,System.Windows.MessageBoxImage.Error);
                }
                await Task.Delay(3000);
                return string.Empty;
            });
 
        }

        public static async Task<string> FetchOnlineOperators()
        {
            return await Task.Run(async () =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();//Clear all headers
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));//set the return type to JSON
                        string query = string.Format("{0}GetAllOperators", HttpClientInstance.Address);
                        string resp = "";
                        var task = client.GetAsync(query)
                               .ContinueWith((taskwithresponse) =>
                               {
                                   var response = taskwithresponse.Result;
                                   var jsonString = response.Content.ReadAsStringAsync();
                                   jsonString.Wait();
                                   resp = jsonString.Result;
                               });
                        task.Wait();
                        return resp;
                    }
                }
                catch (Exception ex)
                {
                    SafeGuiWpf.ShowError(ex.Message);
                }
                await Task.Delay(3000);
                return string.Empty;
            });
        }

        /* public static async Task<string> UploadTemplates(string Json)
         {
             string res = "";
             //string url = BaseAddressLayer.BaseAddress();
             // "http://www.mynecoexams.com/BiometricService/RESTWS";
             //http://www.mynecoexams.com/BiometricService/RESTWS/CreateBiodata
                  ///CreateBiodata

             try
             {
                 //RestSharp.RestClient client = new RestSharp.RestClient();
                 //client.BaseUrl = new Uri(url);

                 var client = HttpClientInstance.Instance;
                 RestRequest request = new RestRequest();
                 request.Method = Method.POST;
                 request.RequestFormat = DataFormat.Json;
                 request.Timeout = 60 * 60000;//100000;

                 //Pass String Data  to the request body
                 request.AddBody(Json);
                 request.Resource = "CreateBiodata";

                 // Post the Data to the Server.
                 //dynamic response = client.Execute(request) as RestResponse;
                 RestResponse response= client.Execute(request) as RestResponse;
                 //client.ExecuteAsync(request, response => {
                     if ((response != null) && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                     {
                         dynamic obj = response.Content;

                         res = response.Content;
                         System.Windows.MessageBox.Show("Upload was successful", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                         //response.Content);
                     }
                     else if ((response != null))
                     {
                         //System.Windows.MessageBox.Show(string.Format("Status code is {0} ({1}); response status is {2}", response.StatusCode, response.StatusDescription, response.ResponseStatus), "Upload Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                         System.Windows.MessageBox.Show("Upload failed!\n" +
                                                         "check your Internet connection", "Upload error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                         List<Templates> model = new List<Templates>();
                         model.Add(new Templates
                         {
                             RegNo = "Error",
                             Status = "0"
                         });
                         TemplatesUploadResultModel t = new TemplatesUploadResultModel
                         {
                             Templates = model
                         };
                         res = JsonLayer.CreateUploadErrorJson(t);
                         //return  Task.FromResult(res);
                     }
                 //});

                 //RestResponse response = client.Execute(request) as RestResponse;
                 //var execTask = client.Execute(request);
                 //return await Task.FromResult(res);

             }
             catch (Exception ex)
             {
                 System.Windows.MessageBox.Show(ex.Message,"Upload Error",System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                 List<Templates> model = new List<Templates>();
                 model.Add(new Templates
                 {
                     RegNo = "Error",
                     Status = "0"
                 });
                 TemplatesUploadResultModel t = new TemplatesUploadResultModel
                 {
                     Templates = model
                 };
                 res = JsonLayer.CreateUploadErrorJson(t);
                 return await Task.FromResult(res);
             }

             return await Task.FromResult(res.Replace("Templates", "Templates:"));
         }*/

        //UpdateOperatorBankDetails
        //http://www.mynecoexams.com/BiometricService/RESTWS/CreateBiodataSummary




        public static async Task<string> PostUploadSummary(string Json)
        {
            string res = "";

            try
            {
                var client = HttpClientInstance.Instance;
                RestRequest request = new RestRequest();
                request.Method = Method.POST;
                request.RequestFormat = DataFormat.Json;
                request.Timeout = 60 * 60000;//100000;

                //Pass String Data  to the request body
                request.AddBody(Json);
                request.Resource = "CreateBiodataSummary";

                // Post the Data to the Server.
                dynamic response = client.Execute(request) as RestResponse;
                
                if ((response != null) && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    dynamic obj = response.Content;
                    res = response.Content;
                    //System.Windows.MessageBox.Show("Payment details Upload was successful", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    return await Task.FromResult(res);
                }
                else if ((response != null))
                {
                    string RespMessage = string.Format("Status code is {0} ({1}); response status is {2}", response.StatusCode, response.StatusDescription, response.ResponseStatus);
                    SafeGuiWpf.ShowError("Upload failed!\n" +
                                          RespMessage + "\n" +
                                                     "Check your Internet connection");
                    res = response.Content;
                    return await Task.FromResult(string.Empty);
                }

            }
            catch (Exception ex)
            {
                SafeGuiWpf.ShowError(ex.Message);
            }

            return await Task.FromResult(string.Empty);
        }

        public static async Task<string> SetOperatorStatus(string Json)
        {
            string res = "";

            try
            {
                var client = HttpClientInstance.Instance;
                RestRequest request = new RestRequest();
                request.Method = Method.POST;
                request.RequestFormat = DataFormat.Json;
                request.Timeout = 60 * 60000;//100000;

                //Pass String Data  to the request body
                request.AddBody(Json);
                request.Resource = "SetOperatorStatus";

                // Post the Data to the Server.
                dynamic response = client.Execute(request) as RestResponse;
                
                if ((response != null) && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    dynamic obj = response.Content;

                    res = response.Content;
                    //System.Windows.MessageBox.Show("Payment details Upload was successful", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    return await Task.FromResult(res);
                }
                else if ((response != null))
                {
                    string RespMessage = string.Format("Status code is {0} ({1}); response status is {2}", response.StatusCode, response.StatusDescription, response.ResponseStatus);
                    SafeGuiWpf.ShowError("Update failed!\n" +
                                          RespMessage + "\n" +
                                                     "Check your Internet connection");
                    res = response.Content;
                    return await Task.FromResult(string.Empty);
                }

            }
            catch (Exception ex)
            {
                SafeGuiWpf.ShowError(ex.Message);
            }

            return await Task.FromResult(string.Empty);
        }

        public static async Task<string> AddNewOperator(string Json)
        {
            string res = "";

            try
            {
                var client = HttpClientInstance.Instance;
                RestRequest request = new RestRequest();
                request.Method = Method.POST;
                request.RequestFormat = DataFormat.Json;
                request.Timeout = 60 * 60000;//100000;

                //Pass String Data  to the request body
                request.AddBody(Json);
                request.Resource = "AddOperatorDetails";

                // Post the Data to the Server.
                dynamic response = client.Execute(request) as RestResponse;
               
                if ((response != null) && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    dynamic obj = response.Content;

                    res = response.Content;
                    //System.Windows.MessageBox.Show("Payment details Upload was successful", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    return await Task.FromResult(res);
                }
                else if ((response != null))
                {
                    string RespMessage = string.Format("Status code is {0} ({1}); response status is {2}", response.StatusCode, response.StatusDescription, response.ResponseStatus);
                    SafeGuiWpf.ShowError("Download failed!\n" +
                                          RespMessage + "\n" +
                                                     "Check your Internet connection");
                    res = response.Content;
                    return await Task.FromResult(string.Empty);
                }
            }
            catch (Exception ex)
            {
                SafeGuiWpf.ShowError(ex.Message);
            }

            return await Task.FromResult(string.Empty);
        }

        public async Task<string> UpdateOperatorBankDetails(string Json)
        {
            string res = "";
            
            try
            {
                var client = HttpClientInstance.Instance;
                RestRequest request = new RestRequest();
                request.Method = Method.POST;
                request.RequestFormat = DataFormat.Json;
                request.Timeout = 60 * 60000;//100000;

                //Pass String Data  to the request body
                request.AddBody(Json);
                request.Resource = "CreateOperatorBankDetails";

                // Post the Data to the Server.
                dynamic response = client.Execute(request) as RestResponse;
                
                if ((response != null) && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    dynamic obj = response.Content;

                    res = response.Content;
                    SafeGuiWpf.ShowSuccess("Payment details Upload was successful");//, "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    await Task.Delay(3000);
                    return await Task.FromResult(res);
                }
                else if ((response != null))
                {
                    //System.Windows.MessageBox.Show(string.Format("Status code is {0} ({1}); response status is {2}", response.StatusCode, response.StatusDescription, response.ResponseStatus), "Upload Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    string RespMessage = string.Format("Status code is {0} ({1}); response status is {2}", response.StatusCode, response.StatusDescription, response.ResponseStatus);
                    SafeGuiWpf.ShowError("Update failed!\n" +
                                          RespMessage + "\n" +
                                                     "Check your Internet connection");
                    res = response.Content;
                    await Task.Delay(3000);
                    return await Task.FromResult(string.Empty);
                }
                
            }
            catch (Exception ex)
            {
              SafeGuiWpf.ShowError("Upload Error\n" +
                    ex.Message);//, "Upload Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                //res = response.Content;
                await Task.Delay(3000);
            }
            
            return await Task.FromResult(string.Empty);
        }


        public  async Task<string> UploadTemplate(string Json)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    using (var client = HttpClientInstance.AliInstance)
                    {
                        client.DefaultRequestHeaders.Accept.Clear();//Clear all headers
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/json"));//set the return type to JSON
                        string query = string.Format("template");
                        string resp = "";

                        var task = client.PostAsync(query, new StringContent(Json,
                                                                                Encoding.UTF8,
                                                                                "Application/json"))
                               .ContinueWith((taskwithresponse) =>
                               {
                                   var response = taskwithresponse.Result;
                                   var jsonString = response.Content.ReadAsStringAsync();
                                   jsonString.Wait();
                                   resp = jsonString.Result;
                               });
                        task.Wait();
                        return resp;
                    }
                }
                catch (Exception ex)
                {
                    SafeGuiWpf.ShowError(ex.Message + "\n" +
                         "Templates upload failed");
                    await Task.Delay(3000);
                }
                return string.Empty;
            });
        }


      
        public static async Task<string> FetchTotalSummary()
        {
            string res = "";

            try
            {
                var client = HttpClientInstance.AliInstance2;
                RestRequest request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("content-type", "application/json");
                request.Timeout = 60 * 100000;

                request.Resource = "template/summary";

                // Post the Data to the Server.
                dynamic response = client.Execute(request) as RestResponse;
                
                if ((response != null) && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    dynamic obj = response.Content;

                    res = response.Content;
                    await Task.Delay(3000);
                    return await Task.FromResult(res);
                }
                else if ((response != null))
                {
                    string RespMessage = string.Format("Status code is {0} ({1}); response status is {2}", response.StatusCode, response.StatusDescription, response.ResponseStatus);
                    SafeGuiWpf.ShowError("Download failed!\n" +
                                          RespMessage + "\n" +
                                                     "Check your Internet connection");
                    res = response.Content;
                    await Task.Delay(3000);
                    return await Task.FromResult(string.Empty);
                }

            }
            catch (Exception ex)
            {
                SafeGuiWpf.ShowError("Download Error\n" +
                      ex.Message);//, "Upload Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                //res = response.Content;
                await Task.Delay(3000);
            }

            return await Task.FromResult(string.Empty);
        }


        public static async Task<string> FetchTemplateBySchool(string Schnum)
        {
            string res = "";

            try
            {
                var client = HttpClientInstance.AliInstance2;
                RestRequest request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("content-type", "application/json");
                request.Timeout = 60 * 60000;

                request.Resource = string.Format("template/download/{0}/templates",Schnum);

                // Post the Data to the Server.
                dynamic response = client.Execute(request) as RestResponse;
                
                if ((response != null) && ((response.StatusCode == HttpStatusCode.OK) && (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    dynamic obj = response.Content;
                    res = response.Content;
                    await Task.Delay(3000);
                    return await Task.FromResult(res);
                }
                else if ((response != null))
                {
                    string RespMessage=string.Format("Status code is {0} ({1}); response status is {2}", response.StatusCode, response.StatusDescription, response.ResponseStatus);
                    SafeGuiWpf.ShowError("Download failed!\n" +
                                          RespMessage+"\n"+
                                                     "Check your Internet connection");//, "Upload error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    res = response.Content;
                    await Task.Delay(3000);
                    return await Task.FromResult(string.Empty);
                }
            }
            catch (Exception ex)
            {
                SafeGuiWpf.ShowError("Download Error\n" +
                      ex.Message);
                await Task.Delay(3000);
            }

            return await Task.FromResult(string.Empty);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                   // SafeGuiWpf.Unloaded();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NetworkClass() {
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
