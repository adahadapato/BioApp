using BioApp.Activities;
using BioApp.DB;
using BioApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Helpers;
using System.IO;
using System.Windows.Media.Imaging;

namespace BioApp.Tools
{
    public class JsonLayer
    {
        // private static GriauleFingerprintLibrary.DataTypes.FingerprintRawImage rawImage;
        public static string CreateUploadErrorJson(TemplatesUploadResultModel model)
        {
            try
            {
                string Json = JsonConvert.SerializeObject(model);
                return Json;
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            return string.Empty;

        }
        public static List<TemplatesUploadResultModel> CreateUploadResultJson(string result)
        {
            var model = new List<TemplatesUploadResultModel>();
            var  t = JsonConvert.DeserializeObject<List<TemplatesUploadResultModel>>(result);
            foreach(var tm in t)
            {
                if(tm.status=="Success")
                {
                    model.Add(new TemplatesUploadResultModel
                    {
                        regNo=tm.regNo,
                        status=tm.status
                    });
                }
            }
            return model;
        }

        public static TDecode DecodeJsonToModel<TDecode>(string apiResponse)
        {
            return Json.Decode<TDecode>(apiResponse);
        }

        public static string EncodeModelToJson<T>(T model)
        {
            string Json = JsonConvert.SerializeObject(model);
            return Json;
        }
       
        public static void CreateSubjectsStorageJson(string FileName)
        {
            string Json = LoadJson(FileName);
            try
            {
                SubjectsModel model= JsonConvert.DeserializeObject<SubjectsModel>(Json);
                List<SubjectModel> subj = new List<SubjectModel>();
                if (!string.IsNullOrWhiteSpace(Json) && model != null)
                {
                    foreach (var s in model.Subjects)
                    {
                        subj.Add(new SubjectModel
                        {
                            Code=s.Code,
                            subj_code=s.subj_code,
                            Subject=s.Subject,
                            Paper=s.Paper,
                            Descript=s.Descript,
                            Date=s.Date,
                            StartTime=s.StartTime,
                            StopTime=s.StopTime
                        });
                    }
                }

                IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                dl.SaveDataToSubject(subj);
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        public static void CreateBankStorageJson(string FileName)
        {
            string Json = LoadJson(FileName);
            try
            {
                List<BankSaveModel> model = JsonConvert.DeserializeObject<List<BankSaveModel>>(Json);
                List<BankSaveModel> bank = new List<BankSaveModel>();
                if (!string.IsNullOrWhiteSpace(Json) && model != null)
                {
                    foreach (var s in model)
                    {
                        bank.Add(new BankSaveModel
                        {
                           bank_code = s.bank_code,
                           bank_name = s.bank_name
                        });
                    }
                }

                using (WriteDataClass wd = new WriteDataClass())
                {
                    IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                    dl.SaveDataToBank(bank);
                }
                   
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        public static void CreateStateStorageJson(string FileName)
        {
            string Json = LoadJson(FileName);
            try
            {
                List<StateModel> model = JsonConvert.DeserializeObject<List<StateModel>>(Json);
                List<StateModel> state = new List<StateModel>();
                if (!string.IsNullOrWhiteSpace(Json) && model != null)
                {
                    foreach (var s in model)
                    {
                        state.Add(new StateModel
                        {
                           code = s.code,
                           state = s.state
                        });
                    }
                }

                using (WriteDataClass wd = new WriteDataClass())
                {
                    IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                    dl.SaveDataToState(state);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }


        public static void CreateStaffStorageJson(string FileName)
        {
            string Json = LoadJson(FileName);
            try
            {
                StaffRootObject model = JsonConvert.DeserializeObject<StaffRootObject>(Json);
                List<StaffModel> staff = new List<StaffModel>();
                if (!string.IsNullOrWhiteSpace(Json) && model != null)
                {
                    foreach (var s in model.staff)
                    {
                        staff.Add(new StaffModel
                        {
                            per_no = s.per_no,
                            name = s.name,
                        });
                    }
                }

                IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                dl.SaveDataToStaff(staff);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

       /* public static string CreateExportJson(ExportDataModel model)
        {
            string Json = JsonConvert.SerializeObject(model);
            return Json;
        }*/
        public static void CreateTemplatesStorageJson(string FileName,  out List<PersonalInfoModel> p, out List<TemplatesModel> t)
        {
            string Json = LoadJson(FileName,true);
            t = new List<TemplatesModel>();
            p = new List<PersonalInfoModel>();
           
            try
            {

                    DataDownloadModel model = JsonConvert.DeserializeObject<DataDownloadModel>(Json);
                    if (!string.IsNullOrWhiteSpace(Json) && model != null)
                    {
                    SchoolDetailsClass.SchoolName = model.SchoolName;
                    SchoolDetailsClass.SchoolNo = model.SchoolNo;
                        foreach (var PersonalInfo in model.PersonalInfo)
                        {
                            System.Drawing.Image img = ProcessImageData.stringToImage(PersonalInfo.passport);
                            BitmapSource bmp = ProcessImageData.ToBitmapSource(img);
                            p.Add(
                                new PersonalInfoModel
                                {
                                    schnum = SchoolDetailsClass.SchoolNo,
                                 //sch_name=PersonalInfo.sch_name,
                                 reg_no = PersonalInfo.reg_no,
                                    cand_name = PersonalInfo.cand_name,
                                    sex = PersonalInfo.sex,
                                    passport = ProcessImageData.ConvertBitmapSourceToByteArray(bmp),
                                    subj1 = PersonalInfo.subj1,
                                    subj2 = PersonalInfo.subj2,
                                    subj3 = PersonalInfo.subj3,
                                    subj4 = PersonalInfo.subj4,
                                    subj5 = PersonalInfo.subj5,
                                    subj6 = PersonalInfo.subj6,
                                    subj7 = PersonalInfo.subj7,
                                    subj8 = PersonalInfo.subj8,
                                    subj9 = PersonalInfo.subj9
                                });
                        }

                        foreach (var Template in model.Templates)
                        {
                            t.Add(new TemplatesModel
                            {
                                schnum = Template.schnum,
                                reg_no = Template.reg_no,
                                finger = null,
                                template = ProcessImageData.StringToImage(Template.template.ToString()),
                                quality = Template.quality
                            });
                            /*string fileName = @"c:\works\extract\" + Template.reg_no;
                            GrCaptureImageFormat imformat = new GrCaptureImageFormat();
                            imformat = GrCaptureImageFormat.GRCAP_IMAGE_FORMAT_BMP; 
                            EnrollPage.FingerCore.SaveRawImageToFile(rawImage, fileName, imformat);*/
                        }
                    }
                
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
               
        }


        public static string CreateOperatorJson(CafeOperatorModel m)
        {
            string Json = JsonConvert.SerializeObject(m);
            return Json;
        }


        public static string CreateOperatorBankDetailsJson(UpdateBankDetailsModel m)
        {
            string Json = JsonConvert.SerializeObject(m);
            return Json;
        }

        public static UpdateBankDetailsResult CreateOperatorBankDetailsResultJson(string Json)
        {
            var result = JsonConvert.DeserializeObject<UpdateBankDetailsResult>(Json);
            return result;
        }
        //UpdateBankDetailsResult
        public static CafeOperatorModel FetchOperator(string Json)
        {
            if(!string.IsNullOrWhiteSpace(Json))
            {
                OperatorActivationResult m = JsonConvert.DeserializeObject<OperatorActivationResult>(Json);
                CafeOperatorModel model = new CafeOperatorModel
                {
                    Operatorid = m.OperatorID,
                    Cafe = m.Name,
                    BankValidated = (m.BankValidated == "1") ? true : false
                };
                return model;
            }
            return null;
        }
        public static string LoadJson(string FileName)
        {
            using (StreamReader r = new StreamReader(FileName))
            {
                string json = r.ReadToEnd();
                return json;
            }
        }

        public static string LoadJson(string FileName, bool Unzip)
        {
            using (StreamReader r = new StreamReader(FileName))
            {
                string json = r.ReadToEnd();
                return GZipClassLayer.DeCompress(json);
            }
        }

        public static void WriteJson(string FileName, string Json)
        {
            File.WriteAllText(FileName, Json);
        }
    }
}
