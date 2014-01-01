//************************************************************************************
// Card Deck Version 1.00
//
// Copyright (c) 2004 Wesley Varela & Jonathan Feldkamp
// All rights reserved.
//
// Permission is hereby granted, to any person obtaining a
// copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, and/or sell copies of the Software, provided that they obtain
// written permission from the author(s) for its use and that the above
// copyright notice(s) and this permission notice appear in all copies of
// the Software and that both the above copyright notice(s) and this
// permission notice appear in supporting documentation.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT
// OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL
// INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING
// FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT,
// NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION
// WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//
//
// Disclaimer
// ----------
// Although reasonable care has been taken to ensure the correctness of this
// implementation, this code should never be used in any application without
// proper verification and testing.  I disclaim all liability and responsibility
// to any person or entity with respect to any loss or damage caused, or alleged
// to be caused, directly or indirectly, by the use of this class or any class 
// authored by me (us).
//************************************************************************************
using System;
using System.Collections;

namespace SurfacePoker
{
    /// <summary>
    /// The deck class represents a regular 52 card deck
    /// The deck is created and then shuffled so the cards
    /// are in randomized order
    /// </summary>
    public class Deck
    {
        //instance of the random class used for shuffling
        Random random = new Random(DateTime.Now.Millisecond);

        // the 52 cards in a deck
        Card[] cards;

        int deckIndex = 0;

        public Deck()
        {
            cards = new Card[52];
            int k = 0;

            for (int i = 2; i < 15; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    cards[k] = new Card(j, i);
                    k++;
                }
            }
            Shuffle();
        }

        /// <summary>
        /// shuffles the deck of card by iterating through all the cards and 
        /// replacing it with some other random card in the deck
        /// </summary>
        public void Shuffle()
        {
            // reset the index into the newly shuffled cards
            deckIndex = 0;

            // temp variable need to do the swaping
            int temp = 0;
            Card card;

            // for every card in the deck switch it with another
            for (int i = 0; i < cards.Length; i++)
            {
                temp = random.Next(0, cards.Length);
                card = cards[temp];
                cards[temp] = cards[i];
                cards[i] = card;
            }
        }

        /// <summary>
        /// returns true if there is still a card in the deck that can be dealt
        /// return false if we are at the end of the deck. HasNext should be called
        /// before trying to deal the next card, otherwise an OutOfCardsException
        /// might be thrown
        /// </summary>
        /// <returns></returns>
        public bool HasNext()
        {
            return (deckIndex < cards.Length);
        }

        /// <summary>
        /// returns the next card on the deck, if there is one to deal
        /// throws an OutOfCardsException if there are no cards left to be dealt
        /// </summary>
        /// <returns>Card</returns>
        public Card DealNext()
        {
            try
            {
                return cards[deckIndex++];
            }
            catch (IndexOutOfRangeException i)
            {
                throw new OutOfCardsException("No more cards are left in the deck\n" +
                        "Additional information: " + i.Message);
            }
        }
    }
    class OutOfCardsException : Exception
    { public OutOfCardsException(string msg) : base(msg) { } }
}