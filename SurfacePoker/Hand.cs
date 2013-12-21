using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    /// <summary>
    /// Jeder Spieler besitzt eine Hand
    /// diese beeinhaltet ein Card Array das aus zwei Karten besteht
    /// </summary>
    public class Hand
    {
        private Card[] cards;

        public Hand()
        {
            cards = new Card[2];
        }

        public void setCard(int i, Card c)
        {
            cards[i] = c;
        }

        public int getHandValue()
        {
            return cards[0].getPip().getPipValue()
                     + cards[1].getPip().getPipValue();

        }

        public String getCard(int i)
        {

            String card = "";
            card += cards[i];
          

            return card;
        }

        public Card getCardObj(int cardNumber)
        {
            return cards[cardNumber];
        }

        public override String ToString()
        {
            String str = "";

            for (int i = 0; i < cards.Length; i++)
            {
                str += "\t" + (i + 1) + ": ";
                str += cards[i];
                if (cards[i].isDiscarded() == true)
                    str += " DISCARDED";
                str += "\n";
            }

            return str;
        }
    }
}
