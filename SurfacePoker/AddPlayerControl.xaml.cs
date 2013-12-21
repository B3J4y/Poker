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
            //TODO: Handle "System.FormatException"
            int tag1 =Convert.ToInt32(tagid1.Text);
            int tag2 =Convert.ToInt32(tagid2.Text);

            xmlHandler.savePlayer(spielern, tag1, tag2);

		}
	}
}