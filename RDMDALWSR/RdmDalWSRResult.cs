using System;
using System.IO;
using System.Runtime.CompilerServices;
using ConsumeWebServiceRest;

namespace RDMDALWSR
{
    public class RdmDalWSRResult
    {
        // PLAGE DES CODES ERREUR POUR L'APPEL DU WebService RDMService ---> [300 - 400[
        public const int CodeRet_Login = 300;
        public const int CodeRet_PseudoObligatoire = 301;
        public const int CodeRet_Logout = 302;
        public const int CodeRet_PasswordObligatoire = 303;
        public const int CodeRet_PseudoDownloadObligatoire = 304;
        public const int CodeRet_SerialisationParams = 305;

        #region Constructeurs

        internal RdmDalWSRResult(WSR_Result codeRet)
        {
            IsSuccess = codeRet.IsSuccess;
            ErrorMessage = codeRet.ErrorMessage;
            ErrorSourceFile = codeRet.ErrorSourceFile;
            ErrorSourceLineNumber = codeRet.ErrorSourceLineNumber;
            ErrorSourceMemberName = codeRet.ErrorSourceMemberName;
            ErrorCode = codeRet.ErrorCode;
            Data = codeRet.Data;
        }

        internal RdmDalWSRResult(int errorCode, string errorMessage,
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

        public bool IsSuccess { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorSourceMemberName { get; set; }

        public string ErrorSourceFile { get; set; }

        public int ErrorSourceLineNumber { get; set; }

        public Object Data { get; set; }

        #endregion Propriétés
    }
}
