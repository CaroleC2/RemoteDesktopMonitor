using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace RDMService
{
   
     /// <summary>
      /// Codes erreurs retournés par la classeAccount
      /// </summary>
        public enum AccountError
        {
            Ok = 0,
            KeyNullOrEmpty = 10,
            KeyExist = 11,
            keyNotFound = 12,
            keyDownloadNullOrEmpty = 13,
            keyDownloadNotFound = 14,
            PasswordNullOrEmpty = 20,
            PasswordWrong = 21,
        }

        /// <summary>
        /// 
        /// Cette classe permet de manipuler un objet de type MemoryCache. Tous les accès à MemoryCache sont thread safe, donc pas besoin de verrous pour accèder et
        /// mettre à jour les données. Pour tous les accès il faut fournir un identifiant, c'est la clé du compte utilisateur stocké. Si le compte n'est pas accédé
        /// en lecture ou en écriture durant une certaine période (ici 60s), il est détruit. Ceci permet de ne pas conserver des données inutiles en memoire.
        /// 
        /// Seul le propriétaire d'un compte peut écrire dessus. Par contre la lecture est ouverte à tous les utilisateur logués
        /// 
        /// </summary>
        public static class Account
        {
            private const string NOM_MEMACCOUNTS = "SharedMemoryAccounts";
            private const int EXPIRATION_ACCOUNT = 60; // 60s

            private static MemoryCache _memAccounts = new MemoryCache(NOM_MEMACCOUNTS);

            #region Méthodes de gestion des comptes

            /// <summary>
            /// Permet d'ajouter un compte utilisateur dans la mémoire cache
            /// </summary>
            /// <param name="key">Clé de votre mémoire cache à insérer (votre identifiant)</param>
            /// <param name="password">Mot de passe en retour alloué par la méthode. Il devra être fourni pour les modifications ultérieures (WriteData)</param>
            /// <returns>Code retour de la méthode</returns>
            public static AccountError Add(string key, out string password)
            {
                password = null;

                if (String.IsNullOrWhiteSpace(key))
                {
                    return AccountError.KeyNullOrEmpty;
                }

                Value value = new Value(Guid.NewGuid().ToString());

                // On ajoute la clé dans le MemoryCache avec un trigger qui sera déclenché à la supression de cette clé
                if (_memAccounts.Add(key, value, new CacheItemPolicy() { SlidingExpiration = new TimeSpan(0, 0, EXPIRATION_ACCOUNT), RemovedCallback = RemovedKey }))
                {
                    password = value.Password;
                    return AccountError.Ok;
                }
                else
                {
                    return AccountError.KeyExist;
                }
            }

            /// <summary>
            /// Permet de supprimer un compte utilisateur dans la mémoire cache
            /// </summary>
            /// <param name="key">Clé de votre mémoire cache à supprimer (votre identifiant)</param>
            /// <param name="password">Votre mot de passe (retourné par la méthode Add)</param>
            /// <returns>Code retour de la méthode</returns>
            public static AccountError Remove(string key, string password)
            {
                if (String.IsNullOrEmpty(key))
                {
                    return AccountError.KeyNullOrEmpty;
                }
                else if (String.IsNullOrEmpty(password))
                {
                    return AccountError.PasswordNullOrEmpty;
                }

                Value value = (Value)_memAccounts.Get(key);

                if (value != null)
                {
                    if (value.Password == password)
                    {
                        // Pas d'erreur si la clé n'existe plus. Elle peut avoir été supprimée depuis le test d'existence (multi threading)
                        _memAccounts.Remove(key);
                        return AccountError.Ok;
                    }
                    else
                    {
                        return AccountError.PasswordWrong;
                    }
                }
                else
                {
                    return AccountError.keyNotFound;
                }
            }

            /// <summary>
            /// Permet de connaitre la liste des comptes utilisateur stockés dans la mémoire cache
            /// </summary>
            /// <param name="key">Clé de votre mémoire cache (votre identifiant)</param>
            /// <param name="password">Votre mot de passe (retourné par la méthode Add)</param>
            /// <param name="keys">Liste des comptes utilisateur (clés) contenus dans la mémoire cache</param>
            /// <returns>Code retour de la méthode</returns>
            public static AccountError GetKeys(string key, string password, out List<string> keys)
            {
                keys = new List<string>();

                if (String.IsNullOrEmpty(key))
                {
                    return AccountError.KeyNullOrEmpty;
                }
                else if (String.IsNullOrEmpty(password))
                {
                    return AccountError.PasswordNullOrEmpty;
                }

                Value value = (Value)_memAccounts.Get(key);

                if (value != null)
                {
                    if (value.Password == password)
                    {
                        keys = _memAccounts.Select(k => k.Key).ToList();
                        return AccountError.Ok;
                    }
                    else
                    {
                        return AccountError.PasswordWrong;
                    }
                }
                else
                {
                    return AccountError.keyNotFound;
                }
            }

            /// <summary>
            /// Permet d'écrire des données dans le compte utilisateur spécifié
            /// </summary>
            /// <param name="key">Clé de votre mémoire cache (votre identifiant)</param>
            /// <param name="password">Votre mot de passe (retourné par la méthode Add)</param>
            /// <param name="data">Les données à écrire</param>
            /// <returns>Code retour de la méthode</returns>
            public static AccountError WriteData(string key, string password, object data)
            {
                if (String.IsNullOrEmpty(key))
                {
                    return AccountError.KeyNullOrEmpty;
                }
                else if (String.IsNullOrEmpty(password))
                {
                    return AccountError.PasswordNullOrEmpty;
                }

                Value value = (Value)_memAccounts.Get(key);

                if (value != null)
                {
                    if (value.Password == password)
                    {
                        value.Data = data;
                        return AccountError.Ok;
                    }
                    else
                    {
                        return AccountError.PasswordWrong;
                    }
                }
                else
                {
                    return AccountError.keyNotFound;
                }
            }

            /// <summary>
            /// Permet de lire les données dans un compte utilisateur spécifié
            /// </summary>
            /// <param name="key">Clé de votre mémoire cache (votre identifiant)</param>
            /// <param name="password">Votre mot de passe (retourné par la méthode Add)</param>
            /// <param name="keyDownload">Clé de la mémoire mémoire cache à lire</param>
            /// <param name="data">Données lues en retour</param>
            /// <returns>Code retour de la méthode</returns>
            public static AccountError ReadData(string key, string password, string keyDownload, out object data)
            {
                data = null;

                if (String.IsNullOrEmpty(key))
                {
                    return AccountError.KeyNullOrEmpty;
                }
                else if (String.IsNullOrEmpty(password))
                {
                    return AccountError.PasswordNullOrEmpty;
                }
                else if (String.IsNullOrEmpty(keyDownload))
                {
                    return AccountError.keyDownloadNullOrEmpty;
                }

                Value valueDownLoad = (Value)_memAccounts.Get(keyDownload);

                if (valueDownLoad != null)
                {
                    Value value = (Value)_memAccounts.Get(key);

                    if (value != null)
                    {
                        if (value.Password == password)
                        {
                            data = valueDownLoad.Data;
                            return AccountError.Ok;
                        }
                        else
                        {
                            return AccountError.PasswordWrong;
                        }
                    }
                    else
                    {
                        return AccountError.keyNotFound;
                    }
                }
                else
                {
                    return AccountError.keyDownloadNotFound;
                }
            }

            #endregion Méthodes de gestion des comptes

            #region Evenements MemoryCache

            private static void RemovedKey(CacheEntryRemovedArguments args)
            {
                // Traitement spécifique à la suppression d'un Account
            }

            #endregion Evenements MemoryCache

        }
    }
