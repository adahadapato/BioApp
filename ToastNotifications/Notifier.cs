using System;
using System.Windows;
using ToastNotifications.Core;
using ToastNotifications.Display;
using ToastNotifications.Lifetime;

namespace ToastNotifications
{
    public class Notifier : IDisposable
    {
        private readonly object _syncRoot = new object();

        private readonly Action<NotifierConfiguration> _configureAction;
        private NotifierConfiguration _configuration;
        private INotificationsLifetimeSupervisor _lifetimeSupervisor;
        private NotificationsDisplaySupervisor _displaySupervisor;

        public Notifier(Action<NotifierConfiguration> configureAction)
        {
            _configureAction = configureAction;
        }

        public void Notify<T>(Func<INotification> createNotificationFunc)
        {
            Configure();
            _lifetimeSupervisor.PushNotification(createNotificationFunc());
        }

        private void Configure()
        {
            lock (_syncRoot)
            {
                if (_configuration != null)
                    return;

                var cfg = new NotifierConfiguration
                {
                    Dispatcher = Application.Current.Dispatcher
                };
                _configureAction(cfg);

                if (cfg.LifetimeSupervisor == null)
                    throw new ArgumentNullException(nameof(cfg.LifetimeSupervisor), "Missing configuration argument");

                if (cfg.PositionProvider == null)
                    throw new ArgumentNullException(nameof(cfg.PositionProvider), "Missing configuration argument");

                _configuration = cfg;
                _lifetimeSupervisor = cfg.LifetimeSupervisor;
                _lifetimeSupervisor.UseDispatcher(cfg.Dispatcher);

                _displaySupervisor = new NotificationsDisplaySupervisor(
                    cfg.Dispatcher, 
                    cfg.PositionProvider, 
                    cfg.LifetimeSupervisor, 
                    cfg.DisplayOptions);
            }
        }

        public void ClearMessages()
        {
            ClearMessages("");
        }

        public void ClearMessages(string msg)
        {
            this._lifetimeSupervisor?.ClearMessages(msg);
        }

        private bool _disposed = false;

        public object SyncRoot => _syncRoot;

        public void Dispose()
        {
            if (_disposed == false)
            {
                _disposed = true;
                _configuration?.PositionProvider?.Dispose();
                _displaySupervisor?.Dispose();
                _lifetimeSupervisor?.Dispose();
            }
        }
    }
}
