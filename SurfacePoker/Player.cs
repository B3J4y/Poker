﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

namespace SurfacePoker
{
    /// <summary>
    /// Spieler und dessen Attribute sowie States für das Handling mit dem UI
    /// </summary>
    public class Player
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool isActive { get; set; }
        public int stack { get; set; }
        public List<Card> cards { get; set; }
        public int position { get; set; }
        public int ingamePosition { get; set; }
        public bool inGame { get; set; }
        public int inPot { get; set; }
        public String name { get; set; }
        public bool hasChecked { get; set; }
        public bool isAllin { get; set; }
        public long winChance { get; set; }
        public int totalInPot { get; set; }
        public Player(String name, int stack, int position)
        {
            log.Debug("Player(String " + name + ", int"+ stack +", int" + position + ") - Begin");
            this.name = name;
            this.stack = stack;
            isActive = true;
            inGame = true;
            inPot = 0;
            this.cards = new List<Card>();
            hasChecked = false;
            this.position = position;
            this.ingamePosition = -1;
            this.isAllin = false;
            log.Debug("Player() - End");
        }

        public Player(Player player)
        {
            log.Debug("Player(Player " + player.name + ")");
            this.isActive = player.isActive;
            this.stack = player.stack;
            this.cards = player.cards;
            this.position = player.position;
            this.ingamePosition = player.ingamePosition;
            this.inGame = player.inGame;
            this.inPot = player.inPot;
            this.name = player.name;
            this.hasChecked = player.hasChecked;
            this.isAllin = player.isAllin;
            log.Debug("Player() - End");
        }

        public int toCall(List<Player> players)
        {
            int highest = inPot;
            foreach (Player player in players)
            {
                if (player.inGame)
                {
                    if (highest < player.inPot)
                    {
                        highest = player.inPot;
                    }
                }
            }
            return (highest - inPot);
        }

        //auf wie viel erhöht der Player
        public int action(int move)
        {
            log.Debug("action(int " + move + ") - Begin");
            if (move < this.stack)
            {
                //int diff = (move - this.inPot); 
                this.stack -= move;
                inPot = (this.inPot + move);
                log.Debug("action() - End");
                return move;
            }
            else
            {
                //int diff = this.stack -this.inPot;
                inPot += this.stack;
                this.stack = 0;
                this.isAllin = true;
                this.isActive = false;
                log.Debug("action() - End");
                return move;
            }
            
        }

        public void setOneCard(Card card)
        {
            log.Debug("setOneCard(Card " + card.ToString() + ") - Begin");
            cards.Add(card);
            log.Debug("setOneCard() - End");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Handrepresentation like 2d tc or as</returns>
        public String getCardsToString()
        {
            return cards[0].ToString() + " " + cards[1].ToString();
        }
    }

}
