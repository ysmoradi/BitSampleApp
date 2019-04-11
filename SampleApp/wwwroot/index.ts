(async () => {

    const SecurityService = new Bit.Implementations.DefaultSecurityService();
    const MessageReciever = new Bit.Implementations.SignalRMessageReceiver();
    const GuidUtils = new Bit.Implementations.DefaultGuidUtils();
    const MetadataProvider = new Bit.Implementations.DefaultMetadataProvider();
    const EntityContextProvider = new Bit.Implementations.EntityContextProviderBase(GuidUtils, MetadataProvider, SecurityService);
    const SyncService = new Bit.Implementations.DefaultSyncService();
    const DateTimeService = new Bit.Implementations.DefaultDateTimeService();
    const ClientAppProfileManager = Bit.ClientAppProfileManager.getCurrent();
    const ClientAppProfile = ClientAppProfileManager.getClientAppProfile();

    await SecurityService.loginWithCredentials("Test", "Test", "SampleApp-ResOwner", "secret");

    const onlineContext = await EntityContextProvider.getContext<SampleAppContext>("SampleApp");

    // function
    const categories = await onlineContext.categories.getEmptyCategories()
        .withInlineCount()
        .filter(c => c.Name.includes("C"))
        .orderBy(c => c.Id)
        .take(1)
        .skip(1)
        .toArray();

    onlineContext.categories.getEmptyCategories()
        .filter((c, arg) => c.Name == arg, { arg: new Date().getDay() /*Some variable for example...*/ })
        .toArray();

    onlineContext.categories.getEmptyCategories()
        .filter(`(c, arg) => c.Name == arg`, { arg: new Date().getDay() /*Some variable for example...*/ })
        .toArray();

    // action
    try {
        await onlineContext.products.deactivateProductById(GuidUtils.newGuid());
    }
    catch (e) {
        // error messages
        console.log(e.message);
    }

    // batch save
    const category = onlineContext.categories.add(new SampleAppModel.CategoryDto({ Id: GuidUtils.newGuid(), Name: "C1" }));
    const product1 = onlineContext.products.add(new SampleAppModel.ProductDto({ Id: GuidUtils.newGuid(), Name: "P1", CategoryId: category.Id, IsActive: true }));
    const product2 = onlineContext.products.add(new SampleAppModel.ProductDto({ Id: GuidUtils.newGuid(), Name: "P1", CategoryId: category.Id, IsActive: true }));
    await onlineContext.saveChanges();

    // batch read
    const result = await onlineContext.batchExecuteQuery([onlineContext.categories.filter(c => c.AllProductsAreActive == true), onlineContext.products]);

    // offline context:
    const offlineContext = await EntityContextProvider.getContext<SampleAppContext>("SampleApp", { isOffline: true });

    // sync
    SyncService.init(() => EntityContextProvider.getContext<SampleAppContext>("SampleApp"), () => EntityContextProvider.getContext<SampleAppContext>("SampleApp", { isOffline: true }));
    SyncService.addEntitySetConfig<SampleAppContext>({ name: "categories", dtoType: SampleAppModel.CategoryDto });

    await SyncService.syncContext();

    const categories2 = await offlineContext.categories.filter(c => c.ProductsCount == 0).toArray();

    const someConfigFromServer = ClientAppProfile.getConfig<boolean>("SomeConfig");

})();
