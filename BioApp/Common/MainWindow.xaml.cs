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
using BioApp.RegistryHelper;
using BioApp.Activities;
using BioApp.Pages;
using System.Linq;
using System.Windows.Navigation;
using System.Windows.Input;
//using WPF.Themes;

namespace BioApp
{

    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IRegistryToken regToken = new RegistryToken();
        //Finger Print Variables
        private FingerprintCore fingerPrint;
        public string ReaderName;
        public int Opt;

        private bool IsLaunchFromWithin = false;
        private string fileName = "";
        private string SchoolNo = "";
        private string SchoolName = "";
        private string Operatorid = "";
        private string Cafe = "";


        DispatcherTimer Timer = new DispatcherTimer();

        public MainWindow()
            :this("")
        {

        }
        public MainWindow(string fileName)
        {
            InitializeComponent();
            //this.ApplyTheme("ExpressionDark");
            //this.TheLabel.ApplyTheme("BureauBlack");

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

            // Attach OpenOnHover event handler.
            foreach (AccordionItem item in this.MainNav.Items)
            {
                item.MouseEnter += this.AccordionItem_MouseEnter;
            }

            CollapseAll();
        }

        /// <summary>
        /// Collapses all AccordionItems (if allowed).
        /// </summary>
        /// <param name="sender">Sender of the event (a Button).</param>
        /// <param name="e">Event arguments.</param>
        private void CollapseAll()
        {
            foreach (AccordionItem item in this.MainNav.Items)
            {
                if (!item.IsLocked)
                {
                    item.IsSelected = false;
                }
            }
        }
        private void Timer_Click(object sender, EventArgs e)
        {
            DateTime d;
            d = DateTime.Now;
            string date = string.Format("{0:T}", d);
            txtTime.Text = "Time: " + date;// d.Hour + " : " + d.Minute + " : " + d.Second;
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
                        //if(SafeGuiWpf.WMsgBox(w, "Exit Application", "Do you want to exit", MessageBoxButton.YesNo, WpfMessageBox.MessageBoxImage.Error)== MessageBoxResult.Yes)
                        MessageBoxResult result = SafeGuiWpf.WMsgBox(this, "Launch Application", "No centre file was selected, do you want to select one now",  MessageBoxButton.YesNoCancel, WpfMessageBox.MessageBoxImage.Question);
                        switch(result)
                        {
                            case MessageBoxResult.Yes:
                                OpenFileDialog openFileDialog = new OpenFileDialog
                                {
                                    Filter = "NECO Biometrics files (*.neco)|*.neco|All files (*.*)|*.*",
                                    Title = "NECO SSCE 2018 Biometrics"
                                };
                                if (openFileDialog.ShowDialog(this) == true)
                                {
                                    this.fileName = openFileDialog.FileName;
                                }
                                else
                                {
                                    SafeGuiWpf.AddUserControlToGrid<Pages.DashBoardPage>(contentGrid, true);
                                }
                                break;

                            case MessageBoxResult.No:
                                return;
                            case MessageBoxResult.Cancel:
                                //CleanUpFiles();
                                SafeGuiWpf.AddUserControlToGrid<Pages.DashBoardPage>(contentGrid, true);
                                break;


                        }
                    }
                    else
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog
                        {
                            Filter = "NECO Biometrics files (*.neco)|*.neco|All files (*.*)|*.*",
                            Title = "NECO SSCE 2018 Biometrics",
                            DefaultExt = ".neco"
                        };
                        if (openFileDialog.ShowDialog(this) == true)
                        {
                            this.fileName = openFileDialog.FileName;
                        }
                        else
                        {
                            SafeGuiWpf.AddUserControlToGrid<Pages.DashBoardPage>(contentGrid, true);
                        }
                    }
                }

                if(!string.IsNullOrWhiteSpace(this.fileName))
                    Start(this.fileName);
               
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
        //private static MainWindow frm;
        public  void Start(string fileName)
        {
            
            try
            {
               /* if (Application.Current.Dispatcher.CheckAccess())
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
                }*/

                
                
                //This enables the UI to stay unfrozen
                int Balance = 0;
                Task.Run(async() =>
                {
                    
                    string temp = AppPathClass.FetcAppPath() + @"\temp.neco";
                    if (File.Exists(temp))
                        File.Delete(temp);
                    CryptograhyClass.Decrypt(fileName, temp, CryptograhyClass.EncryptionPWD);
                    List<TemplatesModel> t = new List<TemplatesModel>();
                    List<PersonalInfoModel> p = new List<PersonalInfoModel>();
                    JsonLayer.CreateTemplatesStorageJson(temp, out p, out t);
                    using (WriteDataClass wd = new WriteDataClass())
                    {
                        await DoSchool();
                        wd.SaveDataToPersonalInfo(p);
                        wd.SaveDataToTemp(t);


                        using (FetchDataClass fd = new FetchDataClass())
                        {
                            Balance = await fd.CountTotalRecords(SchoolNo);
                        }


                        FinModel f = new FinModel
                        {
                            SchoolNo = SchoolNo,
                            SchoolName = SchoolName,
                            Captured = 0,
                            Balance = Balance
                        };
                      var keep= await wd.SaveDataToFin(f);
                        if (IsLaunchFromWithin)
                        {
                            //frm.contentGrid.Children.Clear();
                            //frm.contentGrid.Children.Add(new EnrollPage(ref frm.fingerPrint));
                            SafeGuiWpf.AddUserControlToGrid<EnrollPage, FingerprintCore>(contentGrid, fingerPrint, true);
                        }
                        IsLaunchFromWithin = false;
                    }
                });

              
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
           
        }
        public static void ShutDown()
        {
            MainWindow w = new MainWindow("");
            //Tools.SafeGuiWpf.UMsgBox(this, "Please enter operator Id", "Change Operator status", MessageBoxButton.OK, WpfMessageBox.MessageBoxImage.Error);
            //if (Xceed.Wpf.Toolkit.MessageBox.Show("Do you want to exit", "Exit Application", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            if(SafeGuiWpf.WMsgBox(w, "Exit Application", "Do you want to exit", MessageBoxButton.YesNo, WpfMessageBox.MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                w.CleanUpFiles();// = new Action(w..NonStaticMethod);
                
            }
            else
                return ;
        }

        private  void CleanUpFiles()
        {
            //This enables the UI to stay unfrozen
            Task.Run(() =>
            {
                LongActionDialog.ShowDialog("Exiting ... ", Task.Run(() =>
                {
                    try
                    {
                        SafeGuiWpf.AddUserControlToGrid(contentGrid);
                        string[] files = Directory.GetFiles(AppPathClass.FetcAppPath() + @"\", "*.bmp");
                        foreach (string f in files)
                        {
                            try
                            {
                                File.Delete(f);
                            }
                            catch { continue; }
                        }

                        string[] file = Directory.GetFiles(AppPathClass.FetcAppPath() + @"\", "temp*.neco");
                        foreach (string f in file)
                        {
                            try
                            {
                                File.Delete(f);
                            }
                            catch { continue; }
                        }

                    }
                    catch { }

                    
                    fingerPrint.onStatus -= new StatusEventHandler(fingerPrint_onStatus);
                    fingerPrint.CaptureFinalize();
                    Environment.Exit(0);
                }));
                
            });
        }
    
    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            this.WindowState = WindowState.Normal;
            
            //DisplayOperatorDetails(Cafe.Trim());
            fingerPrint.Initialize();
            fingerPrint.CaptureInitialize();
            fingerPrint.SetBiometricDisplayColors(FingerprintConstants.GR_IMAGE_NO_COLOR, FingerprintConstants.GR_IMAGE_NO_COLOR,
                                                FingerprintConstants.GR_IMAGE_NO_COLOR, FingerprintConstants.GR_IMAGE_NO_COLOR,
                                                FingerprintConstants.GR_IMAGE_NO_COLOR, FingerprintConstants.GR_IMAGE_NO_COLOR);

            //DO_Menu();
            SafeGuiWpf.SetText(txtUserName, Cafe.Trim());
            await Task.Run(async () =>
            {
                DoDate();
                DO_Menu();
               await DoSchool();
                DoDetails();
            });

            PrepareFiles(this.fileName);
        }

        private async Task<bool> DoSchool()
        {
          return await Task.Run(() =>
            {
                this.SchoolNo = regToken.Getvalue("SchoolNo").ToString();
                this.SchoolName = regToken.Getvalue("SchoolName").ToString();
                return true;
            });
        }
        private void DoDetails()
        {
            Task.Run(() =>
            {
                // SchoolNo = regToken.Getvalue("SchoolNo").ToString();
                // SchoolName = regToken.Getvalue("SchoolName").ToString();
                 Operatorid = regToken.Getvalue("OperatorId").ToString();
                 Cafe = regToken.Getvalue("Cafe").ToString();
                SafeGuiWpf.SetText(txtUserName, Cafe);
                //SafeGuiWpf.SetText(txtOperatorDisplay, Operatorid);
            });
        }
        private void DoDate()
        {
            var date = DateTime.Now;
            string VerifyDate = string.Format("{0:D}", date);
            SafeGuiWpf.SetText(txtDate, VerifyDate);
        }
        private async void DO_Menu()
        {
           await Task.Run(() =>
            {
                string role = regToken.Getvalue("Role").ToString();
                switch(role)
                {
                    case "Admin":
                        SafeGuiWpf.SetVisible(accSupport, Visibility.Visible);
                        SafeGuiWpf.SetVisible(btnDeactivateOperators, Visibility.Visible);
                        SafeGuiWpf.SetVisible(btnAddOperators, Visibility.Visible);
                        SafeGuiWpf.SetVisible(btnActiveOperatorList, Visibility.Visible);
                        SafeGuiWpf.SetVisible(btnFetchTotalUploadSummary, Visibility.Visible);
                        SafeGuiWpf.SetVisible(btnLogout, Visibility.Visible);
                        SafeGuiWpf.SetVisible(accVerify, Visibility.Visible);
                        break;
                    case "Support":
                        SafeGuiWpf.SetVisible(accSupport, Visibility.Visible);
                        SafeGuiWpf.SetVisible(btnDeactivateOperators, Visibility.Visible);
                        SafeGuiWpf.SetVisible(btnActiveOperatorList, Visibility.Visible);
                        SafeGuiWpf.SetVisible(btnLogout, Visibility.Visible);
                        

                        SafeGuiWpf.SetVisible(btnAddOperators, Visibility.Collapsed);
                        SafeGuiWpf.SetVisible(btnFetchTotalUploadSummary, Visibility.Collapsed);
                        SafeGuiWpf.SetVisible(accVerify, Visibility.Visible);
                        break;
                    case "Staff":
                        SafeGuiWpf.SetVisible(accSupport, Visibility.Collapsed);
                        SafeGuiWpf.SetVisible(accVerify, Visibility.Visible);
                        break;
                    default:
                        SafeGuiWpf.SetVisible(accSupport, Visibility.Collapsed);
                        SafeGuiWpf.SetVisible(accVerify, Visibility.Collapsed);
                        break;
                }
            });
           
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
                        SafeGuiWpf.SetText(lblDeviceStatus, "Connect Device");
                      //  ShowStatusText("Connect device");
                    }
                    else
                    {
                        ShowDeviceStatus(this.imgStatus, "green_dot.png");// pict_device_status.Image = Properties.Resources.on;
                        SafeGuiWpf.SetText(lblDeviceStatus, "Device Connected");
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
                    SafeGuiWpf.SetText(lblDeviceStatus, "Connect Device");
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
            if (SafeGuiWpf.WMsgBox(this, "Exit Application", "Do you want to exit",  MessageBoxButton.YesNo, WpfMessageBox.MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CleanUpFiles();
            }
            else
                return;
        }

        protected override void OnClosed(EventArgs e)
        {
            //MessageBox.Show("closing");
            base.OnClosed(e);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            string btnText = SafeGuiWpf.GetText(btnLogout);
            if(btnText== "Log out")
            {
                SafeGuiWpf.SetText(btnLogout, "Log in");
                contentGrid.Children.Clear();
                regToken.Setvalue("IsLogin", "false");
            }
            else
            {
               SafeGuiWpf.SetText(btnLogout, "Log out");
               SafeGuiWpf.ShowWindow<LogInPage>();
               SafeGuiWpf.CloseWindow(this);
               
                // SafeGuiWpf.SetVisible(this, Visibility.Collapsed);
            }    
        }

        private void btnVerifyStatistics_Click(object sender, RoutedEventArgs e)
        {
            SafeGuiWpf.AddUserControlToGrid<VerifyStatisticsPage>(contentGrid, true);
        }
        
        private void Verify_Candidates(object sender, RoutedEventArgs e)
        {
            contentGrid.Children.Clear();
            contentGrid.Children.Add(new VerifyPage(ref fingerPrint));
        }

        private void btnEnrolledStat_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                SafeGuiWpf.AddUserControlToGrid<EnrollStatisticsPage>(contentGrid, true);
            });
            
        }

        private void btnExportEnrolled_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(SchoolNo))
                {
                    SafeGuiWpf.ShowError("Please a centre file to export data");
                    return;
                }
                var model = new List<FinModel>();
                model.Add(new FinModel
                {
                    SchoolNo=SchoolNo,
                    SchoolName=SchoolName
                });
                ExportRecordsLayer.Export(model);
            });
           
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
            //contentGrid.Children.Clear();
            //contentGrid.Children.Add(new TemplatesUploadPage());
            SafeGuiWpf.AddUserControlToGrid<TemplatesUploadPage>(contentGrid,true);
            //UploadEnrolledDataModel.Upload();
        }

        private void btnDashBoard_Click(object sender, RoutedEventArgs e)
        {
            SafeGuiWpf.AddUserControlToGrid<Pages.DashBoardPage>(contentGrid, true);
        }

        private void btnDeactivateOperators_Click(object sender, RoutedEventArgs e)
        {
            SafeGuiWpf.AddUserControlToGrid<Pages.SetOperatorStatusPage>(contentGrid, true);
        }

        private void btnAddOperators_Click(object sender, RoutedEventArgs e)
        {
            SafeGuiWpf.AddUserControlToGrid<NewOperatorPage>(contentGrid, true);
        }

       
        private async void btnActiveOperatorList_Click(object sender, RoutedEventArgs e)
        {
            using (FetchDataClass fd = new FetchDataClass())
            {
                var result = await fd.FetchOperators("");
                if (result != null)
                {
                    SafeGuiWpf.AddUserControlToGrid<OnlineOperatorListPage, List<OnlineOperatorListApiModel>>(contentGrid, result, true);
                }
            }

        }

        private  void btnFetchTotalUploadSummary_Click(object sender, RoutedEventArgs e)
        {
            LongActionDialog.ShowDialog("Working ... ", Task.Run(async () =>
            {
                using (FetchDataClass fd = new FetchDataClass())
                {
                    var result = await fd.FetchTotalUploadSummary("");
                    if (result != null)
                    {
                        var ans = (from item in result
                                   where !item.cafe.Contains("NECO ICT TEST")
                                   select item).ToList();

                        SafeGuiWpf.AddUserControlToGrid<TotalUploadSummaryPage, List<TotalSummryApiModel>>(contentGrid, ans, true);
                    }
                }
            }));
          
        }

        private  void btnDownloadCentre_Click(object sender, RoutedEventArgs e)
        {
          LongActionDialog.ShowDialog("Working ... ", Task.Run(async () =>
           {
               using (RemoteDataClass rd = new RemoteDataClass())
               {
                   var result = await rd.FetchCentreForVerification("");
               }
           }));
          
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Process.Start(e.Uri.AbsoluteUri);
            //  e.Handled = true;
        }
        /// <summary>
        /// Applies the Accordion's AutoExpand behavior.
        /// </summary>
        /// <param name="sender">Sender of the event (an AccordionItem).</param>
        /// <param name="e">Event arguments.</param>
        private void AccordionItem_MouseEnter(object sender, MouseEventArgs e)
        {
            
                AccordionItem item = sender as AccordionItem;
                item.IsSelected = true;
            
        }
       
    }
}
