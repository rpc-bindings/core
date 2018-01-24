import { Duplex } from 'stream';
import * as readline from 'readline';

var builtInModules;
var Module = require('module');
var originalLoader = Module._load;
Module._load = function (request, parent) {
    if (builtInModules[request]) {
        return originalLoader.apply(this, [builtInModules[request], parent]);
    }

    return originalLoader.apply(this, arguments);
};

const buildMap = o => Object.keys(o).reduce((m, k) => m.set(Number(k), o[k]), new Map());
const methodInvocations = new Map<number, any>();
const savedCallbacks = new Map<number, (...args:any[]) => any>();
let lastMethodInvocationId = 1;
let lastCallbackId = 1;
let stream: Duplex;

interface DeleteCallback {
    id: number;
}

interface CallbackExecution {
    id: number;
    parameters: any[];
}

interface MethodResult {
    executionId: number;
    success: boolean;
    result: any;
}

interface RpcRequest {
    callbackExecution: CallbackExecution;
    methodResult: MethodResult;
    deleteCallback: DeleteCallback;
}

interface MethodDescriptor {
    id: number;
    name: string;
}

interface ObjectDescriptor {
    id: number;
    name: string;
    methods: any;
}

function createObjectBindings(bindings: Map<number, ObjectDescriptor>) {
    for (let entry of Array.from(bindings.entries())) {
        let key = entry[0];
        let value: ObjectDescriptor = entry[1];

        global[value.name] = {};

        for (let methodId of Object.keys(value.methods)) {
            let method: MethodDescriptor = value.methods[methodId];
            let bound = {};
            bound['__id'] = method.id;
            bound['__objectId'] = value.id;
            global[value.name][method.name] = function () {
                const methodId = this['__id'];
                const objectId = this['__objectId'];
                const args = Array.prototype.slice.call(arguments);

                for (var i = 0; i < args.length; i++) {
                    if (typeof (args[i]) === 'function') {
                        console.log("FUNCTION");
                        savedCallbacks.set(lastCallbackId, args[i]);
                        args[i] = { id: lastCallbackId++ };
                    }
                }

                const promise = new Promise<any>((resolve, reject) => {
                    const executionId = lastMethodInvocationId++;

                    console.log("SETTING EXECID: " + executionId);
                    methodInvocations.set(executionId, {resolve: resolve, reject: reject});

                    stream.write(JSON.stringify({
                        methodExecution: {
                            executionId: executionId,
                            methodId: methodId,
                            objectId: objectId,
                            parameters: args
                        }
                    }));
                    stream.write('\n');
                    console.log('written');
                });

                return promise;
            }.bind(bound);
        }
    }
}

function bindingMessageArrived(line: string) {
    console.log("RESULT: " + line);

    const result = <RpcRequest>JSON.parse(line);

    if (result.methodResult) {
        const methodResult = result.methodResult;
        const executionId = Number(methodResult.executionId);
        console.log("ID: " + methodResult.executionId);
        if (methodInvocations.has(executionId)) {
            const callbacks = methodInvocations.get(executionId);
            methodInvocations.delete(executionId);
            if (methodResult.success) {
                callbacks.resolve(methodResult.result);
            } else {
                callbacks.reject();
            }
        }
    } else if (result.callbackExecution) {
        if (savedCallbacks.has(result.callbackExecution.id)) {
            const callbackId = result.callbackExecution.id;
            try {
                var savedCallback = savedCallbacks.get(callbackId);
                console.log(savedCallback);
                var callbackResult = savedCallback.apply(null, result.callbackExecution.parameters);
                //promise
                if (callbackResult && typeof callbackResult.then === 'function') {
                    callbackResult.then(function (result) {
                        stream.write(JSON.stringify(
                            {
                                callbackResult: {
                                    id: callbackId,
                                    success: true,
                                    result: result
                                }
                            }));
                        stream.write('\n');
                    }, function (error) {
                        stream.write(JSON.stringify(
                            {
                                callbackResult: {
                                    id: callbackId,
                                    success: false,
                                    error: error
                                }
                            }));
                        stream.write('\n');
                    });
                } else {
                    stream.write(JSON.stringify(
                        {
                            callbackResult: {
                                id: callbackId,
                                success: true,
                                result: callbackResult
                            }
                        }));
                    stream.write('\n');
                }
            }
            catch (e) {
                stream.write(JSON.stringify(
                    {
                        callbackResult: {
                            id: callbackId,
                            success: false,
                            error: ''
                        }
                    }));
                stream.write('\n');
            }
        }

        console.log('callback: ' + result.callbackExecution.id);
    } else if (result.deleteCallback) {
        savedCallbacks.delete(result.deleteCallback.id);
    }
}

export function initializeBinding(callback, modules, bindings) {
    builtInModules = modules;
    createObjectBindings(buildMap(bindings));

    stream = callback.stream;
    readline.createInterface(stream, null).on('line', line => {
        bindingMessageArrived(line);
    });
}