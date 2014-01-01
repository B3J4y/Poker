using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    public class Action
    {
        public Action(playerAction pa, int amount)
        {
            action = pa;
            this.amount = amount;
        }

        public Action()
        {

        }
        public enum playerAction
        {
            bet, raise, call, check, fold
        }
        public int amount { get; set; }
        public playerAction action { get; set; }
    }
}
