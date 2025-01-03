using BioApp.Models;
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
    /// Interaction logic for SetOperatorStatusPage.xaml
    /// </summary>
    public partial class SetOperatorStatusPage : UserControl
    {
        //private readonly ToastViewModel _vm;
        public SetOperatorStatusPage()
        {
            InitializeComponent();
            //_vm = new ToastViewModel();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Tools.SafeGuiWpf.SetText(txtmsg, "");
            });
        }

        private async void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            string CafeOperatorId;
            await Task.Run(async () =>
            {
                CafeOperatorId = Tools.SafeGuiWpf.GetText(txtOperatorId);
                if(string.IsNullOrWhiteSpace(CafeOperatorId))
                {
                   Tools.SafeGuiWpf.ShowWarning("Please enter Operator Id to Activate");//, "Change Operator status", MessageBoxButton.OK, WpfMessageBox.MessageBoxImage.Error);
                    return;
                }
                Tools.SafeGuiWpf.SetText(txtmsg, "Activating operator, please wait ... ");
                Tools.SafeGuiWpf.SetEnabled(btnActivate, false);
                Tools.SafeGuiWpf.SetEnabled(btnDeactivate, false);
                Tools.SafeGuiWpf.SetEnabled(btnCancel, false);
                var model = new SetOperatorStatusModel
                {
                    OperatorID= CafeOperatorId,
                    Status="1"
                };

                using (Activities.RemoteDataClass rd = new Activities.RemoteDataClass())
                {
                    var result = await rd.SetOpertorStatus(model);
                    Tools.SafeGuiWpf.SetEnabled(btnActivate, true);
                    Tools.SafeGuiWpf.SetEnabled(btnDeactivate, true);
                    Tools.SafeGuiWpf.SetEnabled(btnCancel, true);
                    Tools.SafeGuiWpf.SetText(txtmsg, "");
                }
                //Tools.SafeGuiWpf.Unloaded();
            });
        }

        private async void btnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            string CafeOperatorId;
            await Task.Run(async () =>
            {
                CafeOperatorId = Tools.SafeGuiWpf.GetText(txtOperatorId);
                if (string.IsNullOrWhiteSpace(CafeOperatorId))
                {
                    Tools.SafeGuiWpf.ShowWarning("Please enter Operator Id to Deactivate");//"Change Operator status",MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Tools.SafeGuiWpf.SetText(txtmsg, "Deactivating operator, please wait ... ");
                Tools.SafeGuiWpf.SetEnabled(btnActivate, false);
                Tools.SafeGuiWpf.SetEnabled(btnDeactivate, false);
                Tools.SafeGuiWpf.SetEnabled(btnCancel, false);
                var model = new SetOperatorStatusModel
                {
                    OperatorID = CafeOperatorId,
                    Status = "0"
                };

                using (Activities.RemoteDataClass rd = new Activities.RemoteDataClass())
                {
                    var result = await rd.SetOpertorStatus(model);
                    Tools.SafeGuiWpf.SetEnabled(btnActivate, true);
                    Tools.SafeGuiWpf.SetEnabled(btnDeactivate, true);
                    Tools.SafeGuiWpf.SetEnabled(btnCancel, true);
                    Tools.SafeGuiWpf.SetText(txtmsg, "");
                }
                //Tools.SafeGuiWpf.Unloaded();
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var grd = this.Parent as Grid;
            Tools.SafeGuiWpf.AddDashbordToGrid<DashBoardPage>(grd, this);
            Tools.SafeGuiWpf.SetVisible(this, Visibility.Collapsed);
        }

       
    }
}
