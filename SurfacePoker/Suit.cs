using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    /// <summary>
    /// Farbe des Karte Herz, Kreuz, Pik, Karo
    /// </summary>
    class Suit
    {
        const int CLUBS = 1;
        const int DIAMONDS = 2;
        const int HEARTS = 3;
        const int SPADES = 4;

        private int suitValue;

        public Suit(int i)
        {
            this.suitValue = i;
        }

        public int getSuitValue()
        {
            return suitValue;
        }

        public override string ToString()
        {
            switch (suitValue)
            {
                case CLUBS: return "kreuz";
                case DIAMONDS: return "karo";
                case HEARTS: return "herz";
                case SPADES: return "pik";
                default: return "error";
            }
        }

    }
}
