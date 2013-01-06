using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;

namespace SurfacePoker
{
    /// <summary>
    /// Auslesen/Schreiben der Spieler aus der Spieler XML mit Hilfe der ByteTags 
    /// </summary>
    class XmlHandler
    {
        XmlDocument xmlDoc;
        String applicationPath;

        public XmlHandler()
        {
            xmlDoc = new XmlDocument();
            applicationPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(XmlHandler)).CodeBase);
        }

        public String readPlayer(int tagID) {

            bool isPlayer=false;
            XmlTextReader reader = new XmlTextReader(applicationPath+"\\Spielerdaten.xml");
            while (reader.Read())
            {
                if (isPlayer && reader.NodeType == XmlNodeType.Text)
                {
                    return reader.Value;
                }

                if (reader.NodeType == XmlNodeType.Text)
                {
                    if (reader.Value.Equals(tagID.ToString()))
                        isPlayer = true;
                }
            }
            Console.ReadLine();

            return "";
        }

        public bool savePlayer(String spielername, int id, String password)
        {

            //vorhandene xml-Datei laden
            xmlDoc.Load(applicationPath + "\\Spielerdaten.xml");

            //Eintrag erzeugen
            XmlElement subRoot = xmlDoc.CreateElement("eintrag");

            //id einfügen
            XmlElement appendedElementID = xmlDoc.CreateElement("id");
            XmlText xmlTextID = xmlDoc.CreateTextNode(id.ToString());
            appendedElementID.AppendChild(xmlTextID);
            subRoot.AppendChild(appendedElementID);
            xmlDoc.DocumentElement.AppendChild(subRoot);

            //spielername einfügen
            XmlElement appendedElementUsername = xmlDoc.CreateElement("spielername");
            XmlText xmlTextUserName = xmlDoc.CreateTextNode(spielername);
            appendedElementUsername.AppendChild(xmlTextUserName);
            subRoot.AppendChild(appendedElementUsername);
            xmlDoc.DocumentElement.AppendChild(subRoot);

            //password einfügen
            XmlElement appendedElementPassword = xmlDoc.CreateElement("password");
            XmlText xmlTextPassword = xmlDoc.CreateTextNode(id.ToString());
            appendedElementPassword.AppendChild(xmlTextPassword);
            subRoot.AppendChild(appendedElementPassword);
            xmlDoc.DocumentElement.AppendChild(subRoot);

            // xml datei speichern
            xmlDoc.Save(applicationPath + "\\Spielerdaten.xml");

            return true;
        }


    }
}
