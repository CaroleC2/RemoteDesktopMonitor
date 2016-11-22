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
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    
    public interface IServiceRDM
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "Login", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        WSR_Result Login(WSR_Params p);

        [OperationContract]
        [WebInvoke(UriTemplate = "Logout", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        WSR_Result Logout(WSR_Params p);

        [OperationContract]
        [WebInvoke(UriTemplate = "GetPseudos", Method = "POST", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare)]
        WSR_Result GetPseudos(WSR_Params p);

        [OperationContract]
        [WebInvoke(UriTemplate = "UploadData", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        WSR_Result UploadData(WSR_Params p);

        [OperationContract]
        [WebInvoke(UriTemplate = "DownloadData", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        WSR_Result DownloadData(WSR_Params p);
    }
}