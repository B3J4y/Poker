using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    /// <summary>
    /// Card Objekt mit Pip und Suit der Karte
    /// </summary>
    class Card
    {
        Suit suit;
        Pips pip;

        private Boolean discarded;

        public Card(Suit s, Pips p) { this.suit = s; this.pip = p; this.discarded = false; }
        public Card(Card c) { this.suit = c.suit; this.pip = c.pip; this.discarded = false; }
        public Card() { }

        public int CompareTo(Object o)
        {
            if (o is Card)
                return this.pip.getPipValue() - ((Card)o).pip.getPipValue();
            return 0;
        }

        public override string ToString()
        {
            return suit.ToString() + ""
                + pip.ToString();
        }

        public Suit getSuit() { return suit; }
        public Pips getPip() { return pip; }

        public Boolean isDiscarded() { return discarded; }
        public void setDiscarded(Boolean value) { discarded = value; }
    }
}
