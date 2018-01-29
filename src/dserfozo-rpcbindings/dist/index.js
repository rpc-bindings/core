module.exports =
/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
/******/ 			});
/******/ 		}
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 5);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const readline = __webpack_require__(8);
const events_1 = __webpack_require__(9);
class BindingConnection extends events_1.EventEmitter {
    constructor(stream) {
        super();
        this.stream = stream;
        readline.createInterface(stream, null).on('line', line => {
            const rpcRequest = JSON.parse(line);
            if (rpcRequest.callbackExecution) {
                this.emit(BindingConnection.CALLBACK_EXECUTION, rpcRequest.callbackExecution);
            }
            else if (rpcRequest.methodResult) {
                this.emit(BindingConnection.METHOD_RESULT, rpcRequest.methodResult);
            }
            else if (rpcRequest.deleteCallback) {
                this.emit(BindingConnection.DELETE_CALLBACK, rpcRequest.deleteCallback);
            }
        });
    }
    sendCallbackResult(result) {
        this.writeLine(JSON.stringify({ callbackResult: result }));
    }
    sendMethodExecution(execution) {
        this.writeLine(JSON.stringify({ methodExecution: execution }));
    }
    writeLine(line) {
        this.stream.write(line);
        this.stream.write('\n');
    }
}
BindingConnection.CALLBACK_EXECUTION = 'callback-execution';
BindingConnection.METHOD_RESULT = 'method-result';
BindingConnection.DELETE_CALLBACK = 'delete-callback';
exports.BindingConnection = BindingConnection;


/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
class Registry {
    constructor() {
        this.savedCallbacks = new Map();
        this.lastId = 0;
    }
    save(callback) {
        const newId = ++this.lastId;
        this.savedCallbacks.set(newId, callback);
        return newId;
    }
    get(id) {
        if (this.savedCallbacks.has(id)) {
            return this.savedCallbacks.get(id);
        }
        return null;
    }
    delete(id) {
        if (this.savedCallbacks.has(id)) {
            this.savedCallbacks.delete(id);
        }
    }
}
exports.Registry = Registry;


/***/ }),
/* 2 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
class SavedCall {
    constructor(resolve, reject) {
        this.resolve = resolve;
        this.reject = reject;
    }
}
exports.SavedCall = SavedCall;


/***/ }),
/* 3 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const SavedCall_1 = __webpack_require__(2);
class FunctionBinder {
    constructor(objectId, descriptor, functionCallRegistry, callbackRegistry, connection) {
        this.objectId = objectId;
        this.descriptor = descriptor;
        this.functionCallRegistry = functionCallRegistry;
        this.callbackRegistry = callbackRegistry;
        this.connection = connection;
    }
    bind(obj) {
        obj[this.descriptor.name] = (function () {
            const args = Array.prototype.slice.call(arguments);
            for (var i = 0; i < args.length; i++) {
                if (typeof (args[i]) === 'function') {
                    const callbackId = this.callbackRegistry.save(args[i]);
                    args[i] = { functionId: callbackId };
                }
            }
            const registry = this.functionCallRegistry;
            const connection = this.connection;
            const methodId = this.descriptor.id;
            const objectId = this.objectId;
            const promise = new Promise((resolve, reject) => {
                const executionId = registry.save(new SavedCall_1.SavedCall(resolve, reject));
                connection.sendMethodExecution({
                    executionId: executionId,
                    methodId: methodId,
                    objectId: objectId,
                    parameters: args
                });
            });
            return promise;
        }).bind(this);
    }
}
exports.FunctionBinder = FunctionBinder;


/***/ }),
/* 4 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const FunctionBinder_1 = __webpack_require__(3);
class ObjectBinder {
    constructor(objectDescriptor, connection, functionCallRegistry, callbackRegistry) {
        this.objectDescriptor = objectDescriptor;
        this.functions = new Map();
        this.objectDescriptor.methods.forEach(function (f) {
            this.functions.set(f.id, new FunctionBinder_1.FunctionBinder(objectDescriptor.id, f, functionCallRegistry, callbackRegistry, connection));
        }.bind(this));
    }
    bind(rootObject) {
        rootObject[this.objectDescriptor.name] = {};
        this.functions.forEach(e => e.bind(rootObject[this.objectDescriptor.name]));
    }
}
exports.ObjectBinder = ObjectBinder;


/***/ }),
/* 5 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

function __export(m) {
    for (var p in m) if (!exports.hasOwnProperty(p)) exports[p] = m[p];
}
Object.defineProperty(exports, "__esModule", { value: true });
var ModuleLoader_1 = __webpack_require__(6);
exports.overrideModuleLoad = ModuleLoader_1.overrideModuleLoad;
var BindingConnection_1 = __webpack_require__(0);
exports.BindingConnection = BindingConnection_1.BindingConnection;
var Registry_1 = __webpack_require__(1);
exports.Registry = Registry_1.Registry;
var SavedCall_1 = __webpack_require__(2);
exports.SavedCall = SavedCall_1.SavedCall;
var FunctionBinder_1 = __webpack_require__(3);
exports.FunctionBinder = FunctionBinder_1.FunctionBinder;
var ObjectBinder_1 = __webpack_require__(4);
exports.ObjectBinder = ObjectBinder_1.ObjectBinder;
var Binder_1 = __webpack_require__(10);
exports.Binder = Binder_1.Binder;
__export(__webpack_require__(11));


/***/ }),
/* 6 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const Module = __webpack_require__(7);
function overrideModuleLoad(builtInModules) {
    const originalLoader = Module._load;
    Module._load = function (request, parent) {
        if (builtInModules.has(request)) {
            return originalLoader.apply(this, [builtInModules.get(request), parent]);
        }
        return originalLoader.apply(this, arguments);
    };
}
exports.overrideModuleLoad = overrideModuleLoad;


/***/ }),
/* 7 */
/***/ (function(module, exports) {

module.exports = require("module");

/***/ }),
/* 8 */
/***/ (function(module, exports) {

module.exports = require("readline");

/***/ }),
/* 9 */
/***/ (function(module, exports) {

module.exports = require("events");

/***/ }),
/* 10 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const BindingConnection_1 = __webpack_require__(0);
const ObjectBinder_1 = __webpack_require__(4);
const Registry_1 = __webpack_require__(1);
class Binder {
    constructor(connection, objectDescriptors) {
        this.connection = connection;
        this.callRegistry = new Registry_1.Registry();
        this.callbackRegistry = new Registry_1.Registry();
        this.boundObjects = new Map();
        connection.on(BindingConnection_1.BindingConnection.CALLBACK_EXECUTION, this.onCallbackExecution.bind(this));
        connection.on(BindingConnection_1.BindingConnection.DELETE_CALLBACK, this.onDeleteCallback.bind(this));
        connection.on(BindingConnection_1.BindingConnection.METHOD_RESULT, this.onMethodResult.bind(this));
        objectDescriptors.forEach((function (elem) {
            this.boundObjects.set(elem.id, new ObjectBinder_1.ObjectBinder(elem, connection, this.callRegistry, this.callbackRegistry));
        }).bind(this));
    }
    bind(rootObject) {
        this.boundObjects.forEach(e => e.bind(rootObject));
    }
    onDeleteCallback(deleteCallback) {
        this.callbackRegistry.delete(deleteCallback.functionId);
    }
    onCallbackExecution(execution) {
        const callback = this.callbackRegistry.get(execution.functionId);
        const connection = this.connection;
        debugger;
        if (callback) {
            try {
                const callResult = callback.apply(null, execution.parameters);
                if (callResult && typeof callResult.then === 'function') {
                    callResult.then(function (res) {
                        connection.sendCallbackResult({ success: true, executionId: execution.executionId, result: res });
                    }, function (error) {
                        connection.sendCallbackResult({ success: false, executionId: execution.executionId, exception: error.toString() });
                    });
                }
                else {
                    connection.sendCallbackResult({ success: true, executionId: execution.executionId, result: callResult });
                }
            }
            catch (e) {
                connection.sendCallbackResult({ success: false, executionId: execution.executionId, exception: e.toString() });
            }
        }
    }
    onMethodResult(result) {
        const execution = this.callRegistry.get(result.executionId);
        if (execution) {
            this.callRegistry.delete(result.executionId);
            if (result.success) {
                execution.resolve(result.result);
            }
            else {
                execution.reject(result.error);
            }
        }
    }
}
exports.Binder = Binder;


/***/ }),
/* 11 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
function buildMapStringKey(obj) {
    return Object.keys(obj).reduce((m, k) => m.set(k, obj[k]), new Map());
}
exports.buildMapStringKey = buildMapStringKey;
function buildMapIntKeyRecursive(obj, ...recursivePaths) {
    const current = Object.keys(obj).reduce((m, k) => m.set(parseInt(k), obj[k]), new Map());
    recursivePaths.forEach(p => {
        current.forEach((v) => {
            if (v[p]) {
                v[p] = buildMapIntKeyRecursive(v[p], ...recursivePaths);
            }
        });
    });
    return current;
}
exports.buildMapIntKeyRecursive = buildMapIntKeyRecursive;


/***/ })
/******/ ]);