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
	/// Interaktionslogik für AddPlayerControl.xaml
	/// </summary>
	public partial class AddPlayerControl : UserControl
	{

        XmlHandler xmlHandler;
		public AddPlayerControl()
		{
			this.InitializeComponent();
		}

		private void savePlayer(object sender, System.Windows.Input.TouchEventArgs e)
		{
            xmlHandler = new XmlHandler();

            String spielern = spielername.Text;
            String pw = netzwerkpasswort.Text;
            int tag =Convert.ToInt32(tagid.Text);

            xmlHandler.savePlayer(spielern, tag, pw);

		}
	}
}