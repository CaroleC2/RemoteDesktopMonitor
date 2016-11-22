using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ConsumeWebServiceRest
{
    [DataContract]
    public class WSR_Result
    {
        // PLAGE DES CODES ERREUR POUR L'APPEL D'UN WebService ---> [200 - 300[
        [DataMember]
        public const int CodeRet_AppelService = 200;
        [DataMember]
        public const int CodeRet_TimeOutAnnul = 201;
        [DataMember]
        public const int CodeRet_Serialize = 202;
        [DataMember]
        public const int CodeRet_Deserialize = 203;

        [DataMember]
        private string _data;
        [DataMember]
        private string _typeNameData;

        #region Constructeurs

        public WSR_Result()
        {
            IsSuccess = true;
        }

        public WSR_Result(object data, bool serialize,
                         [CallerMemberName] string sourceMemberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                if (data == null)
                {
                    _data = null;
                    _typeNameData = null;
                }
                else
                {
                    if (serialize)
                    {
                        _data = Serialize.SerializeToString(data);
                        _typeNameData = data.GetType().AssemblyQualifiedName;
                    }
                    else
                    {
                        _data = ((string[])data)[0];
                        _typeNameData = ((string[])data)[1];
                    }
                }

                IsSuccess = true;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                ErrorCode = CodeRet_Serialize;
                ErrorMessage = String.Format(Properties.Resources.ERREUR_SERIALISATIONRESULT, ex.Message);
                ErrorSourceMemberName = sourceMemberName;
                ErrorSourceFile = Path.GetFileNameWithoutExtension(sourceFilePath);
                ErrorSourceLineNumber = sourceLineNumber;
            }
        }

        public WSR_Result(int errorCode, string errorMessage,
                         [CallerMemberName] string sourceMemberName = "",
                         [CallerFilePath] string sourceFilePath = "",
                         [CallerLineNumber] int sourceLineNumber = 0)
        {
            IsSuccess = false;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ErrorSourceMemberName = sourceMemberName;
            ErrorSourceFile = Path.GetFileNameWithoutExtension(sourceFilePath);
            ErrorSourceLineNumber = sourceLineNumber;
        }

        #endregion Constructeurs

        #region Propriétés

        public object Data
        {
            get
            {
                // On désérialise la propriété Result dans son type d'origine
                return (_data == null) ? null : Serialize.DeserializeFromString(_data, Type.GetType(_typeNameData));
            }
        }

        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public string ErrorSourceMemberName { get; set; }

        [DataMember]
        public string ErrorSourceFile { get; set; }

        [DataMember]
        public int ErrorSourceLineNumber { get; set; }

        #endregion Propriétés
    }
}
