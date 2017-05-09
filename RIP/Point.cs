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
        }
        public int XCord { get; set; }
        public int YCord { get; set; }
        public char Name { get; set; }
    }
}
