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
        public const int CodeRetOk = 0;
        public const int CodeRetPseudoUtilise = 1;
        public const int CodeRetPseudoObligatoire = 2;
        public const int CodeRetPseudoDownloadObligatoire = 3;
        public const int CodeRetPseudoNonLogue = 4;
        public const int CodeRetPasswordObligatoire = 5;
        public const int CodeRetPasswordIncorrect = 6;
        public const int CodeRetPseudoDownloadNonLogue = 7;
        public const int CodeRetParamKeyInconnu = 10;
        public const int CodeRetParamTypeInvalid = 11;
        public const int CodeRetErreurInterneService = 100;

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
            string password = null;
            WSR_Result ret = null;

            ret = VerifParamType(p, "pseudo", out pseudo);
            if (ret != null)
            {
                return ret;
            }

            AccountError err = Account.Add(pseudo, out password);

            switch (err)
            {
                case AccountError.Ok:
                return new WSR_Result(password, true);

                case AccountError.KeyNullOrEmpty:
                    return new WSR_Result(CodeRetPseudoObligatoire, String.Format(Properties.Resources.PseudoObligatoire));

                case AccountError.KeyExist:
                    return new WSR_Result(CodeRetPseudoUtilise, String.Format(Properties.Resources.PseudoUtilise));

                default :
                return new WSR_Result(CodeRetErreurInterneService, String.Format(Properties.Resources.ErreurInterneService));
            }

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

            AccountError err = Account.Remove(pseudo,  password);

            switch (err)
            {
                case AccountError.Ok:
                    return new WSR_Result();

                case AccountError.KeyNullOrEmpty:
                    return new WSR_Result(CodeRetPseudoObligatoire, String.Format(Properties.Resources.PseudoObligatoire));

                case AccountError.keyNotFound:
                    return new WSR_Result(CodeRetPseudoNonLogue, String.Format(Properties.Resources.PseudoNonLogue));

                case AccountError.PasswordNullOrEmpty:
                    return new WSR_Result(CodeRetPasswordObligatoire, String.Format(Properties.Resources.PasswordObligatoire));

                case AccountError.PasswordWrong:
                    return new WSR_Result(CodeRetPasswordIncorrect, String.Format(Properties.Resources.PasswordIncorrect));

                default:
                    return new WSR_Result(CodeRetErreurInterneService, String.Format(Properties.Resources.ErreurInterneService));
            }

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

            AccountError err = Account.GetKeys(pseudo, password, out lstKeys);

            switch (err)
            {
                case AccountError.Ok:
                    return new WSR_Result(lstKeys, true);

                case AccountError.KeyNullOrEmpty:
                    return new WSR_Result(CodeRetPseudoObligatoire, String.Format(Properties.Resources.PseudoObligatoire));

                case AccountError.keyNotFound:
                    return new WSR_Result(CodeRetPseudoNonLogue, String.Format(Properties.Resources.PseudoNonLogue));

                case AccountError.PasswordNullOrEmpty:
                    return new WSR_Result(CodeRetPasswordObligatoire, String.Format(Properties.Resources.PasswordObligatoire));

                case AccountError.PasswordWrong:
                    return new WSR_Result(CodeRetPasswordIncorrect, String.Format(Properties.Resources.PasswordIncorrect));

                default:
                    return new WSR_Result(CodeRetErreurInterneService, String.Format(Properties.Resources.ErreurInterneService));
            }

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

            AccountError err = Account.WriteData(pseudo, password, data);

            switch (err)
            {
                case AccountError.Ok:
                    return new WSR_Result();

                case AccountError.KeyNullOrEmpty:
                    return new WSR_Result(CodeRetPseudoObligatoire, String.Format(Properties.Resources.PseudoObligatoire));

                case AccountError.keyNotFound:
                    return new WSR_Result(CodeRetPseudoNonLogue, String.Format(Properties.Resources.PseudoNonLogue));

                case AccountError.PasswordNullOrEmpty:
                    return new WSR_Result(CodeRetPasswordObligatoire, String.Format(Properties.Resources.PasswordObligatoire));

                case AccountError.PasswordWrong:
                    return new WSR_Result(CodeRetPasswordIncorrect, String.Format(Properties.Resources.PasswordIncorrect));

                default:
                    return new WSR_Result(CodeRetErreurInterneService, String.Format(Properties.Resources.ErreurInterneService));
            }

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

            AccountError err = Account.ReadData(pseudo, password,pseudoDownload,out data);

            switch (err)
            {
                case AccountError.Ok:
                    return new WSR_Result(data, false);

                case AccountError.KeyNullOrEmpty:
                    return new WSR_Result(CodeRetPseudoObligatoire, String.Format(Properties.Resources.PseudoObligatoire));

                case AccountError.keyNotFound:
                    return new WSR_Result(CodeRetPseudoNonLogue, String.Format(Properties.Resources.PseudoNonLogue));

                case AccountError.PasswordNullOrEmpty:
                    return new WSR_Result(CodeRetPasswordObligatoire, String.Format(Properties.Resources.PasswordObligatoire));

                case AccountError.PasswordWrong:
                    return new WSR_Result(CodeRetPasswordIncorrect, String.Format(Properties.Resources.PasswordIncorrect));

                case AccountError.keyDownloadNullOrEmpty:
                    return new WSR_Result(CodeRetPseudoDownloadObligatoire, String.Format(Properties.Resources.PseudoDownloadObligatoire));

                case AccountError.keyDownloadNotFound:
                    return new WSR_Result(CodeRetPseudoDownloadNonLogue, String.Format(Properties.Resources.PseudoDownloadNonLogue));

                default:
                    return new WSR_Result(CodeRetErreurInterneService, String.Format(Properties.Resources.ErreurInterneService));
            }


        }

        #endregion IService Membres

        #region Fonctions perso

        private static WSR_Result VerifParamExist(WSR_Params p, string key, out object data)
        {
            data = null;

            if (!p.ContainsKey(key))
                return new WSR_Result(CodeRetParamKeyInconnu, String.Format(Properties.Resources.ParamKeyInconnu, key));

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
                    return new WSR_Result(CodeRetParamTypeInvalid, String.Format(Properties.Resources.ParamTypeInvalid, key));
            }

            return null;
        }

        #endregion Fonctions perso
    }
}
