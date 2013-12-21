using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System;

using SurfacePoker;
using System.Collections.Generic;

namespace SurfacePoker
{

    /// <summary>
    /// UDP Kommunikation mit den Mobilen Endgeräten
    /// Karten an Handy übertragen
    /// Spieler verifikation
    /// </summary>
    public class NetworkServer
    {

        String username;
        String password;
        int tagid;

        List<Player> activePlayers;

        public NetworkServer()
        {
            activePlayers = new List<Player>();
        }


        public void setPlayer(Player player)
        {
            activePlayers.Add(player);
        }

        public void listenUDP()
        {
            //TODO: UDP Handler?
            while (true)
            {
                byte[] data = new byte[1024];
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 50010);
                UdpClient newsock = new UdpClient(ipep);

                Console.WriteLine("Waiting for a client...");

                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

                data = newsock.Receive(ref sender);

                Console.WriteLine("Message received from {0}:", sender.ToString());
                Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

                String msgFromClient = Encoding.ASCII.GetString(data, 0, data.Length);

                char separator = '.';

                string[] clientCmd = msgFromClient.Split(separator);

                int counter = 0;
                foreach (string cmd in clientCmd)
                {
                    if (counter == 0)
                        username = cmd;
                    if (counter == 1)
                        password = cmd;
                    if (counter == 2)
                        tagid = Convert.ToInt32(cmd);

                    counter++;

                    Console.WriteLine(cmd + "  ");
                }

                foreach (Player player in activePlayers)
                {
                    if (player.getPlayername().Equals(username))
                    {
                        String card0 = player.getHand().getCard(0);
                        String card1 = player.getHand().getCard(1);
                        string welcome = card0 + "." + card1;
                        data = Encoding.ASCII.GetBytes(welcome);
                        newsock.Send(data, data.Length, sender);
                        player.PlayerConnection = true;
                    }
                }

                newsock.Close();
            }
        }




    }
}
