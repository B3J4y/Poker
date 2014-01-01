using System;
using System.Collections.Generic;
namespace SurfacePoker
{

public class Pot
    {
        public int value { get; set; }
        public int raiseSize { get; set; }
        public int amountPerPlayer { get; set; }
        public List<Pot> sidePots { get; set; }
	    public Pot()
	    {
            value = 0;
            sidePots = new List<Pot>();
	    }

        public void raisePot(int value)
        {
            this.value += value;
            //Todo: Sidepots
        }
    }
}
