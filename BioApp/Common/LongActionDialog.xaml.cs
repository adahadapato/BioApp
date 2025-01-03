using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BioApp
{
    /// <summary>
    /// Interaction logic for LongActionDialog.xaml
    /// </summary>
    public partial class LongActionDialog : Window
    {
        public LongActionDialog(string title)
        {
            InitializeComponent();
           Tools.SafeGuiWpf.SetText(lblTitle, title);
            this.DataContext = this;
        }

        #region Private fields

        private bool _canClose = false;
        private Task _task = null;

        #endregion

        #region Public static methods

        public static T ShowDialog<T>(Window owner, string title, Task<T> task)
        {
           
            if (Application.Current.Dispatcher.CheckAccess())
            {
                var dialog = new LongActionDialog(title)
                {
                    _task = task
                };
                //dialog.ShowDialog(owner);
                dialog.ShowDialog();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                 DispatcherPriority.Background,
                 new Action(() => {
                     var dialog = new LongActionDialog(title)
                     {
                         _task = task
                     };
                     //dialog.ShowDialog(owner);
                     dialog.ShowDialog();
                 }));
            }
            return task.Result;
        }

        public static T ShowDialog<T>(string title, Task<T> task)
        {
            try
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    var dialog = new LongActionDialog(title)
                    {
                        _task = task
                    };
                    dialog.ShowDialog();
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(
                     DispatcherPriority.Background,
                     new Action(() => {
                         var dialog = new LongActionDialog(title)
                         {
                             _task = task
                         };
                         dialog.ShowDialog();

                     }));
                }
                
            }
            catch { }
            return task.Result;
        }


        public static void ShowDialog(Window owner, string title, Task task)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                var dialog = new LongActionDialog(title)
                {
                    _task = task
                };
                dialog.ShowDialog();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                 DispatcherPriority.Background,
                 new Action(() => {
                     var dialog = new LongActionDialog(title)
                     {
                         _task = task
                     };
                     dialog.ShowDialog();
                 }));
            }
        }


        public static void ShowDialog(string title, Task task)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                var dialog = new LongActionDialog(title)
                {
                    _task = task
                };
                dialog.ShowDialog();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                 DispatcherPriority.Background,
                 new Action(() => {
                     var dialog = new LongActionDialog(title)
                     {
                         _task = task
                     };
                     dialog.ShowDialog();
                 }));
            }
        }
        #endregion

        #region Private methods

        /* private void LongTaskFormFormClosing(object sender, FormClosingEventArgs e)
         {

         }*/

        private void LongActionDialogShown(object sender, RoutedEventArgs e)
        {
            _task.ContinueWith(t =>
            {
                _canClose = true;
                /*if (Application.Current.Dispatcher.CheckAccess())
                   Application.Current.Dispatcher.BeginInvoke(new Action(Close));
                else
                    Close();*/
                Tools.SafeGuiWpf.SetVisible(this, Visibility.Collapsed);
            });
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            _task.ContinueWith(t =>
            {
                _canClose = true;
                /*if (Application.Current.Dispatcher.CheckAccess())
                    Application.Current.Dispatcher.BeginInvoke(new Action(Close));
                else*/
                   Tools.SafeGuiWpf.SetVisible(this,Visibility.Collapsed);
            });
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !_canClose;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tools.SafeGuiWpf.ReportProgress(pbStatus, true, 0);
            
           // LongActionDialogShown(sender, e);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
           /* _task.ContinueWith(t =>
            {
                _canClose = true;
                if (Application.Current.Dispatcher.CheckAccess())
                    Application.Current.Dispatcher.BeginInvoke(new Action(Close));
                else
                    Close();
            });*/
        }



       
    }

   
}
