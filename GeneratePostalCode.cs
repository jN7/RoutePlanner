using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RouteplannerTest
{
    class GeneratePostalCode
    {
        public static String plz;

        public static String PostalCode(string lat, string lon)
        {
            GetRequest("https://nominatim.openstreetmap.org/reverse?format=xml&&email=svenhilltree@gmail.com&lat=" + lon + "&lon=" + lat);
            Console.ReadKey();
            return plz;
        }
        async static void GetRequest(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage responseNew = client.GetAsync(url).Result;
            HttpContent contentNew = responseNew.Content;
            plz = await contentNew.ReadAsStringAsync();
            plz = getBetween(plz, "<postcode>", "</postcode>");
        }



        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        public static string getPLZ()
        {
            return plz;
        }
    }
}
