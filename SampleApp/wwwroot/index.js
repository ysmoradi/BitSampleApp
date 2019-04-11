var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
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
var _this = this;
(function () { return __awaiter(_this, void 0, void 0, function () {
    var SecurityService, MessageReciever, GuidUtils, MetadataProvider, EntityContextProvider, SyncService, DateTimeService, ClientAppProfileManager, ClientAppProfile, onlineContext, categories, e_1, category, product1, product2, result, offlineContext, categories2, someConfigFromServer;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                SecurityService = new Bit.Implementations.DefaultSecurityService();
                MessageReciever = new Bit.Implementations.SignalRMessageReceiver();
                GuidUtils = new Bit.Implementations.DefaultGuidUtils();
                MetadataProvider = new Bit.Implementations.DefaultMetadataProvider();
                EntityContextProvider = new Bit.Implementations.EntityContextProviderBase(GuidUtils, MetadataProvider, SecurityService);
                SyncService = new Bit.Implementations.DefaultSyncService();
                DateTimeService = new Bit.Implementations.DefaultDateTimeService();
                ClientAppProfileManager = Bit.ClientAppProfileManager.getCurrent();
                ClientAppProfile = ClientAppProfileManager.getClientAppProfile();
                return [4 /*yield*/, SecurityService.loginWithCredentials("Test", "Test", "SampleApp-ResOwner", "secret")];
            case 1:
                _a.sent();
                return [4 /*yield*/, EntityContextProvider.getContext("SampleApp")];
            case 2:
                onlineContext = _a.sent();
                return [4 /*yield*/, onlineContext.categories.getEmptyCategories()
                        .withInlineCount()
                        .filter(function (c) { return c.Name.includes("C"); })
                        .orderBy(function (c) { return c.Id; })
                        .take(1)
                        .skip(1)
                        .toArray()];
            case 3:
                categories = _a.sent();
                onlineContext.categories.getEmptyCategories()
                    .filter(function (c, arg) { return c.Name == arg; }, { arg: new Date().getDay() /*Some variable for example...*/ })
                    .toArray();
                onlineContext.categories.getEmptyCategories()
                    .filter("(c, arg) => c.Name == arg", { arg: new Date().getDay() /*Some variable for example...*/ })
                    .toArray();
                _a.label = 4;
            case 4:
                _a.trys.push([4, 6, , 7]);
                return [4 /*yield*/, onlineContext.products.deactivateProductById(GuidUtils.newGuid())];
            case 5:
                _a.sent();
                return [3 /*break*/, 7];
            case 6:
                e_1 = _a.sent();
                // error messages
                console.log(e_1.message);
                return [3 /*break*/, 7];
            case 7:
                category = onlineContext.categories.add(new SampleAppModel.CategoryDto({ Id: GuidUtils.newGuid(), Name: "C1" }));
                product1 = onlineContext.products.add(new SampleAppModel.ProductDto({ Id: GuidUtils.newGuid(), Name: "P1", CategoryId: category.Id, IsActive: true }));
                product2 = onlineContext.products.add(new SampleAppModel.ProductDto({ Id: GuidUtils.newGuid(), Name: "P1", CategoryId: category.Id, IsActive: true }));
                return [4 /*yield*/, onlineContext.saveChanges()];
            case 8:
                _a.sent();
                return [4 /*yield*/, onlineContext.batchExecuteQuery([onlineContext.categories.filter(function (c) { return c.AllProductsAreActive == true; }), onlineContext.products])];
            case 9:
                result = _a.sent();
                return [4 /*yield*/, EntityContextProvider.getContext("SampleApp", { isOffline: true })];
            case 10:
                offlineContext = _a.sent();
                // sync
                SyncService.init(function () { return EntityContextProvider.getContext("SampleApp"); }, function () { return EntityContextProvider.getContext("SampleApp", { isOffline: true }); });
                SyncService.addEntitySetConfig({ name: "categories", dtoType: SampleAppModel.CategoryDto });
                return [4 /*yield*/, SyncService.syncContext()];
            case 11:
                _a.sent();
                return [4 /*yield*/, offlineContext.categories.filter(function (c) { return c.ProductsCount == 0; }).toArray()];
            case 12:
                categories2 = _a.sent();
                someConfigFromServer = ClientAppProfile.getConfig("SomeConfig");
                return [2 /*return*/];
        }
    });
}); })();
//# sourceMappingURL=index.js.map