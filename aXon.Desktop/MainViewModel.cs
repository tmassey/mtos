using System;
using System.Collections.Generic;
using OxyPlot;

namespace aXon.Desktop
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            this.Title = "Example 2";
            this.Points = new List<DataPoint>();
            for (int x = 0;x!=100;x++)
            {
                Points.Add(new DataPoint(x, r.Next(0,100)));
            }
        }

        public string Title { get; private set; }

        public IList<DataPoint> Points { get; private set; }
    }
}