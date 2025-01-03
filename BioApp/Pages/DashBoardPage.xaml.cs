using BioApp.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BioApp.Pages
{
    /// <summary>
    /// Interaction logic for DashBoardPage.xaml
    /// </summary>
    public partial class DashBoardPage : UserControl
    {
        public DashBoardPage()
        {
            InitializeComponent();
            showColumnChart();
        }

       
        private async void showColumnChart()
        {
            var values = await FetchChartValues();
            /*List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
            valueList.Add(new KeyValuePair<string, int>("Total", values[0]));
            valueList.Add(new KeyValuePair<string, int>("Captured", 20));
            valueList.Add(new KeyValuePair<string, int>("Uploaded", 50));
            valueList.Add(new KeyValuePair<string, int>("Pending", 30));*/

            /*Dictionary<int, string, string> dictionary =
             new Dictionary<int, string, string>();*/


            ((ColumnSeries)barChart.Series[0]).ItemsSource =
                new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Total", values[0]),
                new KeyValuePair<string,int>("Captured", values[1]),
                new KeyValuePair<string,int>("Uploaded", values[2]),
                new KeyValuePair<string,int>("Pending", values[3])};

            //barChart.ColumnSeries[0].Points[0].Color = Color.Green;

            //Setting data for column chart
            //columnChart.DataContext = valueList;

            // Setting data for pie chart
            //pieChart.DataContext = valueList;

            //Setting data for area chart
            //areaChart.DataContext = valueList;

            //Setting data for bar chart
            //barChart.DataContext = valueList;

            //Setting data for line chart
            //lineChart.DataContext = valueList;
        }

        private async Task<int[]> FetchChartValues()
        {
            int[] vals = LongActionDialog.ShowDialog<int[]>("Loading ... ", Task<int[]>.Run(async () =>
              {
                  int[] values = new int[4];
                  using (FetchDataClass fd = new FetchDataClass())
                  {
                      values[0] = await fd.FetchTotalCount();
                      values[1] = await fd.FetchCapturedCount();
                      values[2] = await fd.FetchUploadedCount();
                      values[3] = await fd.FetchPendingUploadCount();
                  }
                  return values;
              }));
            return await Task.FromResult(vals);
        }
    }
}
