using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
//using Microsoft.Windows.Shell;

namespace BioApp
{
	public class CloseButton : CaptionButton
	{
		static CloseButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CloseButton), new FrameworkPropertyMetadata(typeof(CloseButton)));
		}


       

        protected override void OnClick()
		{
			base.OnClick();
            MainWindow.ShutDown();
		}
    }
}
