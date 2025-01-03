using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ToastNotifications.Core;

namespace ToastNotifications.Display
{
    public class NotificationAnimator : INotificationAnimator
    {
        private readonly NotificationDisplayPart _displayPart;
        private readonly TimeSpan _showAnimationTime;
        private readonly TimeSpan _hideAnimationTime;

        public NotificationAnimator(NotificationDisplayPart displayPart, TimeSpan showAnimationTime, TimeSpan hideAnimationTime)
        {
            _displayPart = displayPart;
            _showAnimationTime = showAnimationTime;
            _hideAnimationTime = hideAnimationTime;
        }

        public void Setup()
        {
            ScaleTransform scale = new ScaleTransform(1, 0);
            _displayPart.RenderTransform = scale;
        }

        public void PlayShowAnimation()
        {
            var scale = (ScaleTransform)_displayPart.RenderTransform;
            scale.CenterY = _displayPart.ActualHeight / 2;
            scale.CenterX = _displayPart.ActualWidth / 2;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation growYAnimation = new DoubleAnimation
            {
                Duration = _showAnimationTime,
                From = 0,
                To = 1
            };
            storyboard.Children.Add(growYAnimation);

            DoubleAnimation growXAnimation = new DoubleAnimation
            {
                Duration = _showAnimationTime,
                From = 0,
                To = 1
            };
            storyboard.Children.Add(growXAnimation);

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                Duration = _showAnimationTime,
                From = 0,
                To = 1
            };
            storyboard.Children.Add(fadeInAnimation);

            Storyboard.SetTargetProperty(growYAnimation, new PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTarget(growYAnimation, _displayPart);

            Storyboard.SetTargetProperty(growXAnimation, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growXAnimation, _displayPart);

            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
            Storyboard.SetTarget(fadeInAnimation, _displayPart);

            storyboard.Begin();
        }

        public void PlayHideAnimation()
        {
            _displayPart.MinHeight = 0;
            var scale = (ScaleTransform)_displayPart.RenderTransform;
            scale.CenterY = _displayPart.ActualHeight / 2;
            scale.CenterX = _displayPart.ActualWidth / 2;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation shrinkYAnimation = new DoubleAnimation
            {
                Duration = _hideAnimationTime,
                From = _displayPart.ActualHeight,
                To = 0
            };

            storyboard.Children.Add(shrinkYAnimation);

            DoubleAnimation shrinkXAnimation = new DoubleAnimation
            {
                Duration = _hideAnimationTime,
                From = 1,
                To = 0
            };

            storyboard.Children.Add(shrinkXAnimation);

            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                Duration = _hideAnimationTime,
                From = 1,
                To = 0
            };

            storyboard.Children.Add(fadeOutAnimation);

            Storyboard.SetTargetProperty(shrinkYAnimation, new PropertyPath("Height"));
            Storyboard.SetTarget(shrinkYAnimation, _displayPart);
            Storyboard.SetTargetProperty(shrinkXAnimation, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(shrinkXAnimation, _displayPart);

            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));
            Storyboard.SetTarget(fadeOutAnimation, _displayPart);

            storyboard.Begin();
        }
    }
}