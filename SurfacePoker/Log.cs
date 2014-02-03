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

        public static void newGame()
        {
            log.Info("Name;Stack;Action;Amount;Hand;Board");
        }

        public static void action(Player player, SurfacePoker.Action.playerAction action, int amount, List<Card> board)
        {
            String str = "";
            foreach(Card c in board){
                str += c.ToString();
            }
            if (player.cards.Count != 0)
            {
                log.Info(player.name+ ";" + player.stack + ";" + action.ToString() + ";" + amount + ";" + player.getCardsToString() + ";" + str);
            }
            else
            {
                log.Info(player.name + ";" + player.stack + ";" + action.ToString() + ";" + amount + "; ;" + str);
            }
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

