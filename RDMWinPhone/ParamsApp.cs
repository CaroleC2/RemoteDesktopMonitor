using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDMWinPhone
{
    static class ParamsApp
    {
        private const string KEY_ADRBASE = "AdresseBase";
        private const string KEY_PSEUDO = "Pseudo";

        internal const string P_ADR_DEFAULT = "http://";
        internal const string P_PSEUDO_DEFAULT = "pseudo";

        internal static string AdresseBase
        {
            get
            {
                string val = Windows.Storage.ApplicationData.Current.LocalSettings.Values[KEY_ADRBASE] as string;
                return string.IsNullOrEmpty(val) ? P_ADR_DEFAULT : val;
            }
            set { Windows.Storage.ApplicationData.Current.LocalSettings.Values[KEY_ADRBASE] = value; }
        }

        internal static string Pseudo
        {
            get
            {
                string val = Windows.Storage.ApplicationData.Current.LocalSettings.Values[KEY_PSEUDO] as string;
                return string.IsNullOrEmpty(val) ? P_PSEUDO_DEFAULT : val;
            }
            set { Windows.Storage.ApplicationData.Current.LocalSettings.Values[KEY_PSEUDO] = value; }
        }
    }
}

