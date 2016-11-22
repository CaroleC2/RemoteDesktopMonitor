using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace ConsumeWebServiceRest
{
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
                        return Serialize.DeserializeFromString(dicParams[key][0], Type.GetType(dicParams[key][1]));
                    }
                }
                set { dicParams[key] = new string[2] { Serialize.SerializeToString(value), value.GetType().AssemblyQualifiedName }; }
            }

            #endregion Propriétés

            #region Méthodes

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

            public bool ContainsKey(string key)
            {
                return dicParams.ContainsKey(key);
            }

            public int Count
            {
                get { return dicParams.Count; }
            }

            public bool Remove(string key)
            {
                return dicParams.Remove(key);
            }

            public void Clear()
            {
                dicParams.Clear();
            }

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

