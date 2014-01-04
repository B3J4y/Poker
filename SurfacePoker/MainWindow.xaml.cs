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
        
        private void shutDown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0); 
        }

        private System.Windows.Controls.Button btn;

        public MainWindow()
        {
            players = new List<Player>();
            canAddPlayer = true;
            position = 0;
            stack = 1000;
            btn = new Button();
            LinearGradientBrush gradientBrush = new  LinearGradientBrush( Color.FromRgb( 24, 24, 24),  Color.FromRgb(47, 47, 47), new Point(0.5, 0), new Point(0.5, 1));            
            Background = gradientBrush;
            btn.Background = Background;
            

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
        }

        private void savePlayer(object sender, RoutedEventArgs e)
        {
           
            SurfaceButton s = (SurfaceButton)sender;
            StackPanel stackPanel = (StackPanel)s.Parent;
            SurfaceTextBox text = (SurfaceTextBox)stackPanel.FindName("playerName");
            
            players.Add(new Player(text.Text, stack, position));

            text.Text = "";
            addplayerscatteru.Visibility = Visibility.Collapsed;
            

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
            canAddPlayer = false;
            Grid.Children.Remove(btn);
            showCards();

        }

        private void showCards()
        {
            ScatterViewItem card1 = new ScatterViewItem();
            BitmapImage bmpimage = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/Cards/herz10.png"));
            Image image = new Image();
            image.Source = bmpimage;
            card1.Height = 123.04;
            card1.Width = 80;
            card1.Content = image;
            card1.CanMove = true;
            card1.CanRotate = true;
            Grid.Children.Add(card1);
            

            
            
        }



    }
}
