using BioApp.Activities;
using BioApp.DB;
using BioApp.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace BioApp.Models
{
   /* public class ExportDataModel
    {
        public string operatorid { get; set; }
        public List<TemplatesModel> Templates { get; set; }
    }*/

    public class ExportRecordsLayer
    {
        public static void Export(List<FinModel> model)
        {
          LongActionDialog.ShowDialog("Eporting records ...",  Task.Run(async () =>
            {
                using (FetchDataClass fd = new FetchDataClass())
                {
                    var result = await fd.FetchTemplatesToUpload(model);
                    if (result.Count > 0)
                    {
                        var templatesUploadModel = new TemplatesUploadModel
                        {
                            cafe = CafeOperatorClass.GetId,
                            templates = result
                        };

                        string Json = JsonLayer.EncodeModelToJson<TemplatesUploadModel>(templatesUploadModel);
                        string Path = @"c:\export\ssce2018Biometrics\";
                        string FIleName = Path + CafeOperatorClass.GetId + "_" + SchoolDetailsClass.SchoolNo + ".neco";
                        if (!Directory.Exists(Path))
                            Directory.CreateDirectory(Path);

                        if (File.Exists(FIleName))
                        {
                            if (SafeGuiWpf.MsgBoxEx("Export data", "The file " + FIleName + " already exist, do you want to replace it", System.Windows.MessageBoxButton.YesNo, WpfMessageBox.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
                            {
                                JsonLayer.WriteJson(FIleName, Json);
                                SafeGuiWpf.ShowSuccess("Records exported successfully\n" +
                                                                 "Find the exported data in :" + Path);//, "Export data", System.Windows.MessageBoxButton.OK, WpfMessageBox.MessageBoxImage.Information);
                                return;
                            }

                        }
                        else
                        {
                            JsonLayer.WriteJson(FIleName, Json);
                            SafeGuiWpf.ShowSuccess("Records exported successfully\n" +
                                                             "Find the exported data in :" + Path);//, "Export data", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                        }
                    }
                    else
                    {
                        SafeGuiWpf.ShowError("Records not found for centre to export"); //, "Export data", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                    }
                }
            }));
        }
    }
}
