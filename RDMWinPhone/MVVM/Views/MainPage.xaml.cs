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
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using RDMWinPhone.Extensions;

// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=391641

namespace RDMWinPhone
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static MonitorViewModel _monitorViewModel = new MonitorViewModel();

        public MainPage()
        {
            this.InitializeComponent();
            
        }

        #region Evenements utilisateur

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Binding de la source de données (MonitorViewModel) avec le contexte de la page
            DataContext = _monitorViewModel;

            // On s'abonne à l'événement système 'HardwareButtons_BackPressed'          
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            if (_monitorViewModel.IsLogged)
            {
                // On rafraichit la liste des pseudos
                await _monitorViewModel.GetListPseudos(CancellationToken.None);
            }
            else
            {
                // Si les paramètres ne sont pas renseignés, on avertit l'utilisateur
                if (ParamsApp.AdresseBase == ParamsApp.P_ADR_DEFAULT || ParamsApp.Pseudo == ParamsApp.P_PSEUDO_DEFAULT)
                {
                    await "MSG_PARAM".ReadResMsg().MsgInformation();

                    // On lance la fenêtre de paramètrage
                    Frame.Navigate(typeof(ParamsPage));
                }
            }
        }

        protected override void OnNavigatingFrom 
            (NavigatingCancelEventArgs e)
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }

        private async void btConnect_Click(object sender, RoutedEventArgs e)
        {
            btConnect.Visibility = Visibility.Collapsed;
            prConnect.IsActive = true;
            prConnect.Visibility = Visibility.Visible;
            _monitorViewModel.StringConnection = ParamsApp.AdresseBase;
            _monitorViewModel.PseudoConnect = ParamsApp.Pseudo;
            await _monitorViewModel.LoginAsync(CancellationToken.None);
            prConnect.IsActive = false;
            prConnect.Visibility = Visibility.Collapsed;
            btConnect.Visibility = Visibility.Visible;
        }

        private async void btDisconnect_Click(object sender, RoutedEventArgs e)
        {
            // On se déconnecte
            btDisconnect.Visibility = Visibility.Collapsed;
            prDisconnect.IsActive = true;
            prDisconnect.Visibility = Visibility.Visible;
            await _monitorViewModel.LogoutAsync(CancellationToken.None);
            prDisconnect.IsActive = false;
            prDisconnect.Visibility = Visibility.Collapsed;
            btDisconnect.Visibility = Visibility.Visible;
        }

        private void btPhoto_Click(object sender, RoutedEventArgs e)
        {
            btPhoto.Visibility = Visibility.Collapsed;
            prPhoto.IsActive = true;
            prPhoto.Visibility = Visibility.Visible;
            CapturePhoto();
            prPhoto.IsActive = false;
            prPhoto.Visibility = Visibility.Collapsed;
            btPhoto.Visibility = Visibility.Visible;
        }
    
        

    private void ListView_Click(object sender, RoutedEventArgs e)
    {
        UserViewModel userViewModel = (UserViewModel)((Button)sender).DataContext;
        Frame.Navigate(typeof(DetailsPage), userViewModel);
    }

    private void mnuParams_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(ParamsPage));
    }

    private async void mnuSynchro_Click(object sender, RoutedEventArgs e)
    {
        // On rafraichi la liste des pseudos
        await _monitorViewModel.GetListPseudos(CancellationToken.None);
    }

    private async void mnuQuitter_Click(object sender, RoutedEventArgs e)
    {
        StringExtensions.MsgResult result = await "MSG_CLOSEAPP".ReadResMsg().MsgChoix("TITRE_CLOSEAPP".ReadResMsg(), StringExtensions.MsgButton.YesNo, StringExtensions.MsgDefaultButton.Button1);

        if (result == StringExtensions.MsgResult.Yes)
        {
            // On se déconnecte
            await _monitorViewModel.LogoutAsync(CancellationToken.None);

            // Fermeture de l'application
            App.Current.Exit();
        }
    }

    private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
    {
        // On interdit la sortie de l'application par ce bouton
        e.Handled = true;
    }

#endregion Evenements utilisateur

        #region Fonctions perso
        private void CapturePhoto()
    {
        // Création de la vue 'FileOpenPicker'
        FileOpenPicker filePicker = new FileOpenPicker();
        filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        filePicker.ViewMode = PickerViewMode.Thumbnail;

        // Ajout un filtre pour ne voir que les images
        filePicker.FileTypeFilter.Clear();
        filePicker.FileTypeFilter.Add(".bmp");
        filePicker.FileTypeFilter.Add(".png");
        filePicker.FileTypeFilter.Add(".jpeg");
        filePicker.FileTypeFilter.Add(".jpg");

        // Abonnement à l'événement 'Activated' qui sera déclenché au retour de la vue 'FileOpenPicker'
        CoreApplication.GetCurrentView().Activated += viewActivated;

        // On affiche la vue 'FileOpenPicker'
        filePicker.PickSingleFileAndContinue();
    }

    private async void viewActivated(CoreApplicationView sender, IActivatedEventArgs activatedArgs)
    {
        // On se désabonne de l'événement 'Activated'
        CoreApplication.GetCurrentView().Activated -= viewActivated;

        // Permet de tester si on vient de 'FileOpenPicker'
        FileOpenPickerContinuationEventArgs paramsFOP = activatedArgs as FileOpenPickerContinuationEventArgs;

        // Permet de tester si on vient de 'FileOpenPicker' et qu'un fichier a été choisi
        if (paramsFOP != null && paramsFOP.Files.Count > 0)
        {
            // Convertion du fichier image en string
            string photo = await FileImageToString(paramsFOP.Files[0], true);

            // Appel du service web pour mettre la photo à jour
            await _monitorViewModel.UploadPhoto(photo, CancellationToken.None);
        }
    }

    private async Task<string> FileImageToString(StorageFile fileImage, bool tansformThumbnail)
    {
        if (fileImage != null)
        {
            try
            {
                if (tansformThumbnail)
                {
                    // Création d'une vignette pour réduire la taille
                    StorageItemThumbnail vignette = await fileImage.GetThumbnailAsync(ThumbnailMode.PicturesView);

                    using (Stream imageStream = vignette.AsStreamForRead())
                    {
                        // Convertion du flux en tableau de bytes
                        byte[] imageBytes = new byte[(int)imageStream.Length];
                        imageStream.Read(imageBytes, 0, (int)imageStream.Length);

                        // Convertion du tableau de bytes en string
                        return Convert.ToBase64String(imageBytes);
                    }
                }
                else
                {
                    using (IRandomAccessStream imageStream = await fileImage.OpenReadAsync())
                    {
                        imageStream.Seek(0);
                        using (Stream ms = imageStream.AsStream())
                        {
                            // Convertion du flux en tableau de bytes
                            byte[] imageBytes = new byte[(int)ms.Length];
                            ms.Read(imageBytes, 0, (int)ms.Length);

                            // Convertion du tableau de bytes en string
                            return Convert.ToBase64String(imageBytes);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
        else
        {
            return String.Empty;
        }
    }

        #endregion Fonctions perso
    }
}
