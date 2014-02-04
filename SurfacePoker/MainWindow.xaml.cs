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
using Microsoft.Surface.Presentation;
using System.Collections.ObjectModel;

namespace SurfacePoker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<Player> players { get; set; }
      
        private bool canAddPlayer { get; set; }

        private bool canChangeStack { get; set; }

        private int stack { get; set; }

        private System.Windows.Controls.Button btn;

        private Game gl {get; set;}

        private int round;

        private KeyValuePair<Player, List<Action>> kvp;

        private int personalStack { get; set; }

        private void shutDown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
            e.Handled = true;
        }


        private int bb = 20;

        public MainWindow()
        {
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
            DataContext = this;

        }

        private void createNewGame(object sender, RoutedEventArgs e)
        {

            TextBlock tb;
            Rectangle r;
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
            hideUI();
            hideActionButton();
            round = 0;
            personalStack = 0;
            canAddPlayer = true;
            canChangeStack = true;
            EMIchangeStack.Visibility = Visibility.Visible;
            //if no stack value is given set default
            if (stack == 0)
            {
                stack = 1000;
            }
            mainPot.Text = "Touch Grey Area To Create A New Player!             Player Stacks: " + stack.ToString();
            players = new List<Player>();
            gl = null;
            e.Handled = true;
        }

        private void openAddPlayer(object sender, RoutedEventArgs e)
        {
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
            e.Handled = true;
        }
        private void closeAddPlayer(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;
            StackPanel stackPanel = (StackPanel)s.Parent;
            SurfaceTextBox text = (SurfaceTextBox)stackPanel.FindName("playerName");
            text.Text = "";
            addplayerscatteru.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        private void setSurfaceName(int pos, String name) {
            TextBlock tb = this.FindName("player" + pos + "name") as TextBlock;
            tb.Text = name;
        }

        private void savePlayer(object sender, RoutedEventArgs e)
        {
            
            if (canAddPlayer) { 
            
                SurfaceButton s = (SurfaceButton)sender;
                StackPanel stackPanel = (StackPanel)s.Parent;
                SurfaceTextBox tb = (SurfaceTextBox)stackPanel.FindName("playerName");
                Label l = this.FindName("addPlayerLabel") as Label;
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
            e.Handled = true;

        }

        private void addStartButton(String text)
        {
            btn.Name = "btnStart";
            btn.Content = text;
            btn.Margin = new Thickness(900, 700, 900, 320);
            btn.Click += new RoutedEventHandler(startGame);         
            if (!Grid.Children.Contains(btn))
            {
                Grid.Children.Add(btn);
            }
        }

        private void startGame(object sender, RoutedEventArgs e)
        {
            //Disable adding new players, changing stacks and hide position fields
            canAddPlayer = false;
            EMIchangeStack.Visibility = Visibility.Collapsed;
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
            //Start new Game
            Mitte.Visibility = Visibility.Visible;
            if (gl == null)
            {
                gl = new Game(players, bb, bb / 2);
            }
            //set dealer button to pos
            Player p = gl.newGame();
            setDealerButtonPos(p.position);
            //get first player
            kvp = gl.nextPlayer();
            showCards();
            showActionButton(kvp);
            e.Handled = true;

        }

        private void showCards()
        {
            foreach (Player iPlayer in gl.players.FindAll(x => x.isActive))
            {
                Rectangle r = this.FindName("Pos" + iPlayer.position) as Rectangle;
                r.Visibility = Visibility.Visible;
                ScatterView sv = this.FindName("player" + iPlayer.position + "cards") as ScatterView;
                sv.Visibility = Visibility.Visible;
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
            //x => x.position == pos
            //TODO: ein Find muss immer mit einem Exist abgesichert werde
            if (gl.players.Exists(x => x.position == pos))
            {
                if (gl.players.Find(x => x.position == pos).cards.Count != 0)
                {
                    BitmapImage bmpimage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.players.Find(x => x.position == pos).cards[pcard - 1] + ".png"));
                    Image image = sender as Image;
                    image.Source = bmpimage;
                }
                else
                {
                    Console.WriteLine("Yay");
                }
            }

            e.Handled = true;
        }

        private void turnCardsToFront(int pos)
        {
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
                    Console.WriteLine("Ney");
                }

            }
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
        //<Thickness x:Key="p1">40,300,1600,300</Thickness>
        //<Thickness x:Key="p2">400,40,1040,760</Thickness>
        //<Thickness x:Key="p3">1040,40,400,760</Thickness>
        //<Thickness x:Key="p4">1600,300,40,300</Thickness>
        //<Thickness x:Key="p5">1040,760,400,40</Thickness>
        //<Thickness x:Key="p6">400,760,1040,40</Thickness>
            Thickness t;
            buttonAction_h.IsEnabled = false;
            buttonAction_v.IsEnabled = false;
            personalStackField_h.Text = "Bet Area";
            personalStackField_v.Text = "Bet Area";
            personalStack = 0;
            updateBalance();
            setActionButtonText();
            

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

        }
        private void setActionButtonText()
        {
            String action = "";
            int i = 0;
            //Console.WriteLine(kvp.Value[0].action);
            buttonAction_h.IsEnabled = false;
            buttonAction_v.IsEnabled = false;
            
            if (personalStack > kvp.Value[1].amount)
            {
                i = kvp.Value[1].amount + kvp.Value[2].amount;
                action = kvp.Value[2].action.ToString() + " " + i;

                if (personalStack >= i)
                {
                    action = kvp.Value[2].action.ToString() + " " + personalStack;
                    buttonAction_h.IsEnabled = true;
                    buttonAction_v.IsEnabled = true;
                }

            }
            else
            {
                action = kvp.Value[1].action.ToString();
                if (kvp.Value[1].amount > 0)
                {
                    action += " " + kvp.Value[1].amount.ToString();
                }
                if (personalStack == kvp.Value[1].amount)
                {
                    buttonAction_h.IsEnabled = true;
                    buttonAction_v.IsEnabled = true;
                }
            }
            buttonAction_h.Content = action;
            buttonAction_v.Content = action;
        }

        /// <summary>
        /// Hides the Grid for the two action buttons
        /// </summary>
        private void hideActionButton()
        {
            Buttons_h.Visibility = Visibility.Collapsed;
            Buttons_v.Visibility = Visibility.Collapsed;
            
        }

        private void foldCards(int i)
        {
            ScatterView sv = this.FindName("player" + i + "cards") as ScatterView;
            sv.Visibility = Visibility.Collapsed;
        }

        private void actionButtonClicked(object sender, RoutedEventArgs e)
        {
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
            try
            {
                kvp = gl.nextPlayer();
                showActionButton(kvp);
            }
            catch (NoPlayerInGameException exp)
            {
                newRound();
                mainPot.Text = "";
                List<KeyValuePair<Player, int>> winners = gl.whoIsWinner(gl.pot);
                updateBalance();
                hideChips();
                foreach (KeyValuePair<Player, int> ikvp in winners)
                {
                    mainPot.Text += ikvp.Key.name + " won " + ikvp.Value + "\n ";
                }
            }
            catch (EndRoundException exp)
            {
                Console.WriteLine(exp.Message.ToString());
                round++;
                try
                {
                    kvp = gl.nextRound();
                }
                catch (EndRoundException inexp)
                {
                    Console.WriteLine("Hey ho");
                    hideChips();
                    hideActionButton();
                    allAreAllin();
                    //gl.nextRound();
                    
                }
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
                        List<KeyValuePair<Player, int>> winners = gl.whoIsWinner(gl.pot);
                        showCommunityCards(gl.board.Count);
                        updateBalance();
                        hideChips();
                        mainPot.Text = "";
                        newRound();
                        foreach (KeyValuePair<Player, int> ikvp in winners)
                        {
                            mainPot.Text += ikvp.Key.name + " won " + ikvp.Value + "\n";
                            turnCardsToFront(ikvp.Key.position);
                        }

                        break;
                    default: Console.WriteLine("Round: " + round); break;
                }

            }
            
        }

        private void allAreAllin()
        {
            try{
                gl.nextRound();
            }
            catch (EndRoundException e){
                if (round == 4)
                {
                    return ;
                }
                else
                {
                    round++;
                    allAreAllin();
                }
            }
        }

        private void newRound()
        {
            //hideUI();
            round = 0;
            addStartButton("start new Round");
        }

        private void updateBalance()
        {
            //Pot
            if (gl.pot.value == 0)
            {
                mainPot.Text = "";
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
        }

        private static string getSidePots(Pot sidePot, int i, string s)
        {
            if (sidePot == null)
            {
                return s;
            }
            else
            {
                return getSidePots(sidePot.sidePot, i++, "SidePod" + i + ": " + sidePot.value.ToString() + " ");
            }
        }

        //private void createChip()
        //{
        //    ScatterView sv = (ScatterView)this.FindName("Test");
            
        //    Image im = new Image();
        //    BitmapImage bitImage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Chips/Pokerchip_final_10.png"));
        //    im.Source = bitImage;
        //    im.Width = 100;
        //    im.Height = 100;
        //    //im.MouseDown += new MouseButtonEventHandler(this.test);
            
        //    ScatterViewItem chip = new ScatterViewItem();
        //    chip.Content = im;
        //    chip.Center = new Point(500,100);
        //    chip.Background = Brushes.Transparent;
        //    chip.CanMove = true;
        //    //chip.MouseDown += new MouseButtonEventHandler(this.test);
        //    sv.Items.Add(chip);
        //    //g.Children.Add()
        //    //player1.setChip(new Chip(p1c10, 300, 750, 208, 10, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_10.png")));
        //}

        //private void test(object sender, RoutedEventArgs e)
        //{
        //    Console.WriteLine("Test");
        //    e.Handled = true;
        //}

        

        //Drag and Drop
        private void DragSourcePreviewInputDeviceDown(object sender, InputEventArgs e)
        {
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
                e.Handled = true;
            }
        }
        //private void DropTargetDragEnter(object sender, SurfaceDragDropEventArgs e)
        //{
        //    e.Cursor.Visual.Tag = "DragEnter";
        //    Console.WriteLine("DropTargetDragEnter");
        //    e.Handled = true;
        //}
        //private void DropTargetDragLeave(object sender, SurfaceDragDropEventArgs e)
        //{
        //    e.Cursor.Visual.Tag = null;
        //    Console.WriteLine("DropTargetDragLeave");
        //    e.Handled = true;
        //}

        private void DragCanceled(object sender, SurfaceDragDropEventArgs e)
        {
            object data = e.Cursor.Data ;
            //ScatterViewItem item = data.DraggedElement as ScatterViewItem;
            if (data != null)
            {
                //Console.WriteLine("Drop Canceled");
            }
            e.Handled = true;
        }
        private void DropTargetDrop(object sender, SurfaceDragDropEventArgs e)
        {

            Image img = e.Cursor.Data as Image;
            //Get Chip Value
            char[] delimiterChars = { '_', '.'};
            string text = img.Source.ToString();
            string[] words = text.Split(delimiterChars);

            //Update the personal stack
            personalStack += (Int32)Convert.ToInt32(words[2]);
            personalStackField_h.Text = personalStack.ToString();
            personalStackField_v.Text = personalStack.ToString();
            checkCash(kvp.Key.position);
            setActionButtonText();
            e.Handled = true;
        }

        private void DropTargetDragEnter(object sender, SurfaceDragDropEventArgs e)
        {

            e.Cursor.Visual.Tag = "DragEnter";
            Image img = e.Cursor.Data as Image;
            //Get Chip Value
            char[] delimiterChars = { '_', '.' };
            string text = img.Source.ToString();
            string[] words = text.Split(delimiterChars);

            //Update the personal stack
            personalStack += (Int32)Convert.ToInt32(words[2]);
            personalStackField_h.Text = personalStack.ToString();
            personalStackField_v.Text = personalStack.ToString();
            checkCash(kvp.Key.position);
            setActionButtonText();
            e.Handled = true;
        }

        private void DropTargetDragLeave(object sender, SurfaceDragDropEventArgs e)
        {

            e.Cursor.Visual.Tag = null;
            Console.WriteLine("DropTargetDragLeave");
            SurfaceDragDrop.CancelDragDrop(e.Cursor);
            e.Handled = true;
            
        }

        private void checkCash(int pos)
        {
            setSVIChipPos(kvp.Key.position);
            ChipImg10_h.Visibility = Visibility.Visible;
            ChipImg10_v.Visibility = Visibility.Visible;
            SVIChip10.Visibility = Visibility.Visible;
            ChipImg20_h.Visibility = Visibility.Visible;
            ChipImg20_v.Visibility = Visibility.Visible;
            SVIChip20.Visibility = Visibility.Visible;
            ChipImg100_h.Visibility = Visibility.Visible;
            ChipImg100_v.Visibility = Visibility.Visible;
            SVIChip100.Visibility = Visibility.Visible;
            ChipImg500_h.Visibility = Visibility.Visible;
            ChipImg500_v.Visibility = Visibility.Visible;
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
                ChipImg500_h.Visibility = Visibility.Collapsed;
                ChipImg500_v.Visibility = Visibility.Collapsed;
                SVIChip500.Visibility = Visibility.Collapsed;
                if ((gl.players.Find(x => x.position == pos).stack - personalStack) < 100)
                {
                    ChipImg100_h.Visibility = Visibility.Collapsed;
                    ChipImg100_v.Visibility = Visibility.Collapsed;
                    SVIChip100.Visibility = Visibility.Collapsed;
                    if ((gl.players.Find(x => x.position == pos).stack - personalStack) < 20)
                    {
                        ChipImg20_h.Visibility = Visibility.Collapsed;
                        ChipImg20_v.Visibility = Visibility.Collapsed;
                        SVIChip20.Visibility = Visibility.Collapsed;
                        if ((gl.players.Find(x => x.position == pos).stack - personalStack) < 10)
                        {
                            ChipImg10_h.Visibility = Visibility.Collapsed;
                            ChipImg10_v.Visibility = Visibility.Collapsed;
                            SVIChip10.Visibility = Visibility.Collapsed;

                        }
                    }
                }
            }
        }

        private void hideChips() {
            ChipImg10_h.Visibility = Visibility.Collapsed;
            ChipImg10_v.Visibility = Visibility.Collapsed;
            SVIChip10.Visibility = Visibility.Collapsed;
            ChipImg20_h.Visibility = Visibility.Collapsed;
            ChipImg20_v.Visibility = Visibility.Collapsed;
            SVIChip20.Visibility = Visibility.Collapsed;
            ChipImg100_h.Visibility = Visibility.Collapsed;
            ChipImg100_v.Visibility = Visibility.Collapsed;
            SVIChip100.Visibility = Visibility.Collapsed;
            ChipImg500_h.Visibility = Visibility.Collapsed;
            ChipImg500_v.Visibility = Visibility.Collapsed;
            SVIChip500.Visibility = Visibility.Collapsed;
        }

        private void hideCards()
        {
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
        }

        private void hideUI()
        {
            hideCards();
            hideChips();
        }

        private void setMinValue(object sender, MouseButtonEventArgs e)
        {
            //Update the personal stack
            personalStack = kvp.Value[1].amount;
            personalStackField_h.Text = personalStack.ToString();
            personalStackField_v.Text = personalStack.ToString();
            checkCash(kvp.Key.position);
            setActionButtonText();
            e.Handled = true;
        }

        private void resetPersonalStack(object sender, RoutedEventArgs e)
        {
            personalStack = 0;
            checkCash(kvp.Key.position);
            setActionButtonText();
            e.Handled = true;
        }

        private void showCommunityCards(int i)
        {
            BitmapImage bi;
            Image cc;
            for (int j = 0; j < i; j++)
            {
                bi = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.board[j].ToString() + ".png"));
                cc = this.FindName("cc" + j) as Image;
                cc.Source = bi;
            }
            communityCards.Visibility = Visibility.Visible;
        }

        private void setDealerButtonPos(int pos)
        {
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
        }
        private void setStack(object sender, RoutedEventArgs e)
        {
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
            e.Handled = true;
        }
        private void setSVIChipPos(int i)
        {
            switch (i)
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
        }
    }

}
