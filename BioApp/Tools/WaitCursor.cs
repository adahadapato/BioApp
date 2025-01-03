using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;


namespace BioApp.Tools
{
    public class WaitCursor : IDisposable
    {
        private Cursor _previousCursor;
        public WaitCursor()
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                _previousCursor = Mouse.OverrideCursor;
                Mouse.OverrideCursor = Cursors.Wait;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() =>
                  {
                      _previousCursor = Mouse.OverrideCursor;
                      Mouse.OverrideCursor = Cursors.Wait;
                  }));
            }
            //SafeGuiWpf.SetCursor(_previousCursor);
        }

        #region IDisposable Members
        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~WaitCursor()
        {
            // Finalizer calls Dispose(false)  
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)  
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    Mouse.OverrideCursor = _previousCursor;
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                      new Action(() =>
                      {
                          Mouse.OverrideCursor = _previousCursor;
                      }));
                }
            }

            if (Application.Current.Dispatcher.CheckAccess())
            {
                Mouse.OverrideCursor = _previousCursor;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() =>
                  {
                      Mouse.OverrideCursor = _previousCursor;
                  }));
            }

        }
        #endregion
    }
}
