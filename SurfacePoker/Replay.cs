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
        public String gsq { get; set; }
        List<KeyValuePair<String, List<String>>> games;
        public Replay(String file)
        {
            gsq = file;
            games = new List<KeyValuePair<string, List<string>>>();
        }

        public List<String> getGames()
        {
            List<String> gameNumbers = new List<string>();
            string[] lines = File.ReadAllLines(gsq);
            int i = -1;
            List<String> actGame = new List<string>();
            foreach (String line in lines)
            {
                string[] keys = line.Split(';');
                if ((i > -1) && (! keys[3].Contains("newgame")))
                {
                    actGame.Add(line);
                }
                if (keys[3].Contains("newgame"))
                {
                    gameNumbers.Add(keys[0]);
                    actGame = new List<string>();
                    games.Add(new KeyValuePair<string, List<string>>(keys[0], actGame));
                    i++;
                }
            }
            return gameNumbers;
        }

        public void beginGame(String str)
        {

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
            foreach (String str in r.getGames())
            {
                Console.WriteLine(str);
            }
        }
    }
    #endregion
}
