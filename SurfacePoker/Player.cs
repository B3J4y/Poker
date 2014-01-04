using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    /// <summary>
    /// Spieler und dessen Attribute sowie States für das Handling mit dem UI
    /// </summary>
    public class Player
    {
        public bool isActive { get; set; }
        public int stack { get; set; }
        public List<Card> cards { get; set; }
        public int position { get; set; }
        public int ingamePosition { get; set; }
        public bool inGame { get; set; }
        public int inPot { get; set; }
        public String name { get; set; }
        public bool hasChecked { get; set; }

        public Player(String name, int stack, int position)
        {
            this.name = name;
            this.stack = stack;
            isActive = true;
            inGame = true;
            inPot = 0;
            this.cards = new List<Card>();
            hasChecked = false;
            this.position = position;
            this.ingamePosition = -1;
        }

        public Player(Player player)
        {
            this.isActive = player.isActive;
            this.stack = player.stack;
            this.cards = player.cards;
            this.position = player.position;
            this.ingamePosition = player.ingamePosition;
            this.inGame = player.inGame;
            this.inPot = player.inPot;
            this.name = player.name;
            this.hasChecked = player.hasChecked;
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
            if (move < this.stack)
            {
                int diff = (move - this.inPot); 
                this.stack -= diff;
                inPot = (move);
                return diff;
            }
            else
            {
                int diff = this.stack -this.inPot;
                inPot = this.stack;
                return diff;
            }
        }

        public void setOneCard(Card card)
        {
            cards.Add(card);
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
