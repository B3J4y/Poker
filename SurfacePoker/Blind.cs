using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfacePoker
{
    /// <summary>
    /// Blinds
    /// small
    /// big
    /// dealer
    /// setzten der Blinds, switchen der blinds, raiser der blinds
    /// </summary>
    public class Blind
    {
        private int smallBlind = 10;
        private int bigBlind = 20;
        int dealerValue;
        List<Player> player;

        //Variable für höchsten aktuellen Raise
        int raiseValue=0;


        public int getDealerValue()
        {
            if (player.Count == 2)
                return dealerValue;
            else
                return dealerValue + 1;
        }

        public Blind()
        {
            player = new List<Player>();
        }

        public void setPlayer(List<Player> player)
        {
            this.player = null;
            this.player = player;

        }

        public void clearPlayerList()
        {

            this.player.Clear();
        }

        //Aus den aktiven Spieler bei Spielstart einen Zufälligen Dealer ermitteln
        public void randomDealer()
        {
            //TODO: Argument out of Range, wenn ein Tag gesetzt wird, aber noch kein Spiel losgegangen ist
            //int min = 0;
            int max = player.Count - 1;
            Random random = new Random();

            this.dealerValue = random.Next(max); 
            this.player.ElementAt(dealerValue).PlayerBlindDealState = Player.BlindDealState.dealer;
            setPlayerBlindStates();
        }

        //Bigblind und Smallblind per zufall nach dem Dealer initalisieren
        public void setPlayerBlindStates()
        {
            //Fall das nur noch zwei Spieler vorhanden sind in diesem Fall ist der Small auch Dealer
            if (player.Count == 2)
            {
                this.player.ElementAt(dealerValue).PlayerBlindDealState = Player.BlindDealState.small;
                this.player.ElementAt(dealerValue).setCurrentPlayer(true);
                this.player.ElementAt(dealerValue + 1).PlayerBlindDealState = Player.BlindDealState.big;
            }
            //Alternative fälle #Spieler >2
            else
            {
                if (dealerValue + 2 == this.player.Count)
                {
                    this.player.ElementAt(0).PlayerBlindDealState = Player.BlindDealState.big;
                    this.player.ElementAt(dealerValue + 1).PlayerBlindDealState = Player.BlindDealState.small;
                    this.player.ElementAt(dealerValue + 1).setCurrentPlayer(true);
                    this.player.ElementAt(dealerValue).PlayerBlindDealState = Player.BlindDealState.dealer;
                  //  Console.WriteLine("Big: " + this.player.ElementAt(0).getPlayername());
                  //  Console.WriteLine("small: " + this.player.ElementAt(dealerValue + 1).getPlayername());
                }
                else
                {
                    this.player.ElementAt(dealerValue + 1).PlayerBlindDealState = Player.BlindDealState.small;
                    this.player.ElementAt(dealerValue + 1).setCurrentPlayer(true);
                    this.player.ElementAt(dealerValue + 2).PlayerBlindDealState = Player.BlindDealState.big;
                   // Console.WriteLine("Big: " + this.player.ElementAt(dealerValue + 2).getPlayername());
                   // Console.WriteLine("small: " + this.player.ElementAt(dealerValue + 1).getPlayername());
                }
            }
        }


        //Bigblind und Smallblind an den jeweilig nächsten Spieler weitergeben
        //wichtig für berechnung der korekkten bets und raises
        //Ringfortsetzung berückstichtigt
        public void switchBlindsAndDealer()
        {

            //Fall das nur noch zwei Spieler vorhanden sind in diesem Fall ist der Small auch Dealer
            if (player.Count == 2)
            {
                foreach (Player playerStateCheck in this.player)
                {
                    if (playerStateCheck.PlayerBlindDealState == Player.BlindDealState.big)
                        playerStateCheck.PlayerBlindDealState = Player.BlindDealState.small;
                    else if (playerStateCheck.PlayerBlindDealState == Player.BlindDealState.small)
                        playerStateCheck.PlayerBlindDealState = Player.BlindDealState.big;


                   // Console.WriteLine(playerStateCheck.PlayerBlindDealState + "    " + playerStateCheck.getPlayername());
                }
            }
            //Alternative fälle #Spieler >2
            else
            {



                int currentBig = 0;

                foreach (Player playerStateCheck in this.player)
                {
                    if (playerStateCheck.PlayerBlindDealState != Player.BlindDealState.big)
                    {
                        currentBig++;
                    }
                    else
                    {
                        break;
                    }
                }


                foreach (Player playerStateCheck in this.player)
                {
                    playerStateCheck.PlayerBlindDealState = Player.BlindDealState.unknow;
                }


                //Fall das der Big am Ende angekommen ist un wieder zu Spieler 0 übergeht
                if (currentBig + 1 == this.player.Count)
                {
                    this.player.ElementAt(0).PlayerBlindDealState = Player.BlindDealState.big;
                    this.player.ElementAt(currentBig).PlayerBlindDealState = Player.BlindDealState.small;
                    this.player.ElementAt(currentBig - 1).PlayerBlindDealState = Player.BlindDealState.dealer;

                   // Console.WriteLine("Big: " + this.player.ElementAt(0).getPlayername());
                   // Console.WriteLine("small: " + this.player.ElementAt(currentBig).getPlayername());
                   // Console.WriteLine("dealer: " + this.player.ElementAt(currentBig - 1).getPlayername());

                }
                //Big ist noch nicht über die Grenze hinaus
                else
                {

                    this.player.ElementAt(currentBig + 1).PlayerBlindDealState = Player.BlindDealState.big;
                    this.player.ElementAt(currentBig).PlayerBlindDealState = Player.BlindDealState.small;
                    // this.player.ElementAt(currentBig -1).PlayerBlindDealState = Player.BlindDealState.dealer;


                    //Console.WriteLine("Big: " + this.player.ElementAt(currentBig + 1).getPlayername());
                    //Console.WriteLine("small: " + this.player.ElementAt(currentBig).getPlayername());
                    //Console.WriteLine("dealer: " + this.player.ElementAt(currentBig - 1).getPlayername());

                }


            }


        }

        // Methode um zu berechnen des raiseValue
        public void setRaiseValue(Player player, int round)
        {

            if (round == -1) {
                raiseValue = 0;
            }
            else if (player.PlayerBlindDealState == Player.BlindDealState.big && (player.Gesetzt > raiseValue))
            {
                if(round==0)
                    raiseValue = player.Gesetzt-bigBlind;
                if (round > 0)
                    raiseValue = player.Gesetzt;
            }
            else if (player.PlayerBlindDealState == Player.BlindDealState.small && (player.Gesetzt > raiseValue))
            {
                if (round == 0)
                    raiseValue = player.Gesetzt - smallBlind;
                if (round == 1)
                    raiseValue = player.Gesetzt-smallBlind;
                if (round > 1)
                    raiseValue = player.Gesetzt;

            }
            else if (player.PlayerBlindDealState == Player.BlindDealState.unknow || player.PlayerBlindDealState == Player.BlindDealState.dealer)
            {
                if (player.Gesetzt > raiseValue)
                    raiseValue = player.Gesetzt - bigBlind;
            }


            //Console.WriteLine("RaiseValue:   " + raiseValue);
        }

        public Boolean nextWettRoundTwo(List<Player> players)
        {
            int counter = 0;
            int maxBet = 0;


            foreach (Player player in players)
            {
                if (player.GesetztRunde > maxBet)
                    maxBet = player.GesetztRunde;
            }

            

            foreach (Player player in players)
            {
                if(player.GesetztRunde!=maxBet)
                    counter++;
            }

            if (counter == 0 &&maxBet>0)
                return true;

            return false;
            
        
        }

        public Boolean nextWettRound(List<Player>players) {
            int counter = 0;
            double maxBet = 0;



            foreach (Player player in players)
            {
                double calc = 10000-player.getBudget();

                if (calc > maxBet)
                {
                    maxBet = calc;
                }

                if (calc == maxBet)
                {
                    counter++;
                }

            }

            if (counter > players.Count-1)
                return true;

            return false;

        
        }


        //Methode zum berechnen des minBetValue
        public int minBetValue(Player player, int round)
        {
            int minBet = 0;

            if (player.PlayerBlindDealState == Player.BlindDealState.small)
            {
                if (round == 1) {
                    if (raiseValue > 0)
                        minBet += raiseValue+smallBlind;
                }
                if (round == 0)
                    minBet += smallBlind;

                if (round > 1)
                    minBet += raiseValue;
            }
            else if (player.PlayerBlindDealState == Player.BlindDealState.big)
            {
                if (round > 0)
                {
                    if (raiseValue > 0)
                        minBet += raiseValue;
                }
                if (round == 0)
                    minBet += bigBlind;

            }
            else
            {
                if (round > 0)
                {
                    if (raiseValue > 0)
                        minBet += raiseValue;
                }
                else
                    minBet += bigBlind;
            }

           // Console.WriteLine("RV"+raiseValue);
           // Console.WriteLine("minbet" + minBet);

            return minBet;

        }

        public Player getbigBlindPlayer () {
            foreach (Player player in this.player)
            {
                if (player.PlayerBlindDealState == Player.BlindDealState.big)
                    return player;
            }

            return null;
        }


        public int SmallBlind
        {
            get { return smallBlind; }
            set { smallBlind = value; }
        }

        public int BigBlind
        {
            get { return bigBlind; }
            set { bigBlind = value; }
        }

        public int RaiseValue
        {
            get { return raiseValue; }
            set { raiseValue = value; }
        }

        public void raiseBlind(int blindraise)
        {
            this.bigBlind = this.bigBlind + blindraise;
            this.smallBlind = this.smallBlind + (blindraise / 2);
        }


        public int getSmallPlayerPosition()
        {
            int currentSmall=0;
            foreach (Player playerStateCheck in this.player)
            {
                if (playerStateCheck.PlayerBlindDealState != Player.BlindDealState.small)
                {
                    currentSmall++;
                }
                else {
                    break;
                }
            }

            return currentSmall;
        }

        public int getBigPlayerPosition()
        {
            int currentSmall = 0;
            foreach (Player playerStateCheck in this.player)
            {
                if (playerStateCheck.PlayerBlindDealState != Player.BlindDealState.big)
                {
                    currentSmall++;
                }
                else
                {
                    break;
                }
            }

            return currentSmall;
        }

        public int getAfterBigPlayerPosition()
        {
            int currentSmall = 0;
            foreach (Player playerStateCheck in this.player)
            {
                if (playerStateCheck.PlayerBlindDealState != Player.BlindDealState.big)
                {
                    currentSmall++;
                }
                else
                {
                    break;
                }
            }


            if (currentSmall-1 > this.player.Count())
                currentSmall = 0;

            return currentSmall;
        }


        public void clearRaises() {

            raiseValue = 0;
        }

    }
}
