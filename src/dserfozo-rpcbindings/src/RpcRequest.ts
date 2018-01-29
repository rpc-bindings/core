export interface CallbackExecution {
    executionId: number;
    functionId: number;
    parameters: any[];
}

export interface MethodResult {
    executionId: number;
    success: boolean;
    result: any;
    error: string;
}

export interface DeleteCallback {
    functionId: number;
}

export interface MethodExecution {
    executionId: number;
    methodId: number;
    objectId: number;
    parameters: any[];
}

export interface CallbackResult {
    executionId: number;
    success: boolean;
    result: any;
    exception: string;
}

export interface RpcRequest {
    callbackExecution: CallbackExecution;
    methodResult: MethodResult;
    deleteCallback: DeleteCallback;
}

export interface RpcResponse {
    methodExecution: MethodExecution;
    callbackResult:CallbackResult;
}