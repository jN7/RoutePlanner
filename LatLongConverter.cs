using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteplannerTest
{
    class LatLongConverter
    {

        public static String Mercator2GeoDec(string X, string Y)
        {
            try
            {
                double XArg = Convert.ToDouble(X);
                double YArg = Convert.ToDouble(Y);
                double Lambda, Phi;
                double EARTH_RADIUS = 6371000.0;

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

    }
}
