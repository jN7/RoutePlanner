using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteplannerTest
{
    class Car
    {
        //enum PlugType { DOMESTIC_F, IEC_62196_T2, CHADEMO, IEC_62196_T2_COMBO, TESLA };
        public int kWh;
        public int range;
        public String plug;

        public Car(int kWh, int range, String plug)
        {
            this.kWh = kWh;
            this.range = range;
            this.plug = plug;
        }

        public Car()
        {
        }
    }
}
