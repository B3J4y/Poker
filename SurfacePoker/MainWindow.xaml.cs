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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
      
        private bool canAddPlayer { get; set; }

        
        private void start(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0); 
        }

        private void openAddPlayer(object sender, RoutedEventArgs e)
        {
            Rectangle r = (Rectangle)sender;
            String name = r.Name;
            Console.Out.WriteLine("Spieler an" + name + "meldet sich an");
            Point point = new Point();

            switch (r.Name) {
                case "Pos1" : point.X = 270; point.Y = 550; break;
                case "Pos2" : point.X = 470; point.Y = 170; break;
                case "Pos3" : point.X = 1210; point.Y = 170; break;
                case "Pos4" : point.X = 1620; point.Y = 550; break;
                case "Pos5" : point.X = 1210; point.Y = 930; break;
                case "Pos6" : point.X = 470; point.Y = 930; break;
            }
            
            if (canAddPlayer) {
                addplayerscatteru.Center = point;
                addplayerscatteru.Visibility = Visibility.Visible;
                
            }
        }
        private void closeAddPlayer(object sender, RoutedEventArgs e)
        {
            addplayerscatteru.Visibility = Visibility.Hidden;
        }

        private void savePlayer(object sender, RoutedEventArgs e)
        {
            Rectangle r = (Rectangle)sender;
            String name = r.Name;
            int ingamepos = 0;

            switch (r.Name)
            {
                case "Pos1": ingamepos = 1; break;
                case "Pos2": ingamepos = 2; break;
                case "Pos3": ingamepos = 3; break;
                case "Pos4": ingamepos = 4; break;
                case "Pos5": ingamepos = 5; break;
                case "Pos6": ingamepos = 6; break;
            }



        }

        private void switchAdd(object sender, RoutedEventArgs e)
        {
            if (canAddPlayer)
            {
                canAddPlayer = false;
            }
            else
            {
                canAddPlayer = true;
            }
        }







    }
}
