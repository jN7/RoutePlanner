using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteplannerTest
{
    public class Maneuver
    {
        public int Bearing_after { get; set; }
        public List<double> Location { get; set; }
        public int Bearing_before { get; set; }
        public string Type { get; set; }
        public int? Exit { get; set; }
        public string Modifier { get; set; }
    }
}
