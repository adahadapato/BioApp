using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BioApp.Tools
{
    public class SafeGuiWpf
    {

       
        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        public static object GetTag(Control C)
        {
            if (C.Dispatcher.CheckAccess()) return C.Tag;
            else return C.Dispatcher.Invoke(new Func<Control, object>(GetTag), C);
        }
        public static string GetText(TextBox TB)
        {
            if (TB.Dispatcher.CheckAccess()) return TB.Text;
            else return (string)TB.Dispatcher.Invoke(new Func<TextBox, string>(GetText), TB);
        }

        public static string GetText(Button TB)
        {
            if (TB.Dispatcher.CheckAccess()) return TB.Content.ToString();
            else return (string)TB.Dispatcher.Invoke(new Func<Button, string>(GetText), TB);
        }
        public static string GetText(ComboBox TB)
        {
            if (TB.Dispatcher.CheckAccess()) return TB.Text;
            else return (string)TB.Dispatcher.Invoke(new Func<ComboBox, string>(GetText), TB);
        }

        public static string GetText(PasswordBox TB)
        {
            if (TB.Dispatcher.CheckAccess()) return TB.Password;
            else return (string)TB.Dispatcher.Invoke(new Func<PasswordBox, string>(GetText), TB);
        }

        public static void SetItems<T>(ListView TB, List<T> Str) where T : new()
        {
            if (TB.Dispatcher.CheckAccess())
            {
                TB.ItemsSource = Str;
                TB.Items.Refresh();
            }
            else TB.Dispatcher.Invoke(new Action<ListView, List<T>>(SetItems<T>), TB, Str);
        }

        //private static ToastViewModel _vm = new ToastViewModel();
        public static void ShowError(string message)//ShowWarning
        {
            ToastViewModel _vm = new ToastViewModel();
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _vm.ShowError(message);
            }
            else Application.Current.Dispatcher.Invoke(new Action<string>(ShowError), message);
        }

        public static void ShowWarning(string message)//ShowWarning
        {
            ToastViewModel _vm = new ToastViewModel();
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _vm.ShowWarning(message);
            }
            else Application.Current.Dispatcher.Invoke(new Action<string>(ShowWarning), message);
        }

        public static void ShowInformation(string message)
        {
            ToastViewModel _vm = new ToastViewModel();
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _vm.ShowInformation(message);
            }
            else Application.Current.Dispatcher.Invoke(new Action<string>(ShowInformation), message);
        }

        public static void ShowSuccess(string message)
        {
            ToastViewModel _vm = new ToastViewModel();
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _vm.ShowSuccess(message);
            }
            else Application.Current.Dispatcher.Invoke(new Action<string>(ShowSuccess), message);
        }

        /*public static void Unloaded()
        {
            ToastViewModel _vm = new ToastViewModel();
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _vm.OnUnloaded();
            }
            else Application.Current.Dispatcher.Invoke(new Action(Unloaded));
        }*/
        public static void SetItems(Chart TB, KeyValuePair<string, int>[] Str) 
        {
            if (TB.Dispatcher.CheckAccess())
            {
                //(ColumnSeries)mcChart.Series[0]).ItemsSource
                ((ColumnSeries)TB.Series[0]).ItemsSource = Str;
            }
            else TB.Dispatcher.Invoke(new Action<Chart, KeyValuePair<string, int>[]>(SetItems), TB, Str);
        }
        public static ImageSource GetImageSource(Image TB)
        {
            if (TB.Dispatcher.CheckAccess()) return TB.Source;
            else return (ImageSource)TB.Dispatcher.Invoke(new Func<Image, ImageSource>(GetImageSource), TB);
        }

        public static byte[] ByteFromImageSource(ImageSource TB)
        {
            if (TB.Dispatcher.CheckAccess()) return ProcessImageData.ConvertBitmapSourceToByteArray(TB);
            else return (byte[])TB.Dispatcher.Invoke(new Func<ImageSource, byte[]>(ByteFromImageSource), TB);
        }

        public static void ClearImage(Image TB)
        {
            if (TB.Dispatcher.CheckAccess()) TB.Source = null;
            else TB.Dispatcher.Invoke(new Action<Image>(ClearImage), TB);
        }

        public static void SetImage(Image TB, byte[] bytes)
        {
            if (TB.Dispatcher.CheckAccess()) TB.Source = ProcessImageData.ConvertByteArrayToBitmapImage(bytes);
            else TB.Dispatcher.Invoke(new Action<Image, byte[]>(SetImage), TB, bytes);
        }

        //Application.Current.Dispatcher.Invoke(new Action<Notifier, string, string>(Toast), TB, message, mType);

        public static BitmapImage SetImage(byte[] bytes)
        {
            if (Application.Current.Dispatcher.CheckAccess()) return (BitmapImage)ProcessImageData.ConvertByteArrayToBitmapImage(bytes); 
            else return (BitmapImage)Application.Current.Dispatcher.Invoke(new Func<byte[], BitmapImage>(SetImage), bytes);
        }
       /* public static string SetImage(UserControl TB, byte[] bytes)
        {
            if (TB.Dispatcher.CheckAccess()) return (string)ProcessImageData.byteToString(bytes);
            else return (string)TB.Dispatcher.Invoke(new Func<UserControl, byte[], string>(SetImage), TB, bytes);
        }*/

        public static void SetImage(Image TB, System.Drawing.Bitmap bmpImage)
        {
            if (TB.Dispatcher.CheckAccess()) TB.Source = ProcessImageData.ToBitmapSource(bmpImage);
            else TB.Dispatcher.Invoke(new Action<Image, System.Drawing.Bitmap>(SetImage), TB, bmpImage);
        }

        public static void SetImage(Image TB, string Source)
        {
            var imgUri = new Uri("pack://application:,,,/BioApp;component/Images/" + Source, UriKind.RelativeOrAbsolute);
            if (TB.Dispatcher.CheckAccess()) TB.Source = new BitmapImage(imgUri);
            else TB.Dispatcher.Invoke(new Action<Image, string>(SetImage), TB, Source);
        }
        /* public static ImageSource GetImageSource(Image TB)
         {
             if (TB.Dispatcher.CheckAccess()) return TB.Source;
             else return (ImageSource)TB.Dispatcher.Invoke(new Func<Image, ImageSource>(GetImageSource), TB);
         }*/

        /* public static byte[] ByteFromImageSource(ImageSource TB)
         {
             if (TB.Dispatcher.CheckAccess()) return ProcessImageData.ConvertBitmapSourceToByteArray(TB);
             else return (byte[])TB.Dispatcher.Invoke(new Func<ImageSource, byte[]>(ByteFromImageSource), TB);
         }*/
        public static void SetText(TextBlock TB, string[] Str)
        {
            SetText(TB, "");
            if (TB.Dispatcher.CheckAccess())
            {
                foreach (string s in Str)
                    TB.Text += s + "\n";
            }
            else TB.Dispatcher.Invoke(new Action<TextBlock, string[]>(SetText), TB, Str);
        }

        public static void SetText(TextBlock TB, string Str)
        {
            if (TB.Dispatcher.CheckAccess()) TB.Text = Str;
            else TB.Dispatcher.Invoke(new Action<TextBlock, string>(SetText), TB, Str);
        }
        public static void SetText(TextBox TB, string Str)
        {
            if (TB.Dispatcher.CheckAccess()) TB.Text = Str;
            else TB.Dispatcher.Invoke(new Action<TextBox, string>(SetText), TB, Str);
        }

        public static void SetText(Button TB, string Str)
        {
            if (TB.Dispatcher.CheckAccess()) TB.Content = Str;
            else TB.Dispatcher.Invoke(new Action<Button, string>(SetText), TB, Str);
        }
        public static void AppendText(TextBox TB, string Str)
        {
            if (TB.Dispatcher.CheckAccess())
            {
                TB.AppendText(Str);
                TB.ScrollToEnd(); // scroll to end?
            }
            else TB.Dispatcher.Invoke(new Action<TextBox, string>(AppendText), TB, Str);
        }
        public static bool? GetChecked(CheckBox Ck)
        {
            if (Ck.Dispatcher.CheckAccess()) return Ck.IsChecked;
            else return (bool?)Ck.Dispatcher.Invoke(new Func<CheckBox, bool?>(GetChecked), Ck);
        }
        public static void SetChecked(CheckBox Ck, bool? V)
        {
            if (Ck.Dispatcher.CheckAccess()) Ck.IsChecked = V;
            else Ck.Dispatcher.Invoke(new Action<CheckBox, bool?>(SetChecked), Ck, V);
        }
        public static bool GetChecked(MenuItem Ck)
        {
            if (Ck.Dispatcher.CheckAccess()) return Ck.IsChecked;
            else return (bool)Ck.Dispatcher.Invoke(new Func<MenuItem, bool>(GetChecked), Ck);
        }
        public static void SetChecked(MenuItem Ck, bool V)
        {
            if (Ck.Dispatcher.CheckAccess()) Ck.IsChecked = V;
            else Ck.Dispatcher.Invoke(new Action<MenuItem, bool>(SetChecked), Ck, V);
        }
        public static bool? GetChecked(RadioButton Ck)
        {
            if (Ck.Dispatcher.CheckAccess()) return Ck.IsChecked;
            else return (bool?)Ck.Dispatcher.Invoke(new Func<RadioButton, bool?>(GetChecked), Ck);
        }
        public static void SetChecked(RadioButton Ck, bool? V)
        {
            if (Ck.Dispatcher.CheckAccess()) Ck.IsChecked = V;
            else Ck.Dispatcher.Invoke(new Action<RadioButton, bool?>(SetChecked), Ck, V);
        }

        public static void SetVisible(UIElement Emt, Visibility V)
        {
            if (Emt.Dispatcher.CheckAccess()) Emt.Visibility = V;
            else Emt.Dispatcher.Invoke(new Action<UIElement, Visibility>(SetVisible), Emt, V);
        }
        public static Visibility GetVisible(UIElement Emt)
        {
            if (Emt.Dispatcher.CheckAccess()) return Emt.Visibility;
            else return (Visibility)Emt.Dispatcher.Invoke(new Func<UIElement, Visibility>(GetVisible), Emt);
        }
        public static bool GetEnabled(UIElement Emt)
        {
            if (Emt.Dispatcher.CheckAccess()) return Emt.IsEnabled;
            else return (bool)Emt.Dispatcher.Invoke(new Func<UIElement, bool>(GetEnabled), Emt);
        }
        public static void SetEnabled(UIElement Emt, bool V)
        {
            if (Emt.Dispatcher.CheckAccess()) Emt.IsEnabled = V;
            else Emt.Dispatcher.Invoke(new Action<UIElement, bool>(SetEnabled), Emt, V);
        }

        public static void SetSelectedItem(Selector Ic, object Selected)
        {
            if (Ic.Dispatcher.CheckAccess()) Ic.SelectedItem = Selected;
            else Ic.Dispatcher.Invoke(new Action<Selector, object>(SetSelectedItem), Ic, Selected);
        }
        public static object GetSelectedItem(Selector Ic)
        {
            if (Ic.Dispatcher.CheckAccess()) return Ic.SelectedItem;
            else return Ic.Dispatcher.Invoke(new Func<Selector, object>(GetSelectedItem), Ic);
        }
        public static int GetSelectedIndex(Selector Ic)
        {
            if (Ic.Dispatcher.CheckAccess()) return Ic.SelectedIndex;
            else return (int)Ic.Dispatcher.Invoke(new Func<Selector, int>(GetSelectedIndex), Ic);
        }

        /*delegate MessageBoxResult MsgBoxDelegate(Window owner, string text, string caption, MessageBoxButton button, MessageBoxImage icon);
        public static MessageBoxResult MsgBox(Window owner, string text, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            if (owner.Dispatcher.CheckAccess()) return MessageBox.Show(owner, text, caption, button, icon);
            else return (MessageBoxResult)owner.Dispatcher.Invoke(new MsgBoxDelegate(MsgBox), owner, text, caption, button, icon);
        }*/


        delegate MessageBoxResult WXMsgBoxDelegate(Window owner, string caption, string text, MessageBoxButton button, WpfMessageBox.MessageBoxImage icon);
        public static MessageBoxResult WMsgBox(Window owner, string caption, string text, MessageBoxButton button, WpfMessageBox.MessageBoxImage icon)
        {
            if (owner.Dispatcher.CheckAccess()) return WpfMessageBox.Show(caption, text, button, icon);
            else return (MessageBoxResult)owner.Dispatcher.Invoke(new WXMsgBoxDelegate(WMsgBox), owner, caption, text, button, icon);
        }


        delegate MessageBoxResult UXMsgBoxDelegate(UserControl owner, string caption, string text, MessageBoxButton button, WpfMessageBox.MessageBoxImage icon);
        public static MessageBoxResult UMsgBox(UserControl owner, string caption, string text, MessageBoxButton button, WpfMessageBox.MessageBoxImage icon)
        {
            if (owner.Dispatcher.CheckAccess()) return WpfMessageBox.Show(caption, text, button, icon);
            else return (MessageBoxResult)owner.Dispatcher.Invoke(new UXMsgBoxDelegate(UMsgBox), owner, caption, text, button, icon);
        }

        delegate MessageBoxResult MsgBoxDelegateEx(string caption, string text, MessageBoxButton button, WpfMessageBox.MessageBoxImage icon);
        public static MessageBoxResult MsgBoxEx(string caption, string text, MessageBoxButton button, WpfMessageBox.MessageBoxImage icon)
        {
            if (Application.Current.Dispatcher.CheckAccess()) return WpfMessageBox.Show(caption, text, button, icon);
            else return (MessageBoxResult)Application.Current.Dispatcher.Invoke(new MsgBoxDelegateEx(MsgBoxEx), caption, text, button, icon);
        }

        delegate MessageBoxResult WMsgBoxDelegate(Window owner, string text, string caption,  MessageBoxButton button, MessageBoxImage icon);
        public static MessageBoxResult WMsgBox(Window owner, string text, string caption,  MessageBoxButton button, MessageBoxImage icon)
        {
            if (owner.Dispatcher.CheckAccess()) return Xceed.Wpf.Toolkit.MessageBox.Show(text, caption, button, icon);
            else return (MessageBoxResult)owner.Dispatcher.Invoke(new WMsgBoxDelegate(WMsgBox), owner, text, caption,  button, icon);
        }


        delegate MessageBoxResult MsgBoxDelegate(string text, string caption, MessageBoxButton button, MessageBoxImage icon);
        public static MessageBoxResult MsgBox(string text, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            if (Application.Current.Dispatcher.CheckAccess()) return Xceed.Wpf.Toolkit.MessageBox.Show(text, caption, button, icon);
            else return (MessageBoxResult)Application.Current.Dispatcher.Invoke(new MsgBoxDelegate(MsgBox), text, caption, button, icon);
        }


        delegate MessageBoxResult UMsgBoxDelegate(UserControl owner, string text, string caption,  MessageBoxButton button, MessageBoxImage icon);
        public static MessageBoxResult UMsgBox(UserControl owner, string text, string caption,  MessageBoxButton button, MessageBoxImage icon)
        {
            if (owner.Dispatcher.CheckAccess()) return Xceed.Wpf.Toolkit.MessageBox.Show(text, caption,  button, icon);
            else return (MessageBoxResult)owner.Dispatcher.Invoke(new UMsgBoxDelegate(UMsgBox), owner, text, caption,  button, icon);
        }


        public static double GetRangeValue(RangeBase RngBse)
        {
            if (RngBse.Dispatcher.CheckAccess()) return RngBse.Value;
            else return (double)RngBse.Dispatcher.Invoke(new Func<RangeBase, double>(GetRangeValue), RngBse);
        }
        public static void SetRangeValue(RangeBase RngBse, double V)
        {
            if (RngBse.Dispatcher.CheckAccess()) RngBse.Value = V;
            else RngBse.Dispatcher.Invoke(new Action<RangeBase, double>(SetRangeValue), RngBse, V);
        }

        public static void AddDashbordToGrid<T>(Grid grd, UIElement Page)
         where T : UserControl, new()
        {
            if (grd.Dispatcher.CheckAccess())
            {
                var Win = new T();
                grd.Children.Remove(Page);
                if (grd.Children.Count == 0)
                {
                    grd.Children.Add(Win);
                    //grd.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/idenysistime.net;component/Images/herobg.jpg")));
                    return;
                }
                grd.Background = new ImageBrush(null);
            }
            else grd.Dispatcher.Invoke(new Action<Grid, UIElement>(AddDashbordToGrid<T>), grd, Page);
        }

        public static Grid AddUserControlToGrid<T>(Grid grd) where T : UserControl, new()
        {
            if (grd.Dispatcher.CheckAccess())
            {
                var Win = new T(); // Window created on GUI thread
                grd.Children.Clear();
                grd.Children.Add(Win);
                return grd;
            }
            else return (Grid)grd.Dispatcher.Invoke(new Func<Grid, Grid>(AddUserControlToGrid<T>), grd);
        }

        public static void AddUserControlToGrid<T>(Grid grd, bool clearGrid)
        where T : UserControl, new()
        {
            if (grd.Dispatcher.CheckAccess())
            {
                var Win = new T();
                if (clearGrid)
                    grd.Children.Clear();

                grd.Background = new ImageBrush(null);
                grd.Children.Add(Win);

            }
            else grd.Dispatcher.Invoke(new Action<Grid, bool>(AddUserControlToGrid<T>), grd, clearGrid);
        }

        public static void AddUserControlToGrid(Grid grd)
        {
            if (grd.Dispatcher.CheckAccess())
            {
                grd.Children.Clear();
            }
            else grd.Dispatcher.Invoke(new Action<Grid>(AddUserControlToGrid), grd);
        }

        public static void RemoveUserControlFromGrid(Grid grd, UserControl cntrl)
        {
            if (grd.Dispatcher.CheckAccess())
            {
                grd.Children.Remove(cntrl);
            }
            else grd.Dispatcher.Invoke(new Action<Grid, UserControl>(RemoveUserControlFromGrid), grd, cntrl);
        }

        public static void AddUserControlToGrid<T, T2>(Grid grd, T2 model, bool clearGrid)
            where T : UserControl, new()
            where T2 : new()
        {
            if (grd.Dispatcher.CheckAccess())
            {
                var Win = Activator.CreateInstance(typeof(T), new object[] { model }) as T;
                if (clearGrid)
                    grd.Children.Clear();

                grd.Background = new ImageBrush(null);
                grd.Children.Add(Win);
            }
            else grd.Dispatcher.Invoke(new Action<Grid, T2, bool>(AddUserControlToGrid<T, T2>), grd, model, clearGrid);
        }
        public static T CreateWindow<T>(Window Owner) where T : Window, new()
        {
            if (Owner.Dispatcher.CheckAccess())
            {

                var Win = new T(); // Window created on GUI thread
                Win.Owner = Owner;
                return Win;
            }
            else return (T)Owner.Dispatcher.Invoke(new Func<Window, T>(CreateWindow<T>), Owner);
        }


        public static void ReportProgress(ProgressBar TB, bool value, int percent)
        {
            if (TB.Dispatcher.CheckAccess())
            {
                TB.IsIndeterminate = value;
                TB.Value = percent;
            }
            else TB.Dispatcher.Invoke(new Action<ProgressBar, bool, int>(ReportProgress), TB, value, percent);
        }

        public static void ShowWindow<T>(string Args) where T : Window, new()
          {
              if (Application.Current.Dispatcher.CheckAccess())
              {
                var Win = Activator.CreateInstance(typeof(T), new object[] { Args }) as T;
                Win.Show();
              }
              else Application.Current.Dispatcher.Invoke(new Action<string>(ShowWindow<T>), Args);
          }

        public static void ShowWindow<T>() where T : Window, new()
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                var Win = Activator.CreateInstance(typeof(T)) as T;
                Win.Show();
            }
            else Application.Current.Dispatcher.Invoke(new Action<string>(ShowWindow<T>));
        }

        public static void CloseWindow(Window Win) 
        {
            if (Win.Dispatcher.CheckAccess())
            {
                //var Win = Activator.CreateInstance(typeof(T)) as T;
                Win.Close();
                
            }
            else Win.Dispatcher.Invoke(new Action<Window>(CloseWindow), Win);
        }

        public static void ShowWindow<T, T2>(string Args, T2 model)
         where T : Window, new()
         where T2 : new()
         
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                var Win = Activator.CreateInstance(typeof(T), new object[] { Args, model }) as T;
                Win.Show();

            }
            else Application.Current.Dispatcher.Invoke(new Action<string, T2>(ShowWindow<T, T2>), Args, model);
        }

       /* public static void ShowWindow<T, T2, T3>(Grid grd, T2 model, T3 model2, bool clearGrid)
          where T : UserControl, new()
          where T2 : new()
          where T3 : new()
        {
            if (grd.Dispatcher.CheckAccess())
            {
                var Win = Activator.CreateInstance(typeof(T), new object[] { model, model2 }) as T;
                if (clearGrid)
                    grd.Children.Clear();

                grd.Background = new ImageBrush(null);
                grd.Children.Add(Win);

            }
            else grd.Dispatcher.Invoke(new Action<Grid, T2, T3, bool>(AddUserControlToGrid<T, T2, T3>), grd, clearGrid);
        }*/

        //grd.Dispatcher.Invoke(new Action<Grid, T2, T3, bool>(AddUserControlToGrid<T, T2, T3>), grd, clearGrid);
        public static bool? ShowDialog(Window Dialog)
        {
            if (Dialog.Dispatcher.CheckAccess()) return Dialog.ShowDialog();
            else return (bool?)Dialog.Dispatcher.Invoke(new Func<Window, bool?>(ShowDialog), Dialog);
        }

        public static void SetDialogResult(Window Dialog, bool? Result)
        {
            if (Dialog.Dispatcher.CheckAccess()) Dialog.DialogResult = Result;
            else Dialog.Dispatcher.Invoke(new Action<Window, bool?>(SetDialogResult), Dialog, Result);
        }

        public static Window GetWindowOwner(Window window)
        {
            if (window.Dispatcher.CheckAccess()) return window.Owner;
            else return (Window)window.Dispatcher.Invoke(new Func<Window, Window>(GetWindowOwner), window);
        }

      
    }
}
