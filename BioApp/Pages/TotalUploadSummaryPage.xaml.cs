using BioApp.Activities;
using BioApp.Models;
using BioApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BioApp.Pages
{
    /// <summary>
    /// Interaction logic for TotalUploadSummaryPage.xaml
    /// </summary>
    public partial class TotalUploadSummaryPage : UserControl
    {
        List<TotalSummryApiModel> model;
        public TotalUploadSummaryPage(List<TotalSummryApiModel> model)
        {
            InitializeComponent();
            this.model = model;
        }

        public TotalUploadSummaryPage()
            :this(null)
        {
            
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                if (model != null)
                {
                    SafeGuiWpf.SetItems<TotalSummryApiModel>(lv, model);
                    DoCount(model);
                }

                FetchData();
            });
        }

        private void DoCount(List<TotalSummryApiModel> count)
        {
            Task.Run(() =>
            {
                var total = (from item in count
                             select item.total)
                                .Sum();
                SafeGuiWpf.SetText(txtmsg, "Total No. of Candidate records: " + total.ToString());
            });

        }

        private void txtsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.Run(() =>
            {
                string serach = SafeGuiWpf.GetText(txtsearch);
                FetchData(serach);
            });
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                FetchData();
            });
        }

        /* using (RemoteDataClass rd = new RemoteDataClass())
            {
                LongActionDialog.ShowDialog("Loading ...", Task.Run(async () =>
                 {
                     var result = await rd.FetchTotalUploadSummary();
                     if (result != null)
                     {
                         var ans = (from item in result
                                    where !item.cafe.Contains("NECO ICT TEST")
                                    select item).ToList();

                         SafeGuiWpf.AddUserControlToGrid<TotalUploadSummaryPage, List<TotalSummryApiModel>>(contentGrid, ans, true);
                     }
                 }));

            }*/

        private async void FetchData(string OperatorId)
        {
            using (FetchDataClass fd = new FetchDataClass())
            {
                var result = await fd.FetchTotalUploadSummary(OperatorId);
                if (result != null)
                {
                    var ans = (from item in result
                               where !item.cafe.Contains("NECO ICT TEST")
                               select item).ToList();

                    SafeGuiWpf.SetItems<TotalSummryApiModel>(lv, ans);

                }
            }
        }


        private async void FetchData()
        {
            using (RemoteDataClass rd = new RemoteDataClass())
            {
                var result = await rd.FetchTotalUploadSummary();
                if (result != null)
                {
                    var ans = (from item in result
                               where !item.cafe.Contains("NECO ICT TEST")
                               select item).ToList();

                    SafeGuiWpf.SetItems<TotalSummryApiModel>(lv, ans);
                    DoCount(ans);
                }
            }
        }

        #region ListView CheckBoxes
        private static bool RdBtnUnCheckedFlag { get; set; }
        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string Cafe = "";
            if (e.AddedItems.Count > 0)
            {
                //------------
                TotalSummryApiModel model = (TotalSummryApiModel)e.AddedItems[0];
                ListViewItem lvi = (ListViewItem)lv.ItemContainerGenerator.ContainerFromItem(model);
                RadioButton chkBx = SafeGuiWpf.FindVisualChild<RadioButton>(lvi);
                if (chkBx != null)
                {
                    chkBx.IsChecked = true;
                    Cafe = model.operatorId;
                }
                MessageBox.Show(Cafe);
               // DoRecCount(this.RecNo.ToString());
                //------------               
            }
            else // Remove Select All chkBox
            {
                TotalSummryApiModel model = (TotalSummryApiModel)e.RemovedItems[0];
                ListViewItem lvi = (ListViewItem)lv.ItemContainerGenerator.ContainerFromItem(model);
                RadioButton chkBx = SafeGuiWpf.FindVisualChild<RadioButton>(lvi);
                if (chkBx != null)
                {
                    chkBx.IsChecked = false;
                   // this.RecNo--;

                }
               // DoRecCount(this.RecNo.ToString());
            }
            //============        
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

            RdBtnUnCheckedFlag = true;
            //CheckBox headerChk = (CheckBox)((GridView)lv.View).Columns[0].Header;
            //headerChk.IsChecked = false;
        }

       
        #endregion

    }
}
