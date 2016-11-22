using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace ConsumeWebServiceRest
{
    static class Serialize
    {
        /// <summary>
        /// Méthode qui sérialize un 'object' en xml
        /// </summary>
        /// <param name="p">Objet à sérialiser</param>
        /// <returns>Objet sérialisé sous forme de chaîne xml</returns>
        public static string SerializeToString(Object p)
        {
            if (p != null)
            {
                var sb = new StringBuilder();

                using (TextWriter writer = new StringWriter(sb))
                {
                    new XmlSerializer(p.GetType()).Serialize(writer, p);
                }

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Méthode qui permet de désérialiser une chaîne xml en un objet de type T
        /// </summary>
        /// <param name="p">Chaîne xml à désérialiser</param>
        /// <param name="t">Type de l'objet désérialisé</param>
        /// <returns>Objet désérialisé</returns>
        public static Object DeserializeFromString(string p, Type t)
        {
            if (!String.IsNullOrEmpty(p) && t != null)
            {
                using (TextReader reader = new StringReader(p))
                {
                    return new XmlSerializer(t).Deserialize(reader);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
