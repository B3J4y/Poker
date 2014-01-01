//************************************************************************************
// Card Class Version 1.00
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

namespace SurfacePoker
{
    /// <summary>
    /// The card class is meant to represent the basic 
    /// card in a 52 card deck its suit and Member value 
    /// is modifiable and implements the icomparable interface 
    /// that allows the card to be compared to other cards.
    /// </summary>
    /// 

    /// <summary>
    /// The suit enumeration is used to define the suit of each 
    /// card in the deck Suit is a Namespace enum so the it is 
    /// accessible to all classes in the assembly
    /// </summary>
    public enum Suit
    {
        Clubs = 0,
        Diamonds,
        Hearts,
        Spades
    }

    /// <summary>
    /// The Member enumeration is used to define the Member value 
    /// of each card in the deck. Member is a Namespace enum 
    /// so the it is  accessible to all classes in the assembly
    /// </summary>
    public enum Member
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }


    public class Card : IComparable
    {

        #region constructor and setup
        Suit suit;
        Member member;

        // contains the value of the card's Member IE 2 of Hearts 
        // have the value 2;
        int cardMemberValue = 0;

        // contains the value of the card in bits 
        // the first 2 bit contain value of the suit
        // the last 4 contain the Member value.
        // IE 2 of Hearts is
        // 10 0010 = 18
        int cardValue = 0;

        // by default this is set to true. it will sort a deck of cards
        private static bool sortWithSuit = true;


        public Card(int suit, int value)
        {
            this.suit = (Suit)suit;
            this.member = (Member)value;

            cardMemberValue = value;
            cardValue = suit;
            cardValue <<= 4;
            cardValue += value;
        }

        #endregion

        #region  public Methods

        /// <summary>
        /// returns the name of the card in a string
        /// IE "Ace of Hearts" or "Two of Clubs"
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return member + " of " + suit;
        }

        /// <summary>
        /// returns the image of the card to the caller.
        /// the image is not stored in the class to save space
        /// it is far more likely that the image need not be seen
        /// </summary>
        /// <returns></returns>
        /*
        public Image CardImage()
        {
            return (new CardImages.ImageRetrival()).GetImage(cardValue);
        }
        */

        #endregion

        #region public properties

        /// <summary>
        /// Determines if the cards are sorted with respect to
        /// the suit and Member value or just the Member value
        /// </summary>
        public bool CompareWithSuit
        {
            get { return sortWithSuit; }
            set { sortWithSuit = value; }
        }

        /// <summary>
        /// returns the value of the suit of this card
        /// </summary>
        public Suit GetSuit
        {
            get { return suit; }
        }

        /// <summary>
        /// returns the Member value of this card
        /// </summary>
        public Member GetMember
        {
            get { return member; }
        }

        /// <summary>
        /// returns the value of the card, including the suit
        /// </summary>
        public int GetCardValue
        {
            get { return cardValue; }
        }

        /// <summary>
        /// returns the value of the card excluding the suit
        /// </summary>
        public int GetMemberValue
        {
            get { return cardMemberValue; }
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// cards are sorted with the higher Member value moving to the bottom of the array
        /// when sortWithSuit is true the cards are sorted on suit first with the higher suits
        /// moving to the bottom and then Member vale again moving to the bottom.
        /// if sortWithSuit is false it is sorted on Member value and Suit is not 
        /// guaranteed to be in any particular order in the Member value groups
        /// </summary>
        /// <param name="obj">Card</param>
        /// <returns>int</returns>
        public int CompareTo(object obj)
        {
            if (obj is Card)
            {
                Card c = (Card)obj;

                if (sortWithSuit)
                {
                    return c.cardValue.CompareTo(cardValue);
                }
                else
                {
                    return c.cardMemberValue.CompareTo(cardMemberValue);
                }
            }
            else
                throw new ArgumentException("Object is not a card");
        }

        #endregion
    }
}
