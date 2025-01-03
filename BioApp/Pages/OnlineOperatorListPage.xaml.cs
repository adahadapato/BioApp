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
    /// Interaction logic for OnlineOperatorListPage.xaml
    /// </summary>
    public partial class OnlineOperatorListPage : UserControl
    {
        private List<OnlineOperatorListApiModel> model;
        public OnlineOperatorListPage(List<OnlineOperatorListApiModel> model)
        {
            InitializeComponent();
            this.model = model;
        }

        public OnlineOperatorListPage()
            :this(null)
        {
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (model != null)
            {
                Tools.SafeGuiWpf.SetItems<OnlineOperatorListApiModel>(lv, model);
                DoCount(model.Count);
            }
            Task.Run(() =>
            {
                FetchData();
            });
            
        }

        private void DoCount(int count)
        {
            Task.Run(() =>
            {
                SafeGuiWpf.SetText(txtmsg, "Total No. of Operators: "+count.ToString());
            });
            
        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void chkSelect_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkSelect_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        private async void FetchData(string OperatorId)
        {
            using (FetchDataClass fd = new FetchDataClass())
            {
                var result = await fd.FetchOperators(OperatorId);
                if (result != null)
                {
                    SafeGuiWpf.SetItems<OnlineOperatorListApiModel>(lv, result);
                    
                }
            }
        }

        private async void FetchData()
        {
            using (RemoteDataClass rd = new RemoteDataClass())
            {
                var result = await rd.FetchOnlineOperators();
                if (result != null)
                {
                    SafeGuiWpf.SetItems<OnlineOperatorListApiModel>(lv, result);
                    DoCount(result.Count);
                }
            }
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
    }
}
