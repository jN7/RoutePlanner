using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteplannerTest
{
    public class Route
    {
        public List<Leg> Legs { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
        public string Weight_name { get; set; }
        public double Weight { get; set; }
    }
}
