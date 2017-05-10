using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIP
{
    public class Point
    {
        public Point(int x, int y)
        {
            XCord = x;
            YCord = y;
            Neighbors = new Dictionary<Point, int>();
            Hops = new List<Hop>();
        }
        public int XCord { get; set; }
        public int YCord { get; set; }
        public char Name { get; set; }
        public Dictionary<Point, int> Neighbors { get; set;} // name, value
        public List<Hop> Hops { get; set; }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
