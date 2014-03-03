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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation;
using System.Collections.ObjectModel;
using Log;
using log4net;
using log4net.Config;

namespace SurfacePoker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<Player> players { get; set; }
      
        private bool canAddPlayer { get; set; }

        private bool canChangeStack { get; set; }

        private int stack { get; set; }

        private System.Windows.Controls.Button btn;

        private Game gl {get; set;}

        private int round;

        private KeyValuePair<Player, List<Action>> kvp;

        private int personalStack { get; set; }

        private int showCardDelay { get; set; }

        private int bb = 20;

        private bool oneChipCall { get; set; }

        private void shutDown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
            e.Handled = true;
        }

        public MainWindow()
        {
            log.Debug("MainWindow() - Begin");
            btn = new Button();
            LinearGradientBrush gradientBrush = new  LinearGradientBrush( Color.FromRgb( 24, 24, 24),  Color.FromRgb(47, 47, 47), new Point(0.5, 0), new Point(0.5, 1));            
            Background = gradientBrush;
            btn.Background = Background;
            //Player player1 = new Player("Anton", 1000, 1);
            //Player player2 = new Player("Berta", 500, 2);
            //Player player3 = new Player("Cäsar", 1000, 3);
            //Player player4 = new Player("Dora", 1000, 4);
            //Player player5 = new Player("Emil", 1000, 5);
            //Player player6 = new Player("Friedrich", 1000, 6);
            //players.Add(player1);
            //players.Add(player2);
            //players.Add(player3);
            //players.Add(player4);
            //players.Add(player5);
            //players.Add(player6);
            //DataContext = this;
            log.Debug("MainWindow() - End");
        }

        private void createNewGame(object sender, RoutedEventArgs e)
        {
            log.Debug("createNewGame(object: "+ sender.ToString() + " RoutedEventArgs: " +e.ToString() + ") - Begin");
            TextBlock tb;
            Rectangle r;
            //set mainPot Font to default
            mainPot.FontSize = 16;
            mainPot.Foreground = Brushes.Bisque;
            //Set all names and stacks to null
            for (int i = 1; i <= 6; i++)
            {
                r = this.FindName("Pos" + i) as Rectangle;
                r.Visibility = Visibility.Visible;
                setSurfaceName(i, "");
                tb = this.FindName("player" + i + "balance") as TextBlock;
                tb.Text = "";
            }
            Mitte.Visibility = Visibility.Collapsed;
            //Hide Startbtn
            if (Grid.Children.Contains(btn))
            {
                Grid.Children.Remove(btn);
            }
            addplayerscatteru.Visibility = Visibility.Collapsed;
            playerName.Text = "";
            hideUI();
            hideActionButton();
            round = 0;
            personalStack = 0;
            showCardDelay = 1500;
            canAddPlayer = true;
            canChangeStack = true;
            EMIchangeStack.IsEnabled = true;
            //if no stack value is given set default
            if (stack == 0)
            {
                stack = 1000;
            }
            mainPot.Text = "Touch Grey Area To Create A New Player!             Player Stacks: " + stack.ToString();
            players = new List<Player>();
            gl = null;
            log.Debug("createNewGame() - End");
            e.Handled = true;
        }

        private void showAddPlayer(object sender, RoutedEventArgs e)
        {
            log.Debug("openAddPlayer(object: " + sender.ToString() + " RoutedEventArgs: " + e.ToString() + ") - Begin");
            if (canAddPlayer) {
                Rectangle r = (Rectangle)sender;
                String name = r.Name;
                //Console.Out.WriteLine("Spieler an" + name + "meldet sich an");
                Point point = new Point();

                switch (r.Name)
                {
                    case "Pos1": point.X = 270; point.Y = 550; playerPos.Text = "1"; break;
                    case "Pos2": point.X = 470; point.Y = 170; playerPos.Text = "2"; break;
                    case "Pos3": point.X = 1210; point.Y = 170; playerPos.Text = "3"; break;
                    case "Pos4": point.X = 1620; point.Y = 550; playerPos.Text = "4"; break;
                    case "Pos5": point.X = 1210; point.Y = 930; playerPos.Text = "5"; break;
                    case "Pos6": point.X = 470; point.Y = 930; playerPos.Text = "6"; break;
                }
                addplayerscatteru.Center = point;
                addplayerscatteru.Visibility = Visibility.Visible;                
            }
            log.Debug("openAddPlayer() - End");
            e.Handled = true;
        }

        /// <summary>
        /// Set the Visibility for HandRanking Window to true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showHandRanking(object sender, RoutedEventArgs e)
        {
            log.Debug("showHandRanking(object: " + sender.ToString() + " RoutedEventArgs: " + e.ToString() + ") - Begin");
            HandRanking.Visibility = Visibility.Visible;            
            log.Debug("showHandRanking() - End");
            e.Handled = true;
        }

        /// <summary>
        /// Hides the sender Window, sender has to be a Button in StackPanel, ScatterViewItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeWindow(object sender, RoutedEventArgs e)
        {
            log.Debug("closeWindow(object: " + sender.ToString() + " RoutedEventArgs: " + e.ToString() + ") - Begin");
            Button s = sender as Button;
            StackPanel stackPanel = s.Parent as StackPanel;
            ScatterViewItem SVI = stackPanel.Parent as ScatterViewItem;
            log.Debug("closing " + SVI.Name);
            if (SVI.Name == "addplayerscatteru")
            {
                //clean up
                SurfaceTextBox text = (SurfaceTextBox)stackPanel.FindName("playerName");
                text.Text = "";
                Label l = this.FindName("LabelAddPlayer") as Label;
                l.Foreground = Brushes.White;
                l.Content = "Add New Player";
            }
            SVI.Visibility = Visibility.Collapsed;
            log.Debug("closeWindow() - End");
            e.Handled = true;
        }

        /// <summary>
        /// set var round to 0 and adds start button
        /// </summary>
        private void newRound()
        {
            log.Debug("newRound() - Begin");
            round = 0;
            addStartButton("Start New Round");
            log.Debug("newRound() - End");
        }

        /// <summary>
        /// writes the player name on the screen at position (1-6)
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="name"></param>
        private void setSurfaceName(int pos, String name) {
            log.Debug("setSurfaceName(pos: " + pos.ToString() + " name: " + name.ToString() + ") - Begin");
            TextBlock tb = this.FindName("player" + pos + "name") as TextBlock;
            tb.Text = name;
            log.Debug("setSurfaceName() - End");
        }

        /// <summary>
        /// Creates new player or updates name if player exists at this position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePlayer(object sender, RoutedEventArgs e)
        {
            log.Debug("savePlayer(object: " + sender.ToString() + " RoutedEventArgs: " + e.ToString() + ") - Begin");
            
            if (canAddPlayer) { 
            
                SurfaceButton s = (SurfaceButton)sender;
                StackPanel stackPanel = (StackPanel)s.Parent;
                SurfaceTextBox tb = (SurfaceTextBox)stackPanel.FindName("playerName");
                Label l = this.FindName("LabelAddPlayer") as Label;
                int pos = Convert.ToInt32(playerPos.Text);

                //check if player name is taken
                if (players.Exists(x => x.name == tb.Text)) {
                    //player name taken
                    l.Foreground = Brushes.Red;
                    l.Content = "Choose Another Name";
                    tb.Text = "";
                } 
                else
                {
                    setSurfaceName(pos, tb.Text);

                    //check if player allready exits at pos 
                    if (!(players.Exists(x => x.position == pos)))
                    {
                        //add new player at pos
                        players.Add(new Player(tb.Text, stack, pos));
                    }
                    else
                    {
                        //updates Player name at pos
                        players.Find(x => x.position == pos).name = tb.Text;
                    }

                    //clean up
                    l.Foreground = Brushes.White;
                    l.Content = "Add New Player";
                    tb.Text = "";
                    addplayerscatteru.Visibility = Visibility.Collapsed;

                    //add start btn if two or more players
                    if (players.Count >= 2)
                    {
                        addStartButton("Start Game");
                    }
                }
            }
            log.Debug("savePlayer() - End");
            e.Handled = true;

        }

        /// <summary>
        /// Adds a Button in the middle of the screen with the given text as content
        /// </summary>
        /// <param name="text"></param>
        private void addStartButton(String text)
        {
            log.Debug("addStartButton(text: " + text.ToString() + ") - Begin");
            btn.Name = "btnStart";
            btn.Content = text;
            btn.Margin = new Thickness(900, 700, 900, 320);
            btn.Click += new RoutedEventHandler(StartButtonClicked);         
            if (!Grid.Children.Contains(btn))
            {
                Grid.Children.Add(btn);
            }
            log.Debug("addStartButton() - End");
        }

        /// <summary>
        /// Removes StartButton, disables adding new players/ changing stacks, starts new game/Round
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartButtonClicked(object sender, RoutedEventArgs e)
        {
            log.Debug("StartButtonClicked() - Begin");
            //Disable adding new players, changing stacks and hide position fields
            canAddPlayer = false;
            addplayerscatteru.Visibility = Visibility.Collapsed;
            playerName.Text = "";
            EMIchangeStack.IsEnabled = false;
            canChangeStack = false;

            Pos1.Visibility = Visibility.Collapsed;
            Pos2.Visibility = Visibility.Collapsed;
            Pos3.Visibility = Visibility.Collapsed;
            Pos4.Visibility = Visibility.Collapsed;
            Pos5.Visibility = Visibility.Collapsed;
            Pos6.Visibility = Visibility.Collapsed;
            hideUI();
            //Remove 'start new game' button
            Grid.Children.Remove(btn);
            //set mainPot to default
            mainPot.FontSize = 16;
            mainPot.Foreground = Brushes.Bisque;
            mainPot.Text = "";
            e.Handled = true;

            await startGame();
            log.Debug("StartButtonClicked() - End");
        }

        /// <summary>
        /// Starts a new round or if game does not exist creates new game and start new round
        /// </summary>
        private async Task startGame()
        {
            log.Debug("startGame() - Begin");
            await Task.Delay(5);
            if (gl == null)
            {
                gl = new Game(players, bb, bb / 2);
            }
            //set dealer button to pos
            Player p = gl.newGame();
            setDealerButtonPos(p.position);
            //get first player
            kvp = gl.nextPlayer();
            Mitte.Visibility = Visibility.Visible;
            showCards();
            showActionButton(kvp);
            log.Debug("startGame() - End");
            
        }

        /// <summary>
        /// deals player cards to all active players
        /// </summary>
        private void showCards()
        {
            log.Debug("showCards() - Begin");
            foreach (Player iPlayer in gl.players.FindAll(x => x.isActive))
            {
                Rectangle r = this.FindName("Pos" + iPlayer.position) as Rectangle;
                r.Visibility = Visibility.Visible;
                ScatterView sv = this.FindName("player" + iPlayer.position + "cards") as ScatterView;
                sv.Visibility = Visibility.Visible;
                setBackground(iPlayer.position,1);
                setBackground(iPlayer.position,2);
            }
            log.Debug("showCards() - End");
        }

        /// <summary>
        /// sets the Backgound with the true card value eg: ad, 8c
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="pcard"></param>
        private void setBackground(int pos, int pcard)
        {
            log.Debug("setBackground(pos: " + pos.ToString() + " pcard: " + pcard.ToString() + ") - Begin");
            if (gl.players.Exists(x => x.position == pos))
            {
                if (gl.players.Find(x => x.position == pos).cards.Count != 0)
                {
                    ScatterViewItem svi = this.FindName("player" + pos + "card" + pcard ) as ScatterViewItem;
                    BitmapImage bmpimage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.players.Find(x => x.position == pos).cards[pcard - 1] + ".png"));

                    ImageBrush imb = new ImageBrush();
                    imb.ImageSource = bmpimage;

                    svi.Background = imb;
                }
                else
                {
                    //Console.WriteLine("Yay");
                }
            }
            log.Debug("setBackground() - End");
        }

        /// <summary>
        /// show cards from player at pos
        /// </summary>
        /// <param name="pos"></param>
        private void turnCardsToFront(int pos)
        {
            log.Debug("turnCardsToFront(pos: " + pos.ToString() + ") - Begin");
            if (gl.players.Exists(x => x.position == pos))
            {
                if (gl.players.Find(x => x.position == pos).cards.Count != 0)
                {
                    BitmapImage bmpimage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.players.Find(x => x.position == pos).cards[0] + ".png"));
                    BitmapImage bmpimage2 = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.players.Find(x => x.position == pos).cards[1] + ".png"));
                    Image image = this.FindName("player" + pos + "card1image") as Image;
                    image.Source = bmpimage;
                    image = this.FindName("player" + pos + "card2image") as Image;
                    image.Source = bmpimage2;
                    ScatterView sv = this.FindName("player" + pos + "cards") as ScatterView;
                    sv.Visibility = Visibility.Visible;
                }
                else
                {
                    //Console.WriteLine("Ney");
                }
            }
            log.Debug("turnCardsToFront() - End");
        }

        /// <summary>
        /// Shows the two actionbuttons at players position 
        /// </summary>
        /// <param name="pos"></param>
        private void showActionButton(KeyValuePair<Player,List<Action>> ikvp)
        {
            log.Debug("showActionButton(KeyValuePair<Player(Name: "+ ikvp.Key.name.ToString() +" , Pos: "+ ikvp.Key.position.ToString() +")>,List<Action(" + ikvp.Value.ToString() + ")>) - Begin");
        //<Thickness x:Key="p1">40,300,1600,300</Thickness>
        //<Thickness x:Key="p2">400,40,1040,760</Thickness>
        //<Thickness x:Key="p3">1040,40,400,760</Thickness>
        //<Thickness x:Key="p4">1600,300,40,300</Thickness>
        //<Thickness x:Key="p5">1040,760,400,40</Thickness>
        //<Thickness x:Key="p6">400,760,1040,40</Thickness>
            Thickness t;
            ActionButton1_h.IsEnabled = false;
            ActionButton1_v.IsEnabled = false;
            personalStackField_h.Text = "Bet Area";
            personalStackField_v.Text = "Bet Area";
            personalStack = 0;
            updateBalance();
            setActionButtonText();
            oneChipCall = true;

            switch (ikvp.Key.position)
            {
                case 1: 
                    t = new Thickness(40, 300, 1600, 300);
                    Buttons_v.Margin = t;
                    Buttons_v.RenderTransform = new RotateTransform(90);
                    checkCash(ikvp.Key.position);
                    Buttons_v.Visibility = Visibility.Visible;
                    break;
                case 2: 
                    t = new Thickness(400,40,1040,760);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(180);
                    checkCash(ikvp.Key.position);
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
                case 3:
                    t = new Thickness(1040, 40, 400, 760);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(180);
                    checkCash(ikvp.Key.position);
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
                case 4:
                    t = new Thickness(1600, 300, 40, 300);
                    Buttons_v.Margin = t;
                    Buttons_v.RenderTransform = new RotateTransform(-90);
                    checkCash(ikvp.Key.position);
                    Buttons_v.Visibility = Visibility.Visible;
                    break;
                case 5:
                    t = new Thickness(1040, 760, 400, 40);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(0);
                    checkCash(ikvp.Key.position);
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
                case 6:
                    t = new Thickness(400, 760, 1040, 40);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(0);
                    checkCash(ikvp.Key.position);
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
            }
            log.Debug("showActionButton() - End");
        }

        /// <summary>
        /// Sets text for action buttons. eg: check, call, bet, raise
        /// </summary>
        private void setActionButtonText()
        {
            log.Debug("setActionButtonText() - Begin");
            ActionButton2_h.Content = "fold";
            ActionButton2_v.Content = "fold";
            ActionButton2_h.IsEnabled = true;
            ActionButton2_v.IsEnabled = true;
            String action = "";
            int i = 0;
            ActionButton1_h.IsEnabled = false;
            ActionButton1_v.IsEnabled = false;
            
            //One Chip Call
            if (oneChipCall && kvp.Value[1].amount != 0 && ((personalStack == 100 && 100 > kvp.Value[1].amount && 100 < kvp.Value[2].amount) || (personalStack == 500 && 500 > kvp.Value[1].amount && 500 < kvp.Value[2].amount))) 
            {
                action = kvp.Value[1].action.ToString() + " " + kvp.Value[1].amount.ToString();
                ActionButton1_h.IsEnabled = true;
                ActionButton1_v.IsEnabled = true;
                ActionButton1_h.Content = action;
                ActionButton1_v.Content = action;
                String action2 = kvp.Value[2].action.ToString() + " " + kvp.Value[2].amount.ToString();
                ActionButton2_h.Content = action2;
                ActionButton2_v.Content = action2;
                ActionButton2_h.IsEnabled = false;
                ActionButton2_v.IsEnabled = false;
                oneChipCall = false;
                //End One Chip Call
            } else {

                if (!personalStackField_v.Text.Equals("Bet Area") && !personalStackField_h.Text.Equals("Bet Area"))
                {
                    oneChipCall = false;
                }
                
                //Stack > call amount
                if (personalStack > kvp.Value[1].amount)
                {
                    //set button to bet/raise + amount
                    i = kvp.Value[2].amount;
                    action = kvp.Value[2].action.ToString() + " " + i;

                    //if stack >= raise amount enable bet/raise
                    if (personalStack >= i)
                    {
                        action = kvp.Value[2].action.ToString() + " " + personalStack;
                        ActionButton1_h.IsEnabled = true;
                        ActionButton1_v.IsEnabled = true;
                    }

                }
                else
                {
                    //action is check/call
                    action = kvp.Value[1].action.ToString();

                    //what amount to call
                    if (kvp.Value[1].amount > 0)
                    {
                        action += " " + kvp.Value[1].amount.ToString();
                    }
                    //enable if stack equals amount to call
                    if (personalStack == kvp.Value[1].amount)
                    {
                        ActionButton1_h.IsEnabled = true;
                        ActionButton1_v.IsEnabled = true;
                    }
                }
            }
            ActionButton1_h.Content = action;
            ActionButton1_v.Content = action;
            log.Debug("setActionButtonText(action: " + action + ") - End");
        }

        /// <summary>
        /// Hides the Grid with the two action buttons, horizontal AND vertical
        /// </summary>
        private void hideActionButton()
        {
            log.Debug("hideActionButton() - Begin");
            Buttons_h.Visibility = Visibility.Collapsed;
            Buttons_v.Visibility = Visibility.Collapsed;
            log.Debug("hideActionButton() - End");
        }

        /// <summary>
        /// Hide cards from player at pos
        /// </summary>
        /// <param name="pos"></param>
        private void foldCards(int pos)
        {
            log.Debug("foldCards(pos: " + pos.ToString() + ") - Begin");
            ScatterView sv = this.FindName("player" + pos + "cards") as ScatterView;
            sv.Visibility = Visibility.Collapsed;
            log.Debug("foldCards() - End");
        }

        /// <summary>
        /// catches event when action button is clicked. eg: call 100, fold
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void actionButtonClicked(object sender, RoutedEventArgs e)
        {
            log.Debug("actionButtonClicked(object: " + sender.ToString() + " , RoutedEventsArgs: " + e.ToString() + ") - Begin");
            Button s = (Button)sender;
            switch (s.Content.ToString().Split(' ')[0])
            {
                case "fold":
                    gl.activeAction(Action.playerAction.fold, 0);
                    foldCards(kvp.Key.position);                    
                    break;
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
            hideChips();
            updateBalance();
            await UInextPlayer();
            log.Debug("actionButtonClicked() - End");
        }

        /// <summary>
        /// sets UI buttons to next player or announces winner if no next player exists, game ends 
        /// </summary>
        /// <returns></returns>
        private async Task UInextPlayer()
        {
            log.Debug("UInext Player() - Begin");
            await Task.Delay(5);            
            try
            {
                
                kvp = gl.nextPlayer();
                showActionButton(kvp);
            }
            catch (NoPlayerInGameException exp)
            {
                log.Debug("catch NoPlayerInGameException exp: " + exp.ToString());
                newRound();
                announceWinner(gl.whoIsWinner(gl.pot, gl.players.FindAll(x => x.isActive)));
            }
            catch (EndRoundException exp)
            {
                log.Debug("catch EndRoundException exp: " + exp.ToString());
                round++;
                try
                {
                    kvp = gl.nextRound();
                    switch (round)
                    {
                        case 1:
                            showCommunityCards(gl.board.Count);
                            showActionButton(kvp);
                            break;
                        case 2:
                            showCommunityCards(gl.board.Count);
                            showActionButton(kvp);
                            break;
                        case 3:
                            showCommunityCards(gl.board.Count);
                            showActionButton(kvp);
                            break;
                        case 4:
                            showCommunityCards(gl.board.Count);
                            announceWinner(gl.whoIsWinner(gl.pot, gl.players.FindAll(x => x.isActive)));
                            break;
                        default:
                            log.Debug("switch default in round: " + round.ToString()); break;
                    }
                }
                catch (EndRoundException inexp)
                {
                    log.Debug("catch EndRoundException inexp: " + inexp.ToString());
                    allAreAllin();
                }
            }
            log.Debug("UInext Player() - End");
        }

        /// <summary>
        /// deals rest of community cards if no player can do any actions
        /// </summary>
        private async void allAreAllin()
        {
            log.Debug("allAreAllin() - Begin");
            showCommunityCards(gl.board.Count);
            await Task.Delay(showCardDelay);
            try{
                gl.nextRound();
                log.Debug("allAreAllin() Round: " + round.ToString() + " - End");
                return;
            }
            catch (EndRoundException e){
                if (round == 4)
                {
                    announceWinner(gl.whoIsWinner(gl.pot,gl.players.FindAll(x => x.isActive)));
                    log.Debug("allAreAllin() Round: " + round.ToString() + " == 4 - End");
                    return ;
                }
                else
                {
                        round++;
                        allAreAllin();
                }
            }
            log.Debug("allAreAllin() Round: " + round.ToString() + " - End");
        }

        /// <summary>
        /// write all winner names and amount to screen and show cards on show down
        /// </summary>
        /// <param name="winners"></param>
        private void announceWinner(List<Winner> winners)
        {
            log.Debug("announceWinner(List<KVP<Player,int>> winners count: " + winners.Count() + ") - Begin");
            mainPot.Foreground = Brushes.Fuchsia;
            mainPot.FontSize = 20;
            mainPot.Text = "";
            foreach (Winner w in winners)
            {
                if (round >= 4)
                {
                    mainPot.Text += w.player.name + " won " + w.value + " with " + w.hand + "\n";
                    turnCardsToFront(w.player.position);
                } else {
                    mainPot.Text += w.player.name + " won " + w.value + "\n";
                }
            }
            newRound();
            log.Debug("announceWinner() - End");
        }

        /// <summary>
        /// Show pot value and player balance(2000, All in)
        /// </summary>
        private void updateBalance()
        {
            log.Debug("updateBalance() - Begin");
            //Pot
            mainPot.Text = "";
            if (gl.pot.sidePot == null)
            {
                mainPot.Text = "Pot: " + gl.pot.value.ToString() + " ";
            }
            else
            {
                mainPot.Text = "Pot: " + gl.pot.value.ToString() + " ";
                mainPot.Text += getSidePots(gl.pot.sidePot,0,"");
            }


            //Cash for all Players
            foreach (Player iPlayer in gl.players)
            {
                TextBlock tb = this.FindName("player" + iPlayer.position + "balance") as TextBlock;
                if (iPlayer.stack != 0)
                {
                    tb.Text = "";
                    if (iPlayer.inPot != 0)
                    {
                        tb.Text = "Pot: " + iPlayer.inPot.ToString() + "    ";
                    }
                   
                    tb.Text += "Stack: " + iPlayer.stack.ToString();
                }
                else
                {
                    if(iPlayer.isAllin) {
                        tb.Text = "All in";
                    }
                    else
                    {
                    tb.Text = "";
                    }
                }
            }
            log.Debug("updateBalance() - End");
        }

        /// <summary>
        /// builds string for the side pots
        /// </summary>
        /// <param name="sidePot"></param>
        /// <param name="i"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string getSidePots(Pot sidePot, int i, string s)
        {
            log.Debug("getSidePots(Pot, i: " + i.ToString() + ", s: " + s.ToString() + ") - Begin");
            if (sidePot == null)
            {
                log.Debug("getSidePots() == null - End");
                return s;
            }
            else
            {
                log.Debug("getSidePots() - End");
                return getSidePots(sidePot.sidePot, i+1 ,s + "SidePot" + i + ": " + sidePot.value.ToString() + " ");
            }
        }

        //Drag and Drop
        /// <summary>
        /// Creates Cursor and starts drag of image
        /// get help at msdn: Dragging and Dropping Items from ScatterView Controls to SurfaceListBox Controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragSourcePreviewInputDeviceDown(object sender, InputEventArgs e)
        {
            log.Debug("DragSourcePreviewInputDeviceDown(object " + sender.ToString() + ", InputEventArgs " + e.ToString() + ") - Begin");
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            ScatterViewItem draggedElement = null;
            ScatterViewItem sci = sender as ScatterViewItem;
            Image img = sci.Content as Image;

            // Find the ScatterViewItem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as ScatterViewItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            object data = draggedElement.Content;

            // If the data has not been specified as draggable, 
            // or the ScatterViewItem cannot move, return.
            if (data == null)
            {
                return;
            }

            // Set the dragged element. This is needed in case the drag operation is canceled.
            //data.DraggedElement = draggedElement;

            // Create the cursor visual.
            ContentControl cursorVisual = new ContentControl()
            {
                Content = img,
                //Style = FindResource("CursorStyle") as Style
            };

            // Create a list of input devices, 
            // and add the device passed to this event handler.
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.Device);

            // If there are touch devices captured within the element,
            // add them to the list of input devices.
            foreach (InputDevice device in draggedElement.TouchesCapturedWithin)
            {
                if (device != e.Device)
                {
                    devices.Add(device);
                }
            }

            // Get the drag source object.
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            // Start the drag-and-drop operation.
            SurfaceDragCursor cursor =
                SurfaceDragDrop.BeginDragDrop(
                // The ScatterView object that the cursor is dragged out from.
                  dragSource,
                // The ScatterViewItem object that is dragged from the drag source.
                  draggedElement,
                // The visual element of the cursor.
                  cursorVisual,
                // The data attached with the cursor.
                  img,
                // The input devices that start dragging the cursor.
                  devices,
                // The allowed drag-and-drop effects of the operation.
                  DragDropEffects.Move);

            // If the cursor was created, the drag-and-drop operation was successfully started.
            if (cursor != null)
            {
                // Hide the ScatterViewItem.
                //draggedElement.Visibility = Visibility.Hidden;
                //Console.WriteLine("Begin Drag");
                // This event has been handled.
                log.Debug("DragSourcePreviewInputDeviceDown() handled - End");
                e.Handled = true;
            }
            log.Debug("DragSourcePreviewInputDeviceDown() check if handled - End");
        }

        /// <summary>
        /// If Drag get canceld and draged object is a card set card to origin position
        /// get help at msdn: Dragging and Dropping Items from ScatterView Controls to SurfaceListBox Controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragCanceled(object sender, SurfaceDragDropEventArgs e)
        {
            log.Debug("DragCanceled(object " + sender.ToString() + " SurfaceDragDropEventArgs " + e.ToString() + ") - Begin");
            Image data = e.Cursor.Data as Image;
            //Console.WriteLine(data.Name);
            if (data.Name.Contains("card"))
            {
                BitmapImage bimage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Kartenrueckseite/kartenruecken_1.jpg"));
                Image image = this.FindName(data.Name) as Image;
                ScatterViewItem svi = this.FindName(data.Name.Substring(0, (data.Name.Length - 5))) as ScatterViewItem;
                svi.Content = null;
                svi.Content = image;
                image.Visibility = Visibility.Visible;
            }

            log.Debug("DragCanceled() - End");
            e.Handled = true;
        }

        /// <summary>
        /// collectes chip value in TargetArea
        /// get help at msdn: Dragging and Dropping Items from ScatterView Controls to SurfaceListBox Controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DropTargetDragEnter(object sender, SurfaceDragDropEventArgs e)
        {
            log.Debug("DropTargetDragEnter(object " + sender.ToString() + " SurfaceDragDropEventArgs " + e.ToString() + ") - Begin");
            e.Cursor.Visual.Tag = "DragEnter";
            Image img = e.Cursor.Data as Image;
            //Get Chip Value
            string text = img.Source.ToString();
            if (text.Contains("chip")) {

                char[] delimiterChars = { '_', '.' };
                string[] words = text.Split(delimiterChars);

                //Update the personal stack
                personalStack += (Int32)Convert.ToInt32(words[2]);
                personalStackField_h.Text = personalStack.ToString();
                personalStackField_v.Text = personalStack.ToString();
                checkCash(kvp.Key.position);
                setActionButtonText();
            }
            log.Debug("DropTargetDragEnter() - End");
            e.Handled = true;
        }

        private void DropTargetDragLeave(object sender, SurfaceDragDropEventArgs e)
        {
            log.Debug("DropTargetDragLeave(object " + sender.ToString() + " SurfaceDragDropEventArgs " + e.ToString() + ") - Begin");
            e.Cursor.Visual.Tag = null;
            SurfaceDragDrop.CancelDragDrop(e.Cursor);
            log.Debug("DropTargetDragLeave() - End");
            e.Handled = true;
        }

        /// <summary>
        /// checks how many dragable chips are shown at players position
        /// </summary>
        /// <param name="pos"></param>
        private void checkCash(int pos)
        {
            log.Debug("checkCash(pos " + pos.ToString() + ") - Begin");
            ImgChip10_h.Visibility = Visibility.Visible;
            ImgChip10_v.Visibility = Visibility.Visible;
            SVIChip10.Visibility = Visibility.Visible;
            ImgChip20_h.Visibility = Visibility.Visible;
            ImgChip20_v.Visibility = Visibility.Visible;
            SVIChip20.Visibility = Visibility.Visible;
            ImgChip100_h.Visibility = Visibility.Visible;
            ImgChip100_v.Visibility = Visibility.Visible;
            SVIChip100.Visibility = Visibility.Visible;
            ImgChip500_h.Visibility = Visibility.Visible;
            ImgChip500_v.Visibility = Visibility.Visible;
            SVIChip500.Visibility = Visibility.Visible;
            
            if (personalStack == 0)
            {
                personalStackField_h.Text = "Bet Area";
                personalStackField_v.Text = "Bet Area";
            }
            else
            {
                personalStackField_h.Text = personalStack.ToString();
                personalStackField_v.Text = personalStack.ToString();
            }

            if ((gl.players.Find(x => x.position == pos).stack - personalStack) < 500)
            {
                ImgChip500_h.Visibility = Visibility.Collapsed;
                ImgChip500_v.Visibility = Visibility.Collapsed;
                SVIChip500.Visibility = Visibility.Collapsed;
                if ((gl.players.Find(x => x.position == pos).stack - personalStack) < 100)
                {
                    ImgChip100_h.Visibility = Visibility.Collapsed;
                    ImgChip100_v.Visibility = Visibility.Collapsed;
                    SVIChip100.Visibility = Visibility.Collapsed;
                    if ((gl.players.Find(x => x.position == pos).stack - personalStack) < 20)
                    {
                        ImgChip20_h.Visibility = Visibility.Collapsed;
                        ImgChip20_v.Visibility = Visibility.Collapsed;
                        SVIChip20.Visibility = Visibility.Collapsed;
                        if ((gl.players.Find(x => x.position == pos).stack - personalStack) < 10)
                        {
                            ImgChip10_h.Visibility = Visibility.Collapsed;
                            ImgChip10_v.Visibility = Visibility.Collapsed;
                            SVIChip10.Visibility = Visibility.Collapsed;

                        }
                    }
                }
            }
            setSVIChipPos(pos);
            log.Debug("checkCash() - End");
        }

        /// <summary>
        /// Hide all draggable chips
        /// </summary>
        private void hideChips() {
            log.Debug("hideChips() - Begin");
            ImgChip10_h.Visibility = Visibility.Collapsed;
            ImgChip10_v.Visibility = Visibility.Collapsed;
            SVIChip10.Visibility = Visibility.Collapsed;
            ImgChip20_h.Visibility = Visibility.Collapsed;
            ImgChip20_v.Visibility = Visibility.Collapsed;
            SVIChip20.Visibility = Visibility.Collapsed;
            ImgChip100_h.Visibility = Visibility.Collapsed;
            ImgChip100_v.Visibility = Visibility.Collapsed;
            SVIChip100.Visibility = Visibility.Collapsed;
            ImgChip500_h.Visibility = Visibility.Collapsed;
            ImgChip500_v.Visibility = Visibility.Collapsed;
            SVIChip500.Visibility = Visibility.Collapsed;
            log.Debug("hideChips() - End");
        }

        /// <summary>
        /// Hide all player and community cards
        /// </summary>
        private void hideCards()
        {
            log.Debug("hideCards() - Begin");
            player1cards.Visibility = Visibility.Collapsed;
            player2cards.Visibility = Visibility.Collapsed;
            player3cards.Visibility = Visibility.Collapsed;
            player4cards.Visibility = Visibility.Collapsed;
            player5cards.Visibility = Visibility.Collapsed;
            player6cards.Visibility = Visibility.Collapsed;
            communityCards.Visibility = Visibility.Collapsed;
            BitmapImage f1image = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Kartenrueckseite/kartenruecken_1.jpg"));
            cc0.Source = f1image;
            cc1.Source = f1image;
            cc2.Source = f1image;
            cc3.Source = f1image;
            cc4.Source = f1image;
            player1card1image.Source = f1image;
            player1card2image.Source = f1image;
            player2card1image.Source = f1image;
            player2card2image.Source = f1image;
            player3card1image.Source = f1image;
            player3card2image.Source = f1image;
            player4card1image.Source = f1image;
            player4card2image.Source = f1image;
            player5card1image.Source = f1image;
            player5card2image.Source = f1image;
            player6card1image.Source = f1image;
            player6card2image.Source = f1image;
            log.Debug("hideCards() - End");
        }

        /// <summary>
        /// Hide chips and cards
        /// </summary>
        private void hideUI()
        {
            log.Debug("hideUI() - Begin");
            hideCards();
            hideChips();
            log.Debug("hideUI() - End");
        }

        /// <summary>
        /// sets Personal Stack to min value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setMinValue(object sender, MouseButtonEventArgs e)
        {
            log.Debug("setMinValue(object " + sender.ToString() + " MouseButtonEventArgs " + e.ToString() + ") - Begin");
            //Update the personal stack
            personalStack = kvp.Value[1].amount;
            personalStackField_h.Text = personalStack.ToString();
            personalStackField_v.Text = personalStack.ToString();
            checkCash(kvp.Key.position);
            setActionButtonText();
            log.Debug("setMinValue() - End");
            e.Handled = true;
        }

        /// <summary>
        /// set personal stack to 0, undo bet in bet area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetPersonalStack(object sender, RoutedEventArgs e)
        {
            log.Debug("resetPersonalStack(object " + sender.ToString() + " RoutedEventArgs " + e.ToString() + ") - Begin");
            personalStack = 0;
            oneChipCall = true;
            checkCash(kvp.Key.position);
            setActionButtonText();
            log.Debug("resetPersonalStack() - End");
            e.Handled = true;
        }

        /// <summary>
        /// Show i community cards
        /// </summary>
        /// <param name="i"></param>
        private void showCommunityCards(int i)
        {
            log.Debug("showCommunityCards(i " + i.ToString() + ") - Begin");
            BitmapImage bi;
            Image cc;
            for (int j = 0; j < i; j++)
            {
                bi = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.board[j].ToString() + ".png"));
                cc = this.FindName("cc" + j) as Image;
                cc.Source = bi;
            }
            communityCards.Visibility = Visibility.Visible;
            log.Debug("showCommunityCards() - End");
        }

        /// <summary>
        /// sets DealerButton at player pos
        /// </summary>
        /// <param name="pos"></param>
        private void setDealerButtonPos(int pos)
        {
            log.Debug("setDealerButton(pos " + pos.ToString() + ") - Begin");
            Thickness t;
            switch (pos)
            {
                case 1:
                    t = new Thickness(325, 610, 1505, 380);
                    dealerButton.Margin = t;
                    dealerButton.Visibility = Visibility.Visible;
                    break;
                case 2:
                    t = new Thickness(585, 325, 1245, 665);
                    dealerButton.Margin = t;
                    dealerButton.Visibility = Visibility.Visible;
                    break;
                case 3:
                    t = new Thickness(1229, 325, 601, 665);
                    dealerButton.Margin = t;
                    dealerButton.Visibility = Visibility.Visible;
                    break;
                case 4:
                    t = new Thickness(1505, 380, 325, 610);
                    dealerButton.Margin = t;
                    dealerButton.Visibility = Visibility.Visible;
                    break;
                case 5:
                    t = new Thickness(1160, 665, 670, 325);
                    dealerButton.Margin = t;
                    dealerButton.Visibility = Visibility.Visible;
                    break;
                case 6:
                    t = new Thickness(554, 665, 1276, 325);
                    dealerButton.Margin = t;
                    dealerButton.Visibility = Visibility.Visible;
                    break;
            }
            log.Debug("setDealerButton() - End");
        }

        /// <summary>
        /// change player stacks, updates stacks from all existing players 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setStack(object sender, RoutedEventArgs e)
        {
            log.Debug("setStack(object " + sender.ToString() + " RoutedEventArgs " + e.ToString() + ") - Begin");
            if (canChangeStack)
            {
                ElementMenuItem emi = sender as ElementMenuItem;
                string t = emi.Header.ToString();
                stack = (Int32)Convert.ToInt32(emi.Header.ToString().Split(' ')[1]);
                mainPot.Text = "Player Stacks: " + stack.ToString();
                if (players.Exists(x => x.stack != stack))
                {
                    foreach (Player p in players) {
                        p.stack = stack;                    
                    }

                }
            }
            log.Debug("setStack() - End");
            e.Handled = true;
        }

        /// <summary>
        /// Set the dragabe Chips ScatterViewItem at player pos
        /// </summary>
        /// <param name="pos"></param>
        private void setSVIChipPos(int pos)
        {
            log.Debug("setSVIChipPos(pos " + pos.ToString() + ") - Begin");
            switch (pos)
            {
                case 1:
                    
                    SVIChip10.Center = new Point(270, 360);
                    SVIChip20.Center = new Point(270, 455);
                    SVIChip100.Center = new Point(270, 545);
                    SVIChip500.Center = new Point(270, 635);
                    break;
                case 2:
                    SVIChip10.Center = new Point(830, 270);
                    SVIChip20.Center = new Point(740, 270);
                    SVIChip100.Center = new Point(650, 270);
                    SVIChip500.Center = new Point(560, 270);
                    break;
                case 3:
                    SVIChip10.Center = new Point(1470, 270);
                    SVIChip20.Center = new Point(1380, 270);
                    SVIChip100.Center = new Point(1290, 270);
                    SVIChip500.Center = new Point(1200, 270);
                    break;
                case 4:
                    SVIChip10.Center = new Point(1660, 730);
                    SVIChip20.Center = new Point(1660, 640);
                    SVIChip100.Center = new Point(1660, 550);
                    SVIChip500.Center = new Point(1660, 460);
                    break;
                case 5:
                    SVIChip10.Center = new Point(1105, 820);
                    SVIChip20.Center = new Point(1190, 820);
                    SVIChip100.Center = new Point(1280, 820);
                    SVIChip500.Center = new Point(1375, 820);
                    break;
                case 6:
                    SVIChip10.Center = new Point(465, 820);
                    SVIChip20.Center = new Point(555, 820);
                    SVIChip100.Center = new Point(640, 820);
                    SVIChip500.Center = new Point(735, 820);
                    break;
            }
            log.Debug("setSVIChipPos() - End");
        }

    }

}
