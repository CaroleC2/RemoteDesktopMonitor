using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace ConsumeWebServiceRest
{
    /// <summary>
    /// Cette classe contient les paramètre à passer au WebService sous forme de clés/valeurs. Chaque valeur est stockée sérialisée.
    /// </summary>
    [DataContract]
        public class WSR_Params : IEnumerable
        {
            [DataMember]
            private Dictionary<string, string[]> dicParams;

            #region Constructeurs

            public WSR_Params()
            {
                dicParams = new Dictionary<string, string[]>();
            }

            #endregion Constructeurs

            #region Propriétés

            public object this[string key]
            {
                get
                {
                    if (dicParams[key][0] == null)
                    {
                        return null;
                    }
                    else
                    {
                    // La valeur est retournée désérialisée
                    return Serialize.DeserializeFromString(dicParams[key][0], Type.GetType(dicParams[key][1]));
                    }
                }
            // La valeur est stockée sérialisée
            set { dicParams[key] = new string[2] { Serialize.SerializeToString(value), value.GetType().AssemblyQualifiedName }; }
            }

        #endregion Propriétés

        #region Méthodes

        /// <summary>
        /// Permet d'ajouter une paire clé/valeur dans le dictionnaire.
        /// </summary>
        /// <param name="key">Clé à ajouter au dictionnaire</param>
        /// <param name="value">Valeur liée à ajouter au dictionnaire</param>
        public void Add(string key, object value)
            {
                if (value == null)
                {
                    dicParams.Add(key, new string[2] { null, null });
                }
                else
                {
                    dicParams.Add(key, new string[2] { Serialize.SerializeToString(value), value.GetType().AssemblyQualifiedName });
                }
            }


        /// <summary>
        /// Détermine si le dictionnaire contient une clé spécifique.
        /// </summary>
        /// <param name="key">Clé à rechercher dans le dictionnaire</param>
        /// <returns>true si la clé existe, false sinon</returns>
        public bool ContainsKey(string key)
            {
                return dicParams.ContainsKey(key);
            }

        /// <summary>
        /// Retourne le nombre de paires clé/valeur contenues dans le dictionnaire.
        /// </summary>
        public int Count
            {
                get { return dicParams.Count; }
            }

        /// <summary>
        /// Supprime une valeur dans le dictionnaire à partir de sa clé.
        /// </summary>
        /// <param name="key">Clé de l'élément à supprimer.</param>
        /// <returns>true si la recherche et la suppression de l'élément réussissent, false sinon. Cette méthode retourne false si key est introuvable dans le dictionnaire</returns>
        public bool Remove(string key)
            {
                return dicParams.Remove(key);
            }
        /// <summary>
        /// Permet de supprimer toutes les paires clé/valeur dans le dictionnaire.
        /// </summary>

        public void Clear()
            {
                dicParams.Clear();
            }

        /// <summary>
        /// Retourne une valeur associée à une clé sans la désérialiser.
        /// </summary>
        /// <param name="key">Clé correspondant à la valeur</param>
        /// <returns>Valeur liée à la clé</returns>
        public string[] GetValueSerialized(string key)
            {
                return dicParams[key];
            }

            #endregion Méthodes

            #region IEnumerable Membres

            // Permet d'initialiser la collection à la création
            public IEnumerator GetEnumerator()
            {
                return dicParams.GetEnumerator();
            }

            #endregion
        }
    }

