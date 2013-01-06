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

namespace SurfacePoker
{
	/// <summary>
	/// Interaktionslogik für AddPlayer.xaml
	/// </summary>
	public partial class AddPlayer : UserControl
	{
		public AddPlayer()
		{
			this.InitializeComponent();
		}

		private void closePlayerAdd(object sender, System.Windows.Input.TouchEventArgs e)
		{
			addplayerscatteru.Visibility = Visibility.Hidden;
		}
	}
}