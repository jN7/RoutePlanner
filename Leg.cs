using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteplannerTest
{
    public class Leg
    {
        public List<Step> Steps { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
        public string Summary { get; set; }
        public double Weight { get; set; }
    }

}
