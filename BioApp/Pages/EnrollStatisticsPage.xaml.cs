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
    public partial class EnrollStatisticsPage : UserControl
    {

        

        public string CandidateNo = "";
        //private string schnum = "";
        //private string schName = "";
        public string CandName = "";
        private string Sex = "";
        private string[] Subjects;

       
       

        System.Windows.Forms.ListView lvwFPReaders = new System.Windows.Forms.ListView();

        public EnrollStatisticsPage()
        {
            InitializeComponent();
           
        }

        private void EnrollPage_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                //SafeGuiWpf.SetText(txtmsg, "");
                LoadFin();
                
            });

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

      

        private string Progress(int Balance, int Captured)
        {
          // int total = (Captured / Balance) * 100;
           return ColorProcessLayer.CalculateColor(Captured,Balance);
        }
       /* private async void LoadData(string schnum)
        {

            using (FetchDataClass fd = new FetchDataClass())
            {
                var result = await fd.FetchRecordsForStat(schnum);
                SafeGuiWpf.SetItems<PersonalInfoViewModel>(lv_Candidates, result);
            }
        }*/

       

        /// <summary>
        /// Loads Records from fin to display on ListView
        /// </summary>
        private  void LoadFin(string schnum="")
        {
            LongActionDialog.ShowDialog("Loading ...", Task.Run(async () =>
            {
                try
                {
                    using (FetchDataClass fd = new FetchDataClass())
                        {
                            var result = await fd.FetchFinInfo(schnum);
                            if (result != null)
                            {
                                SafeGuiWpf.SetItems<FinModel>(lv, result);
                            }

                        }
                }
                catch (Exception e)
                {
                    SafeGuiWpf.ShowError(e.Message);
                }
            }));
            //SafeGuiWpf.SetText(txtmsg, "Loading ... ");
           

        }
        System.Windows.Forms.PictureBox pictBox = new System.Windows.Forms.PictureBox();
      
        #endregion
        #region Display Messages and Others To UI

        /// <summary>
        /// Diaplay the Tick image as finger is consolidated
        /// </summary>
        /// <param name="img"></param>
        /// <param name="Source"></param>
        private void ShowTickStatus(Image img, string Source)
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

        /// <summary>
        /// Displays Finger capture Progress.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="Source"></param>
        private void ShowFingerProgress(Image img, string Source)
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
        #endregion

      

        private void txtsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.Run(() =>
            {
                string Name = SafeGuiWpf.GetText(txtsearch);
                LoadFin(Name);
            });
  
        }

        private void btnreset_Click(object sender, RoutedEventArgs e)
        {
            LoadFin();
        }
      
        private void lv_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = (sender as ListView).SelectedItem;
                if (item != null)
                {
                    FinModel f = (FinModel)lv.SelectedItems[0];
                    LoadCandidatesRecords(f.SchoolNo);
                    return;
                }
            }
            catch (Exception ex)
            {
               SafeGuiWpf.ShowError(ex.Message);
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var butStaff = button.DataContext as PersonalInfoViewModel;
            if(Convert.ToInt32(butStaff.fingers)==0)
            {
                SafeGuiWpf.UMsgBox(this, "Candidate has no Fingers to delete", "Delete Fingers", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result;
            string Message = "Do you want to delete finger for\n" +
                             butStaff.cand_name + " from the DB";
            result = SafeGuiWpf.UMsgBox(this, Message, "Delete Fingers",  MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            using (WriteDataClass wd = new WriteDataClass())
            {
                var ans = await wd.DeteTemplates(butStaff.schnum, butStaff.reg_no);
                if (ans)
                {
                    SafeGuiWpf.UMsgBox(this, "Candidate Fingers deleted successfully", "Delete Fingers", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCandidatesRecords(butStaff.schnum);
                }
                else
                    SafeGuiWpf.UMsgBox(this, "Unable to delete Candidate Fingers", "Delete Fingers", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        private void LoadCandidatesRecords(string schnum)
        {
            LongActionDialog.ShowDialog("Busy, Please wait ...", Task.Run(async() =>
            {
                using (FetchDataClass fd = new FetchDataClass())
                {
                    var data = await fd.FetchRecordsForStat(schnum);
                    if (data != null)
                    {
                        SafeGuiWpf.SetItems<PersonalInfoViewModel>(lv_Candidates, data);
                    }
                }
            }));
           
        }
        private void lv_PreviewKeyDown(object sender, KeyEventArgs e)
        {
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
                        FinModel p = (FinModel)lv.SelectedItems[0];
                        LoadCandidatesRecords(p.SchoolNo);
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
                        FinModel p = (FinModel)lv.SelectedItems[0];
                        LoadCandidatesRecords(p.SchoolNo);
                    }
                }
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            SafeGuiWpf.RemoveUserControlFromGrid((this.Parent as Grid), this);
        }

        private void btRefresh_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
