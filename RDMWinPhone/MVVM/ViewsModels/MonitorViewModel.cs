using RDMDALWSR;
using RDMWinPhone.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RDMWinPhone.MVVM.ViewsModels
{
    
        /// <summary>
        /// Classe métier de connexion qui intègre le DataBinding
        /// </summary>
        public class MonitorViewModel : ViewModelBase
        {
            private const int INTERVAL_GETUSERS = 1; // 1s

            private RdmDalWSR _rdmDAL;
            private string _errorMessage;
            private volatile bool _isLogged;
            private DispatcherTimer _timer;

            private ObservableCollection<UserViewModel> _colUsersViewModel;

            #region Constructeurs

            public MonitorViewModel()
            {
                _rdmDAL = new RdmDalWSR();
                _colUsersViewModel = new ObservableCollection<UserViewModel>();
                _timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, INTERVAL_GETUSERS) };
                _timer.Tick += dispatcherTimer_Tick;
            }

            public MonitorViewModel(string stringConnection, string pseudo) : this()
            {
                _rdmDAL.StringConnect = stringConnection;
                _rdmDAL.PseudoConnect = pseudo;
            }

            #endregion Constructeurs

            #region Propriétés

            public string StringConnection
            {
                get { return _rdmDAL.StringConnect; }
                set { _rdmDAL.StringConnect = value; }
            }

            public string PseudoConnect
            {
                get { return _rdmDAL.PseudoConnect; }
                set { _rdmDAL.PseudoConnect = value; }
            }

            #endregion Propriétés

            #region Propriétés Bindables

            public ReadOnlyObservableCollection<UserViewModel> Users
            {
                get { return new ReadOnlyObservableCollection<UserViewModel>(_colUsersViewModel); }
            }

            public string ErrorMessage
            {
                get { return _errorMessage; }
                private set
                {
                    if (_errorMessage != value)
                    {
                        _errorMessage = value;
                        RaisePropertyChanged();
                    }
                }
            }

            public bool IsLogged
            {
                get { return _isLogged; }

                private set
                {
                    if (_isLogged != value)
                    {
                        _isLogged = value;

                        if (_isLogged) // Login
                        {
                            _timer.Start();
                        }
                        else // Logout
                        {
                            _timer.Stop();
                            _colUsersViewModel.Clear();
                        }

                        RaisePropertyChanged();
                    }
                }
            }

            #endregion Propriétés Bindables

            #region Méthodes

            public async Task<bool> LoginAsync(CancellationToken cancel)
            {
                RdmDalWSRResult ret = await _rdmDAL.LoginAsync(cancel);

                IsLogged = _rdmDAL.IsLogged;
                ErrorMessage = ret.ErrorMessage;

                return ret.IsSuccess;
            }

            public async Task<bool> LogoutAsync(CancellationToken cancel)
            {
                _timer.Stop();

                RdmDalWSRResult ret = await _rdmDAL.LogoutAsync(cancel);

                IsLogged = _rdmDAL.IsLogged;
                ErrorMessage = ret.ErrorMessage;

                return ret.IsSuccess;
            }

            public async Task<bool> GetListPseudos(CancellationToken cancel)
            {
                RdmDalWSRResult ret = await _rdmDAL.GetPseudosAsync(cancel);

                if (_rdmDAL.IsLogged && ret.IsSuccess)
                {
                    MAJ_ListeUsers((List<String>)ret.Data);
                }

                IsLogged = _rdmDAL.IsLogged;
                ErrorMessage = ret.ErrorMessage;

                return ret.IsSuccess;
            }

            public async Task<bool> UploadPhoto(string photo, CancellationToken cancel)
            {
                RdmDalWSRResult ret = await _rdmDAL.UploadDataAsync(photo, cancel);

                IsLogged = _rdmDAL.IsLogged;
                ErrorMessage = ret.ErrorMessage;

                return ret.IsSuccess;
            }

            #endregion Méthodes

            #region ToString

            public override string ToString()
            {
                return StringConnection;
            }

            #endregion ToString

            #region Fonctions perso

            private async void dispatcherTimer_Tick(object sender, object e)
            {
                await GetListPseudos(CancellationToken.None);
            }

            private void MAJ_ListeUsers(List<String> lstPseudos)
            {
                // Suppression des users qui n'existent plus
                for (int i = _colUsersViewModel.Count() - 1; i >= 0; i--)
                {
                    if (!lstPseudos.Contains(_colUsersViewModel[i].PseudoDownload))
                    {
                        _colUsersViewModel.RemoveAt(i);
                    }
                }

                // Ajout des nouveaux users
                foreach (string pseudo in lstPseudos)
                {
                    UserViewModel userVM = new UserViewModel(pseudo, _rdmDAL);

                    if (!_colUsersViewModel.Contains(userVM))
                    {
                        // On utilise la méthode d'extention de la classe 'IListExtensions'
                        _colUsersViewModel.AddSorted(userVM);
                    }
                }
            }

            #endregion Fonctions perso



        }
    }
