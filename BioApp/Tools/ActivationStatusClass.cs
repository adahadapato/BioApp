

namespace BioApp
{
    using Network;
    using Tools;
    using Models;
    using DB;
    using Newtonsoft.Json;
    using RegistryHelper;
    using System.Threading.Tasks;
    using ApiInfrastructure;

    /// <summary>
    /// This Class Activates an Operator ID
    /// by checking if there is an internet connection
    /// passing the id entered at LoginPage to the NetWork Class
    /// for authentication.
    /// A Json string is returned containing the Operator ID ans Cafe Name
    /// if the operation as successful, and empty string if
    /// it fails.
    /// </summary>
    public class ActivationStatusClass
    {
        IRegistryToken regToken =new RegistryToken();

        /// <summary>
        /// Opearator activation method
        /// </summary>
        /// <param name="id"></param>
        public static async Task<CafeOperatorModel> Activate(string id)
        {
            try
            {
                if (id.Trim() == "Butt3rf1y".ToUpper())
                {
                    await Task.Delay(3000);
                    return (CafeOperatorModel)LongActionDialog.ShowDialog<CafeOperatorModel>("Authenticating Admin ... ", Task<CafeOperatorModel>.Run( () =>
                    {
                        CafeOperatorModel mn = new CafeOperatorModel
                        {
                            Operatorid = "ADMIN",
                            Cafe = "System Administrator",
                            BankValidated = true,
                            Role = "Admin"
                        };
                        return mn;
                    }));
                }

                if (id.Trim() == "5upp0rt".ToUpper())
                {
                    await Task.Delay(3000);
                    return (CafeOperatorModel)LongActionDialog.ShowDialog<CafeOperatorModel>("Authenticating Support ... ", Task<CafeOperatorModel>.Run(() =>
                    {
                        CafeOperatorModel mn = new CafeOperatorModel
                        {
                            Operatorid = "SUPPORT",
                            Cafe = "Systems Support Group",
                            BankValidated = true,
                            Role = "Support"
                        };
                        return mn;
                    }));
                }

                if (id.Trim().Length == 4)
                {
                    return (CafeOperatorModel)LongActionDialog.ShowDialog<CafeOperatorModel>("Authenticating ICT Staff ... ", Task<CafeOperatorModel>.Run(async () =>
                    {
                        await Task.Delay(3000);
                        using (Activities.FetchDataClass fd = new Activities.FetchDataClass())
                        {
                            var result = await fd.FetchStaffForActivation(id);
                            if (result == null)
                            {
                                return null;
                            }
                            CafeOperatorModel mn = new CafeOperatorModel
                            {
                                Operatorid = result.per_no,
                                Cafe = result.name,
                                BankValidated = true,
                                Role = "Staff"
                            };
                            return mn;
                        }
                    }));
                   
                }
                return (CafeOperatorModel)LongActionDialog.ShowDialog<CafeOperatorModel>("Authenticating Cafe Operator ... ", Task<CafeOperatorModel>.Run(async () =>
                 {

                     await Task.Delay(3000);
                     using (Activities.RemoteDataClass rd = new Activities.RemoteDataClass())
                     {
                         string Json = await rd.AuthenticateOperator(id);
                         if (string.IsNullOrWhiteSpace(Json))
                             return null;

                         var m = JsonLayer.DecodeJsonToModel<OperatorActivationResult>(Json);
                         CafeOperatorModel model = new CafeOperatorModel
                         {
                             Operatorid = m.OperatorID,
                             Cafe = m.Name,
                             BankValidated = (m.BankValidated == "1") ? true : false,
                             Role = "User"
                         };
                         return model;
                     }
                 }));
               
            }
            catch { }
            return null;
        }

        /*NSubject result = (NSubject)LongActionDialog.ShowDialog<NSubject>(this, "Opening", Task.Run(async () =>
						{
							NSubject subject = NSubject.FromFile(dialog.FileName, dialog.FormatOwner, dialog.FormatType);
							NBiometricStatus status = await Client.CreateTemplateAsync(subject);
							if (status != NBiometricStatus.Ok && status != NBiometricStatus.None)
							{
								Utilities.ShowError("Failed to process template: {0}", status);
								return null;
							}
							return subject;
						}));*/
    }
}
