using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ConsoleAppBlackJack
{
    public class XMLConvert
    {
        public static string ObjectToXml(List<Player> playerList)
        {
            string returnXml = string.Empty;

            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(List<Player>));
                StringWriter stringWriter = new StringWriter();
                using (XmlWriter writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, playerList);
                    returnXml = stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade. Felkod: {ex}");
            }

            return returnXml;
        }

        public static List<Player> XMLToObject(string xml)
        {
            List<Player> playerList = new List<Player>();

            try
            {
                using (TextReader reader = new StringReader(xml))
                {
                    playerList = (List<Player>)new XmlSerializer(typeof(List<Player>))
                        .Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade. Felkod: {ex}");
            }

            return playerList;
        }
    }
}