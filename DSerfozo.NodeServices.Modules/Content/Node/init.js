(function(e, a) { for(var i in a) e[i] = a[i]; }(exports, /******/ (function(modules) { // webpackBootstrap
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
/******/ 	return __webpack_require__(__webpack_require__.s = 0);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(1);


/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const readline = __webpack_require__(2);
var builtInModules;
var Module = __webpack_require__(3);
var originalLoader = Module._load;
Module._load = function (request, parent) {
    if (builtInModules[request]) {
        return originalLoader.apply(this, [builtInModules[request], parent]);
    }
    return originalLoader.apply(this, arguments);
};
const buildMap = o => Object.keys(o).reduce((m, k) => m.set(Number(k), o[k]), new Map());
const methodInvocations = new Map();
const savedCallbacks = new Map();
let lastMethodInvocationId = 1;
let lastCallbackId = 1;
let stream;
function createObjectBindings(bindings) {
    for (let entry of Array.from(bindings.entries())) {
        let key = entry[0];
        let value = entry[1];
        global[value.name] = {};
        for (let methodId of Object.keys(value.methods)) {
            let method = value.methods[methodId];
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
                const promise = new Promise((resolve, reject) => {
                    const executionId = lastMethodInvocationId++;
                    console.log("SETTING EXECID: " + executionId);
                    methodInvocations.set(executionId, { resolve: resolve, reject: reject });
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
function bindingMessageArrived(line) {
    console.log("RESULT: " + line);
    const result = JSON.parse(line);
    if (result.methodResult) {
        const methodResult = result.methodResult;
        const executionId = Number(methodResult.executionId);
        console.log("ID: " + methodResult.executionId);
        if (methodInvocations.has(executionId)) {
            const callbacks = methodInvocations.get(executionId);
            methodInvocations.delete(executionId);
            if (methodResult.success) {
                callbacks.resolve(methodResult.result);
            }
            else {
                callbacks.reject();
            }
        }
    }
    else if (result.callbackExecution) {
        if (savedCallbacks.has(result.callbackExecution.id)) {
            const callbackId = result.callbackExecution.id;
            try {
                var savedCallback = savedCallbacks.get(callbackId);
                console.log(savedCallback);
                var callbackResult = savedCallback.apply(null, result.callbackExecution.parameters);
                //promise
                if (callbackResult && typeof callbackResult.then === 'function') {
                    callbackResult.then(function (result) {
                        stream.write(JSON.stringify({
                            callbackResult: {
                                id: callbackId,
                                success: true,
                                result: result
                            }
                        }));
                        stream.write('\n');
                    }, function (error) {
                        stream.write(JSON.stringify({
                            callbackResult: {
                                id: callbackId,
                                success: false,
                                error: error
                            }
                        }));
                        stream.write('\n');
                    });
                }
                else {
                    stream.write(JSON.stringify({
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
                stream.write(JSON.stringify({
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
    }
    else if (result.deleteCallback) {
        savedCallbacks.delete(result.deleteCallback.id);
    }
}
function initializeBinding(callback, modules, bindings) {
    builtInModules = modules;
    createObjectBindings(buildMap(bindings));
    stream = callback.stream;
    readline.createInterface(stream, null).on('line', line => {
        bindingMessageArrived(line);
    });
}
exports.initializeBinding = initializeBinding;


/***/ }),
/* 2 */
/***/ (function(module, exports) {

module.exports = require("readline");

/***/ }),
/* 3 */
/***/ (function(module, exports) {

module.exports = require("module");

/***/ })
/******/ ])));