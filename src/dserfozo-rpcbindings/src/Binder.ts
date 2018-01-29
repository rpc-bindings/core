import { BindingConnection } from './BindingConnection';
import { CallbackExecution, DeleteCallback, MethodResult, CallbackResult } from './RpcRequest';
import { ObjectDescriptor, MethodDescriptor } from './ObjectDescriptor';
import { ObjectBinder} from './ObjectBinder';
import { Registry } from './Registry';
import { SavedCall } from './SavedCall';

export class Binder {
    private readonly callRegistry = new Registry<SavedCall>();
    private readonly callbackRegistry = new Registry<(...args: any[]) => any>();
    private readonly boundObjects = new Map<number, ObjectBinder>();

    constructor(private connection:BindingConnection, objectDescriptors: Map<number, ObjectDescriptor>) {
        connection.on(BindingConnection.CALLBACK_EXECUTION, this.onCallbackExecution.bind(this));
        connection.on(BindingConnection.DELETE_CALLBACK, this.onDeleteCallback.bind(this));
        connection.on(BindingConnection.METHOD_RESULT, this.onMethodResult.bind(this));

        objectDescriptors.forEach((function (elem) {
            this.boundObjects.set(elem.id, new ObjectBinder(elem, connection, this.callRegistry, this.callbackRegistry))
        }).bind(this));
    }

    bind(rootObject: any): void {
        this.boundObjects.forEach(e => e.bind(rootObject));
    }

    private onDeleteCallback(deleteCallback:DeleteCallback) : void {
        this.callbackRegistry.delete(deleteCallback.functionId);
    }

    private onCallbackExecution(execution: CallbackExecution) : void {
        const callback = this.callbackRegistry.get(execution.functionId);
        const connection = this.connection;
        debugger;
        if(callback) {
            try {
                const callResult = callback.apply(null, execution.parameters);
                if(callResult && typeof callResult.then === 'function') {
                    callResult.then(function(res) {
                        connection.sendCallbackResult(<any>{success: true, executionId: execution.executionId, result: res});
                    }, function(error){
                        connection.sendCallbackResult(<any>{success: false, executionId: execution.executionId, exception: error.toString()});
                    })
                } else {
                    connection.sendCallbackResult(<any>{success: true, executionId: execution.executionId, result: callResult});
                }
            } catch(e) {
                connection.sendCallbackResult(<any>{success: false, executionId: execution.executionId, exception: e.toString()});
            }
        }
    }

    private onMethodResult(result:MethodResult) : void {
        const execution = this.callRegistry.get(result.executionId);
        if(execution) {
            this.callRegistry.delete(result.executionId);

            if(result.success) {
                execution.resolve(result.result);
            } else {
                execution.reject(result.error);
            }
        }
    }
}