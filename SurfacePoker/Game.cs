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
                if (player.position == players.Count - 1)
                {
                    pot.raisePot(player.action(bigBlind));
                    pot.amountPerPlayer = bigBlind;
                    pot.raiseSize = bigBlind;
                }
                if (player.position == players.Count - 2)
                {
                    pot.raisePot(player.action(smallBlind));
                }
                player.setOneCard(deck.DealNext());
                player.setOneCard(deck.DealNext());
                if (player.ingamePosition < players.Count)
                {
                    player.ingamePosition++;
                }
                else
                {
                    player.ingamePosition = 1;
                    activePlayer = player;
                }
            }
           

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
            if (players.Exists(x => (x.isActive) && (x.ingamePosition != activePlayer.ingamePosition)))
            {
                return players.Find(x => (x.isActive) && (x.ingamePosition != activePlayer.ingamePosition));
            }
            else
            {
                if (players.Exists(x => (x.isActive) && (x.ingamePosition >= 1)))
                {
                    return players.Find(x => (x.isActive) && (x.ingamePosition >= 1));
                }
                else
                {
                    throw new NoPlayerInGameException("No Next Player");
                }
            }

        }
        [TestMethod]
        private void TestGameLoop()
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
            gl.nextPlayer();

        }
    }
}
