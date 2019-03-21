using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteplannerTest
{
    public class Waypoint
    {
        public string Hint { get; set; }
        public double Distance { get; set; }
        public string Name { get; set; }
        public List<double> Location { get; set; }
    }
}
