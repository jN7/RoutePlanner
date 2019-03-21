using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteplannerTest
{
    public class Step
    {
        public List<Intersection> intersections { get; set; }
        public string Driving_side { get; set; }
        public string Geometry { get; set; }
        public string Mode { get; set; }
        public double Duration { get; set; }
        public Maneuver Maneuver { get; set; }
        public double Weight { get; set; }
        public double Distance { get; set; }
        public string Name { get; set; }
        public string @Ref { get; set; }
        public string Destinations { get; set; }
        public string Rotary_name { get; set; }
    }
}
