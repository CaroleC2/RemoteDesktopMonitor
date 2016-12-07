using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ParamsPage : Page
    {
        public ParamsPage()
        {
            this.InitializeComponent();
        }
#region Ouverture/fermeture de la fenêtre
        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // On s'abonne à l'événement système 'HardwareButtons_BackPressed'
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            txtAdrBase.Text = ParamsApp.AdresseBase;
            txtPseudo.Text = ParamsApp.Pseudo;

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // On se désabonne de l'événement système 'HardwareButtons_BackPressed'
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        #endregion Ouverture/fermeture de la fenêtre

        #region Evenements utilisateur

       

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            // Retour à la fenêtre appelante
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        
        private void mnuValider_Click(object sender, RoutedEventArgs e)
        {
            ParamsApp.AdresseBase = txtAdrBase.Text;
            ParamsApp.Pseudo = txtPseudo.Text;

            // Retour à la fenêtre appelante
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }

        }

        private void mnuAnnuler_Click(object sender, RoutedEventArgs e)
        {
            // Retour à la fenêtre appelante
            if (Frame.CanGoBack)
                Frame.GoBack();

        }

        #endregion Evenements utilisateur
    }
}
