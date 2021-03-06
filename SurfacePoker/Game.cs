﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Log;
using log4net;
using log4net.Config;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SurfacePoker
{

    public class Game
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<Player> players { get; set; }
        public Replay rep { get; set; }
        public int smallBlind { get; set; }
        public int bigBlind { get; set; }
        public Pot pot { get; set; }
        public Deck deck { get; set; }
        private Player activePlayer;
        private Player nextActivePlayer;
        private Player dealer = new Player("dealer", 0, 0);
        private int nonActives;
        private bool boolCancel;
        public int blindLevel { get; set; }
        /// <summary>
        ///round = 0 => Preflop
        ///round = 1 => Flop
        ///round = 2 => Turn
        ///round = 3 => River
        /// </summary>
        private int round;
        public List<Card> board { get; set; }
        private bool TrainMode;
        public bool trainMode { get{
                return TrainMode;
            }
            set{
                Logger.newGame(value);
                TrainMode = value;
            } }
        public int[] blindStructur = new int[] { 20, 40, 60, 100, 160, 200, 300, 400, 500 };
        public Game(List<Player> players, int bb, int sb, Boolean train)
	    {
            log.Debug("Game() - Begin");
            log.Debug("New Game - Player: " + players.Count + " Stakes: " + sb + "/" + bb);
            trainMode = train;
            boolCancel = false;
            blindLevel = 0;
            foreach (Player p in players)
            {
                log.Info("Name: " + p.name + "; Position: " + p.position + "; Stack: " + p.stack);
            }
            this.players = players;
            this.smallBlind = sb;
            this.bigBlind = bb;
            pot = new Pot();
            round = 0;
            players.Sort((x, y) => x.position.CompareTo(y.position));
            for (int i = 2; i < players.Count + 2; i++)
            {
                players.Find(x => (x.position > (i - 2)) && (x.ingamePosition == -1)).ingamePosition = i;
            }
            log.Debug("Game() - End");
	    }

        private String boardToString()
        {
            log.Debug("boardToString() - Begin");
            String str = "";
            foreach(Card c in board){
                str += c.ToString() + " ";
            }
            log.Debug("boardToString() - End");
            return str;
        }
        /// <summary>
        /// starts a new game and set back all relevant attributes
        /// </summary>
        public Player newGame()
        {
            
            log.Debug("new Game() - Begin");
            bigBlind = blindStructur[blindLevel];
            smallBlind = blindStructur[blindLevel] / 2;
            log.Debug("Blindlevel: " + smallBlind + "/" + bigBlind);
            deck = new Deck();
            round = 0;
            board = new List<Card>();
            pot.amountPerPlayer = bigBlind;
            nonActives = players.FindAll(x =>  (x.stack == 0)).Count;
            int j = 0;
            players.Sort((x, y) => x.ingamePosition.CompareTo(y.ingamePosition));
            bool firstplayer = !boolCancel;
            if (players.Count - nonActives == 1)
            {
                log.Debug("EndGameException");
                throw new EndGameException("End of Game - Only one player left");
            }
            Logger.action(this, dealer, Action.playerAction.newgame, 0, board);
            for (int k = 0; k < players.Count; k++)
            {
                if (players[k].stack > 0)
                {

                    players[k].isActive = true;
                    players[k].isAllin = false;
                    players[k].hasChecked = false;
                    Logger.action(this, players[k], Action.playerAction.ingame, 0, board);
                }
                else
                {
                    players[k].isActive = false;
                    players[k].isAllin = false;
                    
                }
                players[k].totalInPot = 0;
            }
            for (int i = 0; i < players.Count; i++)
            {
                players[i].inPot = 0;
                players[i].totalInPot = 0;
                if (players[i].stack > 0)
                {
                    if (firstplayer)
                    {
                        players[i].ingamePosition = players.Count - nonActives;
                        firstplayer = false;
                        Logger.action(this, players[i], Action.playerAction.bigblind, bigBlind, board);
                        if(players[i].stack >= bigBlind){
                            pot.raisePot(players[i], bigBlind);
                            players[i].action(bigBlind);
                        }
                        else
                        {
                            pot.raisePot(players[i], smallBlind);
                            players[i].action(smallBlind);
                        }
                    }
                    else
                    {
                        if (!boolCancel)
                        {
                            players[i].ingamePosition = i - j;
                            if (players[i].ingamePosition == players.Count - nonActives - 1)
                            {
                                Logger.action(this, players[i], Action.playerAction.smallblind, smallBlind, board);
                                pot.amountPerPlayer = smallBlind;
                                pot.raisePot(players[i], smallBlind);
                                players[i].action(smallBlind);
                            }
                        }
                        else
                        {
                            if (players[i].ingamePosition == players.Count - nonActives - 1)
                            {
                                Logger.action(this, players[i], Action.playerAction.smallblind, smallBlind, board);
                                pot.amountPerPlayer = smallBlind;
                                pot.raisePot(players[i], smallBlind);
                                players[i].action(smallBlind);
                            }
                            if (players[i].ingamePosition == players.Count - nonActives)
                            {
                                Logger.action(this, players[i], Action.playerAction.bigblind, bigBlind, board);
                                pot.amountPerPlayer = bigBlind;
                                pot.raisePot(players[i], bigBlind);
                                players[i].action(bigBlind);
                                boolCancel = false;
                            }
                        }

                    }
                    players[i].cards = new List<Card>();
                    players[i].setOneCard(deck.DealNext());
                    players[i].setOneCard(deck.DealNext());
                    
                }
                else
                {
                    players[i].cards = new List<Card>();
                    j++;
                    players[i].ingamePosition = 0;
                }
            }
            if (pot.sidePot != null)
            {
                if (pot.sidePot.amountPerPlayer < bigBlind)
                {
                    pot.amountPerPlayer = smallBlind;
                }
            }
            else
            {
                pot.amountPerPlayer = bigBlind;
            }
            pot.raiseSize = bigBlind;

            Logger.calculateWinChance(this);

            foreach (Player p in players)
            {
                Logger.action(this, p, Action.playerAction.nothing, 0, new List<Card>());
                log.Debug("Name: " + p.name + ", Position: " + p.ingamePosition + ", Stack: " + p.stack);
            }
            activePlayer = null;
            nextActivePlayer = null;
            if ((players.Count - nonActives) >= 3)
            {
                log.Debug("new Game() - End");
                return players.Find(x => x.ingamePosition == (players.Count - nonActives - 2));
            }
            else
            {
                log.Debug("new Game() - End");
                return players.Find(x => x.ingamePosition == (players.Count - nonActives));
            }
        }
        /// <summary>
        /// determines which possibilities the active player has
        /// </summary>
        /// <returns>KeyValuePair: key - active Player; value - List of possibile actions</returns>
        public KeyValuePair<Player, List<Action>> nextPlayer()
        {
            log.Debug("nextPlayer() - Begin");
            if ((activePlayer == null) && (nextActivePlayer == null))
            {
                activePlayer = players.Find(x => x.ingamePosition == 1);
                nextActivePlayer = players.Find(x => x.ingamePosition == 2);
            }
            else
            {
                activePlayer = nextActivePlayer;
                try
                {

                    nextActivePlayer = whoIsNext();
                }
                catch (NoPlayerInGameException exp)
                {
                    log.Debug("NoPlayerInGameException");
                    throw exp;
                }
                catch (EndRoundException exp)
                {
                    log.Debug("EndRoundException");
                    throw exp;
                }

            }
            log.Debug("nextPlayer() - End");
            return getActions();
        }

        private KeyValuePair<Player, List<Action>> getActions()
        {
            log.Debug("getActions() - Begin");
            List<Action> actions = new List<Action>();
            actions.Add(new Action(Action.playerAction.fold, 0));
            if (activePlayer.inPot == pot.amountPerPlayer)
            {
                actions.Add(new Action(Action.playerAction.check, 0));
            }
            else
            {
                if (pot.amountPerPlayer - activePlayer.inPot <= activePlayer.stack)
                {
                    actions.Add(new Action(Action.playerAction.call, (pot.amountPerPlayer - activePlayer.inPot)));
                }
                else
                {
                    actions.Add(new Action(Action.playerAction.call, (activePlayer.stack)));
                }
            }
            if (pot.amountPerPlayer == 0)
            {
                if (bigBlind < activePlayer.stack)
                {
                    actions.Add(new Action(Action.playerAction.bet, (bigBlind)));
                }
                else
                {
                    actions.Add(new Action(Action.playerAction.bet, (activePlayer.stack)));
                }
            }
            else
            {
                Action call = actions.Find(x => x.action == Action.playerAction.call);
                if ((pot.raiseSize + call.amount) < activePlayer.stack)
                {
                    actions.Add(new Action(Action.playerAction.raise, (pot.raiseSize) + call.amount));
                }
                else
                {
                    actions.Add(new Action(Action.playerAction.raise, (activePlayer.stack)));
                }
            }

            log.Debug("getActions() - End");
            return new KeyValuePair<Player, List<Action>>(activePlayer, actions);
        }
        /// <summary>
        /// determines who is the next player after the active Player
        /// </summary>
        /// <returns>next Player</returns>
        private Player whoIsNext()
        {
            log.Debug("whoIsNext() - Begin");
            if (!players.Exists(x => x.isActive & (x.name != activePlayer.name)))
            {
                if (players.Exists(x => x.isAllin))
                {
                    if ((activePlayer.inPot == pot.amountPerPlayer) || (activePlayer.isAllin))
                    {
                        log.Debug("EndRoundException");
                        throw new EndRoundException("Finished Round");
                    }
                    else
                    {
                        List<Player> nextPlayers = players.FindAll(x => x.isAllin);
                        nextPlayers.Sort((x, y) => x.ingamePosition.CompareTo(y.ingamePosition));
                        log.Debug("whoIsNext() - End");
                        return nextPlayers.Find(x => x.isAllin);
                    }
                }
                else
                {

                    log.Debug("NoPlayerInGameException");
                    throw new NoPlayerInGameException("No Next Player");
                }
            }
            if (players.FindAll(x => (x.ingamePosition > activePlayer.ingamePosition)).Exists(x => x.isActive))
            {
                List<Player> nextPlayers = players.FindAll(x => (x.ingamePosition > activePlayer.ingamePosition));
                nextPlayers.Sort((x, y) => x.ingamePosition.CompareTo(y.ingamePosition));
                Player player = nextPlayers.Find(x => x.isActive);
                if (activePlayer.inPot == pot.amountPerPlayer)
                {
                    if (! activePlayer.hasChecked && ! activePlayer.isAllin)
                    {
                        log.Debug("whoIsNext() - End");
                        return player;
                    } else {
                        log.Debug("EndRoundException");
                        throw new EndRoundException("Finished Round");
                    }
                }
                else
                {
                    if (activePlayer.isAllin)
                    {
                        log.Debug("EndRoundException");
                        throw new EndRoundException("Finished Round");
                    }
                    else
                    {
                        log.Debug("whoIsNext() - End");
                        return player;

                    }
                }
            }
            else
            {
                if (players.FindAll(x => (x.ingamePosition >= 1)).Exists(x => x.isActive))
                {
                    List<Player> nextPlayers = players.FindAll(x => (x.ingamePosition >= 1));
                    nextPlayers.Sort((x, y) => x.ingamePosition.CompareTo(y.ingamePosition));
                    Player player = nextPlayers.Find(x => x.isActive);

                    if (activePlayer.inPot == pot.amountPerPlayer)
                    {
                        if (!activePlayer.hasChecked && !activePlayer.isAllin)
                        {
                            log.Debug("whoIsNext() - End");
                            return player;
                        }
                        else
                        {
                            log.Debug("EndRoundException");
                            throw new EndRoundException("Finished Round");
                        }
                    }
                    else
                    {
                        if (activePlayer.isAllin)
                        {
                            log.Debug("EndRoundException");
                            throw new EndRoundException("Finished Round");
                        }
                        else
                        {
                            log.Debug("whoIsNext() - End");
                            return player;

                        }
                    }
                }
                else
                {
                    log.Debug("NoPlayerInGameException");
                    throw new NoPlayerInGameException("No Next Player");
                }
            }
        }
        /// <summary>
        /// determines who is the next player
        /// </summary>
        /// <param name="i">i determines fron which position the function has to search</param>
        /// <returns>next Player</returns>
        private Player whoIsNext(int i, bool b)
        {
            log.Debug("whoIsNext(int "+ i+") - Begin");
            if (!players.Exists(x => x.isActive & (x.name != activePlayer.name)) && b)
            {
                if (players.Exists(x => x.isAllin))
                {
                    if (activePlayer.inPot == pot.amountPerPlayer)
                    {
                        List<Player> nextPlayers = players.FindAll(x => x.isAllin);
                        nextPlayers.Sort((x, y) => x.ingamePosition.CompareTo(y.ingamePosition));
                        log.Debug("whoIsNext() - End");
                        return nextPlayers.Find(x => x.isActive);
                    }
                    else
                    {
                        log.Debug("EndRoundException");
                        throw new EndRoundException("Finished Round");
                    }
                    
                }
                else
                {

                    log.Debug("NoPlayerInGameException");
                    throw new NoPlayerInGameException("No Next Player");
                }
            }
            if (players.FindAll(x => (x.ingamePosition >= i)).Exists(x => x.isActive))
            {
                List<Player> nextPlayers = players.FindAll(x => (x.ingamePosition >= i));
                nextPlayers.Sort((x, y) => x.ingamePosition.CompareTo(y.ingamePosition));
                Player player = nextPlayers.Find(x => x.isActive);
                if (activePlayer.inPot == pot.amountPerPlayer)
                {
                    log.Debug("whoIsNext() - End");
                    return player;
                    
                }
                else
                {
                    log.Debug("whoIsNext() - End");
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

                    if (activePlayer.inPot == pot.amountPerPlayer)
                    {
                        log.Debug("whoIsNext() - End");
                        return player;
                        
                    }
                    else
                    {
                        log.Debug("whoIsNext() - End");
                        return player;
                    }
                }
                else
                {
                    log.Debug("NoPlayerInGameException");
                    throw new NoPlayerInGameException("No Next Player");
                }
            }
            
        }

        public void cancel()
        {
            log.Debug("cancel() - Begin");
            foreach (Player p in players)
            {
                p.stack += p.inPot;
                p.stack += p.totalInPot;
            }

            pot.amountPerPlayer = 0;
            pot.potThisRound = 0;
            pot.endOfRound();
            pot.value = 0;
            boolCancel = true;
            log.Debug("cancel() - End");
        }

        /// <summary>
        /// the active player folds, checks, calls, bets, raises with this function
        /// </summary>
        /// <param name="pa"> the action which the player can do</param>
        /// <param name="amount">absolut amount(fold - 0; check - 0)</param>
        public void activeAction(Action.playerAction pa, int amount)
        {
            log.Debug("activeAction(Action.playerAction "+ pa.ToString() +",int " + amount + ") - Begin");
            switch (pa)
            {
                case Action.playerAction.fold:
                    activePlayer.isActive = false;
                    activePlayer.totalInPot += activePlayer.inPot;
                    activePlayer.inPot = 0;
                    activePlayer.cards = new List<Card>();
                    break;

                case Action.playerAction.check:

                    break;
                case Action.playerAction.call:
                    pot.raisePot(activePlayer, amount);
                    activePlayer.action(amount);
                    break;
                case Action.playerAction.bet:
                    pot.amountPerPlayer = amount + activePlayer.inPot;
                    pot.raisePot(activePlayer, amount);
                    activePlayer.action(amount);
                    pot.raiseSize = amount;
                    break;
                case Action.playerAction.raise:
                    pot.raiseSize = amount - pot.amountPerPlayer + activePlayer.inPot;
                    if (pot.sidePot == null)
                    {
                        pot.amountPerPlayer = amount + activePlayer.inPot;
                    }
                    else
                    {
                        pot.amountPerPlayer = amount + activePlayer.inPot - pot.sidePot.amountPerPlayer;
                    }
                    pot.raisePot(activePlayer,  amount);
                    activePlayer.action(amount);
                    break;
            }
            Logger.action(this, activePlayer, pa, amount, this.board);
            activePlayer.hasChecked = true;
            log.Debug("activeAction() - End");
        }


        /// <summary>
        /// initialize the next round
        /// </summary>
        public KeyValuePair<Player, List<Action>> nextRound()
        {
            log.Debug("nextRound() - Begin");
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
            Logger.action(this, dealer, Action.playerAction.nothing, 0, board);
            Logger.calculateWinChance(this);
            foreach (Player player in players)
            {
                player.hasChecked = false;
                player.totalInPot += player.inPot;
                player.inPot = 0;
            }
            pot.amountPerPlayer = 0;
            pot.raiseSize = 0;
            //active Player after DealerButton
            //int nonActives = players.FindAll(x => (x.isAllin) | (!x.isActive)).Count;
            activePlayer = whoIsNext(players.Count - nonActives - 1, true);
            nextActivePlayer = whoIsNext(activePlayer.ingamePosition + 1, true);
            pot.endOfRound();
            log.Debug("nextRound() - End");
            return getActions();
        }

        /// <summary>
        /// determines the winning players and shows how much they won
        /// </summary>
        /// <returns></returns>
        public List<Winner> whoIsWinner(Pot pot, List<Player> playersInGame)
        {
            log.Debug("whoIsWinner() - Begin");
            List<Winner> result = new List<Winner>();
            //List<Player> playersInGame = players.FindAll(x => x.isActive);
            playersInGame.AddRange(pot.player);
            List<KeyValuePair<Player, Hand>> playerHand = new List<KeyValuePair<Player, Hand>>();
            Console.WriteLine(boardToString());
            if (playersInGame.Count > 0)
            {

                //determines Hand Value
                foreach (Player player in playersInGame)
                {
                    playerHand.Add(new KeyValuePair<Player, Hand>(player, new Hand(player.getCardsToString(), boardToString())));
                    Console.WriteLine(player.name + " " + player.getCardsToString() + " " + new Hand(player.getCardsToString(), boardToString()).HandValue + " " + new Hand(player.getCardsToString(), boardToString()).HandTypeValue);
                }
                playerHand.Sort((x, y ) => x.Value.HandValue.CompareTo(y.Value.HandValue));
                //WinnerHands
                result.Add(new Winner(playerHand[playerHand.Count - 1].Key, playerHand[playerHand.Count - 1].Value.HandTypeValue.ToString()));
                for (int i = playerHand.Count - 2; i >= 0; i--)
                {
                    if (playerHand[playerHand.Count - 1].Value.HandValue == playerHand[i].Value.HandValue)
                    {
                        result.Add(new Winner(playerHand[i].Key, playerHand[i].Value.HandTypeValue.ToString()));

                    }
                    else
                    {
                        playerHand[i].Key.isActive = false;
                    }
                }
                if (result.Count == 1)
                {
                    result[0].value = pot.value;
                    result[0].player.stack += result[0].value;
                    result[0].player.isAllin = false;
                }
                else
                {
                    int mod = (pot.value / result.Count) % bigBlind;
                    Player first = new Player(activePlayer);
                    try
                    {
                        // nonActives = players.FindAll(x => (!x.isActive)).Count;
                        first = whoIsNext(players.Count - nonActives - 1, false);
                    }
                    catch (NoPlayerInGameException e)
                    {
                        first = activePlayer;
                    }

                    while (mod > 0)
                    {
                        Winner myPlayer = result.Find(x => x.player.name == first.name);
                        myPlayer.value += smallBlind;
                        pot.value -= smallBlind;
                        mod = (pot.value / result.Count) % bigBlind;
                    }

                    for (int j = 0; j < result.Count; j++)
                    {
                        result[j].value += (pot.value / result.Count);
                        result[j].player.stack += result[j].value;
                        if (result[j].player.stack > 0)
                        {

                            result[j].player.isAllin = false;
                        }
                    }
                }
            
                foreach(Winner w in result){
                    Logger.action(this, w.player, Action.playerAction.wins, w.value, board);
                }
            }
            if (pot.sidePot != null)
            {
                result.AddRange(whoIsWinner(pot.sidePot, playersInGame));
                pot.sidePot = null;
            }
            pot.value = 0;
            pot.potThisRound = 0;
            pot.player = new List<Player>();
            log.Debug("whoIsWinner() - End");
            return result;
        }

        public List<String> getReplay(Replay.action act, String str)
        {
            List<String> result = new List<string>();
            switch(act){
                case Replay.action.game:
                    rep = new Replay(str);
                    result = rep.getGames();
                    break;
                case Replay.action.hand:
                    result = rep.getHands(str);
                    
                    break;
            }
            return result;
        }
    }

    
    #region Testclasses
    [TestClass]
    public class TestRep
    {
        public int bb { get; set; }
        public int sb { get; set; }
        public int PLAYERCOUNT { get; set; }
        public List<Player> players { get; set; }
        public Game gl { get; set; }

        public TestRep()
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
            gl = new Game(players, bb, sb, true);
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

                    List<Winner> winners = gl.whoIsWinner(gl.pot,gl.players.FindAll(x => x.isActive));
                    //give the earnings to the winner
                    Console.WriteLine("---------------------------------------");
                    foreach (Winner kvp in winners)
                    {
                        Console.WriteLine(kvp.player.name + " won " + kvp.value);
                        players.Find(x => x.name == kvp.player.name).stack += kvp.value;
                        players.Find(x => x.name == kvp.player.name).isAllin = false;
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
    #endregion
