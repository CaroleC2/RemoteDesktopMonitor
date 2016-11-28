using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ConsumeWebServiceRest
{
    [DataContract]
    public class WSR_Result
    {
        // PLAGE DES CODES ERREUR POUR L'APPEL D'UN WebService ---> [200 - 300[
        [DataMember]
        public const int CodeRet_AppelService = 200;
        [DataMember]
        public const int CodeRet_TimeOutAnnul = 201;
        [DataMember]
        public const int CodeRet_Serialize = 202;
        [DataMember]
        public const int CodeRet_Deserialize = 203;

        [DataMember]
        private string _data;
        [DataMember]
        private string _typeNameData;

        #region Constructeurs

        /// <summary>
        /// Constructeur qui ne retourne pas de data, et qui indique que tout c'est bien passé.
        /// </summary>
        public WSR_Result()
        {
            IsSuccess = true;
        }

        /// <summary>
        /// Constructeur qui retourne des data, et qui indique que tout c'est bien passé.
        /// </summary>
        /// <param name="data">Data retournés par le WebService</param>
        /// <param name="serialize">Permet de définir si les Data doivent être sérialisés ou non</param>
        /// <param name="sourceMemberName">Permet d'obtenir la méthode ou le nom de la propriété de l'appel de ce constructeur</param>
        /// <param name="sourceFilePath">Permet d'obtenir le chemin d'accès complet du fichier source qui contient l'appel de ce constructeur</param>   
        /// <param name="sourceLineNumber">Permet d'obtenir le numéro de ligne dans le fichier source de l'appel de ce constructeur</param>
        public WSR_Result(object data, bool serialize,
                         [CallerMemberName] string sourceMemberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                if (data == null)
                {
                    _data = null;
                    _typeNameData = null;
                }
                else
                {
                    if (serialize)
                    {
                        _data = Serialize.SerializeToString(data);
                        _typeNameData = data.GetType().AssemblyQualifiedName;
                    }
                    else
                    {
                        _data = ((string[])data)[0];
                        _typeNameData = ((string[])data)[1];
                    }
                }

                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                ErrorCode = CodeRet_Serialize;
                ErrorMessage = String.Format(Properties.Resources.ERREUR_APPELSERVICE, ex.Message);
                ErrorSourceMemberName = sourceMemberName;
                ErrorSourceFile = Path.GetFileNameWithoutExtension(sourceFilePath);
                ErrorSourceLineNumber = sourceLineNumber;
            }
        }

        /// <summary>
        /// Constructeur qui indique une erreur.
        /// </summary>
        /// <param name="errorCode">Numéro de l'erreur</param>
        /// <param name="errorMessage">Message de l'erreur</param>
        /// <param name="sourceMemberName">Permet d'obtenir la méthode ou le nom de la propriété de l'appel de ce constructeur</param>
        /// <param name="sourceFilePath">Permet d'obtenir le chemin d'accès complet du fichier source qui contient l'appel de ce constructeur</param>   
        /// <param name="sourceLineNumber">Permet d'obtenir le numéro de ligne dans le fichier source de l'appel de ce constructeur</param>
        public WSR_Result(int errorCode, string errorMessage,
                         [CallerMemberName] string sourceMemberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
            IsSuccess = false;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ErrorSourceMemberName = sourceMemberName;
            ErrorSourceFile = Path.GetFileNameWithoutExtension(sourceFilePath);
            ErrorSourceLineNumber = sourceLineNumber;
        }

        #endregion Constructeurs

        #region Propriétés

        /// <summary>
        /// Permet de récupérer les data désérialisés
        /// </summary>
        public object Data
        {
            get
            {
                // On désérialise la propriété Result dans son type d'origine
                return (_data == null) ? null : Serialize.DeserializeFromString(_data, Type.GetType(_typeNameData));
            }
        }

        /// <summary>
        /// Permet de savoir si il y a des erreurs à traiter
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }


        /// <summary>
        /// Numéro de l'erreur
        /// </summary>
        [DataMember]
        public int ErrorCode { get; set; }


        /// <summary>
        /// Message de l'erreur
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }


        /// <summary>
        /// Méthode ou nom de la propriété de l'appel de ce constructeur
        /// </summary>
        [DataMember]
        public string ErrorSourceMemberName { get; set; }

        /// <summary>
        /// Chemin d'accès complet du fichier source qui contient l'appel de ce constructeur
        /// </summary>
        [DataMember]
        public string ErrorSourceFile { get; set; }


        /// <summary>
        /// Numéro de ligne dans le fichier source de l'appel de ce constructeur
        /// </summary>
        [DataMember]
        public int ErrorSourceLineNumber { get; set; }

        #endregion Propriétés
    }
}
