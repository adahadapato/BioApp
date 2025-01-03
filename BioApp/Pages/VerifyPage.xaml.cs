using BioApp.Activities;
using BioApp.DB;
using BioApp.Models;
using BioApp.Tools;
using GriauleFingerprintLibrary;
using GriauleFingerprintLibrary.DataTypes;
using GriauleFingerprintLibrary.Exceptions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BioApp
{
    /// <summary>
    /// Interaction logic for VerifyPage.xaml
    /// </summary>
    public partial class VerifyPage : UserControl
    {
        //Griaule SDK Variables Declaration
        public FingerprintCore refFingercore;
        private FingerprintRawImage rawImage;
        private FingerprintTemplate _template;

        private string subj_code = "";
        private string code = "";
        private string descript = "";
        private string paper = "";
        private bool IsSubjectSelected = false;
        private bool IscandidateVerified = false;
        public string CandidateNo = "";
        public string CandName = "";
        private string Sex = "";
        private string[] Subjects;

        System.Windows.Forms.ListView lvwFPReaders = new System.Windows.Forms.ListView();

        /// <summary>
        /// Constructor
        /// This takes the Griaule Finger SDK as argument
        /// and accepts it by reference.
        /// </summary>
        /// <param name="fingerPrint"></param>
        public VerifyPage(ref FingerprintCore fingerPrint)
        {
            InitializeComponent();
            this.refFingercore = fingerPrint;
        }

        private void VerifyPage_Loaded(object sender, RoutedEventArgs e)
        {
            refFingercore.StartEnroll();
            //int thresholdId = 80;
            //int rotationMaxId = 180;

            int thresholdVr = 20;
            int rotationMaxVr = 180;



            //refFingercore.SetIdentifyParameters(thresholdId, rotationMaxId);
            refFingercore.SetVerifyParameters(thresholdVr, rotationMaxVr);
            //Attach events here
            //refFingercore.onFinger += new FingerEventHandler(refFingercore_onFinger); //Trigererd when finger is placed on the reader
            refFingercore.onImage += new ImageEventHandler(refFingercore_onImage); //trigered when an image is captured.
            SafeGuiWpf.SetText(txtcand_name, "");
            SafeGuiWpf.SetText(txtsex, "");
            SafeGuiWpf.SetText(txtreg_no, "");
            SafeGuiWpf.SetText(txtsubjects, "");
            SafeGuiWpf.SetText(txtmsg, "");
            SafeGuiWpf.SetText(txtsch_name, "[" + SchoolDetailsClass.SchoolNo + "] " + SchoolDetailsClass.SchoolName);
            LoadSubjectToVerify();
        }


        private void Start()
        {
            SafeGuiWpf.SetText(txtsch_name, "[" + SchoolDetailsClass.SchoolNo + "] " + SchoolDetailsClass.SchoolName);
        }
        private void VerifyPage_UnLoaded(object sender, RoutedEventArgs e)
        {
            //Attach events here
            //refFingercore.onFinger -= new FingerEventHandler(refFingercore_onFinger); //Trigererd when finger is placed on the reader
            refFingercore.onImage -= new ImageEventHandler(refFingercore_onImage); //trigered when an image is captured.
        }

        private void LoadSubjectToVerify()
        {
            Task.Run(async() =>
            {
                using (FetchDataClass fd = new FetchDataClass())
                {
                    var result = await fd.FetchSubjectsToVerify();
                    if(result!=null)
                    {
                        UpdateSubjectListBox(result);
                    }
                }
                    
            });
        }

        string FetchSubjectDescription(string subj_code)
        {
            if (string.IsNullOrWhiteSpace(subj_code))
                return string.Empty;

            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return dl.FetchSubjectDescription(subj_code);
        }

        private void cmbSubjects_DropDownClosed(object sender, EventArgs e)
        {

            string txt = cmbSubjects.Text.Trim();
            var item = (sender as ComboBox).SelectedItem;
            if (item != null)
            {
                SubjectModel s = (SubjectModel)cmbSubjects.SelectedItem;
                IsSubjectSelected = true;
                subj_code = s.subj_code;
                code = s.Code;
                descript = s.Descript;
                paper = s.Paper;
            }
            SafeGuiWpf.ClearImage(imgpassport);
            SafeGuiWpf.SetText(txtcand_name, "");
            SafeGuiWpf.SetText(txtsubjects, "");
            SafeGuiWpf.SetText(txtsex, "");
            SafeGuiWpf.SetText(txtreg_no, "");
            SafeGuiWpf.SetText(txtmsg, "");
            CandidateNo = "";
            CandName = "";
        }
        #region Griaule SDK Core Functions

        /// <summary>
        /// Displays the finger image on the UI
        /// </summary>
        /// <param name="template"></param>
        /// <param name="identify"></param>
        private void DisplayImage(GriauleFingerprintLibrary.DataTypes.FingerprintTemplate template, bool identify)
        {
            IntPtr hdc = FingerprintCore.GetDC();
            IntPtr image = new IntPtr();
            try
            {
                if (identify)
                {
                    refFingercore.GetBiometricDisplay(template, rawImage, hdc, ref image, FingerprintConstants.GR_DEFAULT_CONTEXT);
                }
                else
                {
                    refFingercore.GetBiometricDisplay(template, rawImage, hdc, ref image, FingerprintConstants.GR_NO_CONTEXT);
                }
            }
            catch (Exception e)
            {
               MessageBox.Show(e.Message,"Display Image",MessageBoxButton.OK,MessageBoxImage.Error);
            }


            // SetImage(Bitmap.FromHbitmap(image));
            System.Drawing.Image im = System.Drawing.Bitmap.FromHbitmap(image);
            im.Save(@"c:\works\imagexx.bmp");
            FingerprintCore.ReleaseDC(hdc);
        }

        /// <summary>
        /// On status event function
        /// </summary>
        /// <param name="source"></param>
        /// <param name="se"></param>
        void refFingercore_onStatus(object source, GriauleFingerprintLibrary.Events.StatusEventArgs se)
        {
            if (se.StatusEventType == GriauleFingerprintLibrary.Events.StatusEventType.SENSOR_PLUG)
            {
                refFingercore.StartCapture(source.ToString());
                SetLvwFPReaders(se.Source, 0);
            }
            else
            {
                refFingercore.StopCapture(source);
                SetLvwFPReaders(se.Source, 1);
            }
        }

        /// <summary>
        /// On Image event function
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ie"></param>
        private void refFingercore_onImage(object source, GriauleFingerprintLibrary.Events.ImageEventArgs ie)
        {
            IscandidateVerified = false;
            if (!IsSubjectSelected)
            {
                //case "Warning":
                //ShowMessageBox("Select Subjects to continue...", "Select Subject","Error");
                SafeGuiWpf.ShowError("Error: Select Subjects to continue...");
                return;
            }
            try
            {
                SafeGuiWpf.SetText(txtmsg, "");
                rawImage = ie.RawImage;
                ExtractTemplate();
                Verify();
            }
            catch (Exception e)
            {
               SafeGuiWpf.ShowError("Error: " + e.Message);
            }

            System.Threading.Thread.Sleep(100);

        }

        public bool Verify(FingerprintTemplate queryTemplate, FingerprintTemplate referenceTemplate, out int verifyScore)
        {
            return refFingercore.Verify(queryTemplate, referenceTemplate, out verifyScore) == 1 ? true : false;
        }
        #endregion

        #region Griaule Helper Functions

        System.Windows.Forms.PictureBox pictBox = new System.Windows.Forms.PictureBox();
        /// <summary>
        /// Retrieves Templates from DB as compares with that captured from the reader.
        /// </summary>
        private void  Verify()
        {
            
            FingerprintTemplate testTemplate = null;
            try
            {
                if ((_template != null) && (_template.Size > 0))
                {
                    refFingercore.IdentifyPrepare(_template);
                    IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                    DataTable dt = dl.FetchTemplatesToVerify(SchoolDetailsClass.SchoolNo);
                   
                    if (dt.Rows.Count>0)
                    {
                        for(int i=0; i<dt.Rows.Count;i++)
                        {
                            CandidateNo = dt.Rows[i]["reg_no"].ToString();
                            byte[] buff = (byte[])dt.Rows[i]["template"];
                            int quality = Convert.ToInt32(dt.Rows[i]["quality"]);
                                                              
                            testTemplate = new FingerprintTemplate();

                            testTemplate.Size = buff.Length;
                            testTemplate.Buffer = buff;
                            testTemplate.Quality = quality;

                            int score;
                            if (Verify(_template, testTemplate, out score))
                            {
                                IscandidateVerified = true;
                                FetchPersonalInfoToPreview(CandidateNo);
                                return;
                            }
                        }
                       SafeGuiWpf.ClearImage(imgpassport);
                        SafeGuiWpf.SetText(txtcand_name, "");
                        SafeGuiWpf.SetText(txtsubjects, "");
                        SafeGuiWpf.SetText(txtsex, "");
                       SafeGuiWpf.SetText(txtreg_no, "");
                        CandidateNo = "";
                        CandName = "";
                       SafeGuiWpf.SetText(txtmsg, "Record not Found!");
                        SafeGuiWpf.SetEnabled(txtreg_no, true);
                    }
                    else
                    {
                       SafeGuiWpf.ShowError("Error: No Candidates available to verify...");
                    }
   
                }
            }
            catch (FingerprintException ge)
            {
               SafeGuiWpf.ShowError(ge.Message);
                if (ge.ErrorCode == -8)
                {
                    FileStream dumpTemplate = File.Create(@".\Dumptemplate.gt");
                    StreamWriter stWriter = new StreamWriter(dumpTemplate);

                    stWriter.WriteLine(BitConverter.ToString(testTemplate.Buffer, 0));
                    stWriter.Close();
                }
            }
            catch (Exception ex)
            {
               SafeGuiWpf.ShowError(ex.Message);
            }
        }

        bool Isverified = true;
        private void FetchPersonalInfoToPreview(string SerialNo)
        {
            Task.Run(async() =>
            {
                using (FetchDataClass fd = new FetchDataClass())
                {
                    var data = await fd.FetchPersonalInfoToPreview(SerialNo);
                    if(data!=null)
                    {
                        IscandidateVerified = true;
                        var Subj = new string[9];
                        Subjects = new string[9];
                        CandName =data.cand_name;
                        CandidateNo = data.reg_no;
                        Sex = data.sex;
                        ShowImage((byte[])data.passport);

                        Subjects[0] = FetchSubjectDescription(data.subj1);
                        Subjects[1] = FetchSubjectDescription(data.subj2);
                        Subjects[2] = FetchSubjectDescription(data.subj3);
                        Subjects[3] = FetchSubjectDescription(data.subj4);
                        Subjects[4] = FetchSubjectDescription(data.subj5);
                        Subjects[5] = FetchSubjectDescription(data.subj6);
                        Subjects[6] = FetchSubjectDescription(data.subj7);
                        Subjects[7] = FetchSubjectDescription(data.subj8);
                        Subjects[8] = FetchSubjectDescription(data.subj9);

                        Subj[0] = data.subj1;
                        Subj[1] = data.subj2;
                        Subj[2] = data.subj3;
                        Subj[3] = data.subj4;
                        Subj[4] = data.subj5;
                        Subj[5] = data.subj6;
                        Subj[6] = data.subj7;
                        Subj[7] = data.subj8;
                        Subj[8] = data.subj9;

                        DisplaySubjects(Subjects);
                        SafeGuiWpf.SetText(txtcand_name, CandName);
                        SafeGuiWpf.SetText(txtsex, Sex);
                        SafeGuiWpf.SetText(txtreg_no, SerialNo);
                        SafeGuiWpf.SetText(txtmsg, "");
                        //SystemSounds.Beep.Play();
                        if (!CheckCandidateSubjectsRegistered(Subj))
                        {
                            SafeGuiWpf.ShowWarning("This Candidate is not Registered for this Subject,\n thus cannot be verified");
                            //SafeGuiWpf.SetEnabled(btntnverified, false);
                            Isverified = false;
                        }
                    }
                    else
                    {
                        SafeGuiWpf.ShowError("Candidate record does not exsit");
                    }
                }
            });
        }
        private void ExtractTemplate()
        {
            _template = new FingerprintTemplate();
            if (rawImage != null)
            {
                try
                {
                    _template = null;
                    refFingercore.Extract(rawImage, ref _template);
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                  SafeGuiWpf.SetText(txtmsg,e.Message);
                }
            }
            else
            {
              SafeGuiWpf.SetText(txtmsg,"Finger not scanned !");
            }
        }

        private void SetLvwFPReaders(string readerName, int op)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                switch (op)
                {
                    case 0:
                        {
                            lvwFPReaders.Items.Add(readerName, readerName, 1);

                        }
                        break;
                    case 1:
                        {
                            lvwFPReaders.Items.RemoveByKey(readerName);

                        }
                        break;
                }
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                 DispatcherPriority.Background,
                 new Action(() => {
                     switch (op)
                     {
                         case 0:
                             {
                                 lvwFPReaders.Items.Add(readerName, readerName, 1);

                             }
                             break;
                         case 1:
                             {
                                 lvwFPReaders.Items.RemoveByKey(readerName);

                             }
                             break;
                     }
                 }));

            }
        }
        #endregion

        #region UI Display Helpers

        void UpdateSubjectListBox(List<SubjectModel> SubjectList)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                cmbSubjects.ItemsSource = SubjectList;
                cmbSubjects.DisplayMemberPath = "Descript" ;
                cmbSubjects.SelectedValuePath = "code";
                //cmbDevice.SelectedValue = "0";
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      cmbSubjects.ItemsSource = SubjectList;
                      cmbSubjects.DisplayMemberPath = "Descript";
                      cmbSubjects.SelectedValuePath = "code";
                      //cmbDevice.SelectedValue = "0";
                  }));
            }
        }
      
        private void ShowImage(byte[] bytes)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                imgpassport.Source = ProcessImageData.ConvertByteArrayToBitmapImage(bytes);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      imgpassport.Source = ProcessImageData.ConvertByteArrayToBitmapImage(bytes);
                  }));
            }
        }

        private void ShowFinger(byte[] bytes)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                imgFinger.Source = ProcessImageData.ConvertByteArrayToBitmapImage(bytes);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      imgFinger.Source = ProcessImageData.ConvertByteArrayToBitmapImage(bytes);
                  }));
            }
        }
        private void DisplaySubjects(string[] Subjects)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                txtsubjects.Text = "";
                foreach (string s in Subjects)
                    txtsubjects.Text += s + "\n";
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      txtsubjects.Text = "";
                      foreach (string s in Subjects)
                          txtsubjects.Text += s + "\n";
                  }));
            }

        }

        private Task<bool> Check(VerifyModel v)
        {
           return Task.Run(async() =>
            {
                using (FetchDataClass fd = new FetchDataClass())
                {
                    return await fd.IsCandidateVerified(v);
                }
            });
           
        }
        private void btnverified_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async() =>
            {
                if (!IscandidateVerified)
                {
                    SafeGuiWpf.ShowError("This action cannot be perfomred untill a Candidate is verified!");
                    return;
                }
                VerifyModel v = new VerifyModel
                {
                    schnum = SchoolDetailsClass.SchoolNo,
                    reg_no = CandidateNo,
                    cand_name=CandName,
                    code = code,
                    subj_code = subj_code,
                    subject = descript,
                    paper = paper,
                    Verified =(Isverified)? "1":"0",
                    status = "1",
                    remark = (Isverified)?"Present":"Present-Subject Not Registered",
                    verifyDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                };

                if (await Check(v))
                {
                    SafeGuiWpf.ShowWarning("Candidate Verification Status is already Updated!");
                    return;
                }
                using (WriteDataClass wd = new WriteDataClass())
                {
                    var result = await wd.SaveVerify(v);
                    if(result)
                    {
                        SafeGuiWpf.ShowSuccess("Candidate Verified Successfully!");
                    }
                }
            });
           
        }

        private void btnnotverified_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async() =>
            {
                if (!IscandidateVerified)
                {
                    SafeGuiWpf.ShowWarning("This action cannot be perfomred untill a Candidate is verified!");
                    return;
                }
                VerifyModel v = new VerifyModel
                {
                    schnum = SchoolDetailsClass.SchoolNo,
                    reg_no = CandidateNo,
                    cand_name= CandName,
                    code = code,
                    subj_code = subj_code,
                    subject = descript,
                    paper = paper,
                    Verified = "0",
                    status = "1",
                    verifyDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    remark="PRESENT IMPERSONATION"
                };
                if (await Check(v))
                {
                    SafeGuiWpf.ShowWarning("Candidate Verification Status is already Updated!");
                    return;
                }
                using (WriteDataClass wd = new WriteDataClass())
                {
                    var result = await wd.SaveVerify(v);
                    if (result)
                    {
                        SafeGuiWpf.ShowSuccess("Candidate Verification status updated Successfully!");
                    }
                }
            });
           
        }

        private bool CheckCandidateSubjectsRegistered(string[] Subjects)
        {
            return Subjects.Any(s =>
            s.Equals(subj_code, StringComparison.InvariantCultureIgnoreCase));
        }

       
        #endregion

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "";
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ae) =>
              {
                  OpenFileDialog openFileDialog = new OpenFileDialog
                  {
                      Filter = "NECO Biometrics files (*.neco)|*.neco|All files (*.*)|*.*",
                      Title = "NECO SSCE 2018 Biometrics",
                      DefaultExt = ".neco"
                  };
                  if (openFileDialog.ShowDialog() == true)
                  {
                      fileName = openFileDialog.FileName;
                  }
                  MainWindow w = new MainWindow();
                  w.Start(fileName);
              };

            worker.RunWorkerCompleted += (o, ae) =>
              {
                  
                  Start();
              };
            worker.RunWorkerAsync();
         
        }

        private void txtVreg_no_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtreg_no_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            var regNumber = SafeGuiWpf.GetText(txtreg_no);
            if(regNumber.Length==10)
            {
                FetchPersonalInfoToPreview(regNumber);
                return;
            }
            if(string.IsNullOrWhiteSpace(regNumber))
            {
                SafeGuiWpf.SetText(txtcand_name, "");
                SafeGuiWpf.SetText(txtsex, "");
                SafeGuiWpf.SetText(txtreg_no, "");
                SafeGuiWpf.ClearImage(imgpassport);
                SafeGuiWpf.SetText(txtsubjects, "");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            SafeGuiWpf.RemoveUserControlFromGrid((this.Parent as Grid), this);
        }
    }
}
