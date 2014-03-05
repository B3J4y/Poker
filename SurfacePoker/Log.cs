using SurfacePoker;
using System;
using System.Collections.Generic;
using log4net;
using log4net.Config;


namespace Log
{
    
    public class Logger 
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<KeyValuePair<string, LogPlayer>> players { get; set; }
	    public Logger()
	    {
            players = new List<KeyValuePair<string, LogPlayer>>();
	    }

        public static void newGame(bool train)
        {
            if (train)
            {
                log.Info("Training;On;;;;;");
                log.Info("Name;Stack;Action;Amount;Hand;Board;Win Probability");

            }
            else
            {
                log.Info("Training;Off;;;;;");
                log.Info("Name;Stack;Action;Amount;Hand;Board;Win Probability");
            }
        }

        public static void action(Game gl, Player player, SurfacePoker.Action.playerAction action, int amount, List<Card> board)
        {
            log.Debug("action(Game gl, Player "+ player.name + ", SurfacePoker.Action.playerAction "+ action+ ", int " + amount + ", List<Card> board) - Begin");
            String str = "";
            foreach(Card c in board){
                str += c.ToString();
            }
            if (player.cards.Count != 0)
            {
                log.Info(player.name+ ";" + player.stack + ";" + action.ToString() + ";" + amount + ";" + player.getCardsToString() + ";" + str + ";" + player.winChance);
            }
            else
            {
                log.Info(player.name + ";" + player.stack + ";" + action.ToString() + ";" + amount + "; ;" + str + ";0");
            }
            log.Debug("action() - End");
        }

        public static void calculateWinChance(Game gl)
        {
            log.Debug("calculateWinChance(Game gl) - Begin");
            long totalHands = 10000000;
            int count = gl.players.FindAll(x => x.cards.Count == 2).Count;
            string[] pockets = new string[count];
            string board = "";
            long[] win = new long[count];
            long[] tie = new long[count];
            long[] los = new long[count];
            int j = 0;
            for(int i = 0; i < gl.players.Count; i++)
            {
                if (gl.players[i].cards.Count == 2)
                {
                    pockets[i - j] = gl.players[i].getCardsToString();
                }
                else
                {
                    j++;
                }
            }
            foreach (Card c in gl.board)
            {
                board += c.ToString() + " ";
            }
            Hand.HandOdds(pockets, board, "", win, tie, los, ref totalHands);
            j = 0;
            Console.WriteLine(totalHands);
            for (int i = 0; i < gl.players.Count; i++)
            {
                if (gl.players[i].cards.Count == 2)
                {
                    gl.players[i].winChance = win[i - j] * 100 / totalHands;
                    log.Debug("Player" + gl.players[i] + " win/tie/loose " +  (win[i - j] * 100 / totalHands)+ "/" + (tie[i - j] * 100 / totalHands) + "/" + (los[i - j] * 100 / totalHands));
                }
                else
                {
                    j++;
                }
            }
            log.Debug("calculateWinChance() - End");
        }

        public void finishRound()
        {
            List<KeyValuePair<DateTime, string>> allActions = new List<KeyValuePair<DateTime, string>>();
            foreach (KeyValuePair<string, LogPlayer> player in players)
            {
                //allActions.Add(new KeyValuePair<DateTime, string>(player.Value.initTime, player.Key + "," + player.Value.hand.ToString() + ",0," + player.Value.initStack.ToString()));
                foreach(KeyValuePair<DateTime, string> kvp in player.Value.actionList){
                    allActions.Add(new KeyValuePair<DateTime, string>(kvp.Key, player.Key + "," + kvp.Value));
                }
            }
            allActions.Sort();
            foreach (KeyValuePair<DateTime, string> kvp in allActions)
            {
                Console.WriteLine(kvp.Key.ToString() + " : " + kvp.Value);
            }
        }
    }

    public class LogPlayer
    {
        public LogPlayer(double init, List<Card> cards)
        {
            initStack = init;
            stack = init;
            this.cards = cards;
            initTime = new DateTime();
            actionList = new List<KeyValuePair<DateTime, string>>();
            actionList.Add(new KeyValuePair<DateTime, string>(new DateTime(), "cards dealt"));
        }
        public void addAction(String action, double amount)
        {
            
            stack = stack - amount;
            actionList.Add(new KeyValuePair<DateTime, string>(new DateTime(), action+ "," + amount.ToString()+ "," + stack.ToString()));
        }

        public double initStack { get; set; }
        public DateTime initTime { get; set; }
        public List<Card> cards { get; set; }
        public List<KeyValuePair<DateTime, string>> actionList { get; set; }
        public double stack { get; set; }
    }
}

