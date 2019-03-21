﻿using System;
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
        public static Car auto = new Car(50, 50, "CHADEMO"); //range in km
        public static string start = "8.4034195,49.0068705";
        public static string end = "9.1800132,48.7784485";
        public static double shortest;

        static void Main(string[] args)
        {
            //EvsesData.dbConnection();
            GetRequest("http://127.0.0.1:7133/route/v1/driving/8.4034195,49.0068705;9.1800132,48.7784485?overview=false&alternatives=true&steps=true&hints=;");

            Console.ReadKey();
        }
        async static void GetRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
                        // ausgabe geojson osrm
                        // Console.WriteLine(mycontent);
                        if (url.Contains("estations[i]"))
                        {
                            checkDistanceCharger(mycontent);
                        }
                        else checkDistance(mycontent);
                    }
                }
            }
        }
            async static void GetRequestCharger(string url)
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        using (HttpContent content = response.Content)
                        {
                            string mycontent = await content.ReadAsStringAsync();
                            checkDistanceCharger(mycontent);
                        }
                    }
                }
            }
            public static void getEcharger(dynamic step)
            {
                var estations = new List<String>();
                String currLat = step.maneuver.location[0];
                String currLong = step.maneuver.location[1];
                String plz = GeneratePostalCode.PostalCode(currLat, currLong);
                estations = EvsesData.dbConnection(plz);
                int counter = 0;
                double shortestWay = Double.MaxValue;
                int station = 0;
                for (var i = 0; i < estations.Count; i++)
                {

                    String testRoute = "http://127.0.0.1:7133/route/v1/driving/" + start + ";" + estations[i] + ";" + end + "?overview=false&alternatives=true&steps=true";
                    GetRequestCharger(testRoute);
                    Console.Read();

                //was liefert mir kürzeste distanz ?
                    if(shortest < shortestWay)
                    {
                        shortestWay = shortest;
                        station = i;
                        counter++;
                    }
                }
            finalRoute(estations[station]);
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
                    }
                    //route zu echarger suchen
                    else getEcharger(step);
                }
            }
            public static void checkDistanceCharger(String mycontent)
            {
                dynamic dynJson = JsonConvert.DeserializeObject(mycontent);
                var route = dynJson.routes[0];
                dist = 0; double distCharger = 0;
            
                for(var c = 0; c < dynJson.waypoints.Count; c++)
                {
                    dist = Convert.ToDouble(dynJson.waypoints[c].distance) + dist;
                    if(c < dynJson.waypoints.Count && c > 0)
                    {
                        distCharger = distCharger + Convert.ToDouble(dynJson.waypoints[c].distance) -50;
                    }
                }
            if ((dist - auto.range * 0.8) < auto.range * 0.8 && distCharger < auto.range * 0.8)
            {
                shortest = dist;
            }
            else
            {
                //TODO
            }
                Console.WriteLine(dist);
            }
        public static void finalRoute(String bestStation)
        {
            String path = "http://127.0.0.1:7133/route/v1/driving/" + start + ";" + bestStation + ";" + end + "?overview=false&alternatives=true&steps=true";
            GetRequest(path);
            Console.Read();
        }
        }
}