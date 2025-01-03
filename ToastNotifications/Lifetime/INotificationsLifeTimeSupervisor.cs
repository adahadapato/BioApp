using System;
using System.Windows.Threading;
using ToastNotifications.Core;

namespace ToastNotifications.Lifetime
{
    public interface INotificationsLifetimeSupervisor : IDisposable
    {
        void PushNotification(INotification notification);
        void CloseNotification(INotification notification);

        void UseDispatcher(Dispatcher dispatcher);
        
        event EventHandler<ShowNotificationEventArgs> ShowNotificationRequested;
        event EventHandler<CloseNotificationEventArgs> CloseNotificationRequested;

        void ClearMessages(string msg);
    }
}