using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIP
{
    public class Hop
    {
        public Hop(Point dest, int value, Point hop)
        {
            Destination = dest;
            Value = value;
            HopStep = hop;
        }
        public Point Destination { get; set; }
        public int Value { get; set; }
        public Point HopStep { get; set; }
    }
}
