using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebcallScript
{
    class Program
    {
static void Main(string[] args)
        {
            //https://rata.digitraffic.fi/api/v1/live-trains/station/HKI/TPE?endDate=2019-12-10T23%3A59%3A59.000Z&include_nonstopping=false&startDate=2019-12-01T00%3A00%3A00.000Z
            

            string filePath = @"C:\Users\Public\RailwayTraffic\";
            string baseURL = "https://rata.digitraffic.fi/api/v1/live-trains/station/";
            string departureStation = "HKI";
            string arrivalStation = "TPE";
            string includeNonstop = "&include_nonstopping=FALSE";
            DateTime startTime = new DateTime(2019, 12, 01, 00, 00, 00);
            DateTime endTime = new DateTime(2019, 12, 10, 00, 00, 00);


            DownloadTraindata(filePath, baseURL, departureStation, arrivalStation, includeNonstop, startTime, endTime);

        }

        //API call to get trains between stations
        static void DownloadTraindata(string filePath, string baseURL, string departureStation, string arrivalStation
            , string includeNonstop, DateTime startTime, DateTime endTime)
        {

            DateTime eoDayTime;
            string fileName = $"live-trains-";
            filePath = @"C:\Users\Public\RailwayTraffic\";


            //Loop through all the dates and write each day json content to it's own file
            while (startTime <= endTime)
            {

                eoDayTime = startTime.AddHours(23).AddMinutes(59).AddSeconds(59);

                //Convert to DateTime UTC format
                var startDateUTC = DateTime.SpecifyKind(startTime, DateTimeKind.Utc)
                    .ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);

                var eoDayTimeUTC = DateTime.SpecifyKind(eoDayTime, DateTimeKind.Utc)
                    .ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);



                //Build string
                var url = $"{baseURL}{departureStation}/{arrivalStation}?endDate={ eoDayTimeUTC.Replace(":", "%3A")}{includeNonstop}&startDate={startDateUTC.Replace(":", "%3A")}";
                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";
                request.UserAgent = "agent";
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                var content = string.Empty;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }


                File.WriteAllText(filePath + fileName + startTime.ToString("yyyy-MM-dd") + ".json", content);
                Console.WriteLine(url);
                startTime = startTime.AddDays(1);
            }


        }
    }
}
