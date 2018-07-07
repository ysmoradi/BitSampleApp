(async () => {

    const securityService = new Bit.Implementations.DefaultSecurityService();
    const guidUtils = new Bit.Implementations.DefaultGuidUtils();
    const metadataProvider = new Bit.Implementations.DefaultMetadataProvider();
    const contextProvider = new Bit.Implementations.EntityContextProviderBase(guidUtils, metadataProvider, securityService);
    const syncService = new Bit.Implementations.DefaultSyncService();

    await securityService.loginWithCredentials("Test", "Test", "SampleApp-ResOwner", "secret");

    const context = await contextProvider.getContext<SampleAppContext>("SampleApp");

    // function
    const t1 = await context.categories.getEmptyCategories()
        .withInlineCount()
        .filter(c => c.Name.includes("C"))
        .orderBy(c => c.Id)
        .take(1)
        .skip(1)
        .toArray();

    context.categories.getEmptyCategories()
        .filter((c, arg) => c.Name == arg, { arg: new Date().getDay() /*Some variable for example...*/ })
        .toArray();

    context.categories.getEmptyCategories()
        .filter(`(c, arg) => c.Name == arg`, { arg: new Date().getDay() /*Some variable for example...*/ })
        .toArray();

    // action
    try {
        await context.products.deactivateProductById(guidUtils.newGuid());
    }
    catch (e) {
        // error messages
        console.log(e.message);
    }

    // batch save
    const category = context.categories.add(new SampleAppModel.CategoryDto({ Id: guidUtils.newGuid(), Name: "C1" }));
    const p1 = context.products.add(new SampleAppModel.ProductDto({ Id: guidUtils.newGuid(), Name: "P1", CategoryId: category.Id, IsActive: true }));
    const p2 = context.products.add(new SampleAppModel.ProductDto({ Id: guidUtils.newGuid(), Name: "P1", CategoryId: category.Id, IsActive: true }));
    await context.saveChanges();

    // batch read
    const result = await context.batchExecuteQuery([context.categories.filter(c => c.AllProductsAreActive == true), context.products]);

    // offline context:
    const offlineContext = await contextProvider.getContext<SampleAppContext>("SampleApp", { isOffline: true });

    // sync
    syncService.init(() => contextProvider.getContext<SampleAppContext>("SampleApp"), () => contextProvider.getContext<SampleAppContext>("SampleApp", { isOffline: true }));
    syncService.addEntitySetConfig<SampleAppContext>({ name: "categories", dtoType: SampleAppModel.CategoryDto });

    await syncService.syncContext();

    const aaa = await offlineContext.categories.filter(c => c.ProductsCount == 0).toArray();

})();