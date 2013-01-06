using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    /// <summary>
    /// Tisch un dessen Karten
    /// </summary>
    class Table
    {
        Card[] cards;

        public Table()
        {
            cards = new Card[5];
        }

        public void setCard(int i, Card c)
        {
            cards[i] = c;
        }

        public String getCard(int i)
        {

            String card = "";
            card += cards[i];
            return card;
        }

        public int getTableValue()
        {
            return cards[0].getPip().getPipValue()
                     + cards[1].getPip().getPipValue()
                     + cards[2].getPip().getPipValue()
                     + cards[3].getPip().getPipValue()
                     + cards[4].getPip().getPipValue();
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
