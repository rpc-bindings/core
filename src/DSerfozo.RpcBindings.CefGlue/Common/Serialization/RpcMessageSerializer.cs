using System.Collections.Generic;
using DSerfozo.RpcBindings.CefGlue.Renderer.Serialization;
using DSerfozo.RpcBindings.Contract.Communication.Model;
using DSerfozo.RpcBindings.Execution.Model;
using Xilium.CefGlue;

namespace DSerfozo.RpcBindings.CefGlue.Common.Serialization
{
    public static class RpcMessageSerializer
    {
        public static CefProcessMessage ToCefProcessMessage(this RpcResponse<CefValue> rpcResponse)
        {
            CefProcessMessage message = null;

            var dynamicObjectRequest = rpcResponse.DynamicObjectRequest;
            var methodExecution = rpcResponse.MethodExecution;
            var callbackResult = rpcResponse.CallbackResult;

            if (dynamicObjectRequest != null)
            {
                message = CefProcessMessage.Create(Messages.DynamicObjectRequestMessageName);
                var arguments = message.Arguments;
                arguments.SetInt64(0, dynamicObjectRequest.ExecutionId);
                arguments.SetString(1, dynamicObjectRequest.Name);
            }
            else if (methodExecution != null)
            {
                message = CefProcessMessage.Create(Messages.MethodExecutionMessageName);
                var arguments = message.Arguments;
                arguments.SetInt64(0, methodExecution.ExecutionId);
                arguments.SetInt64(1, methodExecution.MethodId);
                arguments.SetInt64(2, methodExecution.ObjectId);
                using (var list = CefListValue.Create())
                {
                    for (var i = 0; i < methodExecution.Parameters.Length; i++)
                    {
                        list.SetValue(i, methodExecution.Parameters[i]);
                    }

                    arguments.SetList(3, list);
                }
            }
            else if (callbackResult != null)
            {
                message = CefProcessMessage.Create(Messages.CallbackResultMessageName);
                var arguments = message.Arguments;

                arguments.SetInt64(0, callbackResult.ExecutionId);
                arguments.SetBool(1, callbackResult.Success);
                if (!callbackResult.Success)
                {
                    arguments.SetString(2, callbackResult.Error);
                }
                else if (callbackResult.Result != null)
                {
                    arguments.SetValue(3, callbackResult.Result);
                }
            }

            return message;
        }

        public static CefProcessMessage ToCefProcessMessage(this RpcRequest<CefValue> rpcRequest)
        {
            var methodResult = rpcRequest.MethodResult;
            var dynamicObjectResponse = rpcRequest.DynamicObjectResult;
            var callbackExecution = rpcRequest.CallbackExecution;
            var deleteCallback = rpcRequest.DeleteCallback;
            CefProcessMessage message = null;
            if (methodResult != null)
            {
                message = CefProcessMessage.Create(Messages.MethodResultMessageName);
                var arguments = message.Arguments;
                arguments.SetInt64(0, methodResult.ExecutionId);
                arguments.SetBool(1, methodResult.Success);
                arguments.SetString(2, methodResult.Error);
                if (methodResult.Result != null)
                {
                    arguments.SetValue(3, methodResult.Result);
                }
                else
                {
                    using (var nullValue = CefValue.Create())
                    {
                        nullValue.SetNull();
                        arguments.SetValue(3, nullValue);
                    }
                }
            }
            else if (dynamicObjectResponse != null)
            {
                message = CefProcessMessage.Create(Messages.DynamicObjectResultMessageName);
                var arguments = message.Arguments;
                arguments.SetInt64(0, dynamicObjectResponse.ExecutionId);
                arguments.SetBool(1, dynamicObjectResponse.Success);
                arguments.SetString(2, dynamicObjectResponse.Exception);

                if (dynamicObjectResponse.Success)
                {
                    using (var descriptor = CefDictionaryValue.Create())
                    {
                        dynamicObjectResponse.ObjectDescriptor.ToCefList(descriptor);
                        arguments.SetDictionary(3, descriptor);
                    }
                }
            }
            else if (callbackExecution != null)
            {
                message = CefProcessMessage.Create(Messages.CallbackExecutionMessageName);
                var arguments = message.Arguments;
                arguments.SetInt64(0, callbackExecution.ExecutionId);
                arguments.SetInt64(1, callbackExecution.FunctionId);
                using (var argList = CefListValue.Create())
                {
                    for (var i = 0; i < callbackExecution.Parameters.Length; i++)
                    {
                        argList.SetValue(i, callbackExecution.Parameters[i]);
                    }

                    arguments.SetList(2, argList);
                }
            }
            else if (deleteCallback != null)
            {
                message = CefProcessMessage.Create(Messages.DeleteCallbackMessageName);
                var arguments = message.Arguments;
                arguments.SetInt64(0, deleteCallback.FunctionId);
            }

            return message;
        }

        public static RpcRequest<CefValue> CreateRpcRequest(CefProcessMessage mesage, V8Serializer v8Serializer)
        {
            RpcRequest<CefValue> result = null;

            var args = mesage.Arguments;
            switch (mesage.Name)
            {
                case Messages.DynamicObjectResultMessageName:
                    var dynamicResult = new DynamicObjectResponse
                    {
                        ExecutionId = args.GetInt64(0),
                        Success = args.GetBool(1),
                    };

                    if (dynamicResult.Success)
                    {
                        using (var descriptor = args.GetDictionary(3))
                        {
                            dynamicResult.ObjectDescriptor =
                                ObjectDescriptorSerializer.ReadObjectDescriptor(descriptor, v8Serializer);
                        }
                    }
                    else
                    {
                        dynamicResult.Exception = args.GetString(2);
                    }

                    result = new RpcRequest<CefValue>
                    {
                        DynamicObjectResult = dynamicResult
                    };
                    break;
                case Messages.MethodResultMessageName:
                    var methodResult = new MethodResult<CefValue>
                    {
                        ExecutionId = args.GetInt64(0),
                        Success = args.GetBool(1),
                        Error = args.GetString(2),
                        Result = args.GetValue(3)
                    };

                    result = new RpcRequest<CefValue>
                    {
                        MethodResult = methodResult
                    };
                    break;
                case Messages.CallbackExecutionMessageName:
                    using (var list = args.GetList(2))
                    {
                        var paramList = new CefValue[list.Count];
                        for (var i = 0; i < list.Count; i++)
                        {
                            paramList[i] = list.GetValue(i);
                        }

                        var callbackExecution = new CallbackExecution<CefValue>
                        {
                            ExecutionId = args.GetInt64(0),
                            FunctionId = args.GetInt64(1),
                            Parameters = paramList
                        };

                        result = new RpcRequest<CefValue>
                        {
                            CallbackExecution = callbackExecution
                        };
                    }
                    break;
                case Messages.DeleteCallbackMessageName:
                    var functionId = args.GetInt64(0);

                    result = new RpcRequest<CefValue>
                    {
                        DeleteCallback = new DeleteCallback
                        {
                            FunctionId = functionId
                        }
                    };
                    break;
                    
            }

            return result;
        }

        public static RpcResponse<CefValue> CreateRpcResponse(CefProcessMessage message)
        {
            RpcResponse<CefValue> result = null;

            var args = message.Arguments;
            switch (message.Name)
            {
                case Messages.MethodExecutionMessageName:
                    var execution = new MethodExecution<CefValue>
                    {
                        ExecutionId = args.GetInt64(0),
                        MethodId = args.GetInt64(1),
                        ObjectId = args.GetInt64(2)
                    };
                    var parameters = args.GetList(3);
                    var paramValues = new List<CefValue>();
                    for (var i = 0; i < parameters.Count; i++)
                    {
                        var p = parameters.GetValue(i);
                        paramValues.Add(p.Copy());
                    }
                    execution.Parameters = paramValues.ToArray();

                    result = new RpcResponse<CefValue>
                    {
                        MethodExecution = execution
                    };
                    break;
                case Messages.DynamicObjectRequestMessageName:
                    var req = new DynamicObjectRequest();
                    req.ExecutionId = args.GetInt64(0);
                    req.Name = args.GetString(1);

                    result = new RpcResponse<CefValue>
                    {
                        DynamicObjectRequest = req
                    };
                    break;
                case Messages.CallbackResultMessageName:
                    var resp = new CallbackResult<CefValue>();

                    resp.ExecutionId = args.GetInt64(0);
                    resp.Success = args.GetBool(1);
                    if (!resp.Success)
                    {
                        resp.Error = args.GetString(2);
                    }
                    var cefValue = args.GetValue(3);
                    if (cefValue?.IsValid == true)
                    {
                        resp.Result = cefValue.Copy();
                    }

                    result = new RpcResponse<CefValue>
                    {
                        CallbackResult = resp
                    };
                    break;
            }

            return result;
        }
    }
}
