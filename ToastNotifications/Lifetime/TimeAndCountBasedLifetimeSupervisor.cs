using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using ToastNotifications.Core;
using ToastNotifications.Utilities;

namespace ToastNotifications.Lifetime
{
    public class TimeAndCountBasedLifetimeSupervisor : INotificationsLifetimeSupervisor
    {
        private readonly TimeSpan _notificationLifetime;
        private readonly int _maximumNotificationCount;

        private Dispatcher _dispatcher;
        private NotificationsList _notifications;
        private Queue<INotification> _notificationsPending;

        private IInterval _interval;

        public TimeAndCountBasedLifetimeSupervisor(TimeSpan notificationLifetime, MaximumNotificationCount maximumNotificationCount)
        {
            _notifications = new NotificationsList();

            _notificationLifetime = notificationLifetime;
            _maximumNotificationCount = maximumNotificationCount.Count;

            _notifications = new NotificationsList();
            _interval = new Interval();
        }

        public void PushNotification(INotification notification)
        {
            if (_interval.IsRunning == false)
                TimerStart();

            if (_notifications.Count == _maximumNotificationCount)
            {
                if (_notificationsPending == null)
                {
                    _notificationsPending = new Queue<INotification>();
                }
                _notificationsPending.Enqueue(notification);
                return;
            }

            int numberOfNotificationsToClose = Math.Max(_notifications.Count - _maximumNotificationCount + 1, 0);

            var notificationsToRemove = _notifications
                .OrderBy(x => x.Key)
                .Take(numberOfNotificationsToClose)
                .Select(x => x.Value)
                .ToList();

            foreach (var n in notificationsToRemove)
                CloseNotification(n.Notification);

            _notifications.Add(notification);
            RequestShowNotification(new ShowNotificationEventArgs(notification));
        }

        public void CloseNotification(INotification notification)
        {
            NotificationMetaData removedNotification;
            _notifications.TryRemove(notification.Id, out removedNotification);
            RequestCloseNotification(new CloseNotificationEventArgs(removedNotification.Notification));

            if (_notificationsPending != null && _notificationsPending.Any())
            {
                var not = _notificationsPending.Dequeue();
                PushNotification(not);
            }
        }

        public void Dispose()
        {
            _interval.Stop();
            _interval = null;
            _notifications?.Clear();
            _notifications = null;
        }

        public void UseDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        protected virtual void RequestShowNotification(ShowNotificationEventArgs e)
        {
            ShowNotificationRequested?.Invoke(this, e);
        }

        protected virtual void RequestCloseNotification(CloseNotificationEventArgs e)
        {
            CloseNotificationRequested?.Invoke(this, e);
        }

        private void TimerStart()
        {
            _interval.Invoke(TimeSpan.FromMilliseconds(200), OnTimerTick, _dispatcher);
        }

        private void TimerStop()
        {
            _interval.Stop();
        }

        private void OnTimerTick()
        {
            TimeSpan now = DateTimeNow.Local.TimeOfDay;

            var notificationsToRemove = _notifications
                .Where(x => x.Value.Notification.CanClose && x.Value.CreateTime + _notificationLifetime <= now)
                .Select(x => x.Value)
                .ToList();

            foreach (var n in notificationsToRemove)
                CloseNotification(n.Notification);

            if (_notifications.IsEmpty)
                TimerStop();
        }

        public void ClearMessages(string msg)
        {

            if (string.IsNullOrWhiteSpace(msg))
            {
                var notificationsToRemove = _notifications
                    .Select(x => x.Value)
                    .ToList();
                foreach (var item in notificationsToRemove)
                {
                    CloseNotification(item.Notification);
                }
                return;
            }

            var notificationsToRemove2 = _notifications
                .Where(x => x.Value.Notification.DisplayPart.GetMessage() == msg)
                .Select(x => x.Value)
                .ToList();
            foreach (var item in notificationsToRemove2)
            {
                CloseNotification(item.Notification);
            }
        }



        public event EventHandler<ShowNotificationEventArgs> ShowNotificationRequested;
        public event EventHandler<CloseNotificationEventArgs> CloseNotificationRequested;
    }
}