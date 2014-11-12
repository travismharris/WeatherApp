using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    public class WeatherObject12Hour
    {
//        period-name="Tonight"
//Min:30
//2014-11-07T18:00:00-05:00
//Mostly Clear
//http://www.nws.noaa.gov/weather/images/fcicons/nfew.jpg
        public string min { get; set; }
        public string max { get; set; }
        public string conditions { get; set; }
        public string iconPath { get; set; }
        public string date { get; set; }

        public WeatherObject12Hour(string min, string max, string conditions, string iconPath, string date)
        {
            this.min = min;
            this.max = max;
            this.conditions = conditions;
            this.iconPath = iconPath;
            this.date = date;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(date + "\nMinimum:  " + min + Environment.NewLine);
            sb.Append("Maximum:  " + max + Environment.NewLine);
            sb.Append("Conditions:  " + conditions + Environment.NewLine);
            sb.Append("<img src=\"" + iconPath + "\" \\>" + Environment.NewLine);
            return sb.ToString();
        }
    }
}
