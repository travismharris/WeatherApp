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
        string days, zip, latitude, longitude;
        RestClient coordinates;
        RestClient forecast;

        public WeatherRequest() : this("3", "22901") { }

        public WeatherRequest(string days, string zip)
        {
            this.days = days;
            this.zip = zip;
            coordinates = new RestClient(
                "http://graphical.weather.gov/xml/sample_products/browser_interface/ndfdXMLclient.php");
            forecast = new RestClient(
                "http://graphical.weather.gov/xml/sample_products/browser_interface/ndfdBrowserClientByDay.php");
        }

        public void GetCoordinates()
        {
            var coordRequest = new RestRequest("?listZipCodeList=" + zip, Method.GET);
            var coordinatesResponse = coordinates.Execute(coordRequest);
            var xmlCoords = coordinatesResponse.Content.ToString();
            var coords = ParseCoordinates(xmlCoords);
            SetLatitude(coords);
            SetLongitude(coords);
        }

        public string ParseCoordinates(string coords)
        {
            XElement coordinates = XElement.Parse(coords);
            var coordElement = from c in coordinates.Elements("latLonList")
                               select c.Value;

            return coordElement.FirstOrDefault().ToString() ;
        }

        public void SetLatitude(string coords)
        {
            string result = "lat=";
            result += coords.Substring(0, coords.IndexOf(',', 0));
            latitude = result;
        }

        public void SetLongitude(string coords)
        {
            string result = "lon=";
            result += coords.Substring((coords.IndexOf(',', 0) + 1), (coords.Length - (coords.IndexOf(',', 0) + 1)));
            longitude = result;
        }

        public string GetForecast()
        {
            var forecastRequest = new RestRequest("?" + latitude + "&" + longitude + "&format=12+hourly&numDays="+ days);
            var forecastResponse = forecast.Execute(forecastRequest);
            List<WeatherObject> wOResponse = ParseXml(forecastResponse.Content.ToString());
            return FormatResponse(wOResponse);
        }

        public string GetForecast12Hour()
        {
            var forecastRequest = new RestRequest("?" + latitude + "&" + longitude + "&format=12+hourly&numDays=" + days);
            var forecastResponse = forecast.Execute(forecastRequest);
            List<WeatherObject> wOResponse = ParseXml12Hour(forecastResponse.Content.ToString());
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

        public List<WeatherObject> ParseXml(string rawXml)
        {
            XElement toParse = XElement.Parse(rawXml);

            var dateElements = toParse.Descendants("time-layout")
              .FirstOrDefault(x => x.Element("layout-key").Value == "k-p24h-n" + days +"-1");

            var dates = from x in dateElements.Elements("start-valid-time")
                              select x.Value;

            var descriptions = from x in toParse.Descendants("weather").Elements("weather-conditions").Attributes()
                               select x.Value.ToString();

            var minimums = from x in toParse.Descendants("temperature").Elements("value")
                           where x.Parent.Attribute("type").Value == "minimum"
                           select x.Value.ToString();

            var maximums = from x in toParse.Descendants("temperature").Elements("value")
                           where x.Parent.Attribute("type").Value == "maximum"
                           select x.Value.ToString();

            var icons = toParse.Descendants("icon-link").Nodes();

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

        public IEnumerable<XAttribute> GetPeriodName(XElement dateLayout)
        {
            return from x in dateLayout.Elements("start-valid-time")
                   select x.Attribute("period-name");
        }

        public List<WeatherObject> ParseXml12Hour(string rawXml)
        {
            //break out all the calls in this method
            XElement toParse = XElement.Parse(rawXml);

            var dateLayout24_1 = GetDatesforDay24HourLayout(toParse);

            var dates24_1 = GetPeriodName(dateLayout24_1);

            //var dateLayout24_1 = toParse.Descendants("time-layout")
            //    .FirstOrDefault(x => x.Element("layout-key").Value == "k-p24h-n" + days + "-1");

            //var dates24_1 = from x in dateLayout24_1.Elements("start-valid-time")
            //                select x.Attribute("period-name");

            //can reuse all calls for daytime parsing with nighttime calls!

            var dayTime = dateLayout24_1.Elements("start-valid-time")
                .FirstOrDefault().Value;
            
            //var dateLayout24_2 = toParse.Descendants("time-layout")
            //    .FirstOrDefault(x => x.Element("layout-key").Value == "k-p24h-n" + days + "-2");

            var dateLayout24_2 = GetDatesforNight24HourLayout(toParse);

            var dates24_2 = GetPeriodName(dateLayout24_2);

            //var dates24_2  = from x in dateLayout24_2.Elements("start-valid-time")
            //                 select x.Attribute("period-name");

            var nightTime = dateLayout24_2.Elements("start-valid-time")
                .FirstOrDefault().Value;

            bool dayFirst = DayFirst(dayTime, nightTime);

            var dateLayout12 = toParse.Descendants("time-layout")
                               .FirstOrDefault(x => x.Element("layout-key").Value.StartsWith("k-p12h-n"));

            var dates12 = from x in dateLayout12.Elements("start-valid-time")
                          select x.Value;

            var descriptions = from x in toParse.Descendants("weather").Elements("weather-conditions").Attributes()
                          select x.Value.ToString();

            var minimums = from x in toParse.Descendants("temperature").Elements("value")
                           where x.Parent.Attribute("type").Value == "minimum"
                           select x.Value.ToString();

            var maximums = from x in toParse.Descendants("temperature").Elements("value")
                           where x.Parent.Attribute("type").Value == "maximum"
                           select x.Value.ToString();

            var icons = from x in toParse.Descendants("conditions-icon").Elements("icon-link")
                        select x.Value;

            WeatherObject[] wo = new WeatherObject[icons.Count()];
            if (dayFirst)
            {                    
                for (int i = 0; i < dates12.Count(); i++)
                {
                    WeatherObject tempWO = new WeatherObject();
                    if (i % 2 == 0)
                    {
                        if (i == 0)
                        {
                            tempWO.date=dates24_1.ElementAt(i).ToString();
                            tempWO.max= maximums.ElementAt(i);
                        }
                        if (i > 0)
                        {
                            tempWO.date=dates24_1.ElementAt(i - (i / 2)).ToString();
                            tempWO.max= maximums.ElementAt(i - (i / 2));
                        }
                    }
                    if (i % 2 != 0)
                    {
                        if (i == 1)
                        {
                            tempWO.date=dates24_2.ElementAt(i - 1).ToString();
                            tempWO.min= minimums.ElementAt(i - 1);
                        }
                        if (i > 1)
                        {
                            tempWO.date=dates24_2.ElementAt(i - ((i / 2) + 1)).ToString();
                            tempWO.min= minimums.ElementAt(i - ((i / 2) + 1));
                        }
                    }
                    //sb.Append(dates12.ElementAt(i) + "\n");
                    tempWO.conditions=descriptions.ElementAt(i);
                    tempWO.iconPath=icons.ElementAt(i).ToString();
                    wo[i] = tempWO;
                }
            }
            else
            {
                for (int i = 0; i < dates12.Count(); i++)
                {
                    WeatherObject tempWO = new WeatherObject();
                    if(i%2==0)
                    {
                        if (i == 0)
                        {
                            tempWO.date=dates24_2.ElementAt(i).ToString();
                            tempWO.min= minimums.ElementAt(i);
                        }
                        if (i > 0)
                        {
                            tempWO.date=dates24_2.ElementAt(i - (i / 2) ).ToString();
                            tempWO.min= minimums.ElementAt(i - (i / 2) );
                        }
                    }
                    if(i%2!=0)
                    {
                        if (i == 1)
                        {
                            tempWO.date=dates24_1.ElementAt(i-1).ToString();
                            tempWO.max = maximums.ElementAt(i - 1);
                        }
                        if (i > 1)
                        {
                            tempWO.date=dates24_1.ElementAt(i - ((i / 2)+1)).ToString();
                            tempWO.max= maximums.ElementAt(i - ((i / 2)+1));
                        }
                    }
                    //sb.Append(dates12.ElementAt(i) + "\n");
                    tempWO.conditions=descriptions.ElementAt(i);
                    tempWO.iconPath=icons.ElementAt(i).ToString();
                    wo[i] = tempWO;
                }
            }
            return wo.ToList();
        }

        public string GetFormattedDate(string date)
        {   
            int year = Int32.Parse(date.Substring(0, 4));
            int month = Int32.Parse(date.Substring(5, 2));
            int day = Int32.Parse(date.Substring(8, 2));
            DateTime myDate = new DateTime(year, month, day);
            return myDate.ToShortDateString();
        }

        public static bool DayFirst(string day, string night)
        {
            DateTime testday = GetDate(day);
            DateTime testnight = GetDate(night);
            bool answer = (testday < testnight);
            if (testday < testnight)
            
            {

                return true;
            }
            //if day < night return true
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