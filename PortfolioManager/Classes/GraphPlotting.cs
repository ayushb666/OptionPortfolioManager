using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Classes
{
    class GraphPlotting
    {

        public PlotModel DataPoints { get; private set; }
        public GraphPlotting()
        {
            this.DataPoints = new PlotModel { Title = "Simulated Path (Just showing 50 paths)",
                TextColor = OxyColors.White,
                TitleColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White,
            };          
        }

        public void addNewSeries(Double[] data)
        {
            List<DataPoint> prices = new List<DataPoint>();
            for (int i = 0; i < data.Length; ++i)
            {
                prices.Add(new DataPoint(i, data[i]));
            }
            this.DataPoints.Series.Add(new LineSeries { ItemsSource = prices });
        }


    }
}
