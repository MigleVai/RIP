using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIP
{
    public class Lines
    {
        public Point First { get; set; }
        public Point Second { get; set; }
        public char FirstName { get; set; }
        public char SecondName { get; set; }

        public int Value { get; set; }

        public Lines(Point first, Point second, char fName, char sName)
        {
            First = first;
            Second = second;
            FirstName = fName;
            SecondName = sName;
        }
    }
}
