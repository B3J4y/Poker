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
        private String playername = "";
        private double budget = 0;
        private bool isCurrentPlayer;
        private Hand hand = null;
        private float tagID;
        private int recoHandValue = 0;
        private bool ismconn = false;
        private List<Chip> chips = new List<Chip>();
        private int orientation;
        private bool fold = false;

        private int gesetztInRunde = 0; // mitzählen was spieler gesetzt hat ingesammt um zu erkennen ob die nächste runde beginnt abgleich mit anderen aktiven spielern

        private int gesetzt = 0;

        private int playerid = 0;

        public Player(String playername, double budget, Boolean isCurrentPlayer, int orientation) { this.playername = playername; this.budget = budget; this.orientation = orientation; }
        public Player(String playername, double budget) { this.playername = playername; this.budget = budget; }
        public Player(String playername) { this.playername = playername; }
        public Player() { }

        public int PlayerID
        {
            get { return playerid; }
            set { playerid = value; }
        }

        public int Gesetzt
        {
            get { return gesetzt; }
            set { gesetzt = value; }
        }

        public int GesetztRunde {
            get { return gesetztInRunde; }
            set { gesetztInRunde = value; }
        }


        public bool Fold
        {
            get { return fold; }
            set { fold = value; }
        }

        public int Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        /*
         * 1 - highCard
         * 11 - royalFlush
         * 10 - straightFlush
         * */
        private int highestCardValueHelper = 0;

        public void setChip(Chip chip)
        {
            chips.Add(chip);
        }

        public List<Chip> getChips()
        {
            return this.chips;
        }

        public bool PlayerConnection
        {
            get { return ismconn; }
            set { ismconn = value; }
        }

        public enum HighestHand
        {
            highCard,
            royalFlush,
            straightFlush,
            fourOfAKind,
            fullHouse,
            flush,
            straight,
            threeOfAKind,
            twoPair,
            onePair,

        }

        public enum State
        {
            unknow,
            fold,
            bet,
            raise,
            check
        }

        public enum BlindDealState
        { 
            unknow,
            small,
            big,
            dealer
        }

        private State state;
        private HighestHand winnerHand;
        private BlindDealState blindDealState;

        public int CompareTo(Object o)
        {
            if (o is Player)
                return this.highestCardValueHelper - ((Player)o).highestCardValueHelper;
            return 0;
        }

        public BlindDealState PlayerBlindDealState
        {
            get { return blindDealState; }
            set { blindDealState = value; }
        }

        public State PlayerState
        {
            get { return state; }
            set { state = value; }
        }

        public HighestHand hightH
        {
            get { return winnerHand; }
            set { winnerHand = value; }
        }

        public int RecoHand
        {
            get { return recoHandValue; }
            set { recoHandValue = value; }
        }

        public int CardValueHelper
        {
            get { return highestCardValueHelper; }
            set { highestCardValueHelper = value; }
        }

        public override string ToString()
        {
            return playername;
        }

        public double getBudget()
        {
            return budget;
        }

        public void setBudget(double budget)
        {
            this.budget = budget;
        }

        public bool getIsCurrentPlayer()
        {
            return isCurrentPlayer;
        }

        public void setCurrentPlayer(bool current)
        {
            this.isCurrentPlayer = current;
        }

        public String getPlayername()
        {
            return playername;
        }

        public void setPlayername(String playername)
        {
            this.playername = playername;
        }
        public void setHand(Hand hand)
        {
            this.hand = hand;
        }

        public Hand getHand()
        {
            return hand;
        }

        public float getTagID()
        {
            return this.tagID;
        }

        public void setTagID(int tagID)
        {
            this.tagID = tagID;
        }

    }
}
