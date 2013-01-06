using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    /// <summary>
    /// Kartenwert
    /// </summary>
    class Pips
    {
        private int p;

        public Pips(int i)
        {
            p = i;
            Console.WriteLine(p);
        }

        public int getPipValue()
        {
            return p;
        }

        public override string ToString()
        {
            if (p > 1 && p < 11)
                return Convert.ToString(p);
            else if (p == 11)
                return "bube";
            else if (p == 12)
                return "dame";
            else if (p == 13)
                return "koenig";
            else if (p == 14)
                return "as";
            else return "error";
        }

        public int getValue() { return p; }

    }
}
