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
using System.ComponentModel;

namespace SurfacePoker
{
	/// <summary>
	/// Interaktionslogik für PinkCircle.xaml
	/// </summary>
	public partial class PinkCircle : UserControl
	{
        private static Random randomMain = new Random();

        private double angle;
        private double speed;

        private double xPos;
        private double yPos; 

		public PinkCircle()
		{
			this.InitializeComponent();

            this.Loaded += new RoutedEventHandler(CircleLoaded);
		}

        void CircleLoaded(object sender, RoutedEventArgs e)
        {

            xPos = Canvas.GetLeft(this);
            yPos = Canvas.GetTop(this);

            speed = .01 + randomMain.NextDouble();

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {

                CompositionTarget.Rendering += new EventHandler(AnimateCircle);

            }

        }

        void AnimateCircle(object sender, EventArgs e)
        {

            angle += speed / 10;

            Canvas.SetTop(this, yPos + 50 * Math.Sin(angle));
            Canvas.SetLeft(this, xPos + 50 * Math.Cos(angle));

            if (angle >= 2 * Math.PI)
            {

                angle = 0;

            }

        }
	}
}