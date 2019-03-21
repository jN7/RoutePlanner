using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteplannerTest
{
    public class Intersection
    {
        public int @Out { get; set; }
        public List<bool> Entry { get; set; }
        public List<int> Bearings { get; set; }
        public List<double> Location { get; set; }
        public int? @In { get; set; }
        public List<Lane> Lanes { get; set; }
        public List<string> Classes { get; set; }
    }
}
