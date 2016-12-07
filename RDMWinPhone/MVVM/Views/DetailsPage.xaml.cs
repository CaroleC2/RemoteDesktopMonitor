using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d’élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkID=390556

namespace RDMWinPhone
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class DetailsPage : Page
    {

        private UserViewModel _userViewModel = null;

        public DetailsPage()
        {
            this.InitializeComponent();
        }


        #region Ouverture/fermeture de la fenêtre
        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // On récupère le ViewModel (UserViewModel). UserViewModel est la source de données
            _userViewModel = (UserViewModel)e.Parameter;

            // Binding de la source de données (UserViewModel) avec le contexte de la page
            DataContext = _userViewModel;

            // On fixe le titre de la page
            txtTitre.Text = txtTitre.Text + _userViewModel.PseudoDownload;

            // On s'abonne à l'événement système 'HardwareButtons_BackPressed'
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            // On met à jour l'image une première fois avant de lancer le timer
            prConnect.IsActive = true;
            prConnect.Visibility = Visibility.Visible;
            await _userViewModel.DownloadImage(CancellationToken.None);
            prConnect.IsActive = false;
            prConnect.Visibility = Visibility.Collapsed;

            // On lance le timer qui va interroger le WebService et mettre à jour l'image
            _userViewModel.StartTimerDownloadImage();


        }


        #endregion Ouverture/fermeture de la fenêtre

        #region Evenements utilisateur
       

        

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.Navigate(typeof(MainPage));
        }
        #endregion Evenements utilisateur

        private void mnuFermer_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void mnuSynchro_Click(object sender, RoutedEventArgs e)
        {
            // On rafraichi le Desktop de l'utilisateur
            await _userViewModel.DownloadImage(CancellationToken.None);
        }
    }
}
