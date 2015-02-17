using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeatherApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetWeather_Click(object sender, EventArgs e)
        {
            string days="3", zip="22901";
            if (numberOfDays.Value > 0)
                days = numberOfDays.Value.ToString();
            if (txtZip.Text.Length == 0)
                zip = "22901";
            else zip = txtZip.Text;

            var getWeather = new WeatherRequest(days, zip);
            webBrowser1.DocumentText = getWeather.GetForecast();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string days = "3", zip = "22901";
            if (numberOfDays.Value > 0)
                days = numberOfDays.Value.ToString();
            if (txtZip.Text.Length == 0)
                zip = "22901";
            else zip = txtZip.Text;

            var getWeather = new WeatherRequest(days, zip);
            webBrowser1.DocumentText = getWeather.GetForecast12Hour();

        }
    }
}
