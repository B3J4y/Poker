using System;
using System.Collections.Generic;
using log4net;
using log4net.Config;
namespace SurfacePoker
{

public class Pot
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int value { get; set; }
        public int raiseSize { get; set; }
        private int AmountPerPlayer;
        public int amountPerPlayer { get{
            if (sidePot != null)
            {
                return (AmountPerPlayer + sidePot.AmountPerPlayer);
            }
            else
            {
                return AmountPerPlayer;
            }
        }
            set {
                AmountPerPlayer = value;
            }
        }
        public Pot sidePot { get; set; }
        public List<Player> player { get; set; }
        public int potThisRound { get; set; }
	    public Pot()
	    {
            log.Debug("Pot() - Begin");
            potThisRound = 0;
            value = 0;
            player = new List<Player>();
            log.Debug("Pot() - Begin");
	    }
        public Pot(int value, int amountPerPlayer, int potThisRound, List<Player> player, Pot sidePot)
        {
            log.Debug("Pot(int " + value + ", int " + amountPerPlayer + ", int " + potThisRound + ", List<Player> player, Pot sidePot) - Begin");
            this.value = value;
            this.sidePot = sidePot;
            this.player = player;
            this.amountPerPlayer = amountPerPlayer;
            this.potThisRound = potThisRound;
            log.Debug("Pot() - End");
        }
        public void endOfRound()
        {
            log.Debug("endOfRound() - Begin");
            if (this.sidePot != null)
            {
                this.sidePot.amountPerPlayer = 0;
                this.sidePot.endOfRound();
            }
            potThisRound = 0;
            log.Debug("endOfRound() - End");
        }
        /// <summary>
        /// a player who want to raise the pot
        /// </summary>
        /// <param name="player"></param>
        /// <param name="value"></param>
        public void raisePot(Player player,int value)
        {
            log.Debug("raisePot(Player" +player.name + ",int " + value + ") - Begin");
            if ((player.inPot < amountPerPlayer) || (player.isAllin) || (player.stack == 0))
            {
                if (sidePot != null)
                {
                    if (sidePot.amountPerPlayer > 0)
                    {
                        createSidePot(player, value);
                    }
                    else
                    {
                        addMySidePot(player, value);
                    }
                }
                else
                {
                    addMySidePot(player, value);
                }
            }
            else
            {
                if (this.sidePot == null)
                {
                    this.value += value;
                    this.potThisRound += value;
                }
                else
                {
                    this.value += value - this.sidePot.amountPerPlayer;
                    this.potThisRound += value - this.sidePot.amountPerPlayer;
                    sidePot.raisePot(player, sidePot.amountPerPlayer);
                }
            }
            log.Debug("raisePot() - End");
        }
        /// <summary>
        /// adds a sidepot to the pot
        /// </summary>
        /// <param name="player"></param>
        /// <param name="value"></param>
        private void addMySidePot(Player player, int value)
        {
            log.Debug("addMySidePot(Player "  +player.name + ", int " + value + ") - Begin");
            int times = 1 + (potThisRound / amountPerPlayer);
            List<Player> newPlayers = new List<Player>();
            newPlayers.Add(player);
            int diff = (this.amountPerPlayer - value) * (times - 1);
            Pot p = new Pot(this.value - diff + value, value, this.value - diff + value, newPlayers, sidePot);
            potThisRound = diff;
            this.value = diff;
            amountPerPlayer -= p.amountPerPlayer;
            sidePot = p;
            log.Debug("addMySidePot() - End");
        }

        /// <summary>
        /// creates a sidepot
        /// takes the other sidepots into consideration
        /// </summary>
        /// <param name="player">player who goes allin</param>
        /// <param name="value"></param>
        public void createSidePot(Player player, int value)
        {
            log.Debug("createSidePot(Player " + player.name + ", int " + value + ") - Begin");
            if (this.sidePot == null)
            {
                addMySidePot(player, value);
            }
            else
            {

                if (this.sidePot.amountPerPlayer == value)
                {
                    this.sidePot.potThisRound += value;
                    this.sidePot.value += value;
                    this.player.Add(player);

                }
                else
                {
                    if (this.sidePot.amountPerPlayer > value)
                    {
                        this.sidePot.createSidePot(player, value);
                    }
                    else
                    {
                        List<Player> newPlayers = new List<Player>();
                        newPlayers.Add(player);
                        this.sidePot.value += sidePot.amountPerPlayer;
                        this.potThisRound += sidePot.amountPerPlayer;
                        this.sidePot = new Pot(value-sidePot.amountPerPlayer, 
                            value - sidePot.amountPerPlayer,
                            value - sidePot.amountPerPlayer,
                            newPlayers,
                            this.sidePot);

                    }
                }
            }
            log.Debug("createSidePot() - End");
        }
    }
}
