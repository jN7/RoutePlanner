using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace RouteplannerTest
{
    class LatLongConverter
    {
        public static double EARTH_RADIUS = 6371000.0;
        public static String Mercator2GeoDec(string X, string Y)
        {
            try
            {
                double XArg = Convert.ToDouble(X);
                double YArg = Convert.ToDouble(Y);
                double Lambda, Phi;

                Lambda = (180 / Math.PI) * (XArg / EARTH_RADIUS + 0.0);
                Phi = (180 / Math.PI) * (Math.Atan(Math.Exp(YArg / EARTH_RADIUS)) - (Math.PI / 4)) / 0.5;
                XArg = Lambda;
                YArg = Phi;
                XArg = XArg;
                YArg = YArg;

                X = Convert.ToString(Math.Round(XArg, 5)).Replace(',','.');
                Y = Convert.ToString(Math.Round(YArg, 5)).Replace(',', '.');
                return X + "," + Y;

            }
            catch
            {
                return null;
            }
        }

        public static string GeoDec2Mercator(string X, string Y)
        {
            try
            {
                //X = X.Replace(",", ".");
               // Y = Y.Replace(",", ".");
                double XArg = Convert.ToDouble(X, CultureInfo.InvariantCulture);
                double YArg = Convert.ToDouble(Y, CultureInfo.InvariantCulture);
                double Lambda, Phi;

                if (X.IndexOf(".") < 1 && X.IndexOf(",") < 1)
                {
                    XArg = XArg / 100000;
                    YArg = YArg / 100000;
                }
                Lambda = XArg;
                Phi = YArg;
                XArg = EARTH_RADIUS * ((Math.PI / 180) * ((Lambda) - 0.0));
                YArg = EARTH_RADIUS * Math.Log(Math.Tan((Math.PI / 4) + (Math.PI / 180) * Phi * 0.5));

                X = Convert.ToString(Math.Round(XArg, 0));
                Y = Convert.ToString(Math.Round(YArg, 0));
                return X + "," + Y;
            }
            catch
            {
                return null;
            }
        }

    }
}
