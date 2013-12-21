using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Windows.Ink;
using System.Collections.ObjectModel;
using System.Media;
using System.Windows.Media.Animation;
using System.Threading;
using System.Xml;

namespace SurfacePoker
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        public static readonly RoutedCommand MenuItemCommand = new RoutedCommand();

        //Eventuelle für spätere Erweiterungen
        //private readonly Windows7.Multitouch.GestureHandler _gestureHandler;
        
        //Spielemodus anzahl spieler die eingestellt wurden
        private int gamemode;

        //Spieler die ihre Tags auf den Tisch gelegt haben zählen
        private int activatedPlayersWithTags;

        private const string m_strFileName = "c:\\test.xml";

        //Aktive / Inaktive Spieler
        private bool p1Active = false;
        private bool p2Active = false;
        private bool p3Active = false;
        private bool p4Active = false;
        private bool p5Active = false;
        private bool p6Active = false;

        //Hände der Spieler
        private static Hand hand1;
        private static Hand hand2;
        private static Hand hand3;
        private static Hand hand4;
        private static Hand hand5;
        private static Hand hand6;

        //Deck Kartendeck
        private static Deck deck;

        //Spieler
        private static Player player1;
        private static Player player2;
        private static Player player3;
        private static Player player4;
        private static Player player5;
        private static Player player6;

        //Sounds
        private SoundManager soundmanager;

        //Tisch
        private static Table table;

        //Alternatives Kartendeck
        private Boolean alternativeDeck = false;

        //Wettrunden
        //bool preFlop = false;   //Erste Wettrunde noch keine Karten auf dem Table
        bool flop = false;        //Erste drei Karten
        bool turnCard = false;       //Vierte Karte
        bool riverCard = false;        //Fünfte Karte

        //Liste zum halten von 
        //-aktivenSpieler
        //-aktivenHänden
        //-aktive Spieler in der Runde
        //-Chips on table bzw chips im pot
        private List<Player> activePlayers;
        List<Hand> activeHands;
        List<Player> activeRoundPlayers;
        List<Canvas> chipsOnTable;

        //LinkedList<Player> sentence;

        //Network Connection für mobile Endgeräte
        NetworkServer nws;

        //Blind handling
        Blind blind;

        //Pot und dessen Wert
        private int pot = 0;

        //GameLoop mit der Spielelogik
        CompositionTargetGameLoop gameLoop;

        XmlHandler xmlHandler;

        //Spielrunde
        int playRound = 0;
        int playRoundPlayerCounter = 0;


        int currentPlayerCounter;


        Boolean alInTag = false;
        Boolean isGameRunning = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();


            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            TagVisualizationDefinition definition = new MatchEverythingDefinition();
            definition.Source = new Uri("GlowVisualization.xaml", UriKind.Relative);
            definition.LostTagTimeout = 500;
            Visualizer1.Definitions.Add(definition);

            //Soundmanager
            soundmanager = new SoundManager();

        }


        /// <summary>
        /// Initalisierung der Variablen
        /// </summary>
        public void init()
        {

            //Deck initalisieren
            deck = new Deck();

            //Die 6 möglichen Hände initalisieren
            hand1 = new Hand();
            hand2 = new Hand();
            hand3 = new Hand();
            hand4 = new Hand();
            hand5 = new Hand();
            hand6 = new Hand();

            //Spieler initalisieren
            player1 = new Player("", 10000, false, 1);
            player2 = new Player("", 10000, false, 2);
            player3 = new Player("", 10000, false, 3);
            player4 = new Player("", 10000, false, 1);
            player5 = new Player("", 10000, false, 2);
            player6 = new Player("", 10000, false, 4);

            player1.PlayerID = 0;
            player2.PlayerID = 1;
            player3.PlayerID = 2;
            player4.PlayerID = 3;
            player5.PlayerID = 4;
            player6.PlayerID = 5;

            player1.setTagID(1);
            player2.setTagID(2);
            player3.setTagID(3);

            activePlayers = new List<Player>();         //Aktive Spieler am Tisch
            activeHands = new List<Hand>();             //Händer der Aktiven Spieler am Tisch
            activeRoundPlayers = new List<Player>();    //Aktive Spieler einer Runde
            chipsOnTable = new List<Canvas>();          //Alle Chips die sich auf dem Tisch befinden

            table = new Table();                        //Tisch init
            blind = new Blind();                        //Blind init

        }

        /// <summary>
        /// Die Aktiven Spieler in die activePlayer liste hinzufügen und den activHands die hände hinzufügen
        /// </summary>
        public void activatePlayers()
        {
            //Aktive Spieler ermittlen und die Hände aktivieren sowie die Spieler aktivieren
            if (p1Active) { activeHands.Add(hand1); }
            if (p2Active) { activeHands.Add(hand2); }
            if (p3Active) { activeHands.Add(hand3); }
            if (p4Active) { activeHands.Add(hand4); }
            if (p5Active) { activeHands.Add(hand5); }
            if (p6Active) { activeHands.Add(hand6); }


            optimizePlayerPlaces();
        }

        /// <summary>
        /// Den aktiven Händen die Karten geben
        /// Hände die nicht aktiv sind bekommen keine karten da sie keine
        /// Spieler haben daher Ressource sparen
        /// </summary>
        public void dealHands()
        {
            deck.shuffle();
            foreach (Hand hand in activeHands)
            {
                for (int i = 0; i < 2; i++)
                {
                    hand.setCard(i, deck.dealCard());
                }
            }

            soundmanager.playSoundFallen6();
        }

        /// <summary>
        /// Hände den Spielern zuordnen sobald diese aktiv sind
        /// ansonsten nicht um Ressourcen zu sparen
        /// </summary>
        public void setPlayerHands(int players)
        {
            switch (players)
            {
                case 1:
                    player1.setHand(hand1);
                    break;

                case 2:
                    player1.setHand(hand1);
                    player2.setHand(hand2);
                    break;

                case 3:
                    player1.setHand(hand1);
                    player2.setHand(hand2);
                    player3.setHand(hand3);
                    break;

                case 4:
                    player1.setHand(hand1);
                    player2.setHand(hand2);
                    player3.setHand(hand3);
                    player4.setHand(hand4);
                    break;

                case 5:
                    player1.setHand(hand1);
                    player2.setHand(hand2);
                    player3.setHand(hand3);
                    player4.setHand(hand4);
                    player5.setHand(hand5);
                    break;
                case 6:
                    player1.setHand(hand1);
                    player2.setHand(hand2);
                    player3.setHand(hand3);
                    player4.setHand(hand4);
                    player5.setHand(hand5);
                    player6.setHand(hand6);
                    break;
            }
        }

        /// <summary>
        /// Dem Tisch seine Karten geben
        /// </summary>
        public void dealTable()
        {
            //Deal the table
            for (int i = 0; i < 5; i++)
            {
                table.setCard(i, deck.dealCard());
            }
        }

        /// <summary>
        /// Nicht in benutzung kommt evtl. noch
        /// </summary>
        public void updateConnectedUsers()
        {
            foreach (Player player in activePlayers)
            {
                if (player.PlayerConnection)
                    player.setPlayername(player.getPlayername() + " [Verbunden]");
            }

            updateUI();

        }

        /// <summary>
        /// Updated das Userinterface Visibility etc
        /// Chips visible setzen, Spielernamen anzeigen, Fold/Bet/Check Buttons visible setzen
        /// </summary>
        public void updateUI()
        {

            potLeft.Content = "" + pot;
            potRight.Content = "" + pot;

            if (player1.getHand() != null)
            {
                p1c10.Visibility = Visibility.Visible;
                p1c20.Visibility = Visibility.Visible;
                p1c50.Visibility = Visibility.Visible;
                p1c100.Visibility = Visibility.Visible;
                p1c200.Visibility = Visibility.Visible;
                p1c500.Visibility = Visibility.Visible;

                Player1Grid.Visibility = Visibility.Visible;
                player1Name.Text = player1.getPlayername();

                player1Name.Visibility = Visibility.Visible;
                player1Cash.Content = player2.getBudget();
                player1Cash.Visibility = Visibility.Visible;

                player1Bet.Visibility = Visibility.Visible;
                player1Fold.Visibility = Visibility.Visible;
                player1Check.Visibility = Visibility.Visible;

                if (alternativeDeck)
                {
                    p1c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player1.getHand().getCard(0) + ".png")));
                    p1c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player1.getHand().getCard(1) + ".png")));
                }
                else
                {
                    p1c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player1.getHand().getCard(0) + ".png")));
                    p1c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player1.getHand().getCard(1) + ".png")));
                }

                player1Cash.Content = "" + player1.getBudget();

                if (player1.getIsCurrentPlayer())
                {
                    p1c10.CanMove = true;
                    p1c20.CanMove = true;
                    p1c50.CanMove = true;
                    p1c100.CanMove = true;
                    p1c200.CanMove = true;
                    p1c500.CanMove = true;
                }
                else
                {
                    p1c10.CanMove = false;
                    p1c20.CanMove = false;
                    p1c50.CanMove = false;
                    p1c100.CanMove = false;
                    p1c200.CanMove = false;
                    p1c500.CanMove = false;
                }


                if (player1.getBudget() < 500)
                    p1c500.CanMove = false;
                if (player1.getBudget() < 200)
                    p1c200.CanMove = false;
                if (player1.getBudget() < 100)
                    p1c100.CanMove = false;
                if (player1.getBudget() < 50)
                    p1c50.CanMove = false;
                if (player1.getBudget() < 20)
                    p1c20.CanMove = false;

                if (player1.PlayerState == Player.State.fold)
                {
                    player1Bet.IsEnabled = false;
                    player1Fold.IsEnabled = false;
                    player1Check.IsEnabled = false;


                    p1c1s.Visibility = Visibility.Hidden;
                    p1c2s.Visibility = Visibility.Hidden;



                }

                if (player1.PlayerState == Player.State.unknow)
                {
                    player1Bet.IsEnabled = true;
                    player1Fold.IsEnabled = true;
                    player1Check.IsEnabled = true;


                    p1c1s.Visibility = Visibility.Visible;
                    p1c2s.Visibility = Visibility.Visible;

                    //player1.setCurrentPlayer(false);
                }

            }

            if (player2.getHand() != null)
            {
                p2c10.Visibility = Visibility.Visible;
                p2c20.Visibility = Visibility.Visible;
                p2c50.Visibility = Visibility.Visible;
                p2c100.Visibility = Visibility.Visible;
                p2c200.Visibility = Visibility.Visible;
                p2c500.Visibility = Visibility.Visible;

                Player2Grid.Visibility = Visibility.Visible;
                player2Name.Text = player2.getPlayername();

                player2Name.Visibility = Visibility.Visible;
                player2Cash.Content = player2.getBudget();
                player2Cash.Visibility = Visibility.Visible;

                player2Bet.Visibility = Visibility.Visible;
                player2Fold.Visibility = Visibility.Visible;
                player2Check.Visibility = Visibility.Visible;

                if (alternativeDeck)
                {
                    p2c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player2.getHand().getCard(0) + ".png")));
                    p2c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player2.getHand().getCard(1) + ".png")));
                }
                else
                {
                    p2c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player2.getHand().getCard(0) + ".png")));
                    p2c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player2.getHand().getCard(1) + ".png")));
                }


                player2Cash.Content = "" + player2.getBudget();

                if (player2.getIsCurrentPlayer())
                {
                    p2c10.CanMove = true;
                    p2c20.CanMove = true;
                    p2c50.CanMove = true;
                    p2c100.CanMove = true;
                    p2c200.CanMove = true;
                    p2c500.CanMove = true;
                }
                else
                {
                    p2c10.CanMove = false;
                    p2c20.CanMove = false;
                    p2c50.CanMove = false;
                    p2c100.CanMove = false;
                    p2c200.CanMove = false;
                    p2c500.CanMove = false;
                }


                if (player2.getBudget() < 500)
                    p2c500.CanMove = false;
                if (player2.getBudget() < 200)
                    p2c200.CanMove = false;
                if (player2.getBudget() < 100)
                    p2c100.CanMove = false;
                if (player2.getBudget() < 50)
                    p2c50.CanMove = false;
                if (player2.getBudget() < 20)
                    p2c20.CanMove = false;

                if (player2.PlayerState == Player.State.fold)
                {
                    player2Bet.IsEnabled = false;
                    player2Fold.IsEnabled = false;
                    player2Check.IsEnabled = false;


                    p2c1s.Visibility = Visibility.Hidden;
                    p2c2s.Visibility = Visibility.Hidden;


                }

                if (player2.PlayerState == Player.State.unknow)
                {
                    player2Bet.IsEnabled = true;
                    player2Fold.IsEnabled = true;
                    player2Check.IsEnabled = true;


                    p2c1s.Visibility = Visibility.Visible;
                    p2c2s.Visibility = Visibility.Visible;

                    //player2.setCurrentPlayer(false);

                }
            }

            if (player3.getHand() != null)
            {
                p3c10.Visibility = Visibility.Visible;
                p3c20.Visibility = Visibility.Visible;
                p3c50.Visibility = Visibility.Visible;
                p3c100.Visibility = Visibility.Visible;
                p3c200.Visibility = Visibility.Visible;
                p3c500.Visibility = Visibility.Visible;

                Player3Grid.Visibility = Visibility.Visible;
                player3Name.Text = player3.getPlayername();

                player3Name.Visibility = Visibility.Visible;
                player3Cash.Content = player3.getBudget();
                player3Cash.Visibility = Visibility.Visible;

                player3Bet.Visibility = Visibility.Visible;
                player3Fold.Visibility = Visibility.Visible;
                player3Check.Visibility = Visibility.Visible;

                if (alternativeDeck)
                {
                    p3c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player3.getHand().getCard(0) + ".png")));
                    p3c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player3.getHand().getCard(1) + ".png")));
                }
                else
                {
                    p3c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player3.getHand().getCard(0) + ".png")));
                    p3c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player3.getHand().getCard(1) + ".png")));
                }

                if (player3.getIsCurrentPlayer())
                {
                    p3c10.CanMove = true;
                    p3c20.CanMove = true;
                    p3c50.CanMove = true;
                    p3c100.CanMove = true;
                    p3c200.CanMove = true;
                    p3c500.CanMove = true;
                }
                else
                {
                    p3c10.CanMove = false;
                    p3c20.CanMove = false;
                    p3c50.CanMove = false;
                    p3c100.CanMove = false;
                    p3c200.CanMove = false;
                    p3c500.CanMove = false;
                }

                if (player3.getBudget() < 500)
                    p3c500.CanMove = false;
                if (player3.getBudget() < 200)
                    p3c200.CanMove = false;
                if (player3.getBudget() < 100)
                    p3c100.CanMove = false;
                if (player3.getBudget() < 50)
                    p3c50.CanMove = false;
                if (player3.getBudget() < 20)
                    p3c20.CanMove = false;

                if (player3.PlayerState == Player.State.fold)
                {
                    player3Bet.IsEnabled = false;
                    player3Fold.IsEnabled = false;
                    player3Check.IsEnabled = false;


                    p3c1s.Visibility = Visibility.Hidden;
                    p3c2s.Visibility = Visibility.Hidden;
                }

                if (player3.PlayerState == Player.State.unknow)
                {
                    player3Bet.IsEnabled = true;
                    player3Fold.IsEnabled = true;
                    player3Check.IsEnabled = true;


                    p3c1s.Visibility = Visibility.Visible;
                    p3c2s.Visibility = Visibility.Visible;

                    //player3.setCurrentPlayer(false);
                }
            }

            if (player4.getHand() != null)
            {
                p4c10.Visibility = Visibility.Visible;
                p4c20.Visibility = Visibility.Visible;
                p4c50.Visibility = Visibility.Visible;
                p4c100.Visibility = Visibility.Visible;
                p4c200.Visibility = Visibility.Visible;
                p4c500.Visibility = Visibility.Visible;

                Player4Grid.Visibility = Visibility.Visible;
                player4Name.Text = player4.getPlayername();

                player4Name.Visibility = Visibility.Visible;
                player4Cash.Content = player4.getBudget();
                player4Cash.Visibility = Visibility.Visible;

                player4Bet.Visibility = Visibility.Visible;
                player4Fold.Visibility = Visibility.Visible;
                player4Check.Visibility = Visibility.Visible;

                player4Name.Text = player4.getPlayername();

                if (alternativeDeck)
                {
                    p4c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player4.getHand().getCard(0) + ".png")));
                    p4c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player4.getHand().getCard(1) + ".png")));
                }
                else
                {
                    p4c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player4.getHand().getCard(0) + ".png")));
                    p4c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player4.getHand().getCard(1) + ".png")));
                }

                if (player4.getIsCurrentPlayer())
                {
                    p4c10.CanMove = true;
                    p4c20.CanMove = true;
                    p4c50.CanMove = true;
                    p4c100.CanMove = true;
                    p4c200.CanMove = true;
                    p4c500.CanMove = true;
                }
                else
                {
                    p4c10.CanMove = false;
                    p4c20.CanMove = false;
                    p4c50.CanMove = false;
                    p4c100.CanMove = false;
                    p4c200.CanMove = false;
                    p4c500.CanMove = false;
                }

                if (player4.getBudget() < 500)
                    p4c500.CanMove = false;
                if (player4.getBudget() < 200)
                    p4c200.CanMove = false;
                if (player4.getBudget() < 100)
                    p4c100.CanMove = false;
                if (player4.getBudget() < 50)
                    p4c50.CanMove = false;
                if (player4.getBudget() < 20)
                    p4c20.CanMove = false;

                if (player4.PlayerState == Player.State.fold)
                {
                    player4Bet.IsEnabled = false;
                    player4Fold.IsEnabled = false;
                    player4Check.IsEnabled = false;


                    p4c1s.Visibility = Visibility.Hidden;
                    p4c2s.Visibility = Visibility.Hidden;
                }

                if (player4.PlayerState == Player.State.unknow)
                {
                    player4Bet.IsEnabled = true;
                    player4Fold.IsEnabled = true;
                    player4Check.IsEnabled = true;


                    p4c1s.Visibility = Visibility.Visible;
                    p4c2s.Visibility = Visibility.Visible;

                    // player4.setCurrentPlayer(false);
                }

            }

            if (player5.getHand() != null)
            {
                p5c10.Visibility = Visibility.Visible;
                p5c20.Visibility = Visibility.Visible;
                p5c50.Visibility = Visibility.Visible;
                p5c100.Visibility = Visibility.Visible;
                p5c200.Visibility = Visibility.Visible;
                p5c500.Visibility = Visibility.Visible;

                Player5Grid.Visibility = Visibility.Visible;
                player5Name.Text = player5.getPlayername();

                player5Name.Visibility = Visibility.Visible;
                player5Cash.Content = player5.getBudget();
                player5Cash.Visibility = Visibility.Visible;

                player5Bet.Visibility = Visibility.Visible;
                player5Fold.Visibility = Visibility.Visible;
                player5Check.Visibility = Visibility.Visible;

                player5Name.Text = player5.getPlayername();

                if (alternativeDeck)
                {
                    p5c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player5.getHand().getCard(0) + ".png")));
                    p5c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player5.getHand().getCard(1) + ".png")));
                }
                else
                {
                    p5c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player5.getHand().getCard(0) + ".png")));
                    p5c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player5.getHand().getCard(1) + ".png")));
                }

                if (player5.getIsCurrentPlayer())
                {
                    p5c10.CanMove = true;
                    p5c20.CanMove = true;
                    p5c50.CanMove = true;
                    p5c100.CanMove = true;
                    p5c200.CanMove = true;
                    p5c500.CanMove = true;
                }
                else
                {
                    p5c10.CanMove = false;
                    p5c20.CanMove = false;
                    p5c50.CanMove = false;
                    p5c100.CanMove = false;
                    p5c200.CanMove = false;
                    p5c500.CanMove = false;
                }

                if (player5.getBudget() < 500)
                    p5c500.CanMove = false;
                if (player5.getBudget() < 200)
                    p5c200.CanMove = false;
                if (player5.getBudget() < 100)
                    p5c100.CanMove = false;
                if (player5.getBudget() < 50)
                    p5c50.CanMove = false;
                if (player5.getBudget() < 20)
                    p5c20.CanMove = false;

                if (player5.PlayerState == Player.State.fold)
                {
                    player5Bet.IsEnabled = false;
                    player5Fold.IsEnabled = false;
                    player5Check.IsEnabled = false;


                    p5c1s.Visibility = Visibility.Hidden;
                    p5c2s.Visibility = Visibility.Hidden;
                }

                if (player5.PlayerState == Player.State.unknow)
                {
                    player5Bet.IsEnabled = true;
                    player5Fold.IsEnabled = true;
                    player5Check.IsEnabled = true;


                    p5c1s.Visibility = Visibility.Visible;
                    p5c2s.Visibility = Visibility.Visible;

                    // player5.setCurrentPlayer(false);
                }

            }


            if (player6.getHand() != null)
            {
                p6c10.Visibility = Visibility.Visible;
                p6c20.Visibility = Visibility.Visible;
                p6c50.Visibility = Visibility.Visible;
                p6c100.Visibility = Visibility.Visible;
                p6c200.Visibility = Visibility.Visible;
                p6c500.Visibility = Visibility.Visible;

                Player6Grid.Visibility = Visibility.Visible;
                player6Name.Text = player6.getPlayername();

                player6Name.Visibility = Visibility.Visible;
                player6Cash.Content = player6.getBudget();
                player6Cash.Visibility = Visibility.Visible;

                player6Bet.Visibility = Visibility.Visible;
                player6Fold.Visibility = Visibility.Visible;
                player6Check.Visibility = Visibility.Visible;

                player6Name.Text = player5.getPlayername();

                if (alternativeDeck)
                {
                    p6c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player6.getHand().getCard(0) + ".png")));
                    p6c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + player6.getHand().getCard(1) + ".png")));
                }
                else
                {
                    p6c1.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player6.getHand().getCard(0) + ".png")));
                    p6c2.setImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + player6.getHand().getCard(1) + ".png")));
                }

                if (player6.getIsCurrentPlayer())
                {
                    p6c10.CanMove = true;
                    p6c20.CanMove = true;
                    p6c50.CanMove = true;
                    p6c100.CanMove = true;
                    p6c200.CanMove = true;
                    p6c500.CanMove = true;
                }
                else
                {
                    p6c10.CanMove = false;
                    p6c20.CanMove = false;
                    p6c50.CanMove = false;
                    p6c100.CanMove = false;
                    p6c200.CanMove = false;
                    p6c500.CanMove = false;
                }

                if (player6.getBudget() < 500)
                    p6c500.CanMove = false;
                if (player6.getBudget() < 200)
                    p6c200.CanMove = false;
                if (player6.getBudget() < 100)
                    p6c100.CanMove = false;
                if (player6.getBudget() < 50)
                    p6c50.CanMove = false;
                if (player6.getBudget() < 20)
                    p6c20.CanMove = false;

                if (player6.PlayerState == Player.State.fold)
                {
                    player6Bet.IsEnabled = false;
                    player6Fold.IsEnabled = false;
                    player6Check.IsEnabled = false;


                    p6c1s.Visibility = Visibility.Hidden;
                    p6c2s.Visibility = Visibility.Hidden;
                }

                if (player6.PlayerState == Player.State.unknow)
                {
                    player6Bet.IsEnabled = true;
                    player6Fold.IsEnabled = true;
                    player6Check.IsEnabled = true;


                    p6c1s.Visibility = Visibility.Visible;
                    p6c2s.Visibility = Visibility.Visible;

                    // player6.setCurrentPlayer(false);
                }
            }

            if (alternativeDeck)
            {
                t1.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + table.getCard(0) + ".png"));
                t2.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + table.getCard(1) + ".png"));
                t3.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + table.getCard(2) + ".png"));
                t4.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + table.getCard(3) + ".png"));
                t5.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + table.getCard(4) + ".png"));
            }
            else {
                t1.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + table.getCard(0) + ".png"));
                t2.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + table.getCard(1) + ".png"));
                t3.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + table.getCard(2) + ".png"));
                t4.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + table.getCard(3) + ".png"));
                t5.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + table.getCard(4) + ".png"));
            }


        }

        GlowVisualization glow1;
        GlowVisualization glow2;
        GlowVisualization glow3;
        GlowVisualization glow4;
        GlowVisualization glow5;
        GlowVisualization glow6;

        /// <summary>
        /// Byte Tags behandeln und die Spieler dazu aktivieren
        /// sowie die Daten aus der XML lesen um an den Byte Tags die Namen der Spieler 
        /// anzeigen zu können.
        /// XML Dynamisch veränderbar
        /// 
        /// Wenn alle spieler des gamemode aktiv sind dann beginnt die initalisierung der Variablen,
        /// der UDPThread startet und die gameloop beginnt
        /// </summary>
        private void OnVisualizationEnter(object sender, TagVisualizationEnterLeaveEventArgs e)
        {




            xmlHandler = new XmlHandler();


            //  Console.WriteLine("Test");
            long tagValue = e.Visualization.VisualizedTag.Value;
            //  Console.WriteLine(tagValue);

            //Console.WriteLine(e.Visualization.Center.X);
            // Console.WriteLine(e.Visualization.Center.Y);



            if (tagValue == 1)
            {
                if (!p1Active)
                    activatedPlayersWithTags++;


                glow1 = (GlowVisualization)e.Visualization;
                glow1.Enter();
                p1Active = true;
                glow1.setPlayername(xmlHandler.readPlayer(1));
                player1Name.Text = xmlHandler.readPlayer(1);



            }
            if (tagValue == 2)
            {

                if (!p2Active)
                    activatedPlayersWithTags++;

                glow2 = (GlowVisualization)e.Visualization;
                glow2.Enter();
                p2Active = true;
                glow2.setPlayername(xmlHandler.readPlayer(2));
                player2Name.Text = xmlHandler.readPlayer(2);


            }

            if (tagValue == 3)
            {
                if (!p3Active)
                    activatedPlayersWithTags++;


                glow3 = (GlowVisualization)e.Visualization;
                glow3.Enter();
                p3Active = true;
                glow3.setPlayername(xmlHandler.readPlayer(3));
                player3Name.Text = xmlHandler.readPlayer(3);


            }
            if (tagValue == 4)
            {
                if (!p4Active)
                    activatedPlayersWithTags++;

                glow4 = (GlowVisualization)e.Visualization;
                glow4.Enter();
                p4Active = true;
                glow4.setPlayername(xmlHandler.readPlayer(4));
                player4Name.Text = xmlHandler.readPlayer(4);


            }
            if (tagValue == 5)
            {
                if (!p5Active)
                    activatedPlayersWithTags++;


                glow5 = (GlowVisualization)e.Visualization;
                glow5.Enter();
                p5Active = true;
                glow5.setPlayername(xmlHandler.readPlayer(5));
                player5Name.Text = xmlHandler.readPlayer(5);


            }
            if (tagValue == 6)
            {

                if (!p6Active)
                    activatedPlayersWithTags++;

                glow6 = (GlowVisualization)e.Visualization;
                glow6.Enter();
                p6Active = true;
                glow6.setPlayername(xmlHandler.readPlayer(6));
                player6Name.Text = xmlHandler.readPlayer(6);


            }

            if (tagValue == 0)
                alInTag = true;


            if (!isGameRunning)
            {
                //Wenn die anzahl der Tags mit der Anzahl der im Menü gewählten Spieler übereinstimmt
                //kann das Spiel anfangen
                if (activatedPlayersWithTags == gamemode)
                {
                    init();
                    activatePlayers();
                    dealHands();
                    dealTable();
                    setPlayerHands(activePlayers.Count);

                    initChips();




                    //Network Server Thread zum lauschen ob sich ein Spieler mit
                    //seinem Handy mit dem Surface verbindet
                    /**
                    nws = new NetworkServer();
                    Thread listener = new Thread(() =>
                    {


                        foreach (Player player in activePlayers)
                        {
                            nws.setPlayer(player);
                        }

                        nws.listenUDP();
                    });
                    listener.IsBackground = true;
                    listener.Start();



                    //User Interface updaten
                    updateUI();
                    */
                    if (gamemode > 1)
                    {
                        Player1Grid.Visibility = Visibility.Visible;
                        player1Name.Text = "Sascha";
                        p1c1s.Visibility = Visibility.Visible;
                        p1c2s.Visibility = Visibility.Visible;

                        Player2Grid.Visibility = Visibility.Visible;
                        player2Name.Text = "Kathy";
                        p2c1s.Visibility = Visibility.Visible;
                        p2c2s.Visibility = Visibility.Visible;

                    }

                    if (gamemode > 2)
                    {
                        Player3Grid.Visibility = Visibility.Visible;
                        player3Name.Text = "Daniel";
                        p3c1s.Visibility = Visibility.Visible;
                        p3c2s.Visibility = Visibility.Visible;
                    }

                    if (gamemode > 3)
                    {
                        Player4Grid.Visibility = Visibility.Visible;
                        player4Name.Text = "Julia";
                        p4c1s.Visibility = Visibility.Visible;
                        p4c2s.Visibility = Visibility.Visible;
                    }

                    if (gamemode > 4)
                    {
                        Player5Grid.Visibility = Visibility.Visible;
                        player5Name.Text = "Marc";
                        p5c1s.Visibility = Visibility.Visible;
                        p5c2s.Visibility = Visibility.Visible;
                    }

                    if (gamemode > 5)
                    {
                        Player6Grid.Visibility = Visibility.Visible;
                        player6Name.Text = "Markus";
                        p6c1s.Visibility = Visibility.Visible;
                        p6c2s.Visibility = Visibility.Visible;
                    }




                    Storyboard Animation = (Storyboard)FindResource("HideAnimStoryboard");
                    Storyboard TagHelpText = (Storyboard)FindResource("AddTagsStoryboardEnd");
                    TagHelpText.Begin();
                    Animation.Begin();


                }

                //Ausgabe auf dem Surface als Hilfe für den Spieler wieviele Spieler noch benötigt werden (active Tags auf dem Tisch)
                if (gamemode - activatedPlayersWithTags == 1)
                    helpText.Text = activatedPlayersWithTags + " Spieler erkannt es wird noch " + (gamemode - activatedPlayersWithTags) + " Spieler benötigt";
                if (gamemode - activatedPlayersWithTags > 1)
                    helpText.Text = activatedPlayersWithTags + " Spieler erkannt es werden noch " + (gamemode - activatedPlayersWithTags) + " Spieler benötigt";

                //Wenn alle Spieler aktiv sind dann kann das Spiel starten
                if (gamemode - activatedPlayersWithTags == 0)
                {


                    //Spiele die aktiv sind in die aktive Runde Liste hinzufügen
                    //in dieser Liste befinden sich nur Spieler die in dieser Runde noch drin sind
                    //also noch keinen fold hatten
                    foreach (Player player in activePlayers)
                        activeRoundPlayers.Add(player);




                    //Game Loop starten
                    isGameRunning = true;
                    gameLoop = new CompositionTargetGameLoop();
                    gameLoop.Update += new GameLoop.UpdateHandler(gameLoop_Update);
                    gameLoop.Start();




                    //Die Spieler zu den Blinds übertragen um den Random Dealer ermitteln zu können
                    //und dannach die blinds zu switchen sowie den raise zu handlen
                    blind.setPlayer(activePlayers);

                    //Per Zufall einen Dealer ermitteln
                    blind.randomDealer();
                    currentPlayerCounter = blind.getDealerValue();
                }

            }
        }


        /// <summary>
        /// Spieler Optimal auf dem Surface platzieren
        /// </summary>
        public void optimizePlayerPlaces()
        {
            if (gamemode == 2)
            {
                activePlayers.Add(player1);
                activePlayers.Add(player2);
            }

            if (gamemode == 3)
            {
                activePlayers.Add(player1);
                activePlayers.Add(player2);
                activePlayers.Add(player3);
            }

            if (gamemode == 4)
            {
                activePlayers.Add(player1);
                activePlayers.Add(player4);
                activePlayers.Add(player2);
                activePlayers.Add(player3);
            }

            if (gamemode == 5)
            {
                activePlayers.Add(player1);
                activePlayers.Add(player4);
                activePlayers.Add(player5);
                activePlayers.Add(player2);
                activePlayers.Add(player3);
            }

            if (gamemode == 6)
            {
                activePlayers.Add(player1);
                activePlayers.Add(player4);
                activePlayers.Add(player6);
                activePlayers.Add(player5);
                activePlayers.Add(player2);
                activePlayers.Add(player3);
            }

        }


        private Boolean flopping = false;
        private Boolean turning = false;
        private Boolean rivering = false;

        private Boolean floppingHelper = false;
        private Boolean turningHelper = false;
        private Boolean riveringHelper = false;

        private Boolean floppingBlindFoldHelper = false;



        /// <summary>
        /// GameLoop beeinhaltet die Pokerlogik
        /// </summary>
        public void gameLoop_Update(object sender, TimeSpan elapsed)
        {

            //Console.WriteLine(playRound);

            if (blind.nextWettRound(activeRoundPlayers) && flop == false && flopping == false && playRound > 0 && (blind.getbigBlindPlayer().PlayerState != Player.State.unknow))
            {
                flop = true;

                if (!flopping)
                {
                    //Rundenzähler für gesetzt zurück setzen
                    foreach (Player player in activeRoundPlayers)
                    {
                        player.GesetztRunde = 0;
                        // blind.setRaiseValue(player, -1);
                    }
                }

                flopping = true;
                floppingHelper = true;
                floppingBlindFoldHelper = true;

            }


            // Console.WriteLine(activeRoundPlayers[0].GesetztRunde);

            if (blind.nextWettRoundTwo(activeRoundPlayers) && turnCard == false && turning == false && flopping)
            {
                turnCard = true;

                if (!turning)
                {
                    //Rundenzähler für gesetzt zurück setzen
                    foreach (Player player in activeRoundPlayers)
                    {
                        player.GesetztRunde = 0;
                        // blind.setRaiseValue(player, -1);
                    }
                }

                turning = true;
                turningHelper = true;

            }


            if (blind.nextWettRoundTwo(activeRoundPlayers) && riverCard == false && rivering == false && turning)
            {
                riverCard = true;

                if (!rivering)
                {
                    //Rundenzähler für gesetzt zurück setzen
                    foreach (Player player in activeRoundPlayers)
                    {
                        player.GesetztRunde = 0;
                        // blind.setRaiseValue(player, -1);
                    }
                }

                rivering = true;
                riveringHelper = true;

            }



            //Flop Karten auf dem Tisch anzeigen
            if (flop)
            {

                //currentPlayerCounter = blind.getSmallPlayerPosition() - 1;

                t1.Visibility = Visibility.Visible;
                t2.Visibility = Visibility.Visible;
                t3.Visibility = Visibility.Visible;

                if (alternativeDeck)
                {
                    t1.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + table.getCard(0) + ".png"));
                    t2.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + table.getCard(1) + ".png"));
                    t3.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + table.getCard(2) + ".png"));
                }
                else
                {
                    t1.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + table.getCard(0) + ".png"));
                    t2.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + table.getCard(1) + ".png"));
                    t3.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + table.getCard(2) + ".png"));
                }


                //flop = false;
                flopping = true;
            }


            //Turn Karte auf dem Tisch anzeigen
            if (turnCard)
            {
                // currentPlayerCounter = blind.getSmallPlayerPosition() - 1;

                t4.Visibility = Visibility.Visible;

                if (alternativeDeck)
                {
                    t4.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + table.getCard(3) + ".png"));
                }
                else {
                    t4.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + table.getCard(3) + ".png"));
                }

                //turnCard = false;
                turning = true;
            }

            //River Karte auf dem Tisch anzeigen
            if (riverCard)
            {
                //currentPlayerCounter = blind.getSmallPlayerPosition() - 1;
                t5.Visibility = Visibility.Visible;

                if (alternativeDeck)
                {
                    t5.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/k" + table.getCard(4) + ".png"));
                }
                else {
                    t5.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Cards/" + table.getCard(4) + ".png"));
                }

                rivering = true;
            }


            flop = false;
            turnCard = false;
            riverCard = false;


            //Wenn die Runde rum ist dann prüfen welcher Spieler Gewonnen hat
            //Hande checken
            //Gewinner ermitteln
            //Und die gameloop stopen
            if (rivering == true && blind.nextWettRoundTwo(activeRoundPlayers))
            {

                checkHands();
                checkWinner();
                // gameLoop.Stop();
            }


            //Wenn der aktuelle Spieler > als die Anzahl der gesamt spieler
            //(die Runde hat wieder ihren Anfang gefunden bzw ist am Ende)
            if (currentPlayerCounter > activeRoundPlayers.Count - 1)
            {
                //dann setzte den Player Counter wieder auf 0 weil die Runde wieder bei 
                //Spieler index 0 anfängt und raise die blinds um 10
                currentPlayerCounter = 0;

                blind.raiseBlind(10);
            }

            //Der Aktive Spieler ist wieder der mit dem index -> currentplayercounter
            //activeRoundPlayers.ElementAt(currentPlayerCounter).setCurrentPlayer(true);
            updateUI();



            //Informationen an die Tags weiter geben
            //ob spieler small big bzw dealer
            foreach (Player player in activeRoundPlayers)
            {
                if (player.PlayerBlindDealState == Player.BlindDealState.small)
                {
                    if (player.PlayerID == 0)
                        glow1.setTagInfo("small");
                    if (player.PlayerID == 1)
                        glow2.setTagInfo("small");
                    if (player.PlayerID == 2)
                        glow3.setTagInfo("small");
                    if (player.PlayerID == 3)
                        glow4.setTagInfo("small");
                    if (player.PlayerID == 4)
                        glow5.setTagInfo("small");
                    if (player.PlayerID == 5)
                        glow6.setTagInfo("small");
                }

                if (player.PlayerBlindDealState == Player.BlindDealState.big)
                {
                    if (player.PlayerID == 0)
                        glow1.setTagInfo("big");
                    if (player.PlayerID == 1)
                        glow2.setTagInfo("big");
                    if (player.PlayerID == 2)
                        glow3.setTagInfo("big");
                    if (player.PlayerID == 3)
                        glow4.setTagInfo("big");
                    if (player.PlayerID == 4)
                        glow5.setTagInfo("big");
                    if (player.PlayerID == 5)
                        glow6.setTagInfo("big");
                }

                if (player.PlayerBlindDealState == Player.BlindDealState.unknow)
                {
                    if (player.PlayerID == 0)
                        glow1.setTagInfo("");
                    if (player.PlayerID == 1)
                        glow2.setTagInfo("");
                    if (player.PlayerID == 2)
                        glow3.setTagInfo("");
                    if (player.PlayerID == 3)
                        glow4.setTagInfo("");
                    if (player.PlayerID == 4)
                        glow5.setTagInfo("");
                    if (player.PlayerID == 5)
                        glow6.setTagInfo("");
                }

                if (player.PlayerBlindDealState == Player.BlindDealState.dealer)
                {
                    if (player.PlayerID == 0)
                        glow1.setTagInfo("dealer");
                    if (player.PlayerID == 1)
                        glow2.setTagInfo("dealer");
                    if (player.PlayerID == 2)
                        glow3.setTagInfo("dealer");
                    if (player.PlayerID == 3)
                        glow4.setTagInfo("dealer");
                    if (player.PlayerID == 4)
                        glow5.setTagInfo("dealer");
                    if (player.PlayerID == 5)
                        glow6.setTagInfo("dealer");
                }


            }



            //Wenn nur noch ein aktiver RundenSpieler im aktuellen Spiel runde ist ==> Runde zu Ende Pot geht an einen Gewinner 
            //und nächste runde beginnt
            if (activeRoundPlayers.Count == 1)
            {
                //Pot dem Spieler berechnen
                activeRoundPlayers.ElementAt(0).setBudget(activeRoundPlayers.ElementAt(0).getBudget() + pot);


                deck = new Deck();                      //Deck neu verteilen
                dealHands();                            //Hände neu geben
                setPlayerHands(activePlayers.Count);    //den Aktiven spielern die Hände geben
                table = new Table();                    //Tisch neue Karten geben
                dealTable();                            //Tisch neu dealen

                pot = 0;                                //Pot wieder auf 0 inialisieren


                GewinnerOverlay.Visibility = Visibility.Visible;                            //GewinnerOverlay auf Sichtbar setzen
                Storyboard GewinnerStoryBoard = (Storyboard)FindResource("Gewinner");       //Storyboard für das Gewinner Overlay
                GewinnerStoryBoard.Begin(this);                                             //Gewinner Storyboard starten

                soundmanager.playSoundViele1();

                gewinnerTxt.Text = "" + activeRoundPlayers.ElementAt(0).getPlayername();    //Gewinner auf dem Storyboard ausgeben

                activeRoundPlayers.Clear();     //activeRound liste leeren diese wird gleich neu gesetzt da wieder mehrer Spieler aktiv in der Runde sind                         




                blind.setPlayer(activePlayers);

                blind.switchBlindsAndDealer();  //Blinds weiter switchen

                foreach (Player player in activePlayers)
                {
                    player.setCurrentPlayer(false);
                    if (player.PlayerBlindDealState == Player.BlindDealState.small)
                        player.setCurrentPlayer(true);
                }



                //Die activeRoundPlayers wieder mit den Spielern setzen die sich noch in der ActivePlayers liste befinden
                //also die Spieler die deren Budget noch > 0 ist
                foreach (Player player in activePlayers)
                    activeRoundPlayers.Add(player);


                //nach der Runde raisen
                blind.raiseBlind(10);

                //Die States der Spieler auf unbekannt zurücksetzen
                foreach (Player player in activeRoundPlayers)
                    player.PlayerState = Player.State.unknow;



                //Die Chips vom Tisch entfernen (POT Chips)
                foreach (Canvas chip in chipsOnTable)
                    grid.Children.Remove(chip);

                //User Interface updaten
                updateUI();
            }
            else
            {
                foreach (Player player in activeRoundPlayers)   //Alle Spieler die noch aktiv in der Runde sind 
                {
                    //Console.WriteLine(player.getPlayername() + "  " + player.getIsCurrentPlayer() + "    " + player.PlayerBlindDealState);
                    //Console.WriteLine("--- ---- ___  " + player.getPlayername() + "  " + player.getIsCurrentPlayer() + "    " + player.PlayerBlindDealState + "     " + blind.minBetValue(player, playRound) + "     ");

                    //Console.WriteLine("Current Player Index:    " + currentPlayerCounter);

                    //Console.WriteLine(player.PlayerID + "  " + player.PlayerBlindDealState);

                    if (player.getIsCurrentPlayer() == true)            //Wenn Spieler aktueller Spieler prüf ob sich sein State verändert hat
                    {


                        //Prüfen ob Spielder gefoldet hat dann deaktivieren
                        if (player.PlayerState == Player.State.fold)
                        {
                            if (currentPlayerCounter > activeRoundPlayers.Count - 1)        //Wenn Letzter Spieler +1 erreicht nächste Runde also wieder bei 0 Anfangen
                                currentPlayerCounter = 0;

                            player.setCurrentPlayer(false);         //Aktiver Spieler ist jetzt nichtmehr der currentPlayer
                            player.Fold = true;                     //Spieler hat gefolded
                            updateUI();                             //User Interface updaten
                            activeRoundPlayers.Remove(player);      //Spieler ist nicht mehr aktiv in dieser Runde daher aus Liste entfernen  

                            //blind.setPlayer(activeRoundPlayers);    //Liste der Spieler für die Blinds neu setzen


                            if (currentPlayerCounter > activeRoundPlayers.Count - 1)
                            {
                                activeRoundPlayers.ElementAt(0).setCurrentPlayer(true);
                            }//Nächsten Spieler aktiv setzen
                            else
                                activeRoundPlayers.ElementAt(currentPlayerCounter).setCurrentPlayer(true);  //Nächsten Spieler aktiv setzen

                            playRoundPlayerCounter++;

                            if (playRoundPlayerCounter > activeRoundPlayers.Count - 1)
                                playRound++;

                            break;
                        }

                        if (alInTag)
                        {


                            player.Gesetzt = (int)player.getBudget();

                            pot += (int)player.getBudget();

                            player.GesetztRunde += (int)player.getBudget();
                            player.setBudget(0);


                            //gesetzt auf 0 zurück setzen
                            player.PlayerState = Player.State.unknow;                       //Spiele state ist wieder unbekannt
                            player.setCurrentPlayer(false);                                 //Spieler ist kein Aktiver Spieler mehr

                            blind.setRaiseValue(player, playRound);

                            currentPlayerCounter++;                                         //Nach einer Aktion den aktiven Spieler counter um eins erhöhen
                            if (currentPlayerCounter > activeRoundPlayers.Count - 1)
                            {
                                currentPlayerCounter = 0;                                   //Wenn Letzter Spieler +1 erreicht nächste Runde also wieder bei 0 Anfangen                                             //Spielrunde erhöhen wichtig um karten auf tisch zu legen
                            }

                            playRoundPlayerCounter++;
                            if (playRoundPlayerCounter > activeRoundPlayers.Count - 1)
                            {
                                playRound++;
                                playRoundPlayerCounter = 0;

                            }
                            
                            p2c1.showCard();


                            soundmanager.playSoundEinsatz2();

                            player.Gesetzt = 0;

                            activeRoundPlayers.ElementAt(currentPlayerCounter).setCurrentPlayer(true);  //Nächsten Spieler aktiv setzen

                            alInTag = false;
                            break;

                        }

                        if (player.PlayerState == Player.State.check)
                        {





                            player.setCurrentPlayer(false);         //Aktiver Spieler ist jetzt nichtmehr der currentPlayer
                            // player.Fold = true;                     //Spieler hat gefolded
                            updateUI();                             //User Interface updaten
                            player.PlayerState = Player.State.unknow;

                            //blind.setPlayer(activeRoundPlayers);    //Liste der Spieler für die Blinds neu setzen

                            player.Gesetzt = 0;

                            blind.clearRaises();


                            if (floppingHelper)
                            {
                                activeRoundPlayers.ElementAt(blind.getSmallPlayerPosition()).setCurrentPlayer(true);
                                currentPlayerCounter = blind.getSmallPlayerPosition();
                                floppingHelper = false;
                            }
                            else if (turningHelper)
                            {
                                activeRoundPlayers.ElementAt(blind.getBigPlayerPosition()).setCurrentPlayer(true);
                                currentPlayerCounter = blind.getBigPlayerPosition();
                                turningHelper = false;
                            }
                            else if (riveringHelper)
                            {
                                activeRoundPlayers.ElementAt(blind.getAfterBigPlayerPosition()).setCurrentPlayer(true);
                                currentPlayerCounter = blind.getAfterBigPlayerPosition();
                                riveringHelper = false;
                            }
                            else
                            {
                                currentPlayerCounter++;
                                if (currentPlayerCounter > activeRoundPlayers.Count - 1)
                                {
                                    currentPlayerCounter = 0;
                                }


                                activeRoundPlayers.ElementAt(currentPlayerCounter).setCurrentPlayer(true);  //Nächsten Spieler aktiv setzen
                            }


                            playRoundPlayerCounter++;
                            if (playRoundPlayerCounter > activeRoundPlayers.Count - 1)
                            {
                                playRound++;
                                playRoundPlayerCounter = 0;

                            }
                            break;

                        }

                        //Bet
                        if (player.PlayerState == Player.State.bet)
                        {


                            pot += player.Gesetzt;                                          //Das vom Spieler gesetzte in den Pot
                            player.GesetztRunde += player.Gesetzt;
                            player.setBudget(player.getBudget() - player.Gesetzt);          //Budget des Spieler mit dem gesetzen veringern
                            updateUI();                                                     //User Interface updaten um pot un budget zu aktualisieren
                            //gesetzt auf 0 zurück setzen
                            player.PlayerState = Player.State.unknow;                       //Spiele state ist wieder unbekannt
                            player.setCurrentPlayer(false);                                 //Spieler ist kein Aktiver Spieler mehr



                            blind.setRaiseValue(player, playRound);

                            if (floppingHelper)
                            {
                                activeRoundPlayers.ElementAt(blind.getSmallPlayerPosition()).setCurrentPlayer(true);
                                currentPlayerCounter = blind.getSmallPlayerPosition();
                                floppingHelper = false;
                            }
                            else if (turningHelper)
                            {
                                activeRoundPlayers.ElementAt(blind.getBigPlayerPosition()).setCurrentPlayer(true);
                                currentPlayerCounter = blind.getBigPlayerPosition();
                                turningHelper = false;
                            }
                            else if (riveringHelper)
                            {
                                activeRoundPlayers.ElementAt(blind.getAfterBigPlayerPosition()).setCurrentPlayer(true);
                                currentPlayerCounter = blind.getAfterBigPlayerPosition();
                                riveringHelper = false;
                            }
                            else
                            {
                                currentPlayerCounter++;
                                if (currentPlayerCounter > activeRoundPlayers.Count - 1)
                                {
                                    currentPlayerCounter = 0;
                                }


                                activeRoundPlayers.ElementAt(currentPlayerCounter).setCurrentPlayer(true);  //Nächsten Spieler aktiv setzen
                            }


                            playRoundPlayerCounter++;
                            if (playRoundPlayerCounter > activeRoundPlayers.Count - 1)
                            {
                                playRound++;
                                playRoundPlayerCounter = 0;

                            }


                            soundmanager.playSoundEinsatz2();

                            player.Gesetzt = 0;



                            break;
                        }


                    }
                }





            }






        }


        public void checkHands()
        {
            //Player player = player1;

            HandRecognition recoHand = new HandRecognition();
            for (int i = 0; i < 5; i++)
                recoHand.setCard(i, table.getCardObj(i));

            foreach (Player player in activeRoundPlayers)
            {

                recoHand.setCard(5, player.getHand().getCardObj(0));
                recoHand.setCard(6, player.getHand().getCardObj(1));

                if (recoHand.isRoyalFlush())
                {
                    player.hightH = Player.HighestHand.royalFlush;
                    player.CardValueHelper = 10;
                }
                else if (recoHand.isStraightFlush())
                {
                    player.hightH = Player.HighestHand.straightFlush;
                    player.CardValueHelper = 9;
                    Console.WriteLine("Straight Flush Value; " + recoHand.getHandValueRecoStraightFlush(recoHand.getFlush()));
                }
                else if (recoHand.isFourOfAKind())
                {
                    player.hightH = Player.HighestHand.fourOfAKind;
                    player.CardValueHelper = 8;
                    Console.WriteLine("Two Pair with:    " + recoHand.getHandValueRecoFourOfAKind());
                }
                else if (recoHand.isFullHouse())
                {
                    player.hightH = Player.HighestHand.fullHouse;
                    player.CardValueHelper = 7;
                    Console.WriteLine("Full House Value:    " + recoHand.getHandValueFullHouse());
                }
                else if (recoHand.isFlush())
                {
                    player.hightH = Player.HighestHand.flush;
                    player.CardValueHelper = 6;
                    player.RecoHand = recoHand.getHandValueRecoFlush();
                }
                else if (recoHand.isStraight(1) || recoHand.isStraight(0))
                {
                    player.hightH = Player.HighestHand.straight;
                    player.CardValueHelper = 5;
                    Console.WriteLine("Straight Value; " + recoHand.getHandValueRecoStraight());
                }
                else if (recoHand.isThreeOfAKind())
                {
                    player.hightH = Player.HighestHand.threeOfAKind;
                    player.CardValueHelper = 4;
                }
                else if (recoHand.isTwoPair())
                {
                    player.hightH = Player.HighestHand.twoPair;
                    player.CardValueHelper = 3;
                    Console.WriteLine("Two Pair with:    " + recoHand.getHandValueRecoTwoPair());
                }
                else if (recoHand.isPair())
                {
                    player.hightH = Player.HighestHand.onePair;
                    player.CardValueHelper = 2;
                    Console.WriteLine("Pair with:    " + recoHand.getHandValueRecoPair());
                }
                else
                {
                    player.hightH = Player.HighestHand.highCard;
                    player.CardValueHelper = 1;
                }
            }


            foreach (Player player in activePlayers)
            {
                Console.WriteLine(player.getPlayername() + "  " + player.hightH.ToString());
            }
        }


        public void checkWinner()
        {
            int highestHandValue = 0;
            Player winner = activeRoundPlayers[0];
            List<Player> winPlayerCheck = new List<Player>();

            //Auslesen der höchsten hand
            foreach (Player player in activeRoundPlayers)
            {
                if (player.CardValueHelper >= winner.CardValueHelper)
                {
                    winner = player;
                    highestHandValue = winner.CardValueHelper;

                }
            }

            //ermittele eventuelle duplicated Hand values
            foreach (Player player in activeRoundPlayers)
            {
                if (player.CardValueHelper == highestHandValue)
                {
                    winPlayerCheck.Add(player);
                }
            }


            int maxvalue = 0;
            foreach (Player player in winPlayerCheck)
            {
                maxvalue = player.getHand().getHandValue() + table.getTableValue();

                Console.WriteLine(player.getPlayername() + "   " + maxvalue);
            }


            foreach (Player player in winPlayerCheck)
            {
                if (player.CardValueHelper == maxvalue)
                {
                    winner = player;
                }
            }

            Console.WriteLine("Gewinner ist: " + winner.ToString());




            GewinnerOverlay.Visibility = Visibility.Visible;                            //GewinnerOverlay auf Sichtbar setzen
            Storyboard GewinnerStoryBoard = (Storyboard)FindResource("Gewinner");       //Storyboard für das Gewinner Overlay
            GewinnerStoryBoard.Begin(this);                                             //Gewinner Storyboard starten

            soundmanager.playSoundViele1();

            gewinnerTxt.Text = "" + activeRoundPlayers.ElementAt(0).getPlayername();    //Gewinner auf dem Storyboard ausgeben









            t1.Visibility = Visibility.Hidden;
            t2.Visibility = Visibility.Hidden;
            t3.Visibility = Visibility.Hidden;
            t4.Visibility = Visibility.Hidden;
            t5.Visibility = Visibility.Hidden;



            floppingBlindFoldHelper = false;



            //Pot dem Spieler berechnen
            winner.setBudget(activeRoundPlayers.ElementAt(0).getBudget() + pot);


            deck = new Deck();                      //Deck neu verteilen
            dealHands();                            //Hände neu geben
            setPlayerHands(activePlayers.Count);    //den Aktiven spielern die Hände geben
            table = new Table();                    //Tisch neue Karten geben
            dealTable();                            //Tisch neu dealen

            pot = 0;                                //Pot wieder auf 0 inialisieren

            soundmanager.playSoundViele1();

            flopping = false;
            turning = false;
            rivering = false;

            activeRoundPlayers.Clear();     //activeRound liste leeren diese wird gleich neu gesetzt da wieder mehrer Spieler aktiv in der Runde sind                         

            playRound = 0;


            blind.setPlayer(activePlayers);

            blind.switchBlindsAndDealer();  //Blinds weiter switchen

            foreach (Player player in activePlayers)
            {
                activeRoundPlayers.Add(player);

                player.setCurrentPlayer(false);
                if (player.PlayerBlindDealState == Player.BlindDealState.small)
                    player.setCurrentPlayer(true);
            }



            //Die activeRoundPlayers wieder mit den Spielern setzen die sich noch in der ActivePlayers liste befinden
            //also die Spieler die deren Budget noch > 0 ist
            foreach (Player player in activePlayers)
                activeRoundPlayers.Add(player);


            //nach der Runde raisen
            blind.raiseBlind(20);

            //Die States der Spieler auf unbekannt zurücksetzen
            foreach (Player player in activeRoundPlayers)
                player.PlayerState = Player.State.unknow;



            //Die Chips vom Tisch entfernen (POT Chips)
            foreach (Canvas chip in chipsOnTable)
                grid.Children.Remove(chip);

            //User Interface updaten
            updateUI();

        }



        /// <summary>
        /// A TagVisualizationDefinition that matches all tags.
        /// </summary>
        private class MatchEverythingDefinition : TagVisualizationDefinition
        {
            protected override bool Matches(TagData tag)
            {
                return true;
            }

            protected override Freezable CreateInstanceCore()
            {
                return new MatchEverythingDefinition();
            }
        }


        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("Green"))
                backgroundImg.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Backgrounds/pokertesttextur3.jpg"));
            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("Black"))
                backgroundImg.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Backgrounds/pokertesttextur2.jpg"));
            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("Red"))
                backgroundImg.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Backgrounds/pokertesttextur5.jpg"));
            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("Blue"))
                backgroundImg.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Backgrounds/pokertesttextur8.jpg"));
            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("Grey"))
                backgroundImg.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Backgrounds/pokertesttextur12.jpg"));

            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("Anleitung"))
            {
                Anleitung.Visibility = Visibility.Visible;
            }


            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("SpielerhinzuScatter"))
            {
                SpielerhinzuScatter.Visibility = Visibility.Visible;
            }

            //Sound anschalten
            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("an"))
            {
                soundmanager.Sound = true;
            }

            //Sound ausschalten
            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("aus"))
            {
                soundmanager.Sound = false;
            }


            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("2 Spieler"))
            {
                Storyboard TagHelpText = (Storyboard)FindResource("AddTagsStoryboard");
                TagHelpText.Begin(this);
                gamemode = 2;
                ElementMenuItemPause.Visibility = Visibility.Visible;
                ElementMenuItemSpielStarten.Visibility = Visibility.Hidden;
            }

            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("3 Spieler"))
            {
                Storyboard TagHelpText = (Storyboard)FindResource("AddTagsStoryboard");
                TagHelpText.Begin(this);
                gamemode = 3;
                ElementMenuItemPause.Visibility = Visibility.Visible;
                ElementMenuItemSpielStarten.Visibility = Visibility.Hidden;
            }

            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("4 Spieler"))
            {
                Storyboard TagHelpText = (Storyboard)FindResource("AddTagsStoryboard");
                TagHelpText.Begin(this);
                gamemode = 4;
                ElementMenuItemPause.Visibility = Visibility.Visible;
                ElementMenuItemSpielStarten.Visibility = Visibility.Hidden;
            }


            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("Pause"))
            {
                gameLoop.Stop();
                PauseOverlay.Visibility = Visibility.Visible;

                Storyboard pauseStart = (Storyboard)FindResource("Pause");
                pauseStart.Begin(this);
                ElementMenuItemFortsetzen.Visibility = Visibility.Visible;
                ElementMenuItemPause.Visibility = Visibility.Hidden;

            }

            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("Fortsetzen"))
            {
                gameLoop.Start();

                Storyboard pauseRemove = (Storyboard)FindResource("PauseRemove");
                ElementMenuItemFortsetzen.Visibility = Visibility.Hidden;
                ElementMenuItemPause.Visibility = Visibility.Visible;
                pauseRemove.Begin(this);

            }

            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("5 Spieler"))
            {
                Storyboard TagHelpText = (Storyboard)FindResource("AddTagsStoryboard");
                TagHelpText.Begin(this);
                gamemode = 5;
                ElementMenuItemPause.Visibility = Visibility.Visible;
                ElementMenuItemSpielStarten.Visibility = Visibility.Hidden;
            }

            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("6 Spieler"))
            {
                Storyboard TagHelpText = (Storyboard)FindResource("AddTagsStoryboard");
                TagHelpText.Begin(this);
                gamemode = 6;
                ElementMenuItemPause.Visibility = Visibility.Visible;
                ElementMenuItemSpielStarten.Visibility = Visibility.Hidden;
            }

            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("Deck 1"))
            {
                alternativeDeck = true;
            }

            if ((e.OriginalSource as ElementMenuItem).Header.ToString().Equals("Deck 2"))
            {
                alternativeDeck = false;
            }

        }


        /// <summary>
        /// Cips der Spieler initalisieren
        /// die Koordinaten die relevant sind setzen
        /// nähere in der Chip Klasse
        /// </summary>
        public void initChips()
        {

            player1.setChip(new Chip(p1c10, 300, 750, 208, 10, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_10.png")));
            player1.setChip(new Chip(p1c20, 300, 665, 208, 20, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_20.png")));
            player1.setChip(new Chip(p1c50, 300, 584, 208, 50, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_50.png")));
            player1.setChip(new Chip(p1c100, 300, 498, 208, 100, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_100.png")));
            player1.setChip(new Chip(p1c200, 300, 409, 208, 200, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_200.png")));
            player1.setChip(new Chip(p1c500, 300, 318, 208, 500, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_500.png")));

            player2.setChip(new Chip(p2c10, 780, 320, 836, 10, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_10.png")));
            player2.setChip(new Chip(p2c20, 780, 410, 839, 20, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_20.png")));
            player2.setChip(new Chip(p2c50, 780, 490, 839, 50, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_50.png")));
            player2.setChip(new Chip(p2c100, 780, 570, 838, 100, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_100.png")));
            player2.setChip(new Chip(p2c200, 780, 665, 838, 200, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_200.png")));
            player2.setChip(new Chip(p2c500, 780, 750, 839, 500, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_500.png")));

            player3.setChip(new Chip(p3c10, 300, 218, 320, 10, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_10.png")));
            player3.setChip(new Chip(p3c20, 300, 218, 403, 20, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_20.png")));
            player3.setChip(new Chip(p3c50, 300, 218, 489, 50, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_50.png")));
            player3.setChip(new Chip(p3c100, 300, 218, 573, 100, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_100.png")));
            player3.setChip(new Chip(p3c200, 300, 218, 664, 200, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_200.png")));
            player3.setChip(new Chip(p3c500, 300, 218, 752, 500, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_500.png")));

            player4.setChip(new Chip(p4c10, 300, 1560, 209, 10, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_10.png")));
            player4.setChip(new Chip(p4c20, 300, 1485, 209, 20, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_20.png")));
            player4.setChip(new Chip(p4c50, 300, 1402, 208, 50, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_50.png")));
            player4.setChip(new Chip(p4c100, 300, 1309, 210, 100, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_100.png")));
            player4.setChip(new Chip(p4c200, 300, 1215, 209, 200, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_200.png")));
            player4.setChip(new Chip(p4c500, 300, 1130, 210, 500, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_500.png")));

            player5.setChip(new Chip(p5c10, 780, 1131, 831, 10, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_10.png")));
            player5.setChip(new Chip(p5c20, 780, 1210, 831, 20, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_20.png")));
            player5.setChip(new Chip(p5c50, 780, 1300, 829, 50, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_50.png")));
            player5.setChip(new Chip(p5c100, 780, 1383, 832, 100, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_100.png")));
            player5.setChip(new Chip(p5c200, 780, 1475, 832, 200, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_200.png")));
            player5.setChip(new Chip(p5c500, 780, 1574, 832, 500, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_500.png")));

            player6.setChip(new Chip(p6c10, 1590, 1707, 748, 10, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_10.png")));
            player6.setChip(new Chip(p6c20, 1590, 1704, 666, 20, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_20.png")));
            player6.setChip(new Chip(p6c50, 1590, 1706, 581, 50, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_50.png")));
            player6.setChip(new Chip(p6c100, 1590, 1707, 498, 100, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_100.png")));
            player6.setChip(new Chip(p6c200, 1590, 1707, 404, 200, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_200.png")));
            player6.setChip(new Chip(p6c500, 1590, 1706, 317, 500, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_500.png")));
        }


        /// <summary>
        /// Interaktionslogik der Chips und das erkennen wann ein Chip gesetzt wurde
        /// </summary>
        private void moveChip(object sender, System.Windows.Input.TouchEventArgs e)
        {
            foreach (Player player in activePlayers)
            {
                foreach (Chip chip in player.getChips())
                {

                    int pointy = (Int16)chip.PlayerChip.Center.Y;
                    int pointx = (Int16)chip.PlayerChip.Center.X;

                    // Console.WriteLine(pointy);

                    if (player.Orientation == 1)
                    {
                        if ((Int16)chip.PlayerChip.Center.Y >= chip.ControlLine)
                        {
                            player.Gesetzt += chip.ChipValue;

                            Image image = new Image();
                            image.Source = new BitmapImage(chip.ImagePath);

                            Canvas canvas = new Canvas();
                            canvas.Children.Add(image);

                            soundmanager.playSoundEinChip2();

                            double y = pointy - 42.5;
                            double x = pointx - 42.5;

                            image.SetValue(Canvas.LeftProperty, x);
                            image.SetValue(Canvas.TopProperty, y);

                            image.Width = 85;
                            image.Height = 85;

                            chipsOnTable.Add(canvas);
                            grid.Children.Add(canvas);



                            Point p = new Point();
                            p.X = chip.X;
                            p.Y = chip.Y;

                            chip.PlayerChip.SetValue(ScatterViewItem.CenterProperty, p);



                        }
                    }

                    else if (player.Orientation == 2)
                    {
                        if ((Int16)chip.PlayerChip.Center.Y <= chip.ControlLine)
                        {
                            player.Gesetzt += chip.ChipValue;


                            Image image = new Image();
                            image.Source = new BitmapImage(chip.ImagePath);

                            Canvas canvas = new Canvas();
                            canvas.Children.Add(image);

                            soundmanager.playSoundEinChip2();

                            double y = pointy - 42.5;
                            double x = pointx - 42.5;

                            image.SetValue(Canvas.LeftProperty, x);
                            image.SetValue(Canvas.TopProperty, y);

                            image.Width = 85;
                            image.Height = 85;

                            chipsOnTable.Add(canvas);
                            grid.Children.Add(canvas);

                            Point p = new Point();
                            p.X = chip.X;
                            p.Y = chip.Y;

                            chip.PlayerChip.SetValue(ScatterViewItem.CenterProperty, p);
                        }

                    }
                    else if (player.Orientation == 3)
                    {
                        if ((Int16)chip.PlayerChip.Center.X >= chip.ControlLine)
                        {
                            player.Gesetzt += chip.ChipValue;

                            Image image = new Image();
                            image.Source = new BitmapImage(chip.ImagePath);

                            Canvas canvas = new Canvas();
                            canvas.Children.Add(image);

                            soundmanager.playSoundEinChip2();

                            double y = pointy - 42.5;
                            double x = pointx - 42.5;

                            image.SetValue(Canvas.LeftProperty, x);
                            image.SetValue(Canvas.TopProperty, y);

                            image.Width = 85;
                            image.Height = 85;

                            chipsOnTable.Add(canvas);
                            grid.Children.Add(canvas);



                            Point p = new Point();
                            p.X = chip.X;
                            p.Y = chip.Y;

                            chip.PlayerChip.SetValue(ScatterViewItem.CenterProperty, p);



                        }


                    }
                    else if (player.Orientation == 4)
                    {
                        if ((Int16)chip.PlayerChip.Center.X <= chip.ControlLine)
                        {
                            player.Gesetzt += chip.ChipValue;

                            Image image = new Image();
                            image.Source = new BitmapImage(chip.ImagePath);

                            Canvas canvas = new Canvas();
                            canvas.Children.Add(image);

                            soundmanager.playSoundEinChip2();

                            double y = pointy - 42.5;
                            double x = pointx - 42.5;

                            image.SetValue(Canvas.LeftProperty, x);
                            image.SetValue(Canvas.TopProperty, y);

                            image.Width = 85;
                            image.Height = 85;

                            chipsOnTable.Add(canvas);
                            grid.Children.Add(canvas);



                            Point p = new Point();
                            p.X = chip.X;
                            p.Y = chip.Y;

                            chip.PlayerChip.SetValue(ScatterViewItem.CenterProperty, p);



                        }


                    }


                }
            }

        }


        private void Fold(object sender, System.Windows.Input.TouchEventArgs e)
        {

            Boolean p1FoldBlind = false;

            if (player1.PlayerBlindDealState == Player.BlindDealState.small)
                if (playRound < 1 && !floppingBlindFoldHelper)
                    p1FoldBlind = false;
                else
                    p1FoldBlind = true;

            Boolean p2FoldBlind = false;

            if (player2.PlayerBlindDealState == Player.BlindDealState.small)
                if (playRound < 1 && !floppingBlindFoldHelper)
                    p2FoldBlind = false;
                else
                    p2FoldBlind = true;

            Boolean p3FoldBlind = false;

            if (player3.PlayerBlindDealState == Player.BlindDealState.small)
                if (playRound < 1 && !floppingBlindFoldHelper)
                    p3FoldBlind = false;
                else
                    p3FoldBlind = true;

            Boolean p4FoldBlind = false;

            if (player4.PlayerBlindDealState == Player.BlindDealState.small)
                if (playRound < 1 && !floppingBlindFoldHelper)
                    p4FoldBlind = false;
                else
                    p4FoldBlind = true;

            Boolean p5FoldBlind = false;

            if (player5.PlayerBlindDealState == Player.BlindDealState.small)
                if (playRound < 1 && !floppingBlindFoldHelper)
                    p5FoldBlind = false;
                else
                    p5FoldBlind = true;

            Boolean p6FoldBlind = false;

            if (player6.PlayerBlindDealState == Player.BlindDealState.small)
                if (playRound < 1 && !floppingBlindFoldHelper)
                    p6FoldBlind = false;
                else
                    p6FoldBlind = true;

            if ((e.OriginalSource as SurfaceButton).Name.Equals("player1Fold") && player1.getIsCurrentPlayer() && p1FoldBlind)
                player1.PlayerState = Player.State.fold;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player2Fold") && player2.getIsCurrentPlayer() && p2FoldBlind)
                player2.PlayerState = Player.State.fold;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player3Fold") && player3.getIsCurrentPlayer() && p3FoldBlind)
                player3.PlayerState = Player.State.fold;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player4Fold") && player4.getIsCurrentPlayer() && p4FoldBlind)
                player4.PlayerState = Player.State.fold;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player5Fold") && player5.getIsCurrentPlayer() && p5FoldBlind)
                player5.PlayerState = Player.State.fold;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player6Fold") && player6.getIsCurrentPlayer() && p6FoldBlind)
                player6.PlayerState = Player.State.fold;
        }

        private void Check(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player1Check") && player1.getIsCurrentPlayer())
                player1.PlayerState = Player.State.check;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player2Check") && player2.getIsCurrentPlayer())
                player2.PlayerState = Player.State.check;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player3Check") && player3.getIsCurrentPlayer())
                player3.PlayerState = Player.State.check;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player4Check") && player4.getIsCurrentPlayer())
                player4.PlayerState = Player.State.check;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player5Check") && player5.getIsCurrentPlayer())
                player5.PlayerState = Player.State.check;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player6Check") && player6.getIsCurrentPlayer())
                player6.PlayerState = Player.State.check;

        }


        private void Bet(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player1Bet") && (player1.getIsCurrentPlayer()) && (player1.Gesetzt >= blind.minBetValue(player1, playRound)))
                player1.PlayerState = Player.State.bet;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player2Bet") && (player2.getIsCurrentPlayer()) && (player2.Gesetzt >= blind.minBetValue(player2, playRound)))
                player2.PlayerState = Player.State.bet;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player3Bet") && (player3.getIsCurrentPlayer()) && (player3.Gesetzt >= blind.minBetValue(player3, playRound)))
                player3.PlayerState = Player.State.bet;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player4Bet") && (player4.getIsCurrentPlayer()) && (player4.Gesetzt >= blind.minBetValue(player4, playRound)))
                player4.PlayerState = Player.State.bet;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player5Bet") && (player5.getIsCurrentPlayer()) && (player5.Gesetzt >= blind.minBetValue(player5, playRound)))
                player5.PlayerState = Player.State.bet;
            if ((e.OriginalSource as SurfaceButton).Name.Equals("player6Bet") && (player6.getIsCurrentPlayer()) && (player6.Gesetzt >= blind.minBetValue(player6, playRound)))
                player6.PlayerState = Player.State.bet;
        }






        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        private void closeHelp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            Anleitung.Visibility = Visibility.Hidden;
        }









    }
}