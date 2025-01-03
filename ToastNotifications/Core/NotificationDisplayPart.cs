using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications.Display;

namespace ToastNotifications.Core
{
    public abstract class NotificationDisplayPart : UserControl
    {
        private readonly NotificationAnimator _animator;
        public INotification Notification { get; protected set; }

        protected NotificationDisplayPart()
        {
            _animator = new NotificationAnimator(this, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(300));

            Margin = new Thickness(1);

            _animator.Setup();

            Loaded += OnLoaded;
            MinHeight = 60;
        }

        virtual public MessageOptions GetOptions()
        {
            return null;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            var dc = DataContext as INotification;
            var opts = dc?.DisplayPart?.GetOptions();
            if (opts != null && opts.FreezeOnMouseEnter)
            {
                if (!opts.UnfreezeOnMouseLeave) // message stay freezed, show close button
                {
                    var bord2 = this.Content as Border;
                    if (bord2 != null)
                    {
                        if (dc.CanClose)
                        {
                            dc.CanClose = false;
                            var btn = FindChild<Button>(this, "CloseButton");
                            btn.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    dc.CanClose = false;
                }
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            var dc = DataContext as INotification;
            var opts = dc?.DisplayPart?.GetOptions();
            if (opts != null && opts.FreezeOnMouseEnter && opts.UnfreezeOnMouseLeave)
            {
                dc.CanClose = true;
            }
            base.OnMouseLeave(e);
        }


        virtual public string GetMessage()
        {
            return "?";
        }

        public void Bind<TNotification>(TNotification notification) where TNotification : INotification
        {
            Notification = notification;
            DataContext = Notification;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _animator.PlayShowAnimation();
        }

        public void OnClose()
        {
            _animator.PlayHideAnimation();
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
   where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}