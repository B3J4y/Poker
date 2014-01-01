using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;

namespace SurfacePoker
{
    /// <summary>
    /// Interaktionslogik für PlayerCard.xaml
    /// </summary>
    public partial class PlayerCard : UserControl
    {
        int touchdowncounter = 0;
        int touches = 0;
        int touchtimeFirst;
        int touchtimeSecond;

        DateTime startTime;
        DateTime endTime;
        TimeSpan span;

        public PlayerCard()
        {
            this.InitializeComponent();
        }

        public void setImage(BitmapImage card)
        {
            CardDeck.Source = card;
        }

        public void showCard() {
            //TODO: Image not found
            //Image ist die Kartenrückseite - das Logo
            PlayerCard image = (PlayerCard)FindResource("image");
            image.Visibility = Visibility.Hidden;
        }


        private void Canvas_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {



            if (touchdowncounter == 1)
            {
                endTime = DateTime.Now;
                span = endTime.Subtract(startTime);
            }
            if (touchdowncounter == 0)
                startTime = DateTime.Now;

            touchdowncounter++;
            if (touchdowncounter == 2 && span.Milliseconds <= 145)
            {
                touchdowncounter = 0;
                Storyboard TagHelpText = (Storyboard)FindResource("showcard");

                TagHelpText.Begin(this);


            }


            if (touchdowncounter == 2)
            {
                touchdowncounter = 0;

            }

            Console.WriteLine("First " + span.Milliseconds);
        }
    }
}