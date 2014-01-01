using SurfacePoker;
using System;
using System.Collections.Generic;


namespace Log
{
    
    public class Main 
    {
        public List<KeyValuePair<string, LogPlayer>> players { get; set; }
	    public Main()
	    {
            players = new List<KeyValuePair<string, LogPlayer>>();
	    }

        public void newGame(List<Player> players)
        {
            
            foreach (Player player in players)
            {
                this.players.Add(new KeyValuePair<string, LogPlayer>(player.name, new LogPlayer((double)player.stack, player.cards)));
                
            }
        }

        public void action(Player player, string action, double amount)
        {
            KeyValuePair<string, LogPlayer> current = this.players.Find(x => x.Key.Equals(player.name));
            current.Value.addAction(action, amount);
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

