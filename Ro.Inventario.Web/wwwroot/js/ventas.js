/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ 826:
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
exports.Ventas = void 0;
var binderService_1 = __webpack_require__(575);
var api_1 = __webpack_require__(711);
var Ventas = /** @class */ (function () {
    function Ventas() {
        var _this = this;
        this.url = "ventas/buscarProducto";
        this.api = new api_1.Api();
        this.pattern = ko.observable("");
        this.productos = ko.observableArray([]);
        var self = this;
        this.pattern.subscribe(function (newValue) { return __awaiter(_this, void 0, void 0, function () {
            var apiUrl, prods;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        apiUrl = "".concat(self.url, "?pattern=").concat(newValue);
                        return [4 /*yield*/, self.api.get(apiUrl)];
                    case 1:
                        prods = _a.sent();
                        self.productos.removeAll();
                        prods.forEach(function (element) {
                            console.log(element);
                            self.productos.push(element);
                        });
                        console.log(prods);
                        return [2 /*return*/];
                }
            });
        }); });
    }
    Ventas.prototype.load = function () {
        return __awaiter(this, void 0, void 0, function () {
            var self;
            return __generator(this, function (_a) {
                self = this;
                return [2 /*return*/];
            });
        });
    };
    Ventas.prototype.bind = function () {
        var self = this;
        binderService_1.BinderService.bind(self, "#ventasPage");
    };
    return Ventas;
}());
exports.Ventas = Ventas;
document.addEventListener('DOMContentLoaded', function () {
    var page = new Ventas();
    page.bind();
    console.log("binding ko");
}, false);


/***/ }),

/***/ 711:
/***/ ((__unused_webpack_module, exports) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.Api = void 0;
/** Helper class to make http ajax requests */
var Api = /** @class */ (function () {
    function Api() {
        this.baseURL = "".concat(document.baseURI);
    }
    Api.prototype.getAntiforgeryToken = function () {
        var aftoken = $("input:hidden[name='__RequestVerificationToken']").val();
        return aftoken;
    };
    Api.prototype.get = function (url) {
        var self = this;
        return $.ajax({
            url: "".concat(self.baseURL).concat(url),
            type: "GET",
            dataType: "json",
            contentType: "application/json; charset=utf-8"
        });
    };
    Api.prototype.post = function (url, jsonData) {
        var self = this;
        return $.ajax({
            url: "".concat(self.baseURL).concat(url),
            type: "POST",
            data: JSON.stringify(jsonData),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            headers: {
                'RequestVerificationToken': self.getAntiforgeryToken()
            }
        });
    };
    Api.prototype.put = function (url, jsonData) {
        var self = this;
        return $.ajax({
            url: "".concat(self.baseURL).concat(url),
            type: "PUT",
            data: JSON.stringify(jsonData),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            headers: {
                'RequestVerificationToken': self.getAntiforgeryToken()
            }
        });
    };
    Api.prototype.patch = function (url, jsonData) {
        var self = this;
        return $.ajax({
            url: "".concat(self.baseURL).concat(url),
            type: "PATCH",
            data: JSON.stringify(jsonData),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            headers: {
                'RequestVerificationToken': self.getAntiforgeryToken()
            }
        });
    };
    Api.prototype.del = function (url) {
        var self = this;
        return $.ajax({
            url: "".concat(self.baseURL).concat(url),
            type: "DELETE",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            headers: {
                'RequestVerificationToken': self.getAntiforgeryToken()
            }
        });
    };
    return Api;
}());
exports.Api = Api;


/***/ }),

/***/ 575:
/***/ ((__unused_webpack_module, exports) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.BinderService = void 0;
/** Helper class that binds one Knockout models to a DOM object */
var BinderService = /** @class */ (function () {
    function BinderService() {
    }
    /**
     * Binds a Knockout model to a DOM object.
     * @param model Knockout model object.
     * @param selector Selector of the DOM to apply bindings.
     */
    BinderService.bind = function (model, selector) {
        var jqObj = $(selector);
        if (jqObj.length === 0) {
            return;
        }
        var domObj = jqObj[0];
        ko.cleanNode(domObj);
        ko.applyBindings(model, domObj);
    };
    BinderService.isBound = function (selector) {
        var jqObj = $(selector);
        return !!ko.dataFor(jqObj[0]);
    };
    return BinderService;
}());
exports.BinderService = BinderService;


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
/************************************************************************/
/******/ 	
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	// This entry module is referenced by other modules so it can't be inlined
/******/ 	var __webpack_exports__ = __webpack_require__(826);
/******/ 	
/******/ })()
;