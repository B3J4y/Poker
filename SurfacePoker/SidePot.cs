using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    class SidePot
    {

        private int potValue = 0;
        private Player owner = null;

        public SidePot() { }


        public SidePot(int value, Player player)
        {
            this.potValue = value;
            this.owner = player;
        }

        public int SplitPotValue
        {
            get { return potValue; }
            set { potValue = value; }
        }

        public Player SplitPotOwner
        {
            get { return owner; }
            set { owner = value; }
        }
    }
}
