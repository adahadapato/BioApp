using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;


//Griaule SDK Libs.
using GriauleFingerprintLibrary;
using GriauleFingerprintLibrary.Exceptions;
using System.Windows.Threading;
using System.IO;
using BioApp.DB;
using System.Data;
using BioApp.Models;
using System.ComponentModel;
using BioApp.Tools;
using BioApp.Activities;
using System.Threading.Tasks;
using System.Windows.Controls.DataVisualization.Charting;

namespace BioApp
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// This Class is Solely to Capture, Compare and Store Finger 
    /// Prints and thier corresponding extracted Templates
    /// Using Griaule Finger Print SDK 2009
    /// </summary>
    public partial class EnrollPage : UserControl
    {

        //Griaule SDK Variables Declaration
        public FingerprintCore refFingercore;
        private GriauleFingerprintLibrary.DataTypes.FingerprintRawImage rawImage;
        GriauleFingerprintLibrary.DataTypes.FingerprintTemplate _template;
        GrCaptureImageFormat imformat = new GrCaptureImageFormat();
        

        //Other Local Variables
        private int fingerCount = 1;

        public string CandidateNo = "";
        //private string schnum = "";
        //private string schName = "";
        public string CandName = "";
        private string Sex = "";
        private string[] Subjects;

       
        private int TickCount = 0;

        private bool IsReady = false;
        private bool isCandidateSelected = false;
        private bool isCaptureInProcess = false;

        System.Windows.Forms.ListView lvwFPReaders = new System.Windows.Forms.ListView();

        /// <summary>
        /// Here we are passing the already initialized 
        /// Griaule SDK to this Class
        /// so we don't have to re-initialize it a second time.
        /// </summary>
        /// <param name="fingerPrint"></param> Griaul SDK
        ///
        public EnrollPage(ref FingerprintCore fingerPrint)
        {
            InitializeComponent();
            this.refFingercore = fingerPrint;
        }

        public EnrollPage()
            //:this()
        {
            
        }
        private void EnrollPage_Loaded(object sender, RoutedEventArgs e)
        {
            imformat = GrCaptureImageFormat.GRCAP_IMAGE_FORMAT_BMP;
            //Attach events here
            refFingercore.onFinger += new FingerEventHandler(refFingercore_onFinger); //Trigererd when finger is placed on the reader
            refFingercore.onImage += new ImageEventHandler(refFingercore_onImage); //trigered when an image is captured.
                                                                                   // lv.SelectedIndexChanged += new EventHandler(lv_SelectedIndexChanged);

            int thresholdId = 80;
            int rotationMaxId = 180;

            //int thresholdVr = 10;
            //int rotationMaxVr = 90;


            refFingercore.SetIdentifyParameters(thresholdId, rotationMaxId);
            //fingerPrint.SetVerifyParameters(thresholdVr, rotationMaxVr);
            LoadData("", false);
            SafeGuiWpf.SetText(txtsubjects, "");
            SafeGuiWpf.SetText(txtCand_name, "");
            //SafeGuiWpf.SetText(txtmsg, "");
            SafeGuiWpf.SetText(txtsch_name, "[" + SchoolDetailsClass.SchoolNo + "] " + SchoolDetailsClass.SchoolName);

            //LoadColumnChartData();
            // txtsch_name.Text = MainWindow.SchoolNo;
        }

        private void EnrollPage_UnLoaded(object sender, RoutedEventArgs e)
        {
            //Detach the attached events
            refFingercore.onFinger -= new FingerEventHandler(refFingercore_onFinger); //Trigererd when finger is placed on the reader
            refFingercore.onImage -= new ImageEventHandler(refFingercore_onImage); //trigered when an image is captured.
            // lv.SelectedIndexChanged += new EventHandler(lv_SelectedIndexChanged);
        }

        private async void LoadColumnChartData()
        {
            try
            {
                await Task.Run(async () =>
                {
                    int[] values = await FetchChartValues();

                    var X = new KeyValuePair<string, int>[]{
                     new KeyValuePair<string,int>("Captured", values[1]),
                     new KeyValuePair<string,int>("Pending", values[0]) };

                    SafeGuiWpf.SetItems(mcChart, X);
                });
               

                /* int[] values = await FetchChartValues();
                 ((ColumnSeries)mcChart.Series[0]).ItemsSource =
                     new KeyValuePair<string, int>[]{
                     new KeyValuePair<string,int>("Captured", values[1]),
                     new KeyValuePair<string,int>("Pending", values[0]) };*/
            }
            catch(Exception)// ex)
            {
               // MessageBox.Show(ex.Message);
            }
           
        }

        private async Task<int[]> FetchChartValues()
        {
            int[] values = new int[2];
            using (FetchDataClass fd = new FetchDataClass())
            {
                values[0] = await fd.FetchTotalCount(SchoolDetailsClass.SchoolNo);
                values[1] = await fd.FetchCapturedCount(SchoolDetailsClass.SchoolNo);
            }
            return values;
        }

        #region Griaule SDK Core Functions
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
            if (!isCandidateSelected)
            {
                SafeGuiWpf.ShowWarning("Error: Select Candidate");
                return;
            }
            if (!IsReady)
            {

                SafeGuiWpf.ShowWarning("Click Start Capture when ready");
                return;
            }
            try
            {

                //SafeGuiWpf.SetText(txtmsg, "");
                //SetFingerStatus(ie.Source, 2);
                rawImage = ie.RawImage;
                ExtractTemplate();
                //Identify();
                _template = new GriauleFingerprintLibrary.DataTypes.FingerprintTemplate();
                int ret = (int)refFingercore.Enroll(rawImage, ref _template, GrTemplateFormat.GR_FORMAT_DEFAULT, FingerprintConstants.GR_DEFAULT_CONTEXT);
                if (ret >= FingerprintConstants.GR_ENROLL_SUFFICIENT && TickCount == 4)
                {
                    Identify();
                    TickCount = 0;
                    SafeGuiWpf.SetImage(Tick, "tick4.png");
                }

            }
            catch (Exception e)
            {
               SafeGuiWpf.ShowError("Error: " + e.Message);
            }

            System.Threading.Thread.Sleep(100);

        }

        void refFingercore_onFinger(object source, GriauleFingerprintLibrary.Events.FingerEventArgs fe)
        {
            try
            {
                switch (fe.EventType)
                {
                    case GriauleFingerprintLibrary.Events.FingerEventType.FINGER_DOWN:
                        {
                            //SetStatusMessage("");
                            SetFingerStatus(fe.Source, 0);
                            //pict_device_status.BackColor = Color.WhiteSmoke;
                            SafeGuiWpf.SetImage(imgFinger, "red_dot.png");
                            //ShowFingerProgress(imgFinger,"red_dot.png");
                        }
                        break;
                    case GriauleFingerprintLibrary.Events.FingerEventType.FINGER_UP:
                        {
                            SafeGuiWpf.SetImage(imgFinger, "black_dot.png");
                            SetFingerStatus(fe.Source, 1);
                        }
                        break;
                }
            }
            catch(Exception e)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(e.Message ,"On Finger");
            }

        }

        /// <summary>
        /// Verifies that finger ptiny is unique
        /// </summary>
        /// <param name="queryTemplate"></param>
        /// <param name="referenceTemplate"></param>
        /// <param name="verifyScore"></param>
        /// <returns></returns>
        public bool Verify(GriauleFingerprintLibrary.DataTypes.FingerprintTemplate queryTemplate, GriauleFingerprintLibrary.DataTypes.FingerprintTemplate referenceTemplate, out int verifyScore)
        {
            return refFingercore.Verify(queryTemplate, referenceTemplate, out verifyScore) == 1 ? true : false;
        }

        /// <summary>
        /// Used to identify a finger print.
        /// Not neccessarily for storage reasons.
        /// </summary>
        /// <param name="testTemplate"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        private bool Identify(GriauleFingerprintLibrary.DataTypes.FingerprintTemplate testTemplate, out int score)
        {
            return refFingercore.Identify(testTemplate, out score) == 1 ? true : false;
        }

        /// <summary>
        /// Displays the finger image on the UI
        /// </summary>
        /// <param name="template"></param>
        /// <param name="identify"></param>
        private void DisplayImage(GriauleFingerprintLibrary.DataTypes.FingerprintTemplate template, bool identify, string fileName)
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
               MessageBox.Show(e.Message);
            }

           

            // SetImage(Bitmap.FromHbitmap(image));
            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Bitmap.FromHbitmap(image));
            //im.Save(@"c:\export\ssce2017biometrics\"+ fileName+".bmp");
            FingerprintCore.ReleaseDC(hdc);
        }
        #endregion

        #region Griaule SDK Helper FUnctions
        
       

        private async void LoadData(string CandidateName, bool WithSearch)
        {
            await Task.Run(async () =>
            {
                if (string.IsNullOrWhiteSpace(SchoolDetailsClass.SchoolNo))
                {
                    SafeGuiWpf.UMsgBox(this, "Please select a School file to capture fingers", "Capture Finger", MessageBoxButton.OK, MessageBoxImage.Error);
                    SafeGuiWpf.RemoveUserControlFromGrid((this.Parent as Grid), this);
                    //SafeGuiWpf.SetVisible(this, Visibility.Collapsed);
                    return;
                }
                try
                {
                    using (FetchDataClass fd = new FetchDataClass())
                    {
                        var result = await fd.FetchRecordsForDisplay(SchoolDetailsClass.SchoolNo, CandidateName, WithSearch);
                        SafeGuiWpf.SetItems<PersonalInfoModel>(lv, result);
                        LoadColumnChartData();
                    }
                }
                catch (Exception e)
                {
                    SafeGuiWpf.UMsgBox(this, e.Message, "Load data", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
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


       
        private void SetFingerStatus(string readerName, int status)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                System.Windows.Forms.ListViewItem[] listItens = lvwFPReaders.Items.Find(readerName, false);
                System.Threading.Thread.BeginCriticalRegion();
                foreach (System.Windows.Forms.ListViewItem item in listItens)
                {

                    switch (status)
                    {
                        case 0:
                            {
                                item.ImageIndex = 0;
                                //SetCandidateName(status.ToString());
                            }
                            break;
                        case 1:
                            {
                                item.ImageIndex = 1;
                                //SetStatusMessage(status.ToString(), c);
                                //SetCandidateName(status.ToString());
                            }
                            break;
                        case 2:
                            {
                                item.ImageIndex = 2;
                                //SetCandidateName(status.ToString());
                            }
                            break;

                    }
                }
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                     System.Windows.Forms.ListViewItem[] listItens = lvwFPReaders.Items.Find(readerName, false);
                      System.Threading.Thread.BeginCriticalRegion();
                      foreach (System.Windows.Forms.ListViewItem item in listItens)
                      {

                          switch (status)
                          {
                              case 0:
                                  {
                                      item.ImageIndex = 0;
                                      //SetCandidateName(status.ToString());
                                  }
                                  break;
                              case 1:
                                  {
                                      item.ImageIndex = 1;
                                      //SetStatusMessage(status.ToString(), c);
                                      //SetCandidateName(status.ToString());
                                  }
                                  break;
                              case 2:
                                  {
                                      item.ImageIndex = 2;
                                      //SetCandidateName(status.ToString());
                                  }
                                  break;

                          }
                      }
                  }));


                
                //System.Threading.Thread.BeginCriticalRegion();
                System.Threading.Thread.Sleep(300);
                System.Threading.Thread.EndCriticalRegion();
            }
        }
        private void ExtractTemplate()
        {

            if (rawImage != null)
            {
                try
                {
                    _template = null;
                    refFingercore.Extract(rawImage, ref _template);
                    TickCount++;
                    switch (TickCount)
                    {
                        case 1:
                            SafeGuiWpf.SetImage(Tick,"tick3.png");
                            break;
                        case 2:
                            SafeGuiWpf.SetImage(Tick, "tick2.png");
                            break;
                        case 3:
                            SafeGuiWpf.SetImage(Tick, "tick1.png");
                            break;
                        case 4:
                            SafeGuiWpf.SetImage(Tick, "tick0.png");
                            break;
                    }

                }
                catch (Exception e)
                {
                   SafeGuiWpf.UMsgBox(this, "Extract Template", e.Message,MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
               SafeGuiWpf.ShowError("Error: Finger not Captured !");
            }
        }

        /// <summary>
        /// Show progress as finger is enrolled
        /// </summary>
        /// <param name="FingerIndex"></param>
        private void ShiftFinger(int FingerIndex)
        {
            switch (FingerIndex)
            {
                case 1:
                    SafeGuiWpf.SetImage(imgRightHand,"right_thump_red.jpg");
                    //ShowFingerProgress(imgLeftHand, "left_index_red.jpg");
                    break;
                case 2:
                    SafeGuiWpf.SetImage(imgRightHand,"right_all_green.jpg");
                    SafeGuiWpf.SetImage(imgLeftHand,"left_thump_red.jpg");
                    break;
                case 3:
                    SafeGuiWpf.SetImage(imgLeftHand,"left_index_red.jpg");
                    break;
                case 4:
                    SafeGuiWpf.SetImage(imgLeftHand,"left_all_green.jpg");
                    break;
            }
        }

        int imCount = 0;
        private void GrabImage(int imCount)
        {
            string fileName = AppPathClass.FetcAppPath()+@"\" + CandidateNo + imCount.ToString() + ".bmp";
            refFingercore.SaveRawImageToFile(rawImage, fileName, imformat);
            pictBox.Image = System.Drawing.Image.FromFile(fileName);
        }

        System.Windows.Forms.PictureBox pictBox = new System.Windows.Forms.PictureBox();
        /// <summary>
        /// Retrieves Templates from DB as compares with that captured from the reader.
        /// </summary>
        private void Identify()
        {
            GriauleFingerprintLibrary.DataTypes.FingerprintTemplate testTemplate = null;
            try
            {
                if ((_template != null) && (_template.Size > 0))
                {
                    isCaptureInProcess = true;
                    refFingercore.IdentifyPrepare(_template);
                    IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                    IDataReader dataReader = dl.FetchTemplate(SchoolDetailsClass.SchoolNo);
                    
                    int NumberOfRecords = 0;// Number of fingers captured for the candidates
                    using (dataReader)
                    {
                        while (dataReader.Read())
                        {
                            //int tempId = Convert.ToInt32(dataReader["ID"]);
                            if (dataReader["reg_no"].ToString() == CandidateNo)
                            {
                                NumberOfRecords++;
                            }

                            if (NumberOfRecords >= 4)
                            {
                                SafeGuiWpf.UMsgBox(this, "Too Many Fingers","Candidate has all finger prints captured.\n" +
                                                "Click Reset to Cancel the Finger prints and capture the candidate all over",MessageBoxButton.OK, MessageBoxImage.Warning);
                                refFingercore.StartEnroll();
                                return;
                            }

                            byte[] buff = (byte[])dataReader["template"];
                            int quality = Convert.ToInt32(dataReader["quality"]);

                            testTemplate = new GriauleFingerprintLibrary.DataTypes.FingerprintTemplate();
                           

                            testTemplate.Size = buff.Length;
                            testTemplate.Buffer = buff;
                            testTemplate.Quality = quality;
                            string id = dataReader["reg_no"].ToString();
                            //DisplayImage(testTemplate, false, id);
                            int score;
                            if (Verify(_template, testTemplate, out score))
                            {
                                SafeGuiWpf.ShowError("Error: Duplicate fingers found !");
                                
                                refFingercore.StartEnroll();
                                return;
                            }

                        }
                    }
                    //ChangeEnabledStateForPersonalInfoListView(false);
                    //IsSaved = false;
                    //string ImgFileName = @"c:\export\ssce2017biometrics\img.bmp";
                    
                    if (fingerCount <= 4)
                    {
                        isCaptureInProcess = true;
                        GrabImage(imCount);
                        TemplatesModel t = new TemplatesModel
                        {
                            schnum= SchoolDetailsClass.SchoolNo,
                            reg_no= CandidateNo,
                            finger= ProcessImageData.ConvertFingerPrintsToByte(ref pictBox),
                            template=_template.Buffer,
                            quality=_template.Quality
                            
                        };
                        
                        SaveTemplate(t);
                        ShiftFinger(fingerCount);
                        //SafeGuiWpf.SetText(txtmsg,"");
                        fingerCount++;
                        imCount++;
                        refFingercore.StartEnroll();
                    }
                   
                    if (fingerCount == 5)
                    {
                        imCount = 0;
                        isCaptureInProcess = false;
                        isCandidateSelected = false;
                        //ChangeEnabledStateForPersonalInfoListView(true);
                        
                        SafeGuiWpf.ClearImage(imgpassport);
                        SafeGuiWpf.SetText(txtCand_name,"");
                        fingerCount = 1;
                        SafeGuiWpf.ShowSuccess("Fingers Captured");
                        SafeGuiWpf.SetImage(imgLeftHand,"left_hand.jpg");
                        SafeGuiWpf.SetImage(imgRightHand,"right_hand.jpg");
                        IsReady = false;
                        SafeGuiWpf.SetText(txtsubjects, "");
                        LoadData("",false);
                    }

                }
            }
            catch (FingerprintException ge)
            {
              SafeGuiWpf.UMsgBox(this,"Identify", ge.Message, MessageBoxButton.OK,MessageBoxImage.Error);
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
                SafeGuiWpf.UMsgBox(this, "Identify", ex.Message + " " + ex.StackTrace, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


       
        private async void SaveTemplate(TemplatesModel t)
        {
            try
            {
                if (_template == null)
                {
                    SafeGuiWpf.ShowError("No Templates Extracted");
                    return;
                }
                using (WriteDataClass wd = new WriteDataClass())
                {
                   await wd.SaveTemplate(t);
                }
            }
            catch { }
        }
        #endregion
        #region Display Messages and Others To UI


       /* private void DisplaySubjects(string[] Subjects)
        {
            txtsubjects.Text = "";
            if (Application.Current.Dispatcher.CheckAccess())
            {
                
                foreach (string s in Subjects)
                    txtsubjects.Text += s + "\n";
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      foreach (string s in Subjects)
                          txtsubjects.Text += s + "\n";
                  }));
            }

        }*/

       
        private void ClearImage()
        {

            if (Application.Current.Dispatcher.CheckAccess())
            {
                imgpassport.Source =null;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      imgpassport.Source =null;
                  }));
            }

        }

      

        void ChangeEnabledStateForPersonalInfoListView(bool state)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                lv.Focusable = state;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      lv.Focusable = state;
                  }));
            }
        }

       

        /// <summary>
        /// Diaplay the Tick image as finger is consolidated
        /// </summary>
        /// <param name="img"></param>
        /// <param name="Source"></param>
       /* private void ShowTickStatus(Image img, string Source)
        {
            var imgUri = new Uri("pack://application:,,,/BioApp;component/Images/" + Source, UriKind.RelativeOrAbsolute);
            if (Application.Current.Dispatcher.CheckAccess())
            {
                img.Source = new BitmapImage(imgUri);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      img.Source = new BitmapImage(imgUri);
                  }));
            }

        }*/

        /// <summary>
        /// Displays Finger capture Progress.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="Source"></param>
       /* private void ShowFingerProgress(Image img, string Source)
        {
            var imgUri = new Uri("pack://application:,,,/BioApp;component/Images/" + Source, UriKind.RelativeOrAbsolute);
            if (Application.Current.Dispatcher.CheckAccess())
            {
                img.Source = new BitmapImage(imgUri);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      img.Source = new BitmapImage(imgUri);
                  }));
            }

        }*/
        
       
        #endregion

        private void btnstart_Click(object sender, RoutedEventArgs e)
        {
            if (isCaptureInProcess == true)
            {
               SafeGuiWpf.ShowWarning("Error: Capture is already in progress");
                return;
            }
            refFingercore.StartEnroll();

            SafeGuiWpf.SetImage(imgLeftHand,"left_hand.jpg");
            SafeGuiWpf.SetImage(imgRightHand,"right_index_red.jpg");
            //SafeGuiWpf.SetText(txtmsg,"");
            IsReady = true;
        }

        private void txtsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Name = txtsearch.Text;
            LoadData(Name,true);
        }

        private void btnreset_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CandidateNo) || !isCaptureInProcess)
            {
               SafeGuiWpf.ShowError("There is no enrollment action to reset");
                return;
            }
        
            string Message = "Click 'Yes' to delete the candidate : "+CandName+" finger prints\n"+
                             "'No' to reset the capture process, OR 'Cancel' to continue\n"+
                             "Do you want to delete the candidate finger prints ";
            MessageBoxResult result = SafeGuiWpf.UMsgBox(this, "Reset", Message,  MessageBoxButton.YesNoCancel, WpfMessageBox.MessageBoxImage.Question);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    DeleteTemplate();
                    break;
                case MessageBoxResult.No:
                    ResetCapture();
                    break;
                case MessageBoxResult.Cancel:
                    return;
            }
        }

        private async void ResetCapture()
        {
            await Task.Run(() =>
            {
                CandidateNo = "";
                CandName = "";
                SafeGuiWpf.SetImage(imgLeftHand, "left_hand.jpg");
                SafeGuiWpf.SetImage(imgRightHand, "right_hand.jpg");
                SafeGuiWpf.SetImage(Tick, "tick4.png");
                fingerCount = 1;
                TickCount = 0;
                //SafeGuiWpf.SetText(txtmsg, "");
                SafeGuiWpf.ClearImage(imgpassport);
                SafeGuiWpf.SetText(txtCand_name, "");
                SafeGuiWpf.SetText(txtsubjects, "");
                isCaptureInProcess = false;
                IsReady = false;
                isCandidateSelected = false;

                refFingercore.StartEnroll();
            });
           
        }
        private async void DeleteTemplate()
        {
            using (WriteDataClass wd = new WriteDataClass())
            {
                var result = await wd.DeteTemplates(SchoolDetailsClass.SchoolNo, CandidateNo);
                if(result)
                {
                    CandidateNo = "";
                    CandName = "";
                    SafeGuiWpf.SetImage(imgLeftHand, "left_hand.jpg");
                    SafeGuiWpf.SetImage(imgRightHand, "right_hand.jpg");
                    SafeGuiWpf.SetImage(Tick, "tick4.png");
                    fingerCount = 1;
                    TickCount = 0;
                    //SafeGuiWpf.SetText(txtmsg, "");
                    SafeGuiWpf.ClearImage(imgpassport);
                    SafeGuiWpf.SetText(txtCand_name, "");
                    SafeGuiWpf.SetText(txtsubjects, "");
                    isCaptureInProcess = false;
                    IsReady = false;
                    isCandidateSelected = false;

                    refFingercore.StartEnroll();
                }
            }
        }

        /// <summary>
        /// This uses the up and down arrow keys to select 
        /// Candidates to capture finger print
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lv_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (isCaptureInProcess == true)
            {
               SafeGuiWpf.ShowError("Error: Capture is already in progress");
                e.Handled = false;
                return;
            }
            if (e.Key == Key.Up)//Up arrow key
            {
                e.Handled = true;
                if (lv.SelectedItems.Count > 0)
                {
                    int indexToSelect = lv.Items.IndexOf(lv.SelectedItems[0]) - 1;
                    if (indexToSelect >= 0)
                    {
                        lv.SelectedItem = lv.Items[indexToSelect];
                        lv.ScrollIntoView(lv.SelectedItem);
                        //
                        Subjects = new string[9];
                        PersonalInfoModel p = (PersonalInfoModel)lv.SelectedItems[0];
                        CandidateNo = p.reg_no;
                        CandName = p.cand_name;
                        Sex = p.sex;
                        SafeGuiWpf.SetImage(imgpassport, p.passport);
                        Subjects[0] = p.subj1;
                        Subjects[1] = p.subj2;
                        Subjects[2] = p.subj3;
                        Subjects[3] = p.subj4;
                        Subjects[4] = p.subj5;
                        Subjects[5] = p.subj6;
                        Subjects[6] = p.subj7;
                        Subjects[7] = p.subj8;
                        Subjects[8] = p.subj9;
                        SafeGuiWpf.SetText(txtsubjects, Subjects);
                        txtCand_name.Text = CandName;
                        isCandidateSelected = true;
                        //SafeGuiWpf.SetText(txtmsg, "");
                        return;
                    }
                }
            }
            else if (e.Key == Key.Down)//Down arrow key
            {
                e.Handled = true;
                if (lv.SelectedItems.Count > 0)
                {
                    int indexToSelect = lv.Items.IndexOf(lv.SelectedItems[lv.SelectedItems.Count - 1]) + 1;
                    if (indexToSelect < lv.Items.Count)
                    {
                        lv.SelectedItem = lv.Items[indexToSelect];
                        lv.ScrollIntoView(lv.SelectedItem);

                        //
                        Subjects = new string[9];
                        PersonalInfoModel p = (PersonalInfoModel)lv.SelectedItems[0];
                        CandidateNo = p.reg_no;
                        CandName = p.cand_name;
                        Sex = p.sex;
                        SafeGuiWpf.SetImage(imgpassport, p.passport);
                        
                        Subjects[0] = p.subj1;
                        Subjects[1] = p.subj2;
                        Subjects[2] = p.subj3;
                        Subjects[3] = p.subj4;
                        Subjects[4] = p.subj5;
                        Subjects[5] = p.subj6;
                        Subjects[6] = p.subj7;
                        Subjects[7] = p.subj8;
                        Subjects[8] = p.subj9;
                        SafeGuiWpf.SetText(txtsubjects, Subjects);
                        SafeGuiWpf.SetText(txtCand_name, CandName);
                        isCandidateSelected = true;
                        //SafeGuiWpf.SetText(txtmsg, "");
                        return;
                    }
                }
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }

        private void lv_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if(isCaptureInProcess==true)
                {
                    SafeGuiWpf.ShowError("Error: Capture is already in progress");
                    return;
                }
                var item = (sender as ListView).SelectedItem;
                if (item != null)
                {
                    Subjects = new string[9];
                    PersonalInfoModel p = (PersonalInfoModel)lv.SelectedItems[0];
                    CandidateNo = p.reg_no;
                    CandName = p.cand_name;
                    Sex = p.sex;
                    SafeGuiWpf.SetImage(imgpassport, p.passport);
                    
                    Subjects[0] = p.subj1;
                    Subjects[1] = p.subj2;
                    Subjects[2] = p.subj3;
                    Subjects[3] = p.subj4;
                    Subjects[4] = p.subj5;
                    Subjects[5] = p.subj6;
                    Subjects[6] = p.subj7;
                    Subjects[7] = p.subj8;
                    Subjects[8] = p.subj9;
                    SafeGuiWpf.SetText(txtsubjects, Subjects);
                    SafeGuiWpf.SetText(txtCand_name, CandName);
                    isCandidateSelected = true;
                   //SafeGuiWpf.SetText(txtmsg, "");
                    return;
                }
                isCandidateSelected = false;
            }
            catch (Exception ex)
            {
               SafeGuiWpf.UMsgBox(this,"Error",ex.Message,MessageBoxButton.OK, WpfMessageBox.MessageBoxImage.Error);
            }
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            SafeGuiWpf.RemoveUserControlFromGrid((this.Parent as Grid), this);
        }
    }
}
