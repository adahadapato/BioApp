
using System.Windows;


namespace BioApp
{
	public class MinimizeButton : CaptionButton
	{
		static MinimizeButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MinimizeButton), new FrameworkPropertyMetadata(typeof(MinimizeButton)));
		}

		protected override void OnClick()
		{
			base.OnClick();
            // Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(Window.GetWindow(this));
            Window w = Window.GetWindow(this);
            if (w != null)
            {
                w.WindowState = WindowState.Minimized;
            }
        }
	}
}
