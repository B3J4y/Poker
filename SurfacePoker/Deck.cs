using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    /// <summary>
    /// Deck beeinhaltet die 52 Karten
    /// Karten werden dynamisch erzeugt siehe Konstruktor
    /// dannach gemischt
    /// </summary>
    public class Deck
    {
        Card[] deck;
        private int cardsUsed;
      

        public Deck()
        {
            deck = new Card[52];

            for (int i = 0; i < deck.Length; i++)
            {
                deck[i] = new Card(new Suit(i / 13 + 1), new Pips(i % 13 + 2));
            }
        }


        /// <summary>
        /// Karten mischen
        /// </summary>
        public void shuffle()
        {
            Random random = new Random();
            for (int i = 0; i < deck.Length; i++)
            {
                int k = (int)(random.Next(52));
                Card t = deck[i];
                //      Card t = new Card(deck[i]);
                deck[i] = deck[k];
                deck[k] = t;
            }

            for (int i = 0; i < deck.Length; i++)
                deck[i].setDiscarded(false);

            cardsUsed = 0;
        }

        public int cardsLeft()
        {
            return 52 - cardsUsed;
        }

        /// <summary>
        /// dealt eine Karte und gibt das deck dann wieder zurück
        /// </summary>
        public Card dealCard()
        {

            if (cardsUsed == 52)
                shuffle();
            cardsUsed++;
            return deck[cardsUsed - 1];
        }

        /// <summary>
        /// To String zur verständlichen Ausgabe der in Zahlen repräsentierten Karten
        /// </summary>
        public override String ToString()
        {
            String t = "";
            for (int i = 0; i < 52; i++)
                if ((i + 1) % 5 == 0)
                    t = t + deck[i] + "\n";
                //        t = t + "\n" + deck[i];
                else
                    t = t + deck[i];
            return t;
        }
    }
}
