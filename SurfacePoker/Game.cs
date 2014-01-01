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
            Player nextPlayer = new Player(activePlayer);
            try
            {
                activePlayer = whoIsNext();

            }
            catch (NoPlayerInGameException exp)
            {
                throw exp;
            }
            List<Action> actions = new List<Action>();
            actions.Add(new Action(Action.playerAction.fold, 0));
            if (nextPlayer.inPot == pot.amountPerPlayer)
            {
                actions.Add(new Action(Action.playerAction.check, 0));
            }
            else
            {
                actions.Add(new Action(Action.playerAction.call, (pot.amountPerPlayer - nextPlayer.inPot)));
            }
            if (pot.amountPerPlayer == 0)
            {
                actions.Add(new Action(Action.playerAction.bet, (bigBlind)));
            }
            else
            {
                actions.Add(new Action(Action.playerAction.raise, (pot.raiseSize)));
            }
            return new KeyValuePair<Player, List<Action>>(nextPlayer, actions);
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
        
    }
    [TestClass]
    public class TestGame
    {
        [TestMethod]
        public void TestNextPlayer()
        {
            int bb = 10;
            int sb = 5;
            int PLAYERCOUNT = 6;


            List<Player> players = new List<Player>();
            for (int i = 0; i < PLAYERCOUNT; i++)
            {
                Player player = new Player("Player" + i, 1000);
                player.position = i + 1;
                player.ingamePosition = i;

                players.Add(player);
            }
            Game gl = new Game(players, bb, sb);
            gl.newGame();
            
            for(int j = 0; j < PLAYERCOUNT; j++){
                players[j].isActive = false;
                for (int i = j; i < PLAYERCOUNT; i++)
                {
                    KeyValuePair<Player, List<Action>> actPlayer = new KeyValuePair<Player,List<Action>>(new Player("Fail", 0), new List<Action>());
                    try
                    {
                        actPlayer = gl.nextPlayer();
                        Assert.AreEqual(players[i].name, actPlayer.Key.name);
                    }
                    catch (AssertFailedException e)
                    {
                        Console.WriteLine("Failed: " + players[i].name + " " + actPlayer.Key.name);
                    }
                    catch (NoPlayerInGameException e)
                    {
                        Console.WriteLine("Last Player " + players[i].name);
                    }
                }
            }
        }
    }
}
