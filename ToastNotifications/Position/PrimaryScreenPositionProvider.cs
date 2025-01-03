using System;
using System.Windows;
using ToastNotifications.Core;

namespace ToastNotifications.Position
{
    public class PrimaryScreenPositionProvider : IPositionProvider
    {
        private readonly Corner _corner;
        private readonly double _offsetX;
        private readonly double _offsetY;

        private double ScreenHeight => SystemParameters.PrimaryScreenHeight;
        private double ScreenWidth => SystemParameters.PrimaryScreenWidth;

        public Window ParentWindow { get; }
        public EjectDirection EjectDirection { get; private set; }
        
        public PrimaryScreenPositionProvider(Corner corner, double offsetX, double offsetY)
        {
            _corner = corner;
            _offsetX = offsetX;
            _offsetY = offsetY;
            ParentWindow = null;

            SetEjectDirection(corner);
        }

        public Point GetPosition(double actualPopupWidth, double actualPopupHeight)
        {
            switch (_corner)
            {
                case Corner.TopRight:
                    return GetPositionForTopRightCorner(actualPopupWidth, actualPopupHeight);
                case Corner.TopLeft:
                    return GetPositionForTopLeftCorner(actualPopupWidth, actualPopupHeight);
                case Corner.BottomRight:
                    return GetPositionForBottomRightCorner(actualPopupWidth, actualPopupHeight);
                case Corner.BottomLeft:
                    return GetPositionForBottomLeftCorner(actualPopupWidth, actualPopupHeight);
                case Corner.BottomCenter:
                    return GetPositionForBottomCenterCorner(actualPopupWidth, actualPopupHeight);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public double GetHeight()
        {
            return ScreenHeight;
        }
        
        private void SetEjectDirection(Corner corner)
        {
            switch (corner)
            {
                case Corner.TopRight:
                case Corner.TopLeft:
                    EjectDirection = EjectDirection.ToBottom;
                    break;
                case Corner.BottomRight:
                case Corner.BottomLeft:
                case Corner.BottomCenter:
                    EjectDirection = EjectDirection.ToTop;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(corner), corner, null);
            }
        }

        private Point GetPositionForBottomLeftCorner(double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(_offsetX, ScreenHeight - _offsetY - actualPopupHeight);
        }

        private Point GetPositionForBottomCenterCorner(double actualPopupWidth, double actualPopupHeight)
        {
            return new Point((ScreenWidth - _offsetX - actualPopupWidth) / 2, ScreenHeight - _offsetY - actualPopupHeight);
        }


        private Point GetPositionForBottomRightCorner(double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(ScreenWidth - _offsetX - actualPopupWidth, ScreenHeight - _offsetY - actualPopupHeight);
        }

        private Point GetPositionForTopLeftCorner(double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(_offsetX, _offsetY);
        }

        private Point GetPositionForTopRightCorner(double actualPopupWidth, double actualPopupHeight)
        {
            return new Point(ScreenWidth - _offsetX - actualPopupWidth, _offsetY);
        }

        public void Dispose()
        {
            // nothing to do here
        }

#pragma warning disable CS0067
		public event EventHandler UpdatePositionRequested;

        public event EventHandler UpdateEjectDirectionRequested;

        public event EventHandler UpdateHeightRequested;
#pragma warning restore CS0067
	}
}
