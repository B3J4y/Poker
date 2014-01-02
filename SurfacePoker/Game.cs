using System;
using System.Collections.Generic;
using log4net;
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

	    public Game(List<Player> players, int bb, int sb)
	    {
            this.players = players;
            this.smallBlind = sb;
            this.bigBlind = bb;
            pot = new Pot();
	    }

        public void newGame()
        {
            deck = new Deck();

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
                if (player.position == players.Count)
                {
                    pot.raisePot(player.action(bigBlind));
                    pot.amountPerPlayer = bigBlind;
                    pot.raiseSize = bigBlind;
                }
                if (player.position == players.Count - 1)
                {
                    pot.raisePot(player.action(smallBlind));
                }
                player.setOneCard(deck.DealNext());
                player.setOneCard(deck.DealNext());
                
            }
            activePlayer = players.Find(x => x.ingamePosition == 1);

        }

        public KeyValuePair<Player, List<Action>> nextPlayer()
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

        private Player whoIsNext()
        {
            if (players.FindAll(x => (x.ingamePosition > activePlayer.ingamePosition)).Exists(x => x.isActive))
            {
                
                return players.FindAll(x => (x.ingamePosition > activePlayer.ingamePosition)).Find(x => x.isActive);
            }
            else
            {
                if (players.FindAll(x => (x.ingamePosition >= 1)).Exists(x => x.isActive))
                {
                    return players.FindAll(x => (x.ingamePosition >= 1)).Find(x => x.isActive);
                }
                else
                {
                    throw new NoPlayerInGameException("No Next Player");
                }
            }

        }

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
                    pot.raisePot(activePlayer.action(amount));
                    break;
                case Action.playerAction.bet:
                    pot.raisePot(activePlayer.action(amount));
                    pot.amountPerPlayer = amount;
                    pot.raiseSize = amount;
                    break;
                case Action.playerAction.raise:
                    pot.raisePot(activePlayer.action(amount));
                    pot.raiseSize = amount - pot.raiseSize;
                    pot.amountPerPlayer = amount;
                    break;
            }
            try
            {
                activePlayer = whoIsNext();

            }
            catch (NoPlayerInGameException exp)
            {
                throw exp;
            }
        }
        
    }
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
                Player player = new Player("Player" + i, 1000);
                player.position = i + 1;
                player.ingamePosition = i;

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
    }
}
