﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cogiscan_Utilities.CogiscanWebServices {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:RPCServices", ConfigurationName="CogiscanWebServices.RPCServices")]
    public interface RPCServices {

        [System.ServiceModel.OperationContractAttribute(Action = "", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style = System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults = true, Use = System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "result")]
        Cogiscan_Utilities.CogiscanWebServices.executeCommandResponse executeCommand(Cogiscan_Utilities.CogiscanWebServices.executeCommandRequest request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface RPCServicesChannel : Cogiscan_Utilities.CogiscanWebServices.RPCServices, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "executeCommand", WrapperNamespace = "urn:RPCServices", IsWrapped = true)]
    public partial class executeCommandRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "", Order = 0)]
        public string String_1;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "", Order = 1)]
        public string String_2;

        public executeCommandRequest()
        {
        }

        public executeCommandRequest(string String_1, string String_2)
        {
            this.String_1 = String_1;
            this.String_2 = String_2;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "executeCommandResponse", WrapperNamespace = "urn:RPCServices", IsWrapped = true)]
    public partial class executeCommandResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "", Order = 0)]
        public string result;

        public executeCommandResponse()
        {
        }

        public executeCommandResponse(string result)
        {
            this.result = result;
        }
    }

    public partial class RPCServicesClient : System.ServiceModel.ClientBase<Cogiscan_Utilities.CogiscanWebServices.RPCServices>, Cogiscan_Utilities.CogiscanWebServices.RPCServices {
        
        public RPCServicesClient() {
        }
        
        public RPCServicesClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public RPCServicesClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RPCServicesClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RPCServicesClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        

        public Cogiscan_Utilities.CogiscanWebServices.executeCommandResponse executeCommand(Cogiscan_Utilities.CogiscanWebServices.executeCommandRequest request)
        {
            return base.Channel.executeCommand(request);
        }
    }

}