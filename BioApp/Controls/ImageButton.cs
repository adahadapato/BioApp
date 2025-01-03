using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BioApp.local
{
    /// <summary>
    /// ========================================
    /// .NET Framework 3.0 Custom Control
    /// ========================================
    ///
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ImageButtonProject"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ImageButtonProject;assembly=ImageButtonProject"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file. Note that Intellisense in the
    /// XML editor does not currently work on custom controls and its child elements.
    ///
    ///     <MyNamespace:ImageButton/>
    ///
    /// </summary>
    public class ImageButton : System.Windows.Controls.Button
    {
        static ImageButton()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }


        #region properties

        public string ImageOver
        {
            get { return (string)GetValue(ImageOverProperty); }
            set { SetValue(ImageOverProperty, value); }
        }

        public string ImageNormal
        {
            get { return (string)GetValue(ImageNormalProperty); }
            set { SetValue(ImageNormalProperty, value); }
        }

        public string ImageDown
        {
            get { return (string)GetValue(ImageDownProperty); }
            set { SetValue(ImageDownProperty, value); }
        }


        #endregion

        #region dependency properties

        public static readonly DependencyProperty ImageNormalProperty =
           DependencyProperty.Register(
               "ImageNormal", typeof(string), typeof(ImageButton));
              

        public static readonly DependencyProperty ImageOverProperty =
          DependencyProperty.Register(
              "ImageOver", typeof(string), typeof(ImageButton));

        public static readonly DependencyProperty ImageDownProperty =
        DependencyProperty.Register(
            "ImageDown", typeof(string), typeof(ImageButton));

        #endregion

    }
}
