using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Xml;

namespace WeatherApp
{
    class WeatherRequest
    {
        string days, coordinates; //, zip, latitude, longitude;
        //RestClient coordinates;
        RestClient forecast;
        XElement toParse;

        public WeatherRequest() : this("3", "22901" ) { }

        public WeatherRequest(string days, string zip)
        {
            this.days = days;
            //this.zip = zip;
            var getCoordinates = new CoordinateRequest(zip);
            //getCoordinates.GetCoordinates();
            coordinates = getCoordinates.coordinateValues;
            forecast = new RestClient(
                "http://graphical.weather.gov/xml/sample_products/browser_interface/ndfdBrowserClientByDay.php");
        }

        public string GetForecast()
        {
            var forecastRequest = new RestRequest("?" + coordinates + "&format=12+hourly&numDays="+ days);
            var forecastResponse = forecast.Execute(forecastRequest);
            toParse = XElement.Parse(forecastResponse.Content.ToString());
            List<WeatherObject> wOResponse = ParseXml(toParse);
            return FormatResponse(wOResponse);
        }

        public string GetForecast12Hour()
        {
            var forecastRequest = new RestRequest("?" + coordinates + "&format=12+hourly&numDays=" + days);
            var forecastResponse = forecast.Execute(forecastRequest);
            toParse = XElement.Parse(forecastResponse.Content.ToString());
            List<WeatherObject> wOResponse = ParseXml12Hour(toParse);
            return Format12HourResponse(wOResponse);
        }

        public string FormatResponse(List<WeatherObject> wo)
        {
            StringBuilder sb = new StringBuilder("<html><table cellspacing=10>");
            for (int i = 0; i < wo.Count(); i++)
            {
                sb.Append("<tr><td valign=\"top\">" + GetFormattedDate(wo[i].date) + "</td>");
                sb.Append("<td>Minimum: " + wo[i].min + "<br />");
                sb.Append("Maximum: " + wo[i].max + "<br />");
                sb.Append("Conditions: " + wo[i].conditions + "</td>");
                sb.Append("<td><img src=\"" + wo[i].iconPath + "\"\\></td></tr>");
            }
            sb.Append("</table></html>");

            return sb.ToString();
        }

        public string Format12HourResponse(List<WeatherObject> wo)
        {
            StringBuilder sb = new StringBuilder("<html><table cellspacing=10>");
            for (int i = 0; i < wo.Count(); i++)
            {
                sb.Append("<tr><td valign=\"top\">" + wo[i].date.Substring(13,wo[i].date.Length-14) + "</td>");
                sb.Append("<td>Minimum: " + wo[i].min + "<br />");
                sb.Append("Maximum: " + wo[i].max + "<br />");
                sb.Append("Conditions: " + wo[i].conditions + "</td>");
                sb.Append("<td><img src=\"" + wo[i].iconPath + "\"\\></td></tr>");
            }
            sb.Append("</table></html>");

            return sb.ToString();
        }

        public List<WeatherObject> ParseXml(XElement toParse)
        {
            var dateElements = GetDatesforDay24HourLayout(toParse);
            var dates = GetPeriods(dateElements);

            var descriptions = GetDescriptions(toParse);
            var minimums = GetMinimums(toParse);
            var maximums = GetMaximums(toParse);
            var icons = GetIcons(toParse);

            WeatherObject[] wo = new WeatherObject[minimums.Count()];
            for (int i = 0; i < minimums.Count(); i++)
            {
                WeatherObject tempWO = new WeatherObject(minimums.ElementAt(i), maximums.ElementAt(i),
                    descriptions.ElementAt(i), icons.ElementAt(i).ToString(), dates.ElementAt(i));
                wo[i] = tempWO;
            }

            return wo.ToList();

        }



        public XElement GetDatesforDay24HourLayout(XElement toParse)
        {
            return toParse.Descendants("time-layout")
                .FirstOrDefault(x => x.Element("layout-key").Value == "k-p24h-n" + days + "-1");
        }

        public XElement GetDatesforNight24HourLayout(XElement toParse)
        {
            return toParse.Descendants("time-layout")
                .FirstOrDefault(x => x.Element("layout-key").Value == "k-p24h-n" + days + "-2");
        }

        public XElement Get12HourPeriodsLayout(XElement toParse)
        {
            return toParse.Descendants("time-layout")
                .FirstOrDefault(x => x.Element("layout-key").Value.StartsWith("k-p12h-n"));
        }

        public IEnumerable<string> GetPeriods(XElement toParse)
        {
            return from x in toParse.Elements("start-valid-time")
                   select x.Value;
        }

        public IEnumerable<XAttribute> GetPeriodNames(XElement dateLayout)
        {
            return from x in dateLayout.Elements("start-valid-time")
                   select x.Attribute("period-name");
        }

        public string GetStringValueOfDateTime(XElement dateLayout)
        {
            return dateLayout.Elements("start-valid-time")
                .FirstOrDefault().Value;
        }

        public IEnumerable<string> GetMinimums (XElement toParse)
        {
            return from x in toParse.Descendants("temperature").Elements("value")
                   where x.Parent.Attribute("type").Value == "minimum"
                   select x.Value.ToString();
        }

        public IEnumerable<string> GetMaximums(XElement toParse)
        {
            return from x in toParse.Descendants("temperature").Elements("value")
                   where x.Parent.Attribute("type").Value == "maximum"
                   select x.Value.ToString();
        }

        public IEnumerable<string> GetDescriptions(XElement toParse)
        {
            return from x in toParse.Descendants("weather").Elements("weather-conditions").Attributes()
                   select x.Value.ToString();
        }

        public IEnumerable<string> GetIcons (XElement toParse)
        {
            return from x in toParse.Descendants("conditions-icon").Elements("icon-link")
                   select x.Value;
        }



        public List<WeatherObject> ParseXml12Hour(XElement toParse)
        {
            var dateLayout24_Day = GetDatesforDay24HourLayout(toParse);
            var dayNames = GetPeriodNames(dateLayout24_Day);
            var dayTime = GetStringValueOfDateTime(dateLayout24_Day);

            var dateLayout24_Night = GetDatesforNight24HourLayout(toParse);
            var nightNames = GetPeriodNames(dateLayout24_Night);
            var nightTime = GetStringValueOfDateTime(dateLayout24_Night);

            bool dayFirst = DayFirst(dayTime, nightTime);

            var dateLayout12 = Get12HourPeriodsLayout(toParse);
            var periods_12Hour = GetPeriods(dateLayout12);

            var descriptions = GetDescriptions(toParse);
            var minimums = GetMinimums(toParse);
            var maximums = GetMaximums(toParse);
            var icons = GetIcons(toParse);

            //if day first call assembler one way else -- another
            // or maybe a linq statement?

            List<WeatherObject> wo = new List<WeatherObject>();
            if (dayFirst)
            {                    
                for (int i = 0; i < periods_12Hour.Count(); i++)
                {
                    WeatherObject tempWO = new WeatherObject();
                    if (i % 2 == 0)
                    {
                        if (i == 0)
                        {
                            tempWO.date=dayNames.ElementAt(i).ToString();
                            tempWO.max= maximums.ElementAt(i);
                        }
                        if (i > 0)
                        {
                            tempWO.date=dayNames.ElementAt(i - (i / 2)).ToString();
                            tempWO.max= maximums.ElementAt(i - (i / 2));
                        }
                    }
                    if (i % 2 != 0)
                    {
                        if (i == 1)
                        {
                            tempWO.date=nightNames.ElementAt(i - 1).ToString();
                            tempWO.min= minimums.ElementAt(i - 1);
                        }
                        if (i > 1)
                        {
                            tempWO.date=nightNames.ElementAt(i - ((i / 2) + 1)).ToString();
                            tempWO.min= minimums.ElementAt(i - ((i / 2) + 1));
                        }
                    }
                    tempWO.conditions=descriptions.ElementAt(i);
                    tempWO.iconPath=icons.ElementAt(i).ToString();
                    wo.Add(tempWO);
                }
            }
            else
            {
                for (int i = 0; i < periods_12Hour.Count(); i++)
                {
                    WeatherObject tempWO = new WeatherObject();
                    if(i%2==0)
                    {
                        if (i == 0)
                        {
                            tempWO.date=nightNames.ElementAt(i).ToString();
                            tempWO.min= minimums.ElementAt(i);
                        }
                        if (i > 0)
                        {
                            tempWO.date=nightNames.ElementAt(i - (i / 2) ).ToString();
                            tempWO.min= minimums.ElementAt(i - (i / 2) );
                        }
                    }
                    if(i%2!=0)
                    {
                        if (i == 1)
                        {
                            tempWO.date=dayNames.ElementAt(i-1).ToString();
                            tempWO.max = maximums.ElementAt(i - 1);
                        }
                        if (i > 1)
                        {
                            tempWO.date=dayNames.ElementAt(i - ((i / 2)+1)).ToString();
                            tempWO.max= maximums.ElementAt(i - ((i / 2)+1));
                        }
                    }
                    tempWO.conditions=descriptions.ElementAt(i);
                    tempWO.iconPath=icons.ElementAt(i).ToString();
                    wo.Add(tempWO);
                }
            }
            return wo;
        }

        public string GetFormattedDate(string date)
        {
            DateTime myDate = GetDate(date);
            return myDate.ToShortDateString();
        }

        public static bool DayFirst(string day, string night)
        {
            DateTime testDay = GetDate(day);
            DateTime testNight = GetDate(night);

            if (testDay < testNight)
                return true;
            return false;
        }

        public static DateTime GetDate(string date)
        {
            int year = Int32.Parse(date.Substring(0, 4));
            int month = Int32.Parse(date.Substring(5, 2));
            int day = Int32.Parse(date.Substring(8, 2));
            int hours = Int32.Parse(date.Substring(11, 2));
            int mins = Int32.Parse(date.Substring(14, 2));
            return new DateTime(year, month, day, hours, mins, 0);
        }
    }
}