using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsumeWebServiceRest
{
    public enum TypeSerializer
    {
        Xml = 0,
        Json = 1,
    }

    /// <summary>
    /// Cette classe permet d'appeler un service REST en asynchrone. C'est la méthode POST qui est utilisée pour l'appel du service. L'échange
    /// de données se fait soit en Xml soit en Json.
    /// </summary>
    public static class ConsumeWSR
    {

        /// <summary>
        /// Appel d'un Service REST qui retourne un objet de type WSR_Result en réponse. Cette opération peut être annulée
        /// </summary>
        /// <param name="adresseService">Adresse du service</param>
        /// <param name="parametres">Paramètres passés au service</param>
        /// <param name="typeSerializer">Type de sérialisation (Xml/Json)</param>
        /// <param name="cancel">Jeton permettant d'annuler l'appel en cours</param>
        /// <returns>Objet retourné par le service ou Erreur, de type WSR_Result</returns>
        public static async Task<WSR_Result> Call(string adresseService, WSR_Params parametres, TypeSerializer typeSerializer, CancellationToken cancel)
        {
            try
            {
                // Création de l'instance HttpClient avec Timeout infini car c'est le CancellationToken qui gère l'arrêt ou le TimeOut de la tâche
                // ATTENTION, en Windows phone on a quand même un timeout au bout de 60s
                using (HttpClient client = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite) })
                {
                    // Permet de supprimer la mise en cache. En WindowsPhone, deux requêtes successives identiques retournent le résultat de la première 
                    // qui a été mis en cache
                    client.DefaultRequestHeaders.IfModifiedSince = DateTimeOffset.Now;

                    // Sérialisation des paramètres
                    using (StringContent contentParametres = SerializeParam(parametres, typeSerializer))
                    {
                        // Appel du service Rest (en asynchrone)
                        using (HttpResponseMessage wcfResponse = await client.PostAsync(adresseService, contentParametres, cancel))
                        {
                            if (wcfResponse.IsSuccessStatusCode)
                            {
                                // Désérialisation de la réponse du service
                                return DeserializeHttpContent(wcfResponse.Content, typeSerializer);
                            }
                            else
                            {
                                // ATTENTION en Windows phone on a une erreur 404 au bout de 60s même avec le timeout 'Timeout.Infinite'
                                return new WSR_Result(WSR_Result.CodeRet_AppelService, String.Format(Properties.Resources.ERREUR_APPELSERVICE, wcfResponse.ReasonPhrase));
                            }
                        }
                    }
                }
            }
            catch (Exception ex) // Erreur d'annulation, de sérialisation/désérialisation ou d'appel au service REST ...
            {
                if (ex is TaskCanceledException) { return new WSR_Result(WSR_Result.CodeRet_TimeOutAnnul, String.Format(Properties.Resources.ERREUR_TIMEOUT, adresseService)); }
                else if (ex is SerializationException) { return new WSR_Result(WSR_Result.CodeRet_Serialize, String.Format(Properties.Resources.ERREUR_SERIALISATIONPARAMS)); }
                else { return new WSR_Result(WSR_Result.CodeRet_AppelService, String.Format(Properties.Resources.ERREUR_APPELSERVICE, ex.Message)); }
            }
        }

        /// <summary>
        /// Cette méthode permet de sérialiser un objet
        /// </summary>
        /// <typeparam name="T">Type de l'objet à sérialiser</typeparam>
        /// <param name="param">Objet à sérialiser</param>
        /// <param name="typeSerializer">Type de sérialisation (Xml/Json)</param>
        /// <returns>Objet sérialisé</returns>
        private static StringContent SerializeParam(WSR_Params param, TypeSerializer typeSerializer)
        {
            try
            {
                if (param != null)
                {
                    if (typeSerializer == TypeSerializer.Xml)
                    {
                        using (MemoryStream memStream = new MemoryStream())
                        {
                            new DataContractSerializer(typeof(WSR_Params)).WriteObject(memStream, param);
                            string str = Encoding.UTF8.GetString(memStream.ToArray(), 0, (int)memStream.Length);
                            return new StringContent(str, Encoding.UTF8, Properties.Resources.SERIALISATION_XML);
                        }
                    }
                    else
                    {
                        using (MemoryStream memStream = new MemoryStream())
                        {
                            new DataContractJsonSerializer(typeof(WSR_Params)).WriteObject(memStream, param);
                            string str = Encoding.UTF8.GetString(memStream.ToArray(), 0, (int)memStream.Length);
                            return new StringContent(str, Encoding.UTF8, Properties.Resources.SERIALISATION_JSON);
                        }
                    }
                }
                else
                {
                    return new StringContent(String.Empty);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException(String.Format(Properties.Resources.ERREUR_SERIALISATIONPARAMS), ex);
            }
        }

        /// <summary>
        /// Cette méthode permet de désérialiser un objet
        /// </summary>
        /// <param name="content">Objet à désérialiser</param>
        /// <param name="typeSerializer">Type de sérialisation (Xml/Json)</param>
        /// <returns>Objet désérialisé</returns>
        private static WSR_Result DeserializeHttpContent(HttpContent content, TypeSerializer typeSerializer)
        {
            try
            {
                using (Stream s = content.ReadAsStreamAsync().Result)
                {
                    if (s.Length > 0)
                    {
                        if (typeSerializer == TypeSerializer.Xml)
                        {
                            return (WSR_Result)new DataContractSerializer(typeof(WSR_Result)).ReadObject(s);
                        }
                        else
                        {
                            return (WSR_Result)new DataContractJsonSerializer(typeof(WSR_Result)).ReadObject(s);
                        }
                    }
                    else
                    {
                        return default(WSR_Result);
                    }
                }
            }
            catch (Exception)
            {
                return new WSR_Result(WSR_Result.CodeRet_Deserialize, String.Format(Properties.Resources.ERREUR_DESERIALISATIONRETOUR));
            }
        }


    }
}
