//************************************************************************************
// Card Hand Version 1.00
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
    /// The hand is made up of five cards
    /// the five cards are setup in a int that indicates its overall value
    /// </summary>
    public class Hand : IComparable
    {
        //holds the current cards of this hand
        Card[] cards;

        // holds the type of the hand in string format
        // the hand must be evaluated before it holds
        // anything more then the empty string
        string type = string.Empty;

        // used to hold the ranking of this hand in its type/cards partitioning
        // it is used to compare this hand against any other hand.
        // it can only be set interally, this assembily only.
        int handRanking = 0;

        // used to store the PlayerID. the player that has this hand
        int playerID;

        object tag = null;

        public Hand(object player) { this.tag = player; }

        #region public methods

        /// <summary>
        /// adds the cards to the hand which are then evaluated
        /// there can only be 3 or 5 cards in the hand
        /// </summary>
        /// <param name="cards">the cards to be placed into the hand</param>
        public void AddCards(Card[] cards)
        {
            if (!(cards.Length == 3 || cards.Length == 5))
            {
                throw new ArgumentException("There can only be 3 or five cards per hand");
            }

            this.cards = cards;
        }


        /// <summary>
        /// provides access to the cards being held by the hand
        /// </summary>
        /// <returns>the currently held cards</returns>
        public Card[] ReturnCards()
        {
            return cards;
        }

        #endregion

        #region public properties
        /// <summary>
        /// returns the number of of cards currently being held in the hand
        /// 3 for three card games, 5 for all others
        /// </summary>
        public int Count
        {
            get { return cards.Length; }
        }

        /// <summary>
        /// gets or set the type of the hand in string format.
        /// IE Flush or ThreeOfAKind
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        //		/// <summary>
        //		/// returns the players id, the id of the play who owns this hand
        //		/// </summary>
        //		public int PlayerID
        //		{
        //			get{return playerID;}
        //		}

        /// <summary>
        /// a tag object that can be used to store addintional information
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// set the ranking of a hand in, this can only be done internally
        /// </summary>
        internal int SetHandRanking
        {
            set { handRanking = value; }
        }

        /// <summary>
        /// returns the value of this hand to who ever called this method
        /// </summary>
        public int GetHandRanking
        {
            get { return handRanking; }
        }
        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is Hand)
            {
                Hand temp = (Hand)obj;
                return temp.handRanking.CompareTo(handRanking);
            }
            throw new ArgumentException("Object is not a Hand");
        }

        #endregion
    }
}
