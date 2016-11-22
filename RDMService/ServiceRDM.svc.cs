using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ConsumeWebServiceRest;

namespace RDMService
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" dans le code, le fichier svc et le fichier de configuration.
    // REMARQUE : pour lancer le client test WCF afin de tester ce service, sélectionnez Service1.svc ou Service1.svc.cs dans l'Explorateur de solutions et démarrez le débogage.
    public class ServiceRDM : IServiceRDM
    {
        public WSR_Result DownloadData(WSR_Params p)
        {
            throw new NotImplementedException();
        }

        public WSR_Result GetPseudos(WSR_Params p)
        {
            throw new NotImplementedException();
        }

        public WSR_Result Login(WSR_Params p)
        {
            throw new NotImplementedException();
        }

        public WSR_Result Logout(WSR_Params p)
        {
            throw new NotImplementedException();
        }

        public WSR_Result UploadData(WSR_Params p)
        {
            throw new NotImplementedException();
        }
    }
}
