using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteplannerTest
{
    public class RootObject
    {
        public string code { get; set; }
        public List<Route> routes { get; set; }
        public List<Waypoint> waypoints { get; set; }
    }
}
