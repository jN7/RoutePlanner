using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Project1;

namespace RouteplannerTest
{
    class EvsesData
    {
        public static void dbConnection()
        {
            List<String> converted = new List<String>();
            string queryString = "SELECT TOP 20 xkoord,ykoord FROM  [st_data].[se].basisinfo where xkoord <> '' and ykoord <> '' and (PLZ Like '7%' or PLZ like '8%') and (land = 'D' or land = 'DE')";
            string connectionString = "Server=ymtestdb1;Database=;User Id=se;Password=se;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        String xkoord = "", ykoord = "";
                        xkoord = Convert.ToString(reader["xkoord"]);
                        ykoord = Convert.ToString(reader["ykoord"]);
                        //Console.WriteLine(xkoord);
                        //Console.WriteLine(ykoord);
                        //YM.Geo.CUtilityGeo cUtilityGeo = new YM.Geo.CUtilityGeo();
                        //cUtilityGeo.Transform(ref xkoord, ref ykoord, YM.Geo.CUtilityGeo.COORD_FORMAT.MERCATOR, YM.Geo.CUtilityGeo.COORD_FORMAT.GEODECIMAL, YM.Geo.CUtilityGeo.COORD_ZONE.Default, YM.Geo.CUtilityGeo.COORD_ZONE.Default);
                        String temp = "";
                        temp = LatLongConverter.Mercator2GeoDec(xkoord, ykoord);
                        Console.WriteLine(temp);
                        converted.Add(temp);
                        //Console.WriteLine(String.Format("{0}, {1}",
                        //reader["xkoord"], reader["ykoord"]));
          
                        
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    foreach (var koord in converted)
                    {
                        //Console.WriteLine(koord);
                    }
                }
            }
        }
        public static List<String> dbConnection(String plz)
        {
            List<String> evsesData = new List<String>();                                                                                                  
            string queryString = "SELECT TOP 10 xkoord,ykoord FROM  [st_data].[se].basisinfo where xkoord <> '' and ykoord <> '' and (PLZ Like '" + plz.Substring(0,2) + "%') and (land = 'D' or land = 'DE')";
            string connectionString = "Server=ymtestdb1;Database=;User Id=se;Password=se;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        String xkoord = "", ykoord = "";
                        xkoord = Convert.ToString(reader["xkoord"]);
                        ykoord = Convert.ToString(reader["ykoord"]);
                        String temp = "";
                        temp = LatLongConverter.Mercator2GeoDec(xkoord, ykoord);
                        evsesData.Add(temp);
                        Console.WriteLine(temp);
                        //Console.WriteLine(String.Format("{0}, {1}",
                        //reader["xkoord"], reader["ykoord"]));


                    }
                    return evsesData;
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    //foreach (var koord in evsesData)
                    //{
                        //Console.WriteLine(koord);
                    //}
                }
            }
        }
    }
}
