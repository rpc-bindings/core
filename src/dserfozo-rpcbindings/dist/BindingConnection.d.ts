/// <reference types="@types/node" />
import { Duplex } from 'stream';
import { EventEmitter } from 'events';
import { CallbackResult, MethodExecution } from './RpcRequest';
export declare class BindingConnection extends EventEmitter {
    private stream;
    static readonly CALLBACK_EXECUTION: string;
    static readonly METHOD_RESULT: string;
    static readonly DELETE_CALLBACK: string;
    constructor(stream: Duplex);
    sendCallbackResult(result: CallbackResult): void;
    sendMethodExecution(execution: MethodExecution): void;
    private writeLine(line);
}
