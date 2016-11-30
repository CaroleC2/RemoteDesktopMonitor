using System;
using System.Threading;
using System.Threading.Tasks;
using ConsumeWebServiceRest;

namespace RDMDALWSR
{
    /// <summary>
    /// Classe d'accès au webservice RDMService.
    /// </summary>
    public class RdmDalWSR
    {
        public const int CodeRet_RDMService_Logout = 4;

        private const string ADR_LOGIN = "ServiceRDM.svc/Login";
        private const string ADR_LOGOUT = "ServiceRDM.svc/Logout";
        private const string ADR_GETPSEUDOS = "ServiceRDM.svc/GetPseudos";
        private const string ADR_UPLOADDATAS = "ServiceRDM.svc/UploadData";
        private const string ADR_DOWNLOADDATAS = "ServiceRDM.svc/DownloadData";

        private string _stringConnect = String.Empty;
        private string _pseudoConnect = String.Empty;
        private string _password = String.Empty;
        private volatile bool _isLogged = false;
        private object _verrou = new object();

        #region Constructeurs

        public RdmDalWSR() { }

        public RdmDalWSR(string stringConnect, string pseudoConnect)
        {
            _stringConnect = stringConnect;
            _pseudoConnect = pseudoConnect;
        }

        #endregion Constructeurs

        #region Propriétés

        /// <summary>
        /// Adresse de base du WebService. On rajoute le catatère '/' si nécessaire
        /// </summary>
        public string StringConnect
        {
            get { return _stringConnect; }
            set
            {
                if (value[value.Length - 1] != '/')
                {
                    value += "/";
                }

                _stringConnect = value;
            }
        }

        /// <summary>
        /// Pseudonyme pour le Login 
        /// </summary>
        public string PseudoConnect
        {
            get { return _pseudoConnect; }
            set { _pseudoConnect = value; }
        }

        /// <summary>
        /// Mot de passe retourné par la méthode 'LoginAsync'
        /// </summary>
        public string Password
        {
            get { lock (_verrou) { return _password; } }
            private set
            {
                lock (_verrou)
                {
                    _password = value;
                }
            }
        }

        /// <summary>
        /// Permet de savoir si on est logué au WebService (Après appel de la méthode 'LoginAsync')
        /// </summary>
        /// 
        public bool IsLogged
        {
            get { lock (_verrou) { return _isLogged; } }
            private set
            {
                lock (_verrou)
                {
                    _isLogged = value;

                    if (!_isLogged)
                    {
                        _password = String.Empty;
                    }
                }
            }
        }

        #endregion Propriétés

        #region Méthodes

        /// <summary>
        /// Permet de se loguer. Appel asynchrone
        /// </summary>
        /// <param name="cancel">Permet d'annuler le traitement en cours</param>
        /// <returns>Résutat retourné par le WebService</returns>
        public async Task<RdmDalWSRResult> LoginAsync(CancellationToken cancel)
        {
            if (IsLogged)
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_Login, Properties.Resources.PSEUDODEJALOGUE);
            }
            else if (String.IsNullOrWhiteSpace(_pseudoConnect))
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_PseudoObligatoire, Properties.Resources.PSEUDOOBLIGATOIRE);
            }

            WSR_Params p;
            try
            {
                p = new WSR_Params() { { "pseudo", _pseudoConnect } };
            }
            catch (Exception ex) { return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_SerialisationParams, String.Format(Properties.Resources.SERIALISATIONPARAMS, ex.Message)); }

            WSR_Result ret = await ConsumeWSR.Call(String.Concat(StringConnect, ADR_LOGIN), p, TypeSerializer.Json, cancel);

            if (ret.IsSuccess)
            {
                lock (_verrou)
                {
                    IsLogged = true;
                    Password = (string)ret.Data;
                }
            }

            return new RdmDalWSRResult(ret);
        }

        /// <summary>
        /// Permet de se déloguer. Appel asynchrone
        /// </summary>
        /// <param name="cancel">Permet d'annuler le traitement en cours</param>
        /// <returns>Résutat retourné par le WebService</returns>
        public async Task<RdmDalWSRResult> LogoutAsync(CancellationToken cancel)
        {
            if (!IsLogged)
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_Logout, Properties.Resources.PSEUDONONLOGUE);
            }
            else if (String.IsNullOrWhiteSpace(_pseudoConnect))
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_PseudoObligatoire, Properties.Resources.PSEUDOOBLIGATOIRE);
            }
            else if (String.IsNullOrWhiteSpace(_password))
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_PasswordObligatoire, Properties.Resources.PASSWORDOBLIGATOIRE);
            }

            WSR_Params p;
            try
            {
                p = new WSR_Params() { { "pseudo", _pseudoConnect }, { "password", _password } };
            }
            catch (Exception ex) { return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_SerialisationParams, String.Format(Properties.Resources.SERIALISATIONPARAMS, ex.Message)); }

            WSR_Result ret = await ConsumeWSR.Call(String.Concat(StringConnect, ADR_LOGOUT), p, TypeSerializer.Json, cancel);

            // Même si l'appel au WebService échoue on place la connexion dans l'état déconnectée
            IsLogged = false;

            return new RdmDalWSRResult(ret);
        }

        /// <summary>
        /// Demande la liste des pseudos logués au WebService. Appel asynchrone
        /// </summary>
        /// <param name="cancel">Permet d'annuler le traitement en cours</param>
        /// <returns>Résutat retourné par le WebService</returns>
        public async Task<RdmDalWSRResult> GetPseudosAsync(CancellationToken cancel)
        {
            if (!IsLogged)
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_Logout, Properties.Resources.PSEUDONONLOGUE);
            }
            else if (String.IsNullOrWhiteSpace(_pseudoConnect))
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_PseudoObligatoire, Properties.Resources.PSEUDOOBLIGATOIRE);
            }
            else if (String.IsNullOrWhiteSpace(_password))
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_PasswordObligatoire, Properties.Resources.PASSWORDOBLIGATOIRE);
            }

            WSR_Params p;
            try
            {
                p = new WSR_Params() { { "pseudo", _pseudoConnect }, { "password", _password } };
            }
            catch (Exception ex) { return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_SerialisationParams, String.Format(Properties.Resources.SERIALISATIONPARAMS, ex.Message)); }

            WSR_Result ret = await ConsumeWSR.Call(String.Concat(StringConnect, ADR_GETPSEUDOS), p, TypeSerializer.Xml, cancel);

            // On peut avoir été déconnecté depuis... (Multi-Threading)
            if (!IsLogged)
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_Logout, Properties.Resources.PSEUDONONLOGUE);
            }
            else if (ret.ErrorCode == CodeRet_RDMService_Logout)
            {
                IsLogged = false;
            }

            return new RdmDalWSRResult(ret);
        }

        /// <summary>
        /// Upload des données dans le cloud. Appel asynchrone
        /// </summary>
        /// <param name="data">Données à sauvegarder</param>
        /// <param name="cancel">Permet d'annuler le traitement en cours</param>
        /// <returns>Résutat retourné par le WebService</returns> 
        public async Task<RdmDalWSRResult> UploadDataAsync(object data, CancellationToken cancel)
        {
            if (!IsLogged)
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_Logout, Properties.Resources.PSEUDONONLOGUE);
            }
            else if (String.IsNullOrWhiteSpace(_pseudoConnect))
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_PseudoObligatoire, Properties.Resources.PSEUDOOBLIGATOIRE);
            }
            else if (String.IsNullOrWhiteSpace(_password))
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_PasswordObligatoire, Properties.Resources.PASSWORDOBLIGATOIRE);
            }

            WSR_Params p;
            try
            {
                p = new WSR_Params() { { "pseudo", _pseudoConnect }, { "password", _password }, { "data", data } };
            }
            catch (Exception ex) { return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_SerialisationParams, String.Format(Properties.Resources.SERIALISATIONPARAMS, ex.Message)); }

            WSR_Result ret = await ConsumeWSR.Call(String.Concat(StringConnect, ADR_UPLOADDATAS), p, TypeSerializer.Json, cancel);

            // On peut avoir été déconnecté depuis... (Multi-Threading)
            if (!IsLogged)
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_Logout, Properties.Resources.PSEUDONONLOGUE);
            }
            else if (ret.ErrorCode == CodeRet_RDMService_Logout)
            {
                IsLogged = false;
            }

            return new RdmDalWSRResult(ret);
        }

        /// <summary>
        /// Download des données à partir du cloud. Appel asynchrone
        /// </summary>
        /// <param name="pseudoDownload">Le pseudo dont on veut récupérer les données</param>
        /// <param name="cancel">Permet d'annuler le traitement en cours</param>
        /// <returns>Résutat retourné par le WebService</returns>
        public async Task<RdmDalWSRResult> DownloadDataAsync(string pseudoDownload, CancellationToken cancel)
        {
            if (!IsLogged)
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_Logout, Properties.Resources.PSEUDONONLOGUE);
            }
            else if (String.IsNullOrWhiteSpace(_pseudoConnect))
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_PseudoObligatoire, Properties.Resources.PSEUDOOBLIGATOIRE);
            }
            else if (String.IsNullOrWhiteSpace(_password))
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_PasswordObligatoire, Properties.Resources.PASSWORDOBLIGATOIRE);
            }
            else if (String.IsNullOrWhiteSpace(pseudoDownload))
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_PseudoDownloadObligatoire, Properties.Resources.PSEUDODOWNLOADOBLIGATOIRE);
            }

            WSR_Params p;
            try
            {
                p = new WSR_Params() { { "pseudo", _pseudoConnect }, { "password", _password }, { "pseudoDownload", pseudoDownload } };
            }
            catch (Exception ex) { return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_SerialisationParams, String.Format(Properties.Resources.SERIALISATIONPARAMS, ex.Message)); }

            WSR_Result ret = await ConsumeWSR.Call(String.Concat(StringConnect, ADR_DOWNLOADDATAS), p, TypeSerializer.Json, cancel);

            // On peut avoir été déconnecté depuis... (Multi-Threading)
            if (!IsLogged)
            {
                return new RdmDalWSRResult(RdmDalWSRResult.CodeRet_Logout, Properties.Resources.PSEUDONONLOGUE);
            }
            else if (ret.ErrorCode == CodeRet_RDMService_Logout)
            {
                IsLogged = false;
            }

            return new RdmDalWSRResult(ret);
        }

        #endregion Méthodes
    }
}
