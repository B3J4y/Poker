using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

namespace SurfacePoker
{
    public class Action
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Action(playerAction pa, int amount)
        {
            log.Debug("Action(playerAction" + pa.ToString() + ", int" + amount + ") - Begin");
            action = pa;
            this.amount = amount;
        }

        public Action()
        {

        }
        public enum playerAction
        {
            bet, raise, call, check, fold, nothing, wins, bigblind, smallblind, newgame, ingame
        }
        public int amount { get; set; }
        public playerAction action { get; set; }
    }
}
