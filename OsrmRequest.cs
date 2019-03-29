using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Json.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YM.Geo;
using YM.Container;
using System.Collections;

namespace RouteplannerTest
{
    class OsrmRequest
    {

        public static Car auto = new Car(50, 100, "Typ2"); //range in km
        public static string start = "8.4034195,49.0068705";
        public static string end = "9.1800132,48.7784485";
        public static double shortest, shortestWay = Double.MaxValue;
        public static bool reachable = true;
        public static string path = start;
        public static int counter = 0, stationCount = 0;

        static void Main(string[] args)
        {
            //EvsesData.dbConnection();
            dynamic mycontent = GetRequest("http://127.0.0.1:7133/route/v1/driving/" + start + ";" + end + "?overview=false&alternatives=true&steps=true&hints=;").GetAwaiter().GetResult();

            // Console.ReadKey();

            while(true)
            {
                var found = checkDistance(mycontent);
                if (found != null)
                {
                    //eins vorher weil sonst distanz nicht ausreichend
                    String xkoord = Convert.ToString(found[0]).Replace(",", ".");
                    String ykoord = Convert.ToString(found[1]).Replace(",", ".");
                    getEcharger(xkoord + "," + ykoord);
                } else
                {
                    break;
                }
            }

            Console.WriteLine("Ihre berechnete Route ist: " + mycontent);
            Console.ReadKey();
        }

        async static Task<dynamic> GetRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
                        dynamic dynJson = JsonConvert.DeserializeObject(mycontent);
                        return dynJson;
                    }
                }
            }
        }

        public static bool doCoordsMatch(dynamic x1, dynamic y1, dynamic x2, dynamic y2)
        {
            return Convert.ToDouble(x1) == Convert.ToDouble(x2) && Convert.ToDouble(y1) == Convert.ToDouble(y2);
        }

        public static dynamic[] checkDistance(dynamic dynJson)
        {
            //if(auto.distance)
            //Console.WriteLine(dynJson.routes[0].legs[0].steps[0].driving_side);
            var route = dynJson.routes[0];

            double dist = 0;
        
            for (var l = 0; l < route.legs.Count; l++)
            {
                var leg = route.legs[l];
                var steps = leg.steps[0];
                for (var m = 0; m < leg.steps.Count; m++)
                {
                    steps = leg.steps[m];

                    // skip start & end
                    for (var c = 1; c < dynJson.waypoints.Count - 1; c++)
                    {
                        var currentPos = steps.maneuver.location;
                        var targetPos = dynJson.waypoints[l].location;
                        if (doCoordsMatch(currentPos[0], currentPos[1], targetPos[0], targetPos[1]))
                        {
                            dist = 0;
                        }
                    }

                   double temp = (double)steps.distance / 1000;
                   dist = dist + temp;
                   if (dist > auto.range * 0.8)
                   {
                       return leg.steps[m - 1].maneuver.location;
                    }
                    Console.WriteLine(dist);
                }
            }
            return null;
        }

        public static void getEcharger(dynamic step)
        {
            string currLat;
            string currLong;
            string temp;
            List<string> estations = new List<String>();
            string[] old_new = step.Split(';');
            //nur für hinteren teil brauchen wir coords
            string[] coords = old_new[old_new.Length - 1].Split(',');
            currLat = coords[0];
            currLong = coords[1];
            temp = LatLongConverter.GeoDec2Mercator(currLat, currLong);
            estations = EvsesData.dbConnection(temp, auto.plug);
            counter = 0;
            for (var i = 0; i < estations.Count; i++){
                string testString = "http://127.0.0.1:7133/route/v1/driving/" + step + ";" + estations[i] + "?overview=false&alternatives=true&steps=true";
                stationCount = testString.Count(c => c == ';');
                GetRequest(testString);
                if(counter > 0)
                {
                    counter = 0;
                    GetRequest("http://127.0.0.1:7133/route/v1/driving/" + estations[i] + ";" + end + "?overview=false&alternatives=true&steps=true");
                    if (counter > 0)
                    {
                        start = start + ";" + estations[i];
                        finalRoute(start);
                    }
                }
            }

        }

        public static void finalRoute(String bestStation)
        {
            String path = "http://127.0.0.1:7133/route/v1/driving/" + bestStation + ";" + end + "?overview=false";

            dynamic mycontent = GetRequest(path).GetAwaiter().GetResult();
            Console.WriteLine("Die beste Route ist:\n\n " + mycontent);
        }
    }
}