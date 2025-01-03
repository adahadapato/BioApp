using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace BioApp
{
    public class ToastViewModel : INotifyPropertyChanged
    {
        private readonly Notifier _notifier;

        public ToastViewModel()
        {
            SingleInstanceApplication.MergeResourceDictionary(new Uri(@"/ToastNotifications.Messages;component/Themes/Default.xaml", UriKind.Relative));
            _notifier = new Notifier(cfg =>
            {
                /*cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow, 
                    corner: Corner.TopRight, 
                    offsetX: 6,  
                    offsetY: 32);*/
                cfg.PositionProvider = new PrimaryScreenPositionProvider(
                    corner: Corner.BottomRight, 
                    offsetX: 10,
                    offsetY: 10);
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3), 
                    maximumNotificationCount: MaximumNotificationCount.FromCount(15));
                cfg.Dispatcher = Application.Current.Dispatcher;
                cfg.DisplayOptions.TopMost = false;
                //cfg.DisplayOptions.Width = 250;
            });
            _notifier.ClearMessages();
        }

        public void OnUnloaded()
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _notifier.Dispose();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      _notifier.Dispose();
                  }));
            }
        }

        public void ShowInformation(string message)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _notifier.ShowInformation(message);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      _notifier.ShowInformation(message);
                  }));
            }
            
        }

        public void ShowInformation(string message, MessageOptions opts)
        {
            _notifier.ShowInformation(message, opts);
        }

        public void ShowSuccess(string message)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _notifier.ShowSuccess(message);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      _notifier.ShowSuccess(message);
                  }));
            }
            
        }

        public void ShowSuccess(string message, MessageOptions opts)
        {
            _notifier.ShowSuccess(message, opts);
        }

        internal void ClearMessages(string msg)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _notifier.ClearMessages(msg);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      _notifier.ClearMessages(msg);
                  }));
            }
        }

        public void ShowWarning(string message, MessageOptions opts)
        {
            _notifier.ShowWarning(message, opts);
        }

        public void ShowWarning(string message)
        {
            //_notifier.ShowWarning(message);
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _notifier.ShowWarning(message);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      _notifier.ShowWarning(message);
                  }));
            }
        }
        public void ShowError(string message)
        {
           
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _notifier.ShowError(message);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      _notifier.ShowError(message);
                  }));
            }
        }

        public void ShowError(string message, MessageOptions opts)
        {
            _notifier.ShowError(message, opts);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}