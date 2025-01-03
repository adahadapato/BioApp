using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioApp.Tools
{
    public class ColorProcessLayer
    {
        /// <summary>
        /// Get Color Scheme from percentage supplied.
        /// </summary>
        /// <param name="temp"></param>
        /// This is size of sample size from the population
        /// <param name="total"></param>
        /// Total size of population
        /// <returns></returns>
        public static string CalculateColor(int temp, int total)
        {
            /*double red = (total <= 50) ? 255 : Math.Round(256 - (total - 50) * 5.12);
            double green = (total >= 50) ? 255 : Math.Round(total * 5.12);*/
            
            try
            {
                double green = (255 * temp) / total;
                double red = (255 * (total - temp)) / total;
                Color color = DarkerColor(Color.FromArgb(200, (byte)red, (byte)green, 0));
                string hex = "#" + color.Name;
                return hex;
            }
            catch 
            {
                //System.Windows.MessageBox.Show(e.Message, "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                Color color = DarkerColor(Color.FromArgb(200, (byte)255, (byte)0, 0));
                string hex = "#" + color.Name;
                return hex;
            }
        }


        /// <summary>
        /// Makes the color Darker
        /// </summary>
        /// <param name="color"></param>
        /// <param name="correctionfactory"></param>
        /// <returns></returns>
        private static Color DarkerColor(Color color, float correctionfactory = 50f)
        {
            const float hundredpercent = 70f;
            return Color.FromArgb((int)(((float)color.R / hundredpercent) * correctionfactory),
                (int)(((float)color.G / hundredpercent) * correctionfactory), (int)(((float)color.B / hundredpercent) * correctionfactory));
        }

    }
}
