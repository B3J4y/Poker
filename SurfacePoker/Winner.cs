using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace SurfacePoker
{
    public class Winner
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Player player { get; set; }
        public int value { get; set; }
        public String hand { get; set; }

        public Winner()
        {
            log.Debug("Winner() - Begin");
            log.Debug("Winner() - End");
        }
        public Winner(Player player)
        {
            log.Debug("Winner(Player " + player.name + ") - Begin");
            this.player = player;
            log.Debug("Winner() - End");
        }

        public Winner(Player player, String str)
        {
            log.Debug("Winner(Player " + player.name + ", String " + str +") - Begin");
            this.player = player;
            this.hand = str;
            log.Debug("Winner() - End");
        }
    }
}
