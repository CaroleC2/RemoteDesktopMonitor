using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDMDALWSR;
using Windows.UI.Xaml;
using System.Threading;

namespace RDMWinPhone.MVVM.ViewsModels
{
   
        /// <summary>
        /// Classe métier qui représente un utilisateur et intègre le DataBinding
        /// </summary>
        public class UserViewModel : ViewModelBase, IComparable<UserViewModel>, IEquatable<UserViewModel>
        {
            private const int INTERVAL_DOWNLOADIMAGE = 1; // 1s

            private RdmDalWSR _rdmDAL;
            private string _pseudoDownload;
            private string _errorMessage;
            private volatile bool _isLogged;
            private DispatcherTimer _timer;

            private string _image;

            #region Constructeurs

            // Constructeur internal car c'est la classe MonitorViewModel qui construit les User
            internal UserViewModel(string pseudoDownload, RdmDalWSR rdmDAL)
            {
                _pseudoDownload = pseudoDownload;
                _rdmDAL = rdmDAL;
                _timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, INTERVAL_DOWNLOADIMAGE) };
                _timer.Tick += dispatcherTimer_Tick;
            }

            #endregion Constructeurs

            #region Propriétés

            public string PseudoDownload
            {
                get { return _pseudoDownload; }
            }

            #endregion Propriétés

            #region Propriétés Bindables

            public string Image
            {
                get { return _image; }
                private set
                {
                    if (_image != value)
                    {
                        _image = value;
                        RaisePropertyChanged();
                    }
                }
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

                        if (!_isLogged) // Logout
                        {
                            _timer.Stop();
                            Image = null;
                        }

                        RaisePropertyChanged();
                    }
                }
            }

            #endregion propriétés Bindables

            #region Méthodes

            public async Task<bool> DownloadImage(CancellationToken cancel)
            {
                RdmDalWSRResult ret = await _rdmDAL.DownloadDataAsync(PseudoDownload, cancel);

                if (ret.IsSuccess && IsLogged)
                {
                    Image = (string)ret.Data;
                }

                ErrorMessage = ret.ErrorMessage;
                IsLogged = _rdmDAL.IsLogged;

                return ret.IsSuccess;
            }

            public void StartTimerDownloadImage()
            {
                _timer.Start();
            }

            public void StopTimerDownloadImage()
            {
                _timer.Stop();
            }

            #endregion

            #region ToString

            public override string ToString()
            {
                return PseudoDownload;
            }

            #endregion ToString

            #region Fonctions perso

            private async void dispatcherTimer_Tick(object sender, object e)
            {
                await DownloadImage(CancellationToken.None);
            }

            #endregion Fonctions perso

            #region IComparable<UserVM> Membres

            public int CompareTo(UserViewModel other)
            {
                return PseudoDownload.CompareTo(other.PseudoDownload);
            }

            #endregion

            #region IEquatable<UserVM> Membres

            public bool Equals(UserViewModel other)
            {
                return PseudoDownload.Equals(other.PseudoDownload);
            }

            public override int GetHashCode()
            {
                return PseudoDownload.GetHashCode();
            }

            #endregion


        }
    }
