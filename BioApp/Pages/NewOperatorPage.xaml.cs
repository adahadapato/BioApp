using BioApp.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using BioApp.Tools;
using System.Windows.Threading;
using BioApp.Activities;

namespace BioApp.Pages
{
    /// <summary>
    /// Interaction logic for NewOperatorPage.xaml
    /// </summary>
    public partial class NewOperatorPage : UserControl
    {
        
        private List<NewCafeOperatorViewModel> newCafeOperatorViewModel;
        private int RecNo = 0;

        private string stateCode = "";
        public NewOperatorPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Tools.SafeGuiWpf.SetEnabled(btnAdd, false);
            //Tools.SafeGuiWpf.SetEnabled(btnSave, false);
            newCafeOperatorViewModel = new List<NewCafeOperatorViewModel>();
            FetchStates();
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private async void FetchStates()
        {
            await Task.Run(async () =>
            {
                using (Activities.FetchDataClass fd = new Activities.FetchDataClass())
                {
                    var stateList = await fd.FetchStates();
                    UpdateStatesComboBox(stateList);
                }
            });
          
        }

        private void cmbState_DropDownClosed(object sender, EventArgs e)
        {
            
            var item = SafeGuiWpf.GetSelectedItem(sender as ComboBox) as StateModel;
            if (item != null)
            {

                this.stateCode = item.code.Substring(1,2);
            }
           
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var grd = this.Parent as Grid;
            SafeGuiWpf.AddDashbordToGrid<DashBoardPage>(grd, this);
            SafeGuiWpf.SetVisible(this, Visibility.Collapsed);
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var newCafeOperatorModel = new List<NewCafeOperatorModel>();
            var cafeTemp = (from r in newCafeOperatorViewModel.AsEnumerable()
                              where (r.IsSelected == true)
                              select r).ToList();

            if (cafeTemp.Count == 0)
            {
                SafeGuiWpf.SetText(txtmsg, "Please select Operator(s) to upload");
                return;
            }

            foreach (var r in cafeTemp)
            {
                newCafeOperatorModel.Add(new NewCafeOperatorModel
                {
                    operatorid = r.operatorid,
                    name = r.name,
                    email = r.email,
                    phoneno = r.phoneno,
                    state = r.state
                });
            }

            using (RemoteDataClass rd = new RemoteDataClass())
            {
                var result = await rd.AddNewOpertor(newCafeOperatorModel);
                if (result)
                {
                    //SafeGuiWpf.UMsgBox(this, "Operator added successfully", "Add operator", MessageBoxButton.OK, MessageBoxImage.Information);
                    newCafeOperatorModel.Clear();
                }
             }
        }

        private void Reset()
        {
            Task.Run(() =>
            {
                SafeGuiWpf.SetText(txtOperatorId, "");
                SafeGuiWpf.SetText(txtCafeName, "");
                SafeGuiWpf.SetText(txtEmailAddress, "");
                SafeGuiWpf.SetText(txtPhoneNUmber, "");
            });
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string Operatoid = SafeGuiWpf.GetText(txtOperatorId);
            string cafe = SafeGuiWpf.GetText(txtCafeName);
            string Email = SafeGuiWpf.GetText(txtEmailAddress);
            string Phone = SafeGuiWpf.GetText(txtPhoneNUmber).ToString();
            string state = this.stateCode;

            
            newCafeOperatorViewModel.Add(new NewCafeOperatorViewModel
            {
                operatorid= Operatoid,
                name=cafe,
                phoneno=Phone,
                email=Email,
                state=state
            });
            
            SafeGuiWpf.SetItems<NewCafeOperatorViewModel>(lv,newCafeOperatorViewModel);
            //SafeGuiWpf.SetText(txtmsg, newCafeOperatorViewModel.Count.ToString() + " Records waiting upload");
            DoRecCount(this.RecNo.ToString());

            Reset();
        }

        private void btnImportCSV_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                string fileName = "";
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "NECO Biometrics Operator files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "NECO Biometrics Opertor FIles"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    fileName = openFileDialog.FileName;
                }
                var model = new List<NewCafeOperatorModel>();
                string[] seps = { "\",", ",\"" };
                char[] quotes = { '\"', ' ' };

                var csv = File.ReadAllLines(fileName)
                       .Skip(1)
                       .Select(x => x.Split(seps, StringSplitOptions.None))
                       .Select(x => new
                       {
                           operatorid = x[0].Trim(quotes).Replace("\\\"", "\""),
                           name = x[1].Trim(quotes).Replace("\\\"", "\""),
                           state = x[2].Trim(quotes).Replace("\\\"", "\""),
                           email = x[3].Trim(quotes).Replace("\\\"", "\""),
                           phoneno = x[4].Trim(quotes).Replace("\\\"", "\"")
                       });

                foreach (var r in csv)
                {
                    newCafeOperatorViewModel.Add(new NewCafeOperatorViewModel
                    {
                        operatorid = r.operatorid,
                        name = r.name,
                        email = r.email,
                        phoneno = r.phoneno,
                        state = r.state
                    });
                }
                if (newCafeOperatorViewModel.Count > 0)
                {
                    SafeGuiWpf.SetItems<NewCafeOperatorViewModel>(lv, newCafeOperatorViewModel);
                    DoRecCount(this.RecNo.ToString());
                }
                
                
            });
            
        }

        private void DoRecCount(string Rec)
        {
            SafeGuiWpf.SetText(txtmsg, Rec + " Of " + newCafeOperatorViewModel.Count.ToString() + " Records selected upload");
        }

        #region ListView CheckBoxes
        private static bool individualChkBxUnCheckedFlag { get; set; }
        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SafeGuiWpf.SetText(txtmsg, "");
            if (e.AddedItems.Count > 0)
            {
                //------------
                NewCafeOperatorViewModel model = (NewCafeOperatorViewModel)e.AddedItems[0];
                ListViewItem lvi = (ListViewItem)lv.ItemContainerGenerator.ContainerFromItem(model);
                CheckBox chkBx = SafeGuiWpf.FindVisualChild<CheckBox>(lvi);
                if (chkBx != null)
                {
                    chkBx.IsChecked = true;
                    this.RecNo++;
                }
                DoRecCount(this.RecNo.ToString());
                //------------               
            }
            else // Remove Select All chkBox
            {
                NewCafeOperatorViewModel model = (NewCafeOperatorViewModel)e.RemovedItems[0];
                ListViewItem lvi = (ListViewItem)lv.ItemContainerGenerator.ContainerFromItem(model);
                CheckBox chkBx = SafeGuiWpf.FindVisualChild<CheckBox>(lvi);
                if (chkBx != null)
                {
                    chkBx.IsChecked = false;
                    this.RecNo--;
                    
                }
                DoRecCount(this.RecNo.ToString());
            }
            //============        
        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                individualChkBxUnCheckedFlag = false;
                //=====================
                foreach (NewCafeOperatorViewModel item in lv.ItemsSource)
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
                foreach (NewCafeOperatorViewModel item in lv.ItemsSource)
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

        void UpdateStatesComboBox(List<StateModel> stateList)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                cmbState.ItemsSource = stateList;
                cmbState.DisplayMemberPath = "state";
                cmbState.SelectedValuePath = "code";
                //cmbDevice.SelectedValue = "0";
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      cmbState.ItemsSource = stateList;
                      cmbState.DisplayMemberPath = "state";
                      cmbState.SelectedValuePath = "code";
                      //cmbDevice.SelectedValue = "0";
                  }));
            }
        }

      
    }
}
