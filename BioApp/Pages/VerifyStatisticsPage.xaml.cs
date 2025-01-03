using BioApp.Activities;
using BioApp.DB;
using BioApp.Models;
using BioApp.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Threading;

namespace BioApp
{
    /// <summary>
    /// Interaction logic for VerifyStatisticsPage.xaml
    /// </summary>
    public partial class VerifyStatisticsPage : UserControl
    {
        public VerifyStatisticsPage()
        {
            InitializeComponent();
        }

        private void VerifyStatisticsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void VerifyStatisticsPage_UnLoaded(object sender, RoutedEventArgs e)
        {

        }
        private void LoadData()
        {
           
            Task.Run(async() =>
            {
                using (FetchDataClass fd = new FetchDataClass())
                {
                    var result = await fd.FetchVerifiedRecords();
                    if(result!=null)
                    {
                        //UpdateListView(result);
                        SafeGuiWpf.SetItems<VerifyViewModel>(lv, result);
                    }
                }
            });
        }

       /* private string FetchCandName(string CandidateNo)
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            return dl.FetchCandName(CandidateNo);
        }*/
        void UpdateListView(List<VerifyViewModel> items)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                lv.ItemsSource = items;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      lv.ItemsSource = items;
                  }));
            }
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            SafeGuiWpf.RemoveUserControlFromGrid((this.Parent as Grid), this);
        }

        private void btRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

    }
}
