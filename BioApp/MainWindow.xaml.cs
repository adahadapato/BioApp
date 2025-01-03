//using CustomChromeLibrary;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GriauleFingerprintLibrary;
using System.Windows.Threading;
using BioApp.Tools;
using BioApp.Models;
using BioApp.DB;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media;
using System.Threading.Tasks;

namespace BioApp
{

    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Finger Print Variables
        private FingerprintCore fingerPrint;
        public string ReaderName;
        public int Opt;

        private bool IsLaunchFromWithin = false;
        private string fileName = "";
        public static string SchoolNo = "";
        public static string SchoolName = "";
        public static string Operatorid = "";
        public static string Cafe = "";


        DispatcherTimer Timer = new DispatcherTimer();
        public MainWindow(string fileName)
        {
            InitializeComponent();
            fingerPrint = new FingerprintCore();
            
            fingerPrint.onStatus += new StatusEventHandler(fingerPrint_onStatus);
            this.Top = 0;
            this.Left = 0;
            this.Width = SystemParameters.VirtualScreenWidth;
            this.Height = SystemParameters.VirtualScreenHeight;

            // this.Topmost = true;

            this.fileName = fileName;
            //MessageBox.Show(fileName);
            //this.fileName = AppPathClass.FetcAppPath()+ @"\0010016.neco";
            this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        private void Timer_Click(object sender, EventArgs e)
        {
            DateTime d;
            d = DateTime.Now;
            string date = string.Format("{0:T}", d);
            txtTime.Text = "Time: " + date;// d.Hour + " : " + d.Minute + " : " + d.Second;
        }
        private int CountTotalRecords()
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return dl.CountTotalRecords(SchoolNo);
        }
        /// <summary>
        /// This Method creates the Data Base files and Inserts Default records.
        /// </summary>
        /// <param name="fileName"></param>
        /// This is the centre file name that is opened
        private void PrepareFiles(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    if (!IsLaunchFromWithin)
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("No centre file was selected, do you want to select one now", "Launch Application", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        switch(result)
                        {
                            case MessageBoxResult.Yes:
                                OpenFileDialog openFileDialog = new OpenFileDialog
                                {
                                    Filter = "NECO Biometrics files (*.neco)|*.neco|All files (*.*)|*.*",
                                    Title = "NECO SSCE 2017 Biometrics"
                                };
                                if (openFileDialog.ShowDialog(this) == true)
                                {
                                    fileName = openFileDialog.FileName;
                                }
                                else
                                {
                                    CleanUpFiles();
                                }
                                break;

                            case MessageBoxResult.No:
                                return;
                            case MessageBoxResult.Cancel:
                                CleanUpFiles();
                                break;


                        }
                    }
                    else
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog
                        {
                            Filter = "NECO Biometrics files (*.neco)|*.neco|All files (*.*)|*.*",
                            Title = "NECO SSCE 2017 Biometrics",
                            DefaultExt = ".neco"
                        };
                        if (openFileDialog.ShowDialog(this) == true)
                        {
                            fileName = openFileDialog.FileName;
                        }
                    }
                }

                MainWindow.Start(fileName);
               
            }
            catch { }
        }

       /* private void InitForm()
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
        }*/
        private static MainWindow frm;
        public static void Start(string fileName)
        {
            
            try
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    frm = new MainWindow(fileName);
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      frm = new MainWindow(fileName);
                  }));
                }

                
                
                BackgroundWorker worker = new BackgroundWorker();//This enables the UI to stay unfrozen
                int Balance = 0;
                worker.DoWork += (o, ea) =>
                {
                    string temp = AppPathClass.FetcAppPath() + @"\temp.neco";
                    if (File.Exists(temp))
                        File.Delete(temp);
                    CryptograhyClass.Decrypt(fileName, temp, CryptograhyClass.EncryptionPWD);
                    List<TemplatesModel> t = new List<TemplatesModel>();
                    List<PersonalInfoModel> p = new List<PersonalInfoModel>();
                    JsonLayer.CreateTemplatesStorageJson(temp, out p, out t);
                    IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                    dl.SaveDataToPersonalInfo(p);
                    IGRDal d = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                    d.SaveDataToTemp(t);
                    Balance = frm.CountTotalRecords();
                };

                worker.RunWorkerCompleted += (o, ea) =>
                {
                    // UpdateFinListView(items);
                    IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                    FinModel f = new FinModel
                    {
                        SchoolNo = SchoolNo,
                        SchoolName = SchoolName,
                        Captured = 0,
                        Balance = Balance
                    };
                    dl.SaveDataToFin(f);
                    if (frm.IsLaunchFromWithin)
                    {
                        frm.contentGrid.Children.Clear();
                        frm.contentGrid.Children.Add(new EnrollPage(ref frm.fingerPrint));
                    }
                    frm.IsLaunchFromWithin = false;
                };
                //set the IsBusy before you start the thread
                worker.RunWorkerAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
           
        }
        public static void ShutDown()
        {
            MainWindow w = new MainWindow("");
            if (Xceed.Wpf.Toolkit.MessageBox.Show("Do you want to exit", "Exit Application", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                w.CleanUpFiles();// = new Action(w..NonStaticMethod);
                //this.CleanUpFiles();
            }
            else
                return ;
        }

        private void CleanUpFiles()
        {
            BackgroundWorker worker = new BackgroundWorker();//This enables the UI to stay unfrozen
            worker.DoWork += (o, ea) =>
            {
                try
                {
                    string[] files = Directory.GetFiles(AppPathClass.FetcAppPath() + @"\", "*.bmp");
                    foreach (string f in files)
                        File.Delete(f);

                    string[] file = Directory.GetFiles(AppPathClass.FetcAppPath() + @"\", "temp*.neco");
                    foreach (string f in file)
                        File.Delete(f);
                }
                catch { }

            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                try
                {
                    contentGrid.Children.Clear();
                    fingerPrint.onStatus -= new StatusEventHandler(fingerPrint_onStatus);
                    fingerPrint.CaptureFinalize();
                    //fingerPrint.Finalizer();
                    Application.Current.Shutdown();
                }
                catch { }
               
            };

            worker.RunWorkerAsync();
        }
    
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            this.WindowState = WindowState.Normal;
            PrepareFiles(this.fileName);
            //DisplayOperatorDetails(Cafe.Trim());
            fingerPrint.Initialize();
            fingerPrint.CaptureInitialize();
            fingerPrint.SetBiometricDisplayColors(FingerprintConstants.GR_IMAGE_NO_COLOR, FingerprintConstants.GR_IMAGE_NO_COLOR,
                                                FingerprintConstants.GR_IMAGE_NO_COLOR, FingerprintConstants.GR_IMAGE_NO_COLOR,
                                                FingerprintConstants.GR_IMAGE_NO_COLOR, FingerprintConstants.GR_IMAGE_NO_COLOR);

            //DO_Menu();
            SafeGuiWpf.SetText(txtUserName, Cafe.Trim());
            var t = Task.Run(() =>
            {
                DoDate();
            });
        }

        private void DoDate()
        {
            var date = DateTime.Now;
            string VerifyDate = string.Format("{0:D}", date);
            SafeGuiWpf.SetText(txtDate, VerifyDate);
        }
        private void DO_Menu()
        {
            if(!Operatorid.Contains("NECO"))
            {
                accVerify.Visibility = Visibility.Collapsed;
            }

           // accVerify.Visibility = Visibility.Collapsed;
        }
        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            //MainWindow_Closing(sender, e);

        }
        #region Griaule Functions
        void fingerPrint_onStatus(object source, GriauleFingerprintLibrary.Events.StatusEventArgs se)
        {
            try
            {
                if (se.StatusEventType == GriauleFingerprintLibrary.Events.StatusEventType.SENSOR_PLUG)
                {
                    if (se.Source.ToString().Contains("File"))
                    {
                        ShowDeviceStatus(this.imgStatus, "red_dot.png");
                      //  ShowStatusText("Connect device");
                    }
                    else
                    {
                        ShowDeviceStatus(this.imgStatus, "green_dot.png");// pict_device_status.Image = Properties.Resources.on;
                        //ShowDeviceStatusToolTip();
                        //  ShowStatusText("Connected");
                    }
                    fingerPrint.StartCapture(source.ToString());
                    //SetLvwFPReaders(se.Source, 0);
                }
                else
                {
                    fingerPrint.StopCapture(source);
                    //SetLvwFPReaders(se.Source, 1);
                    ShowDeviceStatus(this.imgStatus, "red_dot.png");
                   // ShowStatusText("Connect device");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        #endregion
        #region Display To UI
        private void ShowDeviceStatus(Image img, string Source)
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

        }

        private void ShowDeviceStatusToolTip()
        {
           
            if (Application.Current.Dispatcher.CheckAccess())
            {
                var tip = ToolTipService.GetToolTip(this.imgStatus) as ToolTip;
                tip.Background = new SolidColorBrush(Colors.Magenta);
                tip.HasDropShadow = true;
                tip.StaysOpen = false;
                tip.IsOpen = true;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      var tip = ToolTipService.GetToolTip(this.imgStatus) as ToolTip;
                      tip.Background = new SolidColorBrush(Colors.Magenta);
                      tip.HasDropShadow = true;
                      tip.StaysOpen = false;
                      tip.IsOpen = true;
                  }));
            }

        }
        private void DisplayOperatorDetails(string Text)
        {
             if (Application.Current.Dispatcher.CheckAccess())
            {
                txtOperatorDisplay.Text ="                      ["+Text+"]";
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      txtOperatorDisplay.Text = "                      [" + Text+"]";
                  }));
            }

        }
        #endregion

        private void CaptionButtons_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.Clear();
            
           // contentGrid.Children.Add(new EnrollPage());
        }
        private void btnenroll_Click(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.Clear();
            contentGrid.Children.Add(new EnrollPage(ref fingerPrint));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (Xceed.Wpf.Toolkit.MessageBox.Show("Do you want to exit", "Exit Application", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CleanUpFiles();
            }
            else
                return;
        }

        //
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //contentGrid.Children.Clear();
            //contentGrid.Children.Add(new VerifyPage(ref fingerPrint));
        }

        private void btnVerifyStatistics_Click(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.Clear();
            contentGrid.Children.Add(new VerifyStatisticsPage());
        }
        //
        private void Verify_Candidates(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.Clear();
            contentGrid.Children.Add(new VerifyPage(ref fingerPrint));
        }

        private void btnEnrolledStat_Click(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.Clear();
            contentGrid.Children.Add(new EnrollStatisticsPage());
        }

        private void btnExportEnrolled_Click(object sender, RoutedEventArgs e)
        {
            ExportRecordsLayer.Export(SchoolNo);
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            IsLaunchFromWithin = true;
            PrepareFiles("");
        }
        private void btnListOfCentres_Click(object sender, RoutedEventArgs e)
        {
           // IsLaunchFromWithin = true;
           // PrepareFiles("");
        }
        //btnListOfCentres
        //btnOpenFile_Click
        private void btnUploadEnrolledData_Click(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.Clear();
            contentGrid.Children.Add(new TemplatesUploadPage());
            //UploadEnrolledDataModel.Upload();
        }
    }
}
