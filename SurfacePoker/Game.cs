using System;
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
        public int smallBlind { get; set; }
        public int bigBlind { get; set; }
        public Pot pot { get; set; }
        public Deck deck { get; set; }
        private Player activePlayer;
        private Player nextActivePlayer;
        private Player dealer = new Player("dealer", 0, 0);

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
            log.Debug("Game() - Begin");
            log.Debug("New Game - Player: " + players.Count + " Stakes: " + sb + "/" + bb);
            Logger.newGame();
            foreach (Player p in players)
            {
                log.Debug("Name: " + p.name + ", Position: " + p.position + ", Stack: " + p.stack);
            }
            this.players = players;
            this.smallBlind = sb;
            this.bigBlind = bb;
            pot = new Pot();
            round = 0;
            players.Sort((x, y) => x.position.CompareTo(y.position));
            for (int i = 0; i < players.Count; i++)
            {
                players.Find(x => (x.position > i) && (x.ingamePosition == -1)).ingamePosition = i;
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
        public KeyValuePair<Player, List<Action>> newGame()
        {
            
            log.Debug("new Game() - Begin");
            deck = new Deck();
            round = 0;
            board = new List<Card>();
            pot.amountPerPlayer = bigBlind;
            int nonActives = players.FindAll(x =>  (x.stack == 0)).Count;
            foreach (Player player in players)
            {
                if (player.stack > 0)
                {
                    player.isActive = true;
                    player.isAllin = false;
                }
                else
                {
                    player.isActive = false;
                    player.isAllin = false;
                }
                
                player.inPot = 0;
                if (player.ingamePosition < (players.Count - nonActives))
                {
                    if (player.isActive)
                    {
                        player.ingamePosition++;
                    }
                    else
                    {
                        player.ingamePosition = 0;
                    }
                }
                else
                {
                    if (player.isActive)
                    {
                        player.ingamePosition = 1;
                    }
                    else
                    {
                        player.ingamePosition = 0;
                    }
                }

                if (player.ingamePosition == (players.Count - nonActives))
                {
                    Logger.action(player, Action.playerAction.bigblind, bigBlind, board);
                    pot.raisePot(player, player.action(bigBlind));
                }
                if (player.ingamePosition == players.Count - nonActives - 1)
                {
                    Logger.action(player, Action.playerAction.smallblind, smallBlind, board);
                    pot.amountPerPlayer = smallBlind;
                    pot.raisePot(player, player.action(smallBlind));
                }
                pot.amountPerPlayer = bigBlind;
                pot.raiseSize = bigBlind;
                player.cards = new List<Card>();
                player.setOneCard(deck.DealNext());
                player.setOneCard(deck.DealNext());
                player.isAllin = false;
                player.hasChecked = false;
            }
            activePlayer = players.Find(x => x.ingamePosition == 1);
            nextActivePlayer = players.Find(x => x.ingamePosition == 2);

            foreach (Player p in players)
            {
                Logger.action(p, Action.playerAction.nothing, 0, new List<Card>());
                log.Debug("Name: " + p.name + ", Position: " + p.ingamePosition + ", Stack: " + p.stack);
            }
            log.Debug("new Game() - End");
            return getActions();
        }
        /// <summary>
        /// determines which possibilities the active player has
        /// </summary>
        /// <returns>KeyValuePair: key - active Player; value - List of possibile actions</returns>
        public KeyValuePair<Player, List<Action>> nextPlayer()
        {
            log.Debug("nextPlayer() - Begin");
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
                actions.Add(new Action(Action.playerAction.bet, (bigBlind)));
            }
            else
            {
                actions.Add(new Action(Action.playerAction.raise, (pot.raiseSize)));
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
                    log.Debug("EndRoundException");
                    throw new EndRoundException("Finished Round");
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
                    if (! activePlayer.hasChecked)
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
                        if (!activePlayer.hasChecked)
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
        /// <summary>
        /// determines who is the next player
        /// </summary>
        /// <param name="i">i determines fron which position the function has to search</param>
        /// <returns>next Player</returns>
        private Player whoIsNext(int i)
        {
            log.Debug("whoIsNext(int "+ i+") - Begin");
            if (!players.Exists(x => x.isActive & (x.name != activePlayer.name)))
            {
                log.Debug("NoPlayerInGameException");
                throw new NoPlayerInGameException("No Next Player");
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

        /// <summary>
        /// the active player folds, checks, calls, bets, raises with this function
        /// </summary>
        /// <param name="pa"> the action which the player can do</param>
        /// <param name="amount">absolut amount(fold - 0; check - 0)</param>
        public void activeAction(Action.playerAction pa, int amount)
        {
            log.Debug("activeAction(Action.playerAction "+ pa.ToString() +",int " + amount + ") - End");
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
                    pot.amountPerPlayer = amount + activePlayer.inPot;
                    pot.raisePot(activePlayer,  activePlayer.action(amount));
                    pot.raiseSize = amount;
                    break;
                case Action.playerAction.raise:
                    pot.raiseSize = amount - pot.amountPerPlayer + activePlayer.inPot;
                    pot.amountPerPlayer = amount + activePlayer.inPot;
                    pot.raisePot(activePlayer,  activePlayer.action(amount));
                    break;
            }
            Logger.action(activePlayer, pa, amount, this.board);
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
            Logger.action(dealer, Action.playerAction.nothing, 0, board);
            foreach (Player player in players)
            {
                player.hasChecked = false;
                player.inPot = 0;
            }
            pot.amountPerPlayer = 0;
            pot.raiseSize = 0;
            //active Player after DealerButton
            int nonActives = players.FindAll(x => (x.isAllin) | (!x.isActive)).Count;
            activePlayer = whoIsNext(players.Count - nonActives - 1);
            nextActivePlayer = whoIsNext(activePlayer.ingamePosition + 1);
            pot.endOfRound();
            log.Debug("nextRound() - End");
            return getActions();
        }

        /// <summary>
        /// determines the winning players and shows how much they won
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<Player, int>> whoIsWinner(Pot pot)
        {
            log.Debug("whoIsWinner() - Begin");
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
            Player first = new Player(activePlayer);
            try {
                int nonActives = players.FindAll(x => (!x.isActive)).Count;
                first = whoIsNext(players.Count- nonActives - 1);
            }
            catch (NoPlayerInGameException e)
            {
                first = activePlayer;
            }
            
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
                result[j].Key.stack += result[j].Value;
                result[j].Key.isAllin = false;
            }
            foreach(KeyValuePair<Player, int> kvp in result){
                Logger.action(kvp.Key, Action.playerAction.wins, kvp.Value, board);
            }
            if (pot.sidePot != null)
            {
                result.AddRange(whoIsWinner(pot.sidePot));
                pot.sidePot = null;
            }
            pot.value = 0;
            pot.potThisRound = 0;
            log.Debug("whoIsWinner() - End");
            return result;
        }
    }

    //TODO: allin
    //TODO: side pot
    #region Testclasses
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
    #endregion
