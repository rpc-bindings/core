import { Duplex } from 'stream';
import * as readline from 'readline';
import { EventEmitter } from 'events';

import { RpcRequest, CallbackResult, MethodExecution } from './RpcRequest';

export class BindingConnection extends EventEmitter {
    public static readonly CALLBACK_EXECUTION = 'callback-execution';
    public static readonly METHOD_RESULT = 'method-result';
    public static readonly DELETE_CALLBACK = 'delete-callback';

    constructor(private stream: Duplex) {
        super();

        readline.createInterface(stream, null).on('line', line => {
            const rpcRequest = <RpcRequest>JSON.parse(line);

            if (rpcRequest.callbackExecution) {
                this.emit(BindingConnection.CALLBACK_EXECUTION, rpcRequest.callbackExecution);
            } else if (rpcRequest.methodResult) {
                this.emit(BindingConnection.METHOD_RESULT, rpcRequest.methodResult);
            } else if (rpcRequest.deleteCallback) {
                this.emit(BindingConnection.DELETE_CALLBACK, rpcRequest.deleteCallback);
            }
        });    
    }

    sendCallbackResult(result:CallbackResult): void {
        this.writeLine(JSON.stringify({ callbackResult: result }));
    }

    sendMethodExecution(execution:MethodExecution): void {
        this.writeLine(JSON.stringify({ methodExecution: execution }));
    }

    private writeLine(line: string): void {
        this.stream.write(line);
        this.stream.write('\n');
    }
}