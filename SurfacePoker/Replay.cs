using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SurfacePoker
{
    class Replay
    {
        public enum action
        {
            game,hand,next
        }
        public String gsq { get; set; }
        public List<String> actualGame { get; set; }
        public List<String> actualHand { get; set; }
        List<KeyValuePair<String, List<String>>> games;
        List<KeyValuePair<String, List<String>>> hands;
        List<KeyValuePair<String,List<Player>>> playerPerGame;
        public Replay(String file)
        {
            gsq = file;
            games = new List<KeyValuePair<string, List<string>>>();
            hands = new List<KeyValuePair<string, List<string>>>();
            playerPerGame = new List<KeyValuePair<string, List<Player>>>();
        }

        public List<String> getGames()
        {
            List<String> gameNumbers = new List<string>();
            string[] lines = File.ReadAllLines(gsq);
            int i = -1;
            List<String> actGame = new List<string>();
            List<Player> players = new List<Player>();
            foreach (String line in lines)
            {
                string[] keys = line.Split(';');
                if (keys.Length == 4)
                {
                    players.Add(new Player(keys[1].Split(' ')[1], Convert.ToInt32(keys[3].Split(' ')[2]), Convert.ToInt32(keys[2].Split(' ')[2])));
                    continue;
                }
                if ((i > -1) && (! line.Contains("Name;Stack;Action;Amount;Hand;Board;Pot;Win Probability")))
                {
                    actGame.Add(line);
                }
                if (line.Contains("Name;Stack;Action;Amount;Hand;Board;Pot;Win Probability"))
                {
                    gameNumbers.Add(keys[0]);
                    actGame = new List<string>();
                    players = new List<Player>();
                    games.Add(new KeyValuePair<string, List<string>>(keys[0], actGame));
                    playerPerGame.Add(new KeyValuePair<string, List<Player>>(keys[0], players));
                    i++;
                }
            }
            return gameNumbers;
        }

        public List<String> getHands(String str)
        {
            List<String> handNumbers = new List<string>();
            actualGame = games.Find(x => x.Key.Contains(str)).Value;
            actualHand = new List<string>();
            int i = -1;
            foreach (String line in actualGame)
            {
                string[] keys = line.Split(';');
                
                if ((i > -1) && (! keys[3].Contains("newgame")) )
                {
                    actualHand.Add(line);
                }
                if (keys[3].Contains("newgame"))
                {
                    handNumbers.Add(keys[0]);
                    games.Add(new KeyValuePair<string, List<string>>(keys[0], actualHand));
                    actualHand = new List<string>();
                    i++;
                }
            }
            return handNumbers;
        }

        public void getPlayerSeats()
        {

        }

        public void beginGame(String str)
        {
            actualGame = games.Find(x => x.Key.Contains(str)).Value;
            
        }
    }
    #region Testclass
    [TestClass]
    public class TestReplay
    {
        Replay r = new Replay("gameSequence01.txt");

        [TestMethod]
        public void testClass()
        {
            List<String> games = r.getGames();
            foreach (String st in r.getHands(games[0]))
            {
                Console.WriteLine(st);
            }
            foreach (String str in r.actualHand)
            {
                Console.WriteLine(str);
            }
        }
    }
    #endregion
}
