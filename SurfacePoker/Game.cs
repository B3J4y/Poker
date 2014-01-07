﻿using System;
using System.Collections.Generic;
using log4net;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SurfacePoker
{

    public class Game
    {
        public List<Player> players { get; set; }
        public int smallBlind { get; set; }
        public int bigBlind { get; set; }
        public Pot pot { get; set; }
        public Deck deck { get; set; }
        private Player activePlayer;
        private Player nextActivePlayer;

        /// <summary>
        ///round = 0 => Preflop
        ///round = 1 => Flop
        ///round = 2 => Turn
        ///round = 3 => River
        /// </summary>
        private int round;
        public List<Card> board { get; set; }
        //private ILog logger;
	    public Game(List<Player> players, int bb, int sb)
	    {
            //logger = LogManager.GetLogger(typeof(Game));
            //logger.Info("Construction Game");
            this.players = players;
            this.smallBlind = sb;
            this.bigBlind = bb;
            pot = new Pot();
            round = 0;
            for (int i = 0; i < players.Count; i++)
            {
                players.Find(x => (x.position > i) && (x.ingamePosition == -1)).ingamePosition = i;
            }
            players.Sort((x, y) => x.position.CompareTo(y.position));
	    }

        private String boardToString()
        {
            String str = "";
            foreach(Card c in board){
                str += c.ToString() + " ";
            }
            return str;
        }
        /// <summary>
        /// starts a new game and set back all relevant attributes
        /// </summary>
        public KeyValuePair<Player, List<Action>> newGame()
        {
            deck = new Deck();
            round = 0;
            board = new List<Card>();
            pot.amountPerPlayer = bigBlind;
            foreach (Player player in players)
            {
                if (player.ingamePosition < players.Count)
                {
                    player.ingamePosition++;
                }
                else
                {
                    player.ingamePosition = 1;
                }
                if (player.ingamePosition == players.Count)
                {
                    
                    pot.raisePot(player, player.action(bigBlind));
                }
                if (player.ingamePosition == players.Count - 1)
                {
                    pot.amountPerPlayer = smallBlind;
                    pot.raisePot(player, player.action(smallBlind));
                }
                pot.amountPerPlayer = bigBlind;
                pot.raiseSize = bigBlind;
                player.setOneCard(deck.DealNext());
                player.setOneCard(deck.DealNext());
                player.isActive = !player.isAllin;
                player.hasChecked = false;
            }
            activePlayer = players.Find(x => x.ingamePosition == 1);
            nextActivePlayer = players.Find(x => x.ingamePosition == 2);
            return getActions();
        }
        /// <summary>
        /// determines which possibilities the active player has
        /// </summary>
        /// <returns>KeyValuePair: key - active Player; value - List of possibile actions</returns>
        public KeyValuePair<Player, List<Action>> nextPlayer()
        {
            activePlayer = nextActivePlayer;
            try
            {

                nextActivePlayer = whoIsNext();
            }
            catch (NoPlayerInGameException exp)
            {
                throw exp;
            }
            catch (EndRoundException exp)
            {
                throw exp;
            }

            return getActions();
        }

        private KeyValuePair<Player, List<Action>> getActions()
        {
            List<Action> actions = new List<Action>();
            actions.Add(new Action(Action.playerAction.fold, 0));
            if (activePlayer.inPot == pot.amountPerPlayer)
            {
                actions.Add(new Action(Action.playerAction.check, 0));
            }
            else
            {
                actions.Add(new Action(Action.playerAction.call, (pot.amountPerPlayer - activePlayer.inPot)));
            }
            if (pot.amountPerPlayer == 0)
            {
                actions.Add(new Action(Action.playerAction.bet, (bigBlind)));
            }
            else
            {
                actions.Add(new Action(Action.playerAction.raise, (pot.raiseSize)));
            }
            return new KeyValuePair<Player, List<Action>>(activePlayer, actions);
        }
        /// <summary>
        /// determines who is the next player after the active Player
        /// </summary>
        /// <returns>next Player</returns>
        private Player whoIsNext()
        {
            if (players.FindAll(x => (x.ingamePosition > activePlayer.ingamePosition)).Exists(x => x.isActive))
            {
                List<Player> nextPlayers = players.FindAll(x => (x.ingamePosition > activePlayer.ingamePosition));
                nextPlayers.Sort((x, y) => x.ingamePosition.CompareTo(y.ingamePosition));
                Player player = nextPlayers.Find(x => x.isActive);
                if (player.inPot == pot.amountPerPlayer)
                {
                    if (! activePlayer.hasChecked)
                    {
                        return player;
                    } else {

                        throw new EndRoundException("Finished Round");
                    }
                }
                else
                {
                    return player;
                }
            }
            else
            {
                if (players.FindAll(x => (x.ingamePosition >= 1)).Exists(x => x.isActive))
                {
                    List<Player> nextPlayers = players.FindAll(x => (x.ingamePosition >= 1));
                    nextPlayers.Sort((x, y) => x.ingamePosition.CompareTo(y.ingamePosition));
                    Player player = nextPlayers.Find(x => x.isActive);

                    if (player.inPot == pot.amountPerPlayer)
                    {
                        if (!activePlayer.hasChecked)
                        {
                            return player;
                        }
                        else
                        {

                            throw new EndRoundException("Finished Round");
                        }
                    }
                    else
                    {
                        return player;
                    }
                }
                else
                {
                    throw new NoPlayerInGameException("No Next Player");
                }
            }

        }
        /// <summary>
        /// determines who is the next player
        /// </summary>
        /// <param name="i">i determines fron which position the function has to search</param>
        /// <returns>next Player</returns>
        private Player whoIsNext(int i)
        {
            if (players.FindAll(x => (x.ingamePosition >= i)).Exists(x => x.isActive))
            {
                Player player = players.FindAll(x => (x.ingamePosition >= i)).Find(x => x.isActive);
                if (player.inPot == pot.amountPerPlayer)
                {
                    
                    return player;
                    
                }
                else
                {
                    return player;
                }
            }
            else
            {
                if (players.FindAll(x => (x.ingamePosition >= 1)).Exists(x => x.isActive))
                {
                    Player player = players.FindAll(x => (x.ingamePosition >= 1)).Find(x => x.isActive);

                    if (player.inPot == pot.amountPerPlayer)
                    {
                        return player;
                        
                    }
                    else
                    {
                        return player;
                    }
                }
                else
                {
                    throw new NoPlayerInGameException("No Next Player");
                }
            }

        }

        /// <summary>
        /// the active player folds, checks, calls, bets, raises with this function
        /// </summary>
        /// <param name="pa"> the action which the player can do</param>
        /// <param name="amount">absolut amount(fold - 0; check - 0)</param>
        public void activeAction(Action.playerAction pa, int amount)
        {
            switch (pa)
            {
                case Action.playerAction.fold:
                    activePlayer.isActive = false;
                    activePlayer.inPot = 0;
                    activePlayer.cards = new List<Card>();
                    break;

                case Action.playerAction.check:

                    break;
                case Action.playerAction.call:
                    pot.raisePot(activePlayer, activePlayer.action(amount));
                    break;
                case Action.playerAction.bet:
                    pot.raisePot(activePlayer,  activePlayer.action(amount));
                    pot.amountPerPlayer = amount;
                    pot.raiseSize = amount;
                    break;
                case Action.playerAction.raise:
                    pot.raisePot(activePlayer,  activePlayer.action(amount));
                    pot.raiseSize = amount - pot.raiseSize;
                    pot.amountPerPlayer = amount;
                    break;
            }
            activePlayer.hasChecked = true;
        }


        /// <summary>
        /// initialize the next round
        /// </summary>
        public KeyValuePair<Player, List<Action>> nextRound()
        {
            round++;
            switch (round) {
                case 1:
                    board.Add(deck.DealNext());
                    board.Add(deck.DealNext());
                    board.Add(deck.DealNext());
                    break;
                case 2:
                    board.Add(deck.DealNext());
                    break;
                case 3:
                    board.Add(deck.DealNext());
                    break;
                case 4:
                    break;
            }
            foreach (Player player in players)
            {
                player.hasChecked = false;
                player.inPot = 0;
            }
            pot.amountPerPlayer = 0;
            pot.raiseSize = 0;
            //active Player after DealerButton
            activePlayer = whoIsNext(players.Count - 1);
            nextActivePlayer = whoIsNext(players.Count);
            pot.endOfRound();

            return getActions();
        }

        /// <summary>
        /// determines the winning players and shows how much they won
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<Player, int>> whoIsWinner(Pot pot)
        {
            List<KeyValuePair<Player, int>> result = new List<KeyValuePair<Player,int>>();
            List<Player> playersInGame = players.FindAll(x => x.isActive);
            playersInGame.AddRange(pot.player);
            List<KeyValuePair<Player, Hand>> playerHand = new List<KeyValuePair<Player, Hand>>();
            Console.WriteLine(boardToString());
            foreach (Player player in playersInGame)
            {
                playerHand.Add(new KeyValuePair<Player, Hand>(player, new Hand(player.getCardsToString(), boardToString())));
                Console.WriteLine(player.name + " " + player.getCardsToString() + " " + new Hand(player.getCardsToString(), boardToString()).HandValue + " " + new Hand(player.getCardsToString(), boardToString()).HandTypeValue);
            }
            playerHand.Sort((x, y ) => x.Value.HandValue.CompareTo(y.Value.HandValue));
            result.Add(new KeyValuePair<Player, int>(playerHand[playerHand.Count - 1].Key, 0));
            for (int i = playerHand.Count - 2; i >= 0; i--)
            {
                if (playerHand[playerHand.Count - 1].Value.HandValue == playerHand[i].Value.HandValue)
                {
                    result.Add(new KeyValuePair<Player, int>(playerHand[i].Key, 0));

                }
                else
                {
                    playerHand[i].Key.isActive = false;
                }
            }
            int mod = (pot.value / result.Count) % bigBlind;
            Player first = whoIsNext(players.Count - 1);
            
            while (mod > 0)
            {
                KeyValuePair<Player, int> myPlayer = result.Find(x => x.Key.name == first.name);
                result.Remove(myPlayer);
                myPlayer = new KeyValuePair<Player, int>(myPlayer.Key, myPlayer.Value + smallBlind);
                result.Add(myPlayer);
                pot.value -= smallBlind;
                mod = (pot.value / result.Count) % bigBlind;
            }

            for (int j = 0; j < result.Count; j++ )
            {
                result[j] = new KeyValuePair<Player, int>(result[j].Key, result[j].Value + (pot.value / result.Count));
            }
            if (pot.sidePot != null)
            {
                result.AddRange(whoIsWinner(pot.sidePot));
                pot.sidePot = null;
            }
            foreach (Player p in playersInGame)
            {
                p.cards = new List<Card>();
            }
            pot.value = 0;
            pot.potThisRound = 0;
            return result;
        }
    }

    //TODO: allin
    //TODO: side pot
    
    [TestClass]
    public class TestGame
    {
        public int bb { get; set; }
        public int sb { get; set; }
        public int PLAYERCOUNT { get; set; }
        public List<Player> players { get; set; }
        public Game gl { get; set; }

        public TestGame()
        {
            bb = 10;
            sb = 5;
            PLAYERCOUNT = 6;

            
            NewPlayRound();
            
        }

        private void NewPlayRound()
        {
            players = new List<Player>();
            for (int i = 0; i < PLAYERCOUNT; i++)
            {
                Player player = new Player("Player" + i, 1000, i+1);
                players.Add(player);
            }
            gl = new Game(players, bb, sb);
        }

        [TestMethod]
        public void TestPlayRound()
        {
            NewPlayRound();
            gl.newGame();
            int test = bb + sb;
            KeyValuePair<Player, List<Action>> actPlayer = gl.nextPlayer();
            gl.activeAction(Action.playerAction.fold, 0);
            try
            {
                Assert.IsFalse(actPlayer.Key.isActive);
            }
            catch (AssertFailedException e)
            {
                Console.WriteLine(e.Message);
            }
            actPlayer = gl.nextPlayer();
            if (actPlayer.Value.Exists(x => x.action == Action.playerAction.call))
            {
                gl.activeAction(Action.playerAction.call, actPlayer.Value.Find(x => x.action == Action.playerAction.call).amount);
                test += bb;
            }
            else
            {
                Console.WriteLine("Failed: Kein Call vorhanden");
            }
            actPlayer = gl.nextPlayer();
            if (actPlayer.Value.Exists(x => x.action == Action.playerAction.raise))
            {
                gl.activeAction(Action.playerAction.raise, actPlayer.Value.Find(x => x.action == Action.playerAction.raise).amount * 3);
                test += 3 * bb;
                
            }
            else
            {
                Console.WriteLine("Failed: Kein Raise vorhanden!");
            }
            try
            {
                Assert.AreEqual((bb + sb + bb + bb * 3), gl.pot.value);


            }
            catch (AssertFailedException e)
            {
                Console.WriteLine("Failed: Potsize exp " + test + " real " + gl.pot.value);
            }
            
        }

        [TestMethod]
        public void TestPreFlopTurnRiver()
        {
            NewPlayRound();
            for (int i = 0; i < 10000; i++)
            {

                gl.newGame();

                //preflop
                try
                {
                    gl.activeAction(Action.playerAction.fold, 0);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.fold, 0);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.fold, 0);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.call, bb);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.call, bb);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.check, 0);
                    gl.nextPlayer();
                }
                catch (NoPlayerInGameException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (EndRoundException e)
                {

                    gl.nextRound();
                }
                //flop
                try
                {
                    gl.activeAction(Action.playerAction.check, 0);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.check, 0);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.check, 0);
                    gl.nextPlayer();
                }
                catch (NoPlayerInGameException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (EndRoundException e)
                {

                    gl.nextRound();
                }
                //turn
                try
                {
                    gl.activeAction(Action.playerAction.check, 0);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.check, 0);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.check, 0);
                    gl.nextPlayer();
                }
                catch (NoPlayerInGameException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (EndRoundException e)
                {

                    gl.nextRound();
                }

                //river
                try
                {
                    gl.activeAction(Action.playerAction.check, 0);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.check, 0);
                    gl.nextPlayer();
                    gl.activeAction(Action.playerAction.check, 0);
                    gl.nextPlayer();
                }
                catch (NoPlayerInGameException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (EndRoundException e)
                {

                    List<KeyValuePair<Player, int>> winners = gl.whoIsWinner(gl.pot);
                    //give the earnings to the winner
                    Console.WriteLine("---------------------------------------");
                    foreach (KeyValuePair<Player, int> kvp in winners)
                    {
                        Console.WriteLine(kvp.Key.name + " won " + kvp.Value);
                        players.Find(x => x.name == kvp.Key.name).stack += kvp.Value;
                        players.Find(x => x.name == kvp.Key.name).isAllin = false;
                    }
                    Console.WriteLine("---------------------------------------");

                    
                }
                int stuck = 0;
                foreach (Player player in players)
                {
                    Console.WriteLine(player.name + " Stack: " + player.stack);
                    stuck += player.stack;
                }
                Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::");
                Console.WriteLine("Stacks gesamt: " + stuck);
                Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::");
            }
        }
    }
}
