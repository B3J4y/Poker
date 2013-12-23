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
    public class XmlHandler
    {
        XmlDocument xmlDoc;
        String applicationPath;

        public XmlHandler()
        {
            xmlDoc = new XmlDocument();
            applicationPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(XmlHandler)).CodeBase);
        }

        public String readPlayer(int tagID) {
            //TODO: FileNotFoundException
            bool isPlayer=false;
            XmlTextReader reader = new XmlTextReader(applicationPath+"\\Res\\Spielerdaten.xml");
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
            //Console.ReadLine();

            return "";
        }

        public bool savePlayer(String spielername, int id1, int id2)
        {
            string URIpath = applicationPath + "\\Res\\Spielerdaten.xml";
            string path = new Uri(URIpath).LocalPath;
            Console.Out.WriteLine(path);
            
            
            //Spielerdaten.xml anlegen öffnen
            if (!(File.Exists(path)))
            {
                var myFile = File.Create(path); //Datei anlegen
                myFile.Close();                 // Ressource freigeben
                
                XmlNode element1 = xmlDoc.CreateElement( "verzeichnis");    //Root erzeugen
                xmlDoc.AppendChild(element1);        

            } else{            
                //vorhandene xml-Datei laden
                xmlDoc.Load(path);
            }

            //Eintrag erzeugen
            XmlElement subRoot = xmlDoc.CreateElement("eintrag");

            //id einfügen
            XmlElement appendedElementID1 = xmlDoc.CreateElement("id1");
            XmlText xmlTextID1 = xmlDoc.CreateTextNode(id1.ToString());
            appendedElementID1.AppendChild(xmlTextID1);
            subRoot.AppendChild(appendedElementID1);
            xmlDoc.DocumentElement.AppendChild(subRoot);

            //id2 einfügen
            XmlElement appendedElementID2 = xmlDoc.CreateElement("id2");
            XmlText xmlTextID2 = xmlDoc.CreateTextNode(id2.ToString());
            appendedElementID2.AppendChild(xmlTextID2);
            subRoot.AppendChild(appendedElementID2);
            xmlDoc.DocumentElement.AppendChild(subRoot);


            //spielername einfügen
            XmlElement appendedElementUsername = xmlDoc.CreateElement("spielername");
            XmlText xmlTextUserName = xmlDoc.CreateTextNode(spielername);
            appendedElementUsername.AppendChild(xmlTextUserName);
            subRoot.AppendChild(appendedElementUsername);
            xmlDoc.DocumentElement.AppendChild(subRoot);

           

            // xml datei speichern
            xmlDoc.Save(path);

            return true;
        }


    }
}
