using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;


using System.Windows.Threading;
using BioApp.DB;
using System.Data;
using BioApp.Models;
using System.ComponentModel;
using BioApp.Tools;
using BioApp.Network;
using System.Threading;
using System.Threading.Tasks;
using BioApp.Activities;

namespace BioApp
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// This Class is Solely to Capture, Compare and Store Finger 
    /// Prints and thier corresponding extracted Templates
    /// Using Griaule Finger Print SDK 2009
    /// </summary>
    public partial class TemplatesUploadPage : UserControl
    {
        private bool IsUploadInProgress = false;
        private bool IsCentreSelected = false;
        private bool HasRows = false;
        private string schnum = "";
        private string SchoolName = "";
        List<UploadStatModel> FinItems = new List<UploadStatModel>();
        System.Windows.Forms.ListView lvwFPReaders = new System.Windows.Forms.ListView();

        public TemplatesUploadPage()
        {
            InitializeComponent();
           
        }

        private void EnrollPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFin("");
            DisplayDetails(txtmsg, "");
        }

        private void EnrollPage_UnLoaded(object sender, RoutedEventArgs e)
        {
            
        }


        #region Griaule SDK Helper FUnctions
        /* if (Application.Current.Dispatcher.CheckAccess())
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
           }*/



       
        private async void LoadFin(string schnum)
        {
            FinItems.Clear();
            try
            {
                using (FetchDataClass fd = new FetchDataClass())
                {
                    var result = await fd.FetchFinInfoEx(schnum);
                    FinItems = result;
                }
               SafeGuiWpf.SetItems<UploadStatModel>(lv, FinItems);
            }
            catch (Exception e)
            {
              SafeGuiWpf.UMsgBox(this, "Load data", e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        System.Windows.Forms.PictureBox pictBox = new System.Windows.Forms.PictureBox();
      
        #endregion
        #region Display Messages and Others To UI


     
       /* void UpdatePersonalInfoListView(List<PersonalInfoViewModel> items)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                lv_Candidates.ItemsSource = items;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      lv_Candidates.ItemsSource = items;
                  }));
            }
        }*/


        void UpdateFinListView(List<UploadStatModel> items)
        {
            //lv.Items.Clear();
            if (Application.Current.Dispatcher.CheckAccess())
            {
                lv.ItemsSource = items;
                lv.Items.Refresh();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      lv.ItemsSource = items;
                      lv.Items.Refresh();
                  }));
            }
        }
        

        /// <summary>
        /// Dispays Candidates, School names and others details.
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="Text"></param>
        private void DisplayDetails(TextBlock txt, string Text)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                txt.Text = Text;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      txt.Text = Text;
                  }));
            }
        }


       

        /// <summary>
        /// Display the Message.
        /// </summary>
        /// <param name="Text"></param>
        private void ShowMessage(string Text)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
               txtmsg.Text =Text;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      txtmsg.Text = Text;
                  }));
            }
        }
        #endregion

      

        private void txtsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.Run(() =>
            {
                string Name = SafeGuiWpf.GetText(txtsearch).Trim();
                LoadFin(Name);
            });
            
        }

       
      
       /* private void lv_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = (sender as ListView).SelectedItem;
                if (item != null)
                {
                    UploadStatModel f = (UploadStatModel)lv.SelectedItems[0];
                    schnum = f.SchoolNo;
                    SchoolName = f.SchoolName;
                    IsCentreSelected = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }*/

        private async void btnUploadCentre_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                SafeGuiWpf.SetEnabled(btnUploadCentre, false);
                SafeGuiWpf.SetEnabled(btnUploadBatch, false);
               
                var centreTemp = (from r in FinItems.AsEnumerable()
                                  where (r.IsSelected == true)
                                  select r).ToList();


                //HashSet<string> selectedCentres = new HashSet<string>(centreTemp.Select(s => s.SchoolNo));
                if (centreTemp.Count == 0)
                {
                    SafeGuiWpf.SetText(txtmsg, "Error: select centre to upload");
                    SafeGuiWpf.SetEnabled(btnUploadCentre, true);
                    SafeGuiWpf.SetEnabled(btnUploadBatch, true);
                    return;
                }

                SafeGuiWpf.SetText(txtmsg, "Upload started, please wait ... ");
                List<FinModel> fin = new List<FinModel>();
                foreach (var m in centreTemp)
                    fin.Add(new FinModel
                    {
                        SchoolNo = m.SchoolNo
                    });

                using (FetchDataClass fd = new FetchDataClass())
                {
                    var result = await fd.FetchTemplatesToUpload(fin);
                    if (result.Count > 0)
                    {
                        using (RemoteDataClass rd = new RemoteDataClass())
                        {
                          var final=await  rd.UploadTemplates(result);
                            if(final)
                                LoadFin("");

                        }
                    }
                    else
                    {
                        SafeGuiWpf.ShowError("No finger recods found to upload ...");
                    }
                }
                SafeGuiWpf.SetText(txtmsg, "");
                SafeGuiWpf.SetEnabled(btnUploadCentre, true);
                SafeGuiWpf.SetEnabled(btnUploadBatch, true);
            });
            
        }

        private void btnUploadAll_Click(object sender, RoutedEventArgs e)
        {
            if (IsUploadInProgress == true)
            {
                DisplayDetails(txtmsg, "Error: cannot perform this upload at this time\n" +
                                                    "upload is already in progress");
                return;
            }

            if (Xceed.Wpf.Toolkit.MessageBox.Show("using this service requires fast internet connection\n"+
                                                   "do you want to continue...", "Upload all", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            SafeGuiWpf.SetText(txtmsg, "Data upload started for all pending candidates ...");
            Thread.Sleep(3000);
            BackgroundWorker worker = new BackgroundWorker();
            try
            {
                worker.DoWork += (o, ea) =>
                {
                    SafeGuiWpf.SetText(txtmsg, "");
                    IsUploadInProgress = true;
                   // Upload("",0);
                };


                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
              SafeGuiWpf.UMsgBox(this, "Error",ex.Message, MessageBoxButton.OK,MessageBoxImage.Error);
            }
          

        }

        /*private async void Upload(string schnum, int batch)
        {
            if(!NetworkClass.ping())
            {
               SafeGuiWpf.UMsgBox(this,  "There is no Internet connectivity, Data upload failed", "Upload records", MessageBoxButton.OK,MessageBoxImage.Error);
                IsUploadInProgress = false;
                SafeGuiWpf.SetText(txtmsg, "");
                //DisplayDetails(txtmsg, "");
                IsCentreSelected = false;
                return;
            }
            BackgroundWorker worker = new BackgroundWorker();
            try
            {
                List<TemplatesModel> t = new List<TemplatesModel>();
                worker.DoWork += (o, ea) =>
                {
                    IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                    DataTable dt = dl.FetchTemplatesForUpload(schnum, batch);

                    if (dt.Rows.Count > 0)
                    {
                        var DistinctTemplates = dt.DefaultView.ToTable(true, "reg_no");
                        var NRow = DistinctTemplates.Rows.Count;

                        SafeGuiWpf.SetText(txtmsg, string.Format("{0} Candidate Record(s) found ready for upload ...", NRow));
                        Thread.Sleep(3000);
                        HasRows = true;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            t.Add(new TemplatesModel
                            {
                                schnum = dt.Rows[i]["schnum"].ToString(),
                                reg_no = dt.Rows[i]["reg_no"].ToString(),
                                finger = (byte[])dt.Rows[i]["finger"],
                                template = (byte[])dt.Rows[i]["template"],
                                quality = (Int32)dt.Rows[i]["quality"]
                            });
                        }
                    }

                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    if (HasRows)
                    {
                        SafeGuiWpf.SetText(txtmsg, string.Format("Uploading Record(s), please wait ..."));
                        Thread.Sleep(3000);
                        TemplatesUploadModel model = new TemplatesUploadModel
                        {
                            cafe = MainWindow.Operatorid,
                            Templates = t
                        };
                        string Json = JsonLayer.CreateTemplateUploadJson(model);
                        string result =await NetworkClass.UploadTemplates(Json);
                        TemplatesUploadResultModel tmp = JsonLayer.CreateUploadResultJson(result);
                        List<Templates> lst = new List<Templates>();
                        foreach (var tm in tmp.Templates)
                        {
                            lst.Add(new Templates
                            {
                                RegNo = tm.RegNo,
                                Status = tm.Status
                            });
                        }
                        SafeGuiWpf.SetText(txtmsg, "Please wait while System update...");
                        Thread.Sleep(1000);
                        IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                        dl.UpdateUploadedRecords(lst);
                        IsUploadInProgress = false;
                        SafeGuiWpf.SetText(txtmsg, "");
                        //DisplayDetails(txtmsg, "");
                        IsCentreSelected = false;
                        HasRows = false;
                        LoadFin(); // UpdateFinListView(items);
                    }
                    else
                    {
                        SafeGuiWpf.SetText(txtmsg, "");
                        IsCentreSelected = false;
                        HasRows = false;
                       SafeGuiWpf.UMsgBox(this,  "No records found to upload", "Upload records", MessageBoxButton.OK, MessageBoxImage.Error);
                        IsUploadInProgress = false;
                        return;
                    }

                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
               SafeGuiWpf.UMsgBox(this, "Error",ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                SafeGuiWpf.SetText(txtmsg, "");
                IsCentreSelected = false;
                HasRows = false;
            }
           
        }*/

        private async void btnUploadBatch_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                SafeGuiWpf.SetEnabled(btnUploadCentre, false);
                SafeGuiWpf.SetEnabled(btnUploadBatch, false);
                /*if (IsUploadInProgress == true)
                {
                    SafeGuiWpf.SetText(txtmsg, "Error: cannot perform this upload at this time\n" +
                                                         "upload is already in progress");
                    return;
                }*/

                var centreTemp = (from r in FinItems.AsEnumerable()
                                  where (r.IsSelected == true)
                                  select r).ToList();


                //HashSet<string> selectedCentres = new HashSet<string>(centreTemp.Select(s => s.SchoolNo));
                if (centreTemp.Count == 0)
                {
                    SafeGuiWpf.SetText(txtmsg, "Error: select centre to upload");
                    SafeGuiWpf.SetEnabled(btnUploadCentre, true);
                    SafeGuiWpf.SetEnabled(btnUploadBatch, true);
                    return;
                }

                string tempBatchSize = Microsoft.VisualBasic.Interaction.InputBox("Enter batch size [No. of Candidates]", "Batched upload", "0");
                int result;
                if (!int.TryParse(tempBatchSize, out result))
                {
                    SafeGuiWpf.ShowError("Invalid batch size, batch must be integer and greater than zero");//, "Upload batch", MessageBoxButton.OK, MessageBoxImage.Error);
                    SafeGuiWpf.SetEnabled(btnUploadCentre, true);
                    SafeGuiWpf.SetEnabled(btnUploadBatch, true);
                    return;
                }

                if (Convert.ToInt32(tempBatchSize) == 0 || Convert.ToInt32(tempBatchSize) < 0)
                {
                    SafeGuiWpf.ShowError("Invalid batch size, batch must be greater than zero");//, "Upload batch", MessageBoxButton.OK, MessageBoxImage.Error);
                    SafeGuiWpf.SetEnabled(btnUploadCentre, true);
                    SafeGuiWpf.SetEnabled(btnUploadBatch, true);
                    return;
                }


                SafeGuiWpf.SetText(txtmsg, "Upload started, please wait ... ");
                List<FinModel> fin = new List<FinModel>();
                foreach (var m in centreTemp)
                    fin.Add(new FinModel
                    {
                        SchoolNo = m.SchoolNo
                    });


                using (FetchDataClass fd = new FetchDataClass())
                {
                    var results = await fd.FetchTemplatesToUpload(fin);
                    if (results.Count > 0)
                    {
                        using (RemoteDataClass rd = new RemoteDataClass())
                        {
                           var final= await rd.UploadTemplates(results, Convert.ToInt32(tempBatchSize));
                            if(final)
                               LoadFin("");
                        }
                    }
                    else
                    {
                        SafeGuiWpf.ShowError("No finger records found to upload ...");
                        return;
                    }
                }
                SafeGuiWpf.SetText(txtmsg, "");
                SafeGuiWpf.SetEnabled(btnUploadCentre, true);
                SafeGuiWpf.SetEnabled(btnUploadBatch, true);
            });
           
            

        }

        #region ListView CheckBoxes
        private static bool individualChkBxUnCheckedFlag { get; set; }
        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SafeGuiWpf.SetText(txtmsg, "");
            if (e.AddedItems.Count > 0)
            {
                //------------
                UploadStatModel model = (UploadStatModel)e.AddedItems[0];
                ListViewItem lvi = (ListViewItem)lv.ItemContainerGenerator.ContainerFromItem(model);
                CheckBox chkBx = SafeGuiWpf.FindVisualChild<CheckBox>(lvi);
                if (chkBx != null)
                    chkBx.IsChecked = true;
                //------------               
            }
            else // Remove Select All chkBox
            {
                UploadStatModel model = (UploadStatModel)e.RemovedItems[0];
                ListViewItem lvi = (ListViewItem)lv.ItemContainerGenerator.ContainerFromItem(model);
                CheckBox chkBx = SafeGuiWpf.FindVisualChild<CheckBox>(lvi);
                if (chkBx != null)
                    chkBx.IsChecked = false;
            }
            //============        
        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                individualChkBxUnCheckedFlag = false;
                //=====================
                foreach (UploadStatModel item in lv.ItemsSource)
                {
                    item.IsSelected = true;
                    lv.SelectedItems.Add(item);
                }
            }
            catch (Exception) { }

        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!individualChkBxUnCheckedFlag)
            {
                foreach (UploadStatModel item in lv.ItemsSource)
                {
                    item.IsSelected = false;
                    //int itemIndex = items.IndexOf(item);
                    lv.SelectedItems.Remove(item);
                    //lstgrd.SelectedItems.Add(lstgrd.Items.GetItemAt(2));               
                }
            }
        }

        private void chkSelect_Checked(object sender, RoutedEventArgs e)
        {
            ListViewItem item = ItemsControl.ContainerFromElement(lv, e.OriginalSource as DependencyObject) as ListViewItem;
            if (item != null)
            {
                item.IsSelected = true;
            }
        }

        private void chkSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            ListViewItem item = ItemsControl.ContainerFromElement(lv, e.OriginalSource as DependencyObject) as ListViewItem;
            if (item != null)
                item.IsSelected = false;

            individualChkBxUnCheckedFlag = true;
            CheckBox headerChk = (CheckBox)((GridView)lv.View).Columns[0].Header;
            headerChk.IsChecked = false;
        }
        #endregion
    }
}
