using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ConsumeWebServiceRest;




namespace RDMService
{
    /// <summary>
    /// Cette classe contient les méthodes fournies par le webservice RDMService. Elle permet de gèrer des comptes utilisateurs
    /// </summary>
    public class ServiceRDM : IServiceRDM
    {
        // PLAGE DES CODES ERREUR POUR LE WebService ---> [1 - 200[
        public const int CodeRet_Ok = 0;
        public const int CodeRet_PseudoUtilise = 1;
        public const int CodeRet_PseudoObligatoire = 2;
        public const int CodeRet_PseudoDownloadObligatoire = 3;
        public const int CodeRet_Logout = 4;
        public const int CodeRet_PasswordObligatoire = 5;
        public const int CodeRet_PasswordIncorrect = 6;
        public const int CodeRet_PseudoDownloadLogout = 7;
        public const int CodeRet_ParamKeyInconnu = 10;
        public const int CodeRet_ParamTypeInvalid = 11;
        public const int CodeRet_ErreurInterneService = 100;

        #region IServiceRDM Membres

        /// <summary>
        /// Permet de se loguer au WebService
        /// </summary>
        /// <param name="p">Dictionnaire contenant votre identifiant</param>
        /// <returns>Valeurs de retour contenant votre mot de passe.
        ///  Il sera nécessaire pour le Logout et l'écriture de vos données</returns>
        public WSR_Result Login(WSR_Params p)
        {
            string pseudo = null;
            //string password = null;
            WSR_Result ret = null;

            ret = VerifParamType(p, "pseudo", out pseudo);
            if (ret != null)
            {
                return ret;
            }
            return ret;

        }

        /// <summary>
        /// Permet de se Déloguer du WebService
        /// </summary>
        /// <param name="p">Dictionnaire contenant votre identifiant et votre mot de passe></param>
        /// <returns>Valeurs de retour</returns>
        public WSR_Result Logout(WSR_Params p)
        {
            string pseudo = null;
            string password = null;
            WSR_Result ret = null;

            ret = VerifParamType(p, "pseudo", out pseudo);
            if (ret != null)
            {
                return ret;
            }
            

            ret = VerifParamType(p, "password", out password);
            if (ret != null)
            {
                return ret;
            }
            return ret;
        }

        /// <summary>
        /// Permet d'obtenir la liste des utilisateurs logués au WebService
        /// </summary>
        /// <param name="p">Dictionnaire contenant votre identifiant et votre mot de passe</param>
        /// <returns>Valeurs de retour contenant la liste des utilisateurs connectés</returns>
        public WSR_Result GetPseudos(WSR_Params p)
        {
            string pseudo = null;
            string password = null;
            List<string> lstKeys = null;
            WSR_Result ret = null;

            ret = VerifParamType(p, "pseudo", out pseudo);
            if (ret != null)
            {
                return ret;
            }

            ret = VerifParamType(p, "password", out password);
            if (ret != null)
            {
                return ret;
            }

            return ret;
        }

        /// <summary>
        /// Permet d'écrire des données associées à votre compte utilisateur
        /// </summary>
        /// <param name="p">Dictionnaire contenant votre identifiant,votre mot de passe et les données à écrire</param>
        /// <returns>Valeurs de retour</returns>
        public WSR_Result UploadData(WSR_Params p)
        {
            string pseudo = null;
            string password = null;
            object data = null;
            WSR_Result ret = null;

            ret = VerifParamType(p, "pseudo", out pseudo);
            if (ret != null)
            {
                return ret;
            }

            ret = VerifParamType(p, "password", out password);
            if (ret != null)
            {
                return ret;
            }

            ret = VerifParamExist(p, "data", out data); // Pas de vérification de type, il peut être personnalisé.
            if (ret != null)
            {
                return ret;
            }

            return ret;
        }

        /// <summary>
        /// Permet de lire les données associées à un compte utilisateur
        /// </summary>
        /// <param name="p">Dictionnaire contenant votre identifiant, votre mot de passe 
        /// et l'identifiant du compte à lire</param>
        /// <returns>Valeurs de retour contenant les données lues</returns>
        public WSR_Result DownloadData(WSR_Params p)
        {
            string pseudo = null;
            string password = null;
            string pseudoDownload = null;
            object data = null;
            WSR_Result ret = null;

            ret = VerifParamType(p, "pseudo", out pseudo);
            if (ret != null)
            {
                return ret;
            }


            ret = VerifParamType(p, "password", out password);
            if (ret != null)
            {
                return ret;
            }

            ret = VerifParamType(p, "pseudoDownload", out pseudoDownload);
            if (ret != null)
            {
                return ret;
            }

            return ret;

        }

        #endregion IService Membres

        #region Fonctions perso

        private static WSR_Result VerifParamExist(WSR_Params p, string key, out object data)
        {
            data = null;

            if (!p.ContainsKey(key))
                return new WSR_Result(CodeRet_ParamKeyInconnu, String.Format(Properties.Resources.PARAMKEYINCONNU, key));

            data = p.GetValueSerialized(key);

            return null;
        }

        private static WSR_Result VerifParamType<T>(WSR_Params p, string key, out T value) where T : class
        {
            object data = null;
            value = null;

            WSR_Result ret = VerifParamExist(p, key, out data);
            if (ret != null)
                return ret;

            if (p[key] != null)
            {
                try
                {
                    value = p[key] as T; // Permet de vérifier le type
                }
                catch (Exception) { } // Il peut y avoir exception si le type est inconnu (type personnalisé qui n'est pas dans les références)

                if (value == null)
                    return new WSR_Result(CodeRet_ParamTypeInvalid, String.Format(Properties.Resources.PARAMTYPEINVALID, key));
            }

            return null;
        }

        #endregion Fonctions perso
    }
}
