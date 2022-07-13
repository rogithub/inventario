/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ 244:
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.Devolucion = void 0;
var binderService_1 = __webpack_require__(575);
var api_1 = __webpack_require__(711);
var toCurrency_1 = __webpack_require__(613);
var ProductLine = /** @class */ (function () {
    function ProductLine(l) {
        var _this = this;
        this.producto = l.producto;
        this.cantidadEnBuenasCondiciones = ko.observable(0);
        this.cantidadEnMalasCondiciones = ko.observable(0);
        this.ajusteProductoId = l.ajusteProductoId;
        this.cantidad = l.cantidad;
        this.precioUnitario = l.precioUnitario;
        this.categoria = l.categoria;
        this.unidadMedida = l.unidadMedida;
        var self = this;
        this.devolucionRowOp = ko.computed(function () {
            var a = _this.cantidadEnBuenasCondiciones();
            var b = _this.cantidadEnMalasCondiciones();
            var c = _this.precioUnitario;
            //return ((a+b) * c);
            return (parseFloat(a.toString()) + parseFloat(b.toString())) * c;
        }, self);
        this.hasError = ko.computed(function () {
            var a = _this.cantidadEnBuenasCondiciones();
            var b = _this.cantidadEnMalasCondiciones();
            if (isNaN(a) || isNaN(b))
                return true;
            var c = _this.cantidad;
            return (parseFloat(a.toString()) + parseFloat(b.toString())) > c;
        }, self);
        this.aMoneda = toCurrency_1.default;
    }
    return ProductLine;
}());
var Devolucion = /** @class */ (function () {
    function Devolucion() {
        this.url = "ventas/GetVentaData";
        this.api = new api_1.Api();
        this.lines = ko.observableArray();
        this.venta = ko.observable();
        var self = this;
        this.isValid = ko.computed(function () {
            var count = 0;
            for (var _i = 0, _a = self.lines(); _i < _a.length; _i++) {
                var it = _a[_i];
                if (it.hasError())
                    return false;
                count += it.cantidadEnBuenasCondiciones();
                count += it.cantidadEnMalasCondiciones();
            }
            return count > 0;
        }, self);
    }
    Devolucion.prototype.load = function () {
        return __awaiter(this, void 0, void 0, function () {
            var self, ventaId, data, _i, _a, it;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        self = this;
                        ventaId = $("#hidVentaId").val();
                        return [4 /*yield*/, self.api.get("".concat(self.url, "?ventaId=").concat(ventaId))];
                    case 1:
                        data = _b.sent();
                        self.venta(data.venta);
                        for (_i = 0, _a = data.devueltos; _i < _a.length; _i++) {
                            it = _a[_i];
                            self.lines.push(new ProductLine(it));
                        }
                        return [2 /*return*/];
                }
            });
        });
    };
    Devolucion.prototype.bind = function () {
        var self = this;
        binderService_1.BinderService.bind(self, "#devolucionesPage");
    };
    Devolucion.prototype.guardar = function () {
        return __awaiter(this, void 0, void 0, function () {
            var self, data, _i, _a, l, item;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        self = this;
                        data = [];
                        for (_i = 0, _a = self.lines(); _i < _a.length; _i++) {
                            l = _a[_i];
                            item = {
                                ajusteProductoId: l.ajusteProductoId,
                                cantidadEnBuenasCondiciones: l.cantidadEnBuenasCondiciones(),
                                cantidadEnMalasCondiciones: l.cantidadEnMalasCondiciones(),
                            };
                            data.push(item);
                        }
                        return [4 /*yield*/, self.api.post("Ventas/DevolverProductos", data)];
                    case 1:
                        _b.sent();
                        alert("¡Guardado!");
                        window.location.href = "".concat(document.baseURI, "Ventas");
                        return [2 /*return*/];
                }
            });
        });
    };
    return Devolucion;
}());
exports.Devolucion = Devolucion;
document.addEventListener('DOMContentLoaded', function () {
    return __awaiter(this, void 0, void 0, function () {
        var page;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    page = new Devolucion();
                    page.bind();
                    console.log("binding ko");
                    return [4 /*yield*/, page.load()];
                case 1:
                    _a.sent();
                    return [2 /*return*/];
            }
        });
    });
}, false);


/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = __webpack_modules__;
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/chunk loaded */
/******/ 	(() => {
/******/ 		var deferred = [];
/******/ 		__webpack_require__.O = (result, chunkIds, fn, priority) => {
/******/ 			if(chunkIds) {
/******/ 				priority = priority || 0;
/******/ 				for(var i = deferred.length; i > 0 && deferred[i - 1][2] > priority; i--) deferred[i] = deferred[i - 1];
/******/ 				deferred[i] = [chunkIds, fn, priority];
/******/ 				return;
/******/ 			}
/******/ 			var notFulfilled = Infinity;
/******/ 			for (var i = 0; i < deferred.length; i++) {
/******/ 				var [chunkIds, fn, priority] = deferred[i];
/******/ 				var fulfilled = true;
/******/ 				for (var j = 0; j < chunkIds.length; j++) {
/******/ 					if ((priority & 1 === 0 || notFulfilled >= priority) && Object.keys(__webpack_require__.O).every((key) => (__webpack_require__.O[key](chunkIds[j])))) {
/******/ 						chunkIds.splice(j--, 1);
/******/ 					} else {
/******/ 						fulfilled = false;
/******/ 						if(priority < notFulfilled) notFulfilled = priority;
/******/ 					}
/******/ 				}
/******/ 				if(fulfilled) {
/******/ 					deferred.splice(i--, 1)
/******/ 					var r = fn();
/******/ 					if (r !== undefined) result = r;
/******/ 				}
/******/ 			}
/******/ 			return result;
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => (Object.prototype.hasOwnProperty.call(obj, prop))
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/jsonp chunk loading */
/******/ 	(() => {
/******/ 		// no baseURI
/******/ 		
/******/ 		// object to store loaded and loading chunks
/******/ 		// undefined = chunk not loaded, null = chunk preloaded/prefetched
/******/ 		// [resolve, reject, Promise] = chunk loading, 0 = chunk loaded
/******/ 		var installedChunks = {
/******/ 			625: 0
/******/ 		};
/******/ 		
/******/ 		// no chunk on demand loading
/******/ 		
/******/ 		// no prefetching
/******/ 		
/******/ 		// no preloaded
/******/ 		
/******/ 		// no HMR
/******/ 		
/******/ 		// no HMR manifest
/******/ 		
/******/ 		__webpack_require__.O.j = (chunkId) => (installedChunks[chunkId] === 0);
/******/ 		
/******/ 		// install a JSONP callback for chunk loading
/******/ 		var webpackJsonpCallback = (parentChunkLoadingFunction, data) => {
/******/ 			var [chunkIds, moreModules, runtime] = data;
/******/ 			// add "moreModules" to the modules object,
/******/ 			// then flag all "chunkIds" as loaded and fire callback
/******/ 			var moduleId, chunkId, i = 0;
/******/ 			if(chunkIds.some((id) => (installedChunks[id] !== 0))) {
/******/ 				for(moduleId in moreModules) {
/******/ 					if(__webpack_require__.o(moreModules, moduleId)) {
/******/ 						__webpack_require__.m[moduleId] = moreModules[moduleId];
/******/ 					}
/******/ 				}
/******/ 				if(runtime) var result = runtime(__webpack_require__);
/******/ 			}
/******/ 			if(parentChunkLoadingFunction) parentChunkLoadingFunction(data);
/******/ 			for(;i < chunkIds.length; i++) {
/******/ 				chunkId = chunkIds[i];
/******/ 				if(__webpack_require__.o(installedChunks, chunkId) && installedChunks[chunkId]) {
/******/ 					installedChunks[chunkId][0]();
/******/ 				}
/******/ 				installedChunks[chunkId] = 0;
/******/ 			}
/******/ 			return __webpack_require__.O(result);
/******/ 		}
/******/ 		
/******/ 		var chunkLoadingGlobal = self["webpackChunkro_inventario_web"] = self["webpackChunkro_inventario_web"] || [];
/******/ 		chunkLoadingGlobal.forEach(webpackJsonpCallback.bind(null, 0));
/******/ 		chunkLoadingGlobal.push = webpackJsonpCallback.bind(null, chunkLoadingGlobal.push.bind(chunkLoadingGlobal));
/******/ 	})();
/******/ 	
/************************************************************************/
/******/ 	
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	// This entry module depends on other loaded chunks and execution need to be delayed
/******/ 	var __webpack_exports__ = __webpack_require__.O(undefined, [712], () => (__webpack_require__(244)))
/******/ 	__webpack_exports__ = __webpack_require__.O(__webpack_exports__);
/******/ 	
/******/ })()
;