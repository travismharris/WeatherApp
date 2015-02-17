using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Xml.Linq;

namespace WeatherApp
{
    public class CoordinateRequest
    {
        RestClient coordinateClient;
        public string zipCode, coordinateValues;

        public CoordinateRequest() : this ("22901") { }

        public CoordinateRequest(string zipCode)
        {
            this.zipCode = zipCode;
            this.coordinateClient = new RestClient("http://graphical.weather.gov/xml/sample_products/browser_interface/ndfdXMLclient.php");
            GetCoordinates();
        }

        public void GetCoordinates()
        {
            var coordRequest = new RestRequest("?listZipCodeList=" + zipCode, Method.GET);
            var coordinatesResponse = coordinateClient.Execute(coordRequest);
            var xmlCoords = coordinatesResponse.Content.ToString();
            var coords = ParseCoordinates(xmlCoords);
            coordinateValues = GetLatitude(coords) + "&" + GetLongitude(coords);
        }
        
        public string ParseCoordinates(string coords)
        {
            XElement coordinates = XElement.Parse(coords);
            var coordElement = from c in coordinates.Elements("latLonList")
                               select c.Value;

            return coordElement.FirstOrDefault().ToString();
        }

        public string GetLatitude(string coords)
        {
            string latitude = "lat=";
            latitude += coords.Substring(0, coords.IndexOf(',', 0));
            return latitude;
        }

        public string GetLongitude(string coords)
        {
            string longitude = "lon=";
            longitude += coords.Substring((coords.IndexOf(',', 0) + 1), (coords.Length - (coords.IndexOf(',', 0) + 1)));
            return longitude;
        }

    }
}
