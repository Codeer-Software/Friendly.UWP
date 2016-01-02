using Friendly.Core.Properties;
using System.Runtime.Serialization;

namespace Friendly.Core
{
    /// <summary>
    /// ローカライズ済みリソース。
    /// </summary>
    [DataContract]
    public class ResourcesLocal
    {
        static public ResourcesLocal Instance { get; set; }

        [DataMember]
        public string ErrorAppCommunication { get; set; }
        [DataMember]
        public string ErrorAppConnection { get; set; }
        [DataMember]
        public string ErrorArgumentInvokeFormat { get; set; }
        [DataMember]
        public string ErrorArgumentInvokeFormatForObjectArray { get; set; }
        [DataMember]
        public string ErrorBinaryInstall { get; set; }
        [DataMember]
        public string ErrorDllLoad { get; set; }
        [DataMember]
        public string ErrorExecuteThreadWindowHandle { get; set; }
        [DataMember]
        public string ErrorFriendlySystem { get; set; }
        [DataMember]
        public string ErrorInvalidThreadCall { get; set; }
        [DataMember]
        public string ErrorManyFoundConstractorFormat { get; set; }
        [DataMember]
        public string ErrorManyFoundInvokeFormat { get; set; }
        [DataMember]
        public string ErrorNotFoundConstractorFormat { get; set; }
        [DataMember]
        public string ErrorNotFoundConstractorFormatForObjectArray { get; set; }
        [DataMember]
        public string ErrorNotFoundInvokeFormat { get; set; }
        [DataMember]
        public string ErrorOperationTypeArgInfoFormat { get; set; }
        [DataMember]
        public string ErrorOperationTypeArgInfoForObjectArrayFormat { get; set; }
        [DataMember]
        public string ErrorProcessAcess { get; set; }
        [DataMember]
        public string ErrorProcessOperation { get; set; }
        [DataMember]
        public string ErrorTargetCpuDifference { get; set; }
        [DataMember]
        public string ErrorUnpredicatableClrVersion { get; set; }
        [DataMember]
        public string HasNotEnumerable { get; set; }
        [DataMember]
        public string NullObjectOperation { get; set; }
        [DataMember]
        public string OutOfCommunicationNo { get; set; }
        [DataMember]
        public string OutOfMemory { get; set; }
        [DataMember]
        public string UnknownTypeInfoFormat { get; set; }
        [DataMember]
        public string ErrorAttachOtherDomainsNeedNet4 { get; set; }

        /// <summary>
        /// 初期化。
        /// </summary>
        public static void Initialize()
        {
            Instance = new ResourcesLocal();
            Instance.InitializeCore();
        }
        
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        void InitializeCore()
        {
            ErrorAppCommunication = Resources.ErrorAppCommunication;
            ErrorAppConnection = Resources.ErrorAppConnection;
            ErrorArgumentInvokeFormat = Resources.ErrorArgumentInvokeFormat;
            ErrorArgumentInvokeFormatForObjectArray = Resources.ErrorArgumentInvokeFormatForObjectArray;
            ErrorBinaryInstall = Resources.ErrorBinaryInstall;
            ErrorDllLoad = Resources.ErrorDllLoad;
            ErrorExecuteThreadWindowHandle = Resources.ErrorExecuteThreadWindowHandle;
            ErrorFriendlySystem = Resources.ErrorFriendlySystem;
            ErrorInvalidThreadCall = Resources.ErrorInvalidThreadCall;
            ErrorManyFoundConstractorFormat = Resources.ErrorManyFoundConstractorFormat;
            ErrorManyFoundInvokeFormat = Resources.ErrorManyFoundInvokeFormat;
            ErrorNotFoundConstractorFormat = Resources.ErrorNotFoundConstractorFormat;
            ErrorNotFoundConstractorFormatForObjectArray = Resources.ErrorNotFoundConstractorFormatForObjectArray;
            ErrorNotFoundInvokeFormat = Resources.ErrorNotFoundInvokeFormat;
            ErrorOperationTypeArgInfoFormat = Resources.ErrorOperationTypeArgInfoFormat;
            ErrorOperationTypeArgInfoForObjectArrayFormat = Resources.ErrorOperationTypeArgInfoForObjectArrayFormat;
            ErrorProcessAcess = Resources.ErrorProcessAcess;
            ErrorProcessOperation = Resources.ErrorProcessOperation;
            ErrorTargetCpuDifference = Resources.ErrorTargetCpuDifference;
            ErrorUnpredicatableClrVersion = Resources.ErrorUnpredicatableClrVersion;
            HasNotEnumerable = Resources.HasNotEnumerable;
            NullObjectOperation = Resources.NullObjectOperation;
            OutOfCommunicationNo = Resources.OutOfCommunicationNo;
            OutOfMemory = Resources.OutOfMemory;
            UnknownTypeInfoFormat = Resources.UnknownTypeInfoFormat;
            ErrorAttachOtherDomainsNeedNet4 = Resources.ErrorAttachOtherDomainsNeedNet4;
        }
    }
}
