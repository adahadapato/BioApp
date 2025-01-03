using BioApp.Activities;
using BioApp.Models;
using BioApp.Tools;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace BioApp
{
    /// <summary>
    /// Interaction logic for OperatorBankDetailsPage.xaml
    /// </summary>
    public partial class OperatorBankDetailsPage : Window
    {
        string bank_name = "";
        string account_no = "";
        string bank_code = "";
        string account_name = "";
        //string Operatorid = "";
        string sort_code = "";
        string bank_address = "";
        string Filename;
        private CafeOperatorModel cafeModel;
        private bool isOperationSucceeded = false;
        public OperatorBankDetailsPage()
        {
            InitializeComponent();
            //this.Filename = Filename;
           
        }

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBanks();
            SafeGuiWpf.SetText(txtmsg, "");
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        public CafeOperatorModel cafeOperatorModel
        {
            set { cafeModel=value; }
        }
        public bool IsOperationSucceeded
        {
            get { return isOperationSucceeded; }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async() =>
            {
                SafeGuiWpf.SetText(txtmsg, "Saving payment details, please wait ... ");
                this.account_name = SafeGuiWpf.GetText(txtAccountName).Trim();
                this.account_no = SafeGuiWpf.GetText(txtAccountNo).Trim();
                this.sort_code = SafeGuiWpf.GetText(txtSortCode);
                this.bank_address = SafeGuiWpf.GetText(txtBankAddress);

                if (string.IsNullOrWhiteSpace(this.bank_name))
                {
                    SafeGuiWpf.ShowError("Invalid Bank Name");//, "Error",  MessageBoxButton.OK, MessageBoxImage.Error);
                    SafeGuiWpf.SetText(txtmsg, "");
                    isOperationSucceeded = false;
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.bank_address))
                {
                    SafeGuiWpf.ShowError("Invalid Bank Address");//, "Error",  MessageBoxButton.OK, MessageBoxImage.Error);
                    SafeGuiWpf.SetText(txtmsg, "");
                    isOperationSucceeded = false;
                    return;
                }

                if (this.sort_code.Length != 9)
                {
                    SafeGuiWpf.ShowError("Invalid Sort Code");//, "Error",  MessageBoxButton.OK, MessageBoxImage.Error);
                    SafeGuiWpf.SetText(txtmsg, "");
                    isOperationSucceeded = false;
                    return;
                }

                if (this.account_no.Length != 10)
                {
                    SafeGuiWpf.ShowError("Invalid Account No");//, "Error",  MessageBoxButton.OK, MessageBoxImage.Error);
                    SafeGuiWpf.SetText(txtmsg, "");
                    isOperationSucceeded = false;
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.account_name) )
                {
                    SafeGuiWpf.ShowError("Invalid Account Name");//, "Error",  MessageBoxButton.OK, MessageBoxImage.Error);
                    SafeGuiWpf.SetText(txtmsg, "");
                    isOperationSucceeded = false;
                    return;
                }
                
                UpdateBankDetailsModel model = new UpdateBankDetailsModel
                {
                    operatorID = cafeModel.Operatorid,
                    bankName = this.bank_name,
                    accountName = this.account_name,
                    accountNumber = this.account_no,
                    accountSortCode = sort_code,
                    bankAddress = bank_address
                };

                using (RemoteDataClass rd = new RemoteDataClass())
                {
                    var result = await rd.UpdateBankDetails(model) as UpdateBankDetailsResult;
                    if(result.bankValidated=="1")
                    {
                        
                        CafeOperatorClass.WriteOperatorId(cafeModel);

                       
                        SafeGuiWpf.ShowSuccess("Your Id is activated successfully for NECO SSCE JUN/JUL 2018 Biometrics Data capture exercise\n" +
                                             "Details: Id [ " + cafeModel.Operatorid + " ] Cafe [ " + CafeOperatorClass.GetName + " ]");
                        isOperationSucceeded = true;
                        SafeGuiWpf.CloseWindow(this);
                    }
                    else
                    {
                        SafeGuiWpf.ShowError("Your Id is cannot be activated for NECO SSCE JUN/JUL 2018 Biometrics Data capture/validation exercise\n" +
                                            "Details: Id [ " + cafeModel.Operatorid + " ] Cafe [ " + cafeModel.Cafe + " ]\n" +
                                            "Please ensure that your payment details is entered correctly.");

                        SafeGuiWpf.SetText(txtmsg, "");
                        isOperationSucceeded = false;
                    }
                    
                }
            });


        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private async void LoadBanks()
        {
            using (FetchDataClass fd = new FetchDataClass())
            {
                var result = await fd.FetchBanks();
                UpdateBnakComboBox(result);
            }
        }

        void UpdateBnakComboBox(List<BankSaveModel> lst)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                cmbBankName.ItemsSource = lst;
                cmbBankName.DisplayMemberPath = "bank_name";
                cmbBankName.SelectedValuePath = "bank_code";
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      cmbBankName.ItemsSource = lst;
                      cmbBankName.DisplayMemberPath = "bank_name";
                      cmbBankName.SelectedValuePath = "bank_code";
                  }));
            }
        }

        private void cmbBankName_DropDownClosed(object sender, EventArgs e)
        {
            var items = SafeGuiWpf.GetSelectedItem(sender as ComboBox) as BankSaveModel;
            if (items != null)
            {
                this.bank_code = items.bank_code;
                this.bank_name = items.bank_name.Trim();
            }
        }
    }
}
