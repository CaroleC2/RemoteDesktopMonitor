using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

namespace RDMWinPhone.Extensions
{
    public static class StringExtensions
    {
        #region MessageDialog

        public enum MsgButton { OK, OKCancel, RetryCancel, YesNo, YesNoCancel }
        public enum MsgDefaultButton { Button1, Button2, Button3 }
        public enum MsgResult { OK, Cancel, Retry, Yes, No };

        private const int BT_OK = 1;
        private const int BT_CANCEL = 2;
        private const int BT_RETRY = 3;
        private const int BT_YES = 4;
        private const int BT_NO = 5;

        /// <summary>
        /// Permet d'afficher un message dans une boite de dialogue
        /// </summary>
        /// <param name="msg">Message à afficher</param>
        /// <param name="title">Titre du message</param>
        /// <param name="msgButton">Boutons à afficher</param>
        /// <param name="msgDefaultButton">Bouton par défaut</param>
        /// <returns>Bouton cliqué par l'utilisateur ou Cancel si le message est annulé</returns>
        public async static Task<MsgResult> MsgChoix(this string msg, string title, MsgButton msgButton, MsgDefaultButton msgDefaultButton)
        {
            MessageDialog msgDialog = null;
            MsgResult reponse = MsgResult.OK;

            // Création du message avec ou sans titre
            if (String.IsNullOrEmpty(title))
            {
                msgDialog = new MessageDialog(msg);
            }
            else
            {
                msgDialog = new MessageDialog(msg, title);
            }

            // Création des boutons qui seront affichés dans le message
            switch (msgButton)
            {
                case MsgButton.OK:
                    msgDialog.Commands.Add(new UICommand("BT_OK".ReadResMsg()) { Id = BT_OK });
                    break;
                case MsgButton.OKCancel:
                    msgDialog.Commands.Add(new UICommand("BT_OK".ReadResMsg()) { Id = BT_OK });
                    msgDialog.Commands.Add(new UICommand("BT_CANCEL".ReadResMsg()) { Id = BT_CANCEL });
                    break;
                case MsgButton.RetryCancel:
                    msgDialog.Commands.Add(new UICommand("BT_RETRY".ReadResMsg()) { Id = BT_RETRY });
                    msgDialog.Commands.Add(new UICommand("BT_CANCEL".ReadResMsg()) { Id = BT_CANCEL });
                    break;
                case MsgButton.YesNo:
                    msgDialog.Commands.Add(new UICommand("BT_YES".ReadResMsg()) { Id = BT_YES });
                    msgDialog.Commands.Add(new UICommand("BT_NO".ReadResMsg()) { Id = BT_NO });
                    break;
                case MsgButton.YesNoCancel:
                    msgDialog.Commands.Add(new UICommand("BT_YES".ReadResMsg()) { Id = BT_YES });
                    msgDialog.Commands.Add(new UICommand("BT_NO".ReadResMsg()) { Id = BT_NO });
                    msgDialog.Commands.Add(new UICommand("BT_CANCEL".ReadResMsg()) { Id = BT_CANCEL });
                    break;
                default:
                    msgDialog.Commands.Add(new UICommand("BT_OK".ReadResMsg()) { Id = BT_OK });
                    break;
            }

            // Définition du bouton par défaut
            switch (msgDefaultButton)
            {
                case MsgDefaultButton.Button1:
                    msgDialog.DefaultCommandIndex = 0;
                    break;
                case MsgDefaultButton.Button2:
                    msgDialog.DefaultCommandIndex = 1;
                    break;
                case MsgDefaultButton.Button3:
                    msgDialog.DefaultCommandIndex = 2;
                    break;
                default:
                    msgDialog.DefaultCommandIndex = 0;
                    break;
            }

            // Affichage du message
            IUICommand result = await msgDialog.ShowAsync();

            if (result != null)
            {
                // Récupération du bouton 'cliqué' par l'utilisateur
                switch ((int)result.Id)
                {
                    case BT_OK:
                        reponse = MsgResult.OK;
                        break;
                    case BT_CANCEL:
                        reponse = MsgResult.Cancel;
                        break;
                    case BT_RETRY:
                        reponse = MsgResult.Retry;
                        break;
                    case BT_YES:
                        reponse = MsgResult.Yes;
                        break;
                    case BT_NO:
                        reponse = MsgResult.No;
                        break;
                    default:
                        reponse = MsgResult.OK;
                        break;
                }
            }
            else // Le message a été annulé (ex : Fermeture de la fenêtre, BackPressed, ...)
            {
                reponse = MsgResult.Cancel;
            }

            // Retour du bouton 'cliqué par l'utilisateur
            return reponse;
        }

        /// <summary>
        /// Permet d'afficher un message dans une boite de dialogue sans titre
        /// </summary>
        /// <param name="msg">Message à afficher</param>
        /// <param name="msgButton">Boutons à afficher</param>
        /// <param name="msgDefaultButton">Bouton par défaut</param>
        /// <returns>Bouton cliqué par l'utilisateur ou Cancel si le message est annulé</returns>
        public async static Task<MsgResult> MsgChoix(this string msg, MsgButton msgButton, MsgDefaultButton msgDefaultButton)
        {
            return await MsgChoix(msg, null, msgButton, msgDefaultButton);
        }

        /// <summary>
        /// Permet d'afficher un message dans une boite de dialogue sans titre avec le bouton 'OK' (message d'information)
        /// </summary>
        /// <param name="msg">Message à afficher</param>
        /// <returns>Bouton cliqué par l'utilisateur ou Cancel si le message est annulé</returns>
        public async static Task<MsgResult> MsgInformation(this string msg)
        {
            return await MsgChoix(msg, null, MsgButton.OK, MsgDefaultButton.Button1);
        }

        /// <summary>
        /// Permet d'afficher un message dans une boite de dialogue avec titre et avec le bouton 'OK' (message d'information)
        /// </summary>
        /// <param name="msg">Message à afficher</param>
        /// <returns>Bouton cliqué par l'utilisateur ou Cancel si le message est annulé</returns>
        public async static Task<MsgResult> MsgInformation(this string msg, string title)
        {
            return await MsgChoix(msg, title, MsgButton.OK, MsgDefaultButton.Button1);
        }

        #endregion MessageDialog

        #region Ressources

        /// <summary>
        /// Permet de récupérer une resource de type chaîne dans le fichier de ressources 'Messages.resw'
        /// </summary>
        /// <param name="resourceName">Nom de la resource à récupérer</param>
        /// <returns>Valeur de la ressource</returns>
        public static string ReadResMsg(this string resourceName)
        {
            string ressource = ResourceLoader.GetForCurrentView("Messages").GetString(resourceName);
            return String.IsNullOrEmpty(ressource) ? ResourceLoader.GetForCurrentView("Messages").GetString("MSG_RESNOTFOUND") : ressource;
        }

        #endregion Ressources
    }
}
