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

        private int position { get; set; }

        private int stack { get; set; }

        private System.Windows.Controls.Button btn;

        private Game gl;

        private int round;

        private KeyValuePair<Player, List<Action>> kvp;

        private void shutDown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0); 
        }

        public MainWindow()
        {
            canAddPlayer = true;
            round = 0;
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
            DataContext = this;

        }
        private void openAddPlayer(object sender, RoutedEventArgs e)
        {
            createChip();
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
        //<Thickness x:Key="p1">40,300,1600,300</Thickness>
        //<Thickness x:Key="p2">400,40,1040,760</Thickness>
        //<Thickness x:Key="p3">1040,40,400,760</Thickness>
        //<Thickness x:Key="p4">1600,300,40,300</Thickness>
        //<Thickness x:Key="p5">1040,760,400,40</Thickness>
        //<Thickness x:Key="p6">400,760,1040,40</Thickness>
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
                    t = new Thickness(40, 300, 1600, 300);
                    Buttons_v.Margin = t;
                    Buttons_v.RenderTransform = new RotateTransform(90);
                    buttonAction_v.Content = action;
                    Buttons_v.Visibility = Visibility.Visible;
                    break;
                case 2: 
                    t = new Thickness(400,40,1040,760);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(180);
                    buttonAction_h.Content = action;
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
                case 3:
                    t = new Thickness(1040, 40, 400, 760);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(180);
                    buttonAction_h.Content = action;
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
                case 4:
                    t = new Thickness(1600, 300, 40, 300);
                    Buttons_v.Margin = t;
                    Buttons_v.RenderTransform = new RotateTransform(-90);
                    buttonAction_v.Content = action;
                    Buttons_v.Visibility = Visibility.Visible;
                    break;
                case 5:
                    t = new Thickness(1040, 760, 400, 40);
                    Buttons_h.Margin = t;
                    Buttons_h.RenderTransform = new RotateTransform(0);
                    buttonAction_h.Content = action;
                    Buttons_h.Visibility = Visibility.Visible;
                    break;
                case 6:
                    t = new Thickness(400, 760, 1040, 40);
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
                case "fold":
                    gl.activeAction(Action.playerAction.fold, 0);
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
            catch (EndRoundException exp)
            {
                round++;
                kvp = gl.nextRound();
                switch (round)
                {
                    case 1:
                        BitmapImage f1image = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.board[0].ToString() + ".png"));
                        f1.Source = f1image;
                        BitmapImage f2image = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.board[1].ToString() + ".png"));
                        f2.Source = f2image;
                        BitmapImage f3image = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.board[2].ToString() + ".png"));
                        f3.Source = f3image;
                        break;
                    case 2:
                        BitmapImage tcimage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.board[3].ToString() + ".png"));
                        tc.Source = tcimage;
                        break;
                    case 3:
                        BitmapImage rcimage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/" + gl.board[4].ToString() + ".png"));
                        rc.Source = rcimage;
                        break;
                    case 4:
                        break;
                }
                communityCards.Visibility = Visibility.Visible;
                showActionButton(kvp);
            }
            
        }

        private void createChip()
        {
            ScatterView sv = (ScatterView)this.FindName("Test");
            
            Image im = new Image();
            BitmapImage bitImage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Chips/Pokerchip_final_10.png"));
            im.Source = bitImage;
            im.Width = 100;
            im.Height = 100;
            //im.MouseDown += new MouseButtonEventHandler(this.test);
            
            ScatterViewItem chip = new ScatterViewItem();
            chip.Content = im;
            chip.Center = new Point(500,100);
            chip.Background = Brushes.Transparent;
            chip.CanMove = true;
            //chip.MouseDown += new MouseButtonEventHandler(this.test);
            sv.Items.Add(chip);
            //g.Children.Add()
            //player1.setChip(new Chip(p1c10, 300, 750, 208, 10, new Uri("pack://siteoforigin:,,,/Chips/Pokerchip_final_10.png")));
        }

        private void test(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Test");
            e.Handled = true;
        }

        

        //Drag and Drop
        private void DragSourcePreviewInputDeviceDown(object sender, InputEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            ScatterViewItem draggedElement = null;

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
                Content = draggedElement.FindName("Chip1"),
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
                  draggedElement.DataContext,
                // The input devices that start dragging the cursor.
                  devices,
                // The allowed drag-and-drop effects of the operation.
                  DragDropEffects.Move);

            // If the cursor was created, the drag-and-drop operation was successfully started.
            if (cursor != null)
            {
                // Hide the ScatterViewItem.
                //draggedElement.Visibility = Visibility.Hidden;
                Console.WriteLine("Begin Drag");
                // This event has been handled.
                e.Handled = true;
            }
        }
        private void DropTargetDragEnter(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = "DragEnter";
            Console.WriteLine("DropTargetDragEnter");
        }
        private void DropTargetDragLeave(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = null;
            Console.WriteLine("DropTargetDragLeave");
        }

        private void DragCanceled(object sender, SurfaceDragDropEventArgs e)
        {
            object data = e.Cursor.Data ;
            //ScatterViewItem item = data.DraggedElement as ScatterViewItem;
            if (data != null)
            {
                Console.WriteLine("Drop Canceled");
            }
        }
        private void DropTargetDrop(object sender, SurfaceDragDropEventArgs e)
        {
            Console.WriteLine("Drop Target");
        }
        private void DragCompleted(object sender, SurfaceDragCompletedEventArgs e)
        {
            // If the operation is Move, remove the data from drag source.
            if (e.Cursor.Effects == DragDropEffects.Move)
            {
               Console.WriteLine("On Move");
            }
            Console.WriteLine("Drag Complete");
        }
    }



}
