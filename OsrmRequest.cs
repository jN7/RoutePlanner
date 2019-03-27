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

        public static double dist = 0;
        public static Car auto = new Car(50,80, "Typ2"); //range in km
        public static string start = "8.4034195,49.0068705";
        public static string end = "9.1800132,48.7784485";
        public static double shortest, shortestWay = Double.MaxValue;
        public static bool reachable = true;
        public static int counter = 0;

        static void Main(string[] args)
        {
            //EvsesData.dbConnection();
            GetRequest("http://127.0.0.1:7133/route/v1/driving/8.4034195,49.0068705;9.1800132,48.7784485?overview=false&alternatives=true&steps=true&hints=;");

            // Console.ReadKey();
        }
        async static void GetRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
                        // ausgabe geojson osrm
                        // Console.WriteLine(mycontent);
                        checkDistance(mycontent);
                    }
                }
            }
        }
        async static void GetRequestCharger(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
                        // ausgabe geojson osrm
                        // Console.WriteLine(mycontent);
                        var comma = url.Count(c => c == ';');
                        checkDistanceCharger(mycontent, comma-1);
                        return;
                    }
                }
            }
        }
        async static void GetRequestFinal(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
                        Console.WriteLine("Die beste Route ist:\n\n " + mycontent);
                    }
                }
            }
        }
        public static void getEcharger(dynamic step)
        {
            int station = 0;
            List<string> estations = new List<String>();
            String currLat, currLong, plz, temp;
            //if (step.maneuver.location[0] != null && step.maneuver.location[1] != null)
            if (step is JObject)
            {
                currLat = step.maneuver.location[0];
                currLong = step.maneuver.location[1];
                //plz = GeneratePostalCode.PostalCode(currLat, currLong);
                temp = LatLongConverter.GeoDec2Mercator(currLat, currLong);
            }
            else
            {
                string[] old_new = step.Split(';');
                //nur für hinteren teil brauchen wir coords
                string[] coords = old_new[old_new.Length-1].Split(',');
                currLat = coords[0];
                currLong = coords[1];
                //plz = GeneratePostalCode.PostalCode(currLat, currLong);
                temp = LatLongConverter.GeoDec2Mercator(currLat, currLong);
            }
            estations = EvsesData.dbConnection(temp,auto.plug);
            counter = 0;
            for (var i = 0; i < estations.Count; i++)
            {

                if (step is string && step.Contains(","))
                {
                    if (string.Compare(step, estations[i]) == 0 || step.Contains(estations[i]))
                    {
                        continue;
                    }
                    String testRoute = "http://127.0.0.1:7133/route/v1/driving/" + start + ";" + step + ";" + estations[i] + ";" + end + "?overview=false&alternatives=true&steps=true";
                    GetRequestCharger(testRoute);
                }
                else
                {
                    String testRoute = "http://127.0.0.1:7133/route/v1/driving/" + start + ";" + estations[i] + ";" + end + "?overview=false&alternatives=true&steps=true";
                    GetRequestCharger(testRoute);
                }

                if (shortestWay > shortest)
                {
                    shortestWay = shortest;
                    station = i;
                    Console.WriteLine(station + "ist kürzeste Station mit einer Entfernung von:" + shortestWay);
                }
            }
            if (reachable)
            {
                finalRoute(step + ";" + estations[station]);
            }
            //kürzesten weg von "bestem" charger zum ziel suchen
            else
            {
                if (step is string && step.Contains(","))
                {
                    shortestWay = Double.MaxValue;
                    //richtig verarbeiten in getEcharger!!!
                    getEcharger(step+";"+estations[station]);
                }
                else
                {
                    shortestWay = Double.MaxValue;
                    getEcharger(estations[station]);
                }

            }

        }
        public static void checkDistance(String mycontent)
        {
            dynamic dynJson = JsonConvert.DeserializeObject(mycontent);
            //if(auto.distance)
            //Console.WriteLine(dynJson.routes[0].legs[0].steps[0].driving_side);
            var route = dynJson.routes[0];
            for (var l = 0; l < route.legs.Count; l++)
            {
                var leg = route.legs[l];
                var step = leg.steps[0];
                if (dist < auto.range * 0.80)
                {
                    for (var m = 0; m < leg.steps.Count; m++)
                    {
                        step = leg.steps[m];
                        double temp = (double)step.distance / 1000;
                        dist = dist + temp;
                        if (dist > auto.range * 0.8)
                        {
                            //eins vorher weil sonst distanz nicht ausreichend
                            getEcharger(leg.steps[m - 1]);
                        }
                        Console.WriteLine(dist);
                    }
                    Console.WriteLine("Die Route ist ohne weitere Aufladung erreichbar: " +dynJson);
                    Console.WriteLine("Press Key to Exit");
                    Console.ReadKey();
                }
                //route zu echarger suchen
                else getEcharger(step);
            }
        }
        public static void checkDistanceCharger(String mycontent, int chargercount)
        {
            dynamic dynJson = JsonConvert.DeserializeObject(mycontent);
            var route = dynJson.routes[0];
            dist = 0;
            //double distCharger = 0;
            double range = auto.range * 0.8;

            for (var c = 0; c < dynJson.waypoints.Count; c++)
            {
                dist = Convert.ToDouble(dynJson.waypoints[c].distance) + dist;
                if(dist > range)
                {
                    reachable = false;
                    //String la = dynJson.waypoints[c - 1].location[0];
                    //String lo = dynJson.waypoints[c - 1].location[1];
                    //getEcharger(la+","+lo);
                    break;
                    
                }
                else
                {   
                    reachable = true;
                    continue;
                }
            }
            if (dist < shortestWay)
            {
                if (dist >= range * 0.5)
                {
                    shortest = dist;
                }
            }
        }


        public static void finalRoute(String bestStation)
        {
            String path = "http://127.0.0.1:7133/route/v1/driving/" + start + ";" + bestStation + ";" + end + "?overview=false";
            GetRequestFinal(path);
            //Console.Read();
           // Console.WriteLine(path);
        }
    }
}