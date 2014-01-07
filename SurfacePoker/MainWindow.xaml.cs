using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface.Presentation.Controls;

namespace SurfacePoker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<Player> players { get; set; }
      
        private bool canAddPlayer { get; set; }

        private int position { get; set; }

        private int stack { get; set; }

        private System.Windows.Controls.Button btn;

        private Game gl;

        private bool startNewGame;

        private KeyValuePair<Player, List<Action>> kvp;

        private void shutDown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0); 
        }

        public MainWindow()
        {
            canAddPlayer = true;
            startNewGame = true;
            position = 0;
            stack = 1000;
            btn = new Button();
            LinearGradientBrush gradientBrush = new  LinearGradientBrush( Color.FromRgb( 24, 24, 24),  Color.FromRgb(47, 47, 47), new Point(0.5, 0), new Point(0.5, 1));            
            Background = gradientBrush;
            btn.Background = Background;
            //Player player1 = new Player("Anton", 1000, 1);
            //Player player2 = new Player("Berta", 1000, 2);
            //Player player3 = new Player("Cäsar", 1000, 3);
            //Player player4 = new Player("Dora", 1000, 4);
            //Player player5 = new Player("Emil", 1000, 5);
            //Player player6 = new Player("Friedrich", 1000, 6);
            players = new List<Player>();
            //players.Add(player1);
            //players.Add(player2);
            //players.Add(player3);
            //players.Add(player4);
            //players.Add(player5);
            //players.Add(player6);
            //gl = new Game(players, 100, 50);
            
            bool b = false;

        }
        private void openAddPlayer(object sender, RoutedEventArgs e)
        {
            //TODO
            //prüfen ob an dieser stelle schon ein Spieler sitzt
            if (canAddPlayer) {
                Rectangle r = (Rectangle)sender;
                String name = r.Name;
                Console.Out.WriteLine("Spieler an" + name + "meldet sich an");
                Point point = new Point();

                switch (r.Name)
                {
                    case "Pos1": point.X = 270; point.Y = 550; position = 1; break;
                    case "Pos2": point.X = 470; point.Y = 170; position = 2; break;
                    case "Pos3": point.X = 1210; point.Y = 170; position = 3; break;
                    case "Pos4": point.X = 1620; point.Y = 550; position = 4; break;
                    case "Pos5": point.X = 1210; point.Y = 930; position = 5; break;
                    case "Pos6": point.X = 470; point.Y = 930; position = 6; break;
                }
                addplayerscatteru.Center = point;
                addplayerscatteru.Visibility = Visibility.Visible;                
            }
            e.Handled = true;
        }
        private void closeAddPlayer(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;
            StackPanel stackPanel = (StackPanel)s.Parent;
            SurfaceTextBox text = (SurfaceTextBox)stackPanel.FindName("playerName");
            text.Text = "";
            position = 0;
            addplayerscatteru.Visibility = Visibility.Collapsed;
            addStartButton();
            e.Handled = true;
        }

        private void setSurfaceName(int pos, String name) {
            switch (pos)
            {
                case 1: 
                    player1name.Text = name;
                    break;
                case 2:
                    player2name.Text = name;
                    break;
                case 3:
                    player3name.Text = name;
                    break;
                case 4:
                    player4name.Text = name;
                    break;
                case 5:
                    player5name.Text = name;
                    break;
                case 6:
                    player6name.Text = name;
                    break;

            }
        }

        private void savePlayer(object sender, RoutedEventArgs e)
        {
           
            SurfaceButton s = (SurfaceButton)sender;
            StackPanel stackPanel = (StackPanel)s.Parent;
            SurfaceTextBox text = (SurfaceTextBox)stackPanel.FindName("playerName");
            setSurfaceName(position, text.Text);

            //TODO Wenn schon ein Spieler am Platz sitz dann Namen überschreiben
            players.Add(new Player(text.Text, stack, position));            

            text.Text = "";
            addplayerscatteru.Visibility = Visibility.Collapsed;
            if (players.Count >= 2)
            {
                addStartButton();
            }
            e.Handled = true;

        }

        private void addStartButton()
        {
            btn.Name = "btnStart";
            btn.Content = "Spiel starten";
            btn.Margin = new Thickness(800, 700, 800, 320);
            btn.Click += new RoutedEventHandler(startGame);         
            if (!Grid.Children.Contains(btn))
            {

                Grid.Children.Add(btn);
            }
        }

        private void startGame(object sender, RoutedEventArgs e)
        {
            //Disable adding new players and hide poition fields
            canAddPlayer = false;
            Pos1.Visibility = Visibility.Collapsed;
            Pos2.Visibility = Visibility.Collapsed;
            Pos3.Visibility = Visibility.Collapsed;
            Pos4.Visibility = Visibility.Collapsed;
            Pos5.Visibility = Visibility.Collapsed;
            Pos6.Visibility = Visibility.Collapsed;
            //Remove 'start new game' button
            Grid.Children.Remove(btn);
            //Start new Game
            gl = new Game(players,100,50);
            kvp = gl.newGame();
            showCards();
            showActionButton(kvp);
            e.Handled = true;

        }

        private void showCards()
        {
            foreach (Player iPlayer in gl.players.FindAll(x => x.isActive))
            {
                switch (iPlayer.position)
                {
                    case 1:
                        Pos1.Visibility = Visibility.Visible;
                        player1interface.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        Pos2.Visibility = Visibility.Visible;
                        player2interface.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        Pos3.Visibility = Visibility.Visible;
                        player3interface.Visibility = Visibility.Visible;
                        break;
                    case 4:
                        Pos4.Visibility = Visibility.Visible;
                        player4interface.Visibility = Visibility.Visible;
                        break;
                    case 5:
                        Pos5.Visibility = Visibility.Visible;
                        player5interface.Visibility = Visibility.Visible;
                        break;
                    case 6:
                        Pos6.Visibility = Visibility.Visible;
                        player6interface.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        
        private void turnToFront(object sender, RoutedEventArgs e)
        {
            Image s = (Image)sender;
            ScatterViewItem svi = (ScatterViewItem)s.Parent;
            //pos gets position from player - can be 1-6
            int pos = (Int32)Convert.ToInt32(svi.Name[6].ToString());
            //pcard gets which card from player - can be 1 or 2
            int pcard = (Int32)Convert.ToInt32(svi.Name[11].ToString());
            
            //Console.WriteLine(gl.players.Find(x => x.position == pos).cards[pcard - 1]);
            
            BitmapImage bmpimage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.players.Find(x => x.position == pos).cards[pcard - 1] + ".png"));
                Image image = (Image)sender;
                image.Source = bmpimage;
                e.Handled = true;
        }

        private void turnToBack(object sender, RoutedEventArgs e)
        {

            BitmapImage bmpimage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Kartenrueckseite/kartenruecken_1.jpg"));
            Image image = (Image)sender;
            image.Source = bmpimage;
            e.Handled = true;           
        }
        /// <summary>
        /// Shows the two Actionbuttons at players position 
        /// </summary>
        /// <param name="pos"></param>
        private void showActionButton(KeyValuePair<Player,List<Action>> ikvp)
        {
            //<Thickness x:Key="p1">50,400,1600,400</Thickness>
            //<Thickness x:Key="p2">450,50,1190,810</Thickness>
            //<Thickness x:Key="p3">1190,50,450,810</Thickness>
            //<Thickness x:Key="p4">1600,400,50,400</Thickness>
            //<Thickness x:Key="p5">1190,810,450,50</Thickness>
            //<Thickness x:Key="p6">450,810,1190,50</Thickness>
            Thickness t;
            String action = "";

            if (ikvp.Value.Exists(x => x.action == Action.playerAction.check))
            {
                action += ikvp.Value.Find(x => x.action == Action.playerAction.check).action.ToString();
            }
            else
            {
                action += ikvp.Value.Find(x => x.action == Action.playerAction.call).action.ToString() + " " + ikvp.Value.Find(x => x.action == Action.playerAction.call).amount;
            }

            switch (ikvp.Key.position)
            {
                case 1: 
                    t = new Thickness(50, 400, 1600, 400);
                    Buttons_v.Margin = t;
                    Buttons_v.RenderTransform = new RotateTransform(90);
                    buttonAction_v.Content = action;
                    Buttons_v.Visibility = Visibility.Visible;
                    break;
                case 2: 
                    t = new Thickness(450,50,1190,810);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(180);
                    buttonAction_h.Content = action;
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
                case 3:
                    t = new Thickness(1190, 50, 450, 810);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(180);
                    buttonAction_h.Content = action;
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
                case 4:
                    t = new Thickness(1600, 400, 50, 400);
                    Buttons_v.Margin = t;
                    Buttons_v.RenderTransform = new RotateTransform(-90);
                    buttonAction_v.Content = action;
                    Buttons_v.Visibility = Visibility.Visible;
                    break;
                case 5:
                    t = new Thickness(1190, 810, 450, 50);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(0);
                    buttonAction_h.Content = action;
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
                case 6:
                    t = new Thickness(450, 810, 1190, 50);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(0);
                    buttonAction_h.Content = action;
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
            }          

        }
        /// <summary>
        /// Hides the Grid for the two action buttons
        /// </summary>
        private void hideActionButton()
        {
            Buttons_h.Visibility = Visibility.Collapsed;
            Buttons_v.Visibility = Visibility.Collapsed;
            
        }

        private void actionButtonClicked(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;
            switch (s.Content.ToString().Split(' ')[0])
            {
                case "check":
                    gl.activeAction(Action.playerAction.check, 0);                    
                    break;
                case "call":
                    gl.activeAction(Action.playerAction.call, (Int32)Convert.ToInt32(s.Content.ToString().Split(' ')[1]));
                    break;
                case "bet":
                    gl.activeAction(Action.playerAction.bet, (Int32)Convert.ToInt32(s.Content.ToString().Split(' ')[1]));
                    break;
                case "raise":
                    gl.activeAction(Action.playerAction.raise, (Int32)Convert.ToInt32(s.Content.ToString().Split(' ')[1]));
                    break;
            }
            hideActionButton();
            try
            {
                kvp = gl.nextPlayer();
                showActionButton(kvp);
            }
            catch (EndRoundException exp)
            {
                throw exp;
            }
            e.Handled = true;
        }

    }
}
