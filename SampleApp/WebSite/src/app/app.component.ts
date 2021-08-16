import { Component } from '@angular/core';
import { EntityContextProvider, SecurityService, GuidUtils, SyncService, ClientAppProfile } from './bit';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  title = 'app';

  public products: SampleAppModel.ProductDto[] = [];

  public async test() {

    const original$dataPromiseHandlerBaseDefaultErrorCallback = $data["PromiseHandlerBase"]["defaultErrorCallback"];

    $data["PromiseHandlerBase"]["defaultErrorCallback"] = function overrided$dataPromiseHandlerBaseDefaultErrorCallback(exception) {
      debugger;
      try {
        const dataList = exception?.data;
        if (dataList != null && dataList[0] != null) {
          const data = dataList[0];
          const statusCode: number = data.response?.statusCode;
          const message = data.response?.body != null && data.response.statusText == "KnownError" ? JSON.parse(data.response.body).Message : "UnknownError";
          exception.statusCode = statusCode;
          exception.message = message;
          if (statusCode == 401) {
            // navigate to login form
          }
        }
      } catch (e) {
        console.error(e);
      }
      finally {
        return original$dataPromiseHandlerBaseDefaultErrorCallback.apply(this, arguments);
      }
    };

    await SecurityService.loginWithCredentials("Test", "Test", "SampleApp-ResOwner", "secret");

    const context = await EntityContextProvider.getContext<SampleAppContext>("SampleApp");

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
      await context.products.deactivateProductById(GuidUtils.newGuid());
    }
    catch (e) {
      debugger;
      // error messages
      console.log(e.message);
      console.log(e.statusCode);
    }

    // batch save
    const category = context.categories.add(new SampleAppModel.CategoryDto({ Id: GuidUtils.newGuid(), Name: "C1" }));
    const p1 = context.products.add(new SampleAppModel.ProductDto({ Id: GuidUtils.newGuid(), Name: "P1", CategoryId: category.Id, IsActive: true }));
    const p2 = context.products.add(new SampleAppModel.ProductDto({ Id: GuidUtils.newGuid(), Name: "P1", CategoryId: category.Id, IsActive: true }));
    await context.saveChanges();

    // batch read
    const result = await context.batchExecuteQuery([context.categories.filter(c => c.AllProductsAreActive == true), context.products]);

    // offline context:
    const offlineContext = await EntityContextProvider.getContext<SampleAppContext>("SampleApp", { isOffline: true });

    // sync
    SyncService.init(() => EntityContextProvider.getContext<SampleAppContext>("SampleApp"), () => EntityContextProvider.getContext<SampleAppContext>("SampleApp", { isOffline: true }));
    SyncService.addEntitySetConfig<SampleAppContext>({ name: "categories", dtoType: SampleAppModel.CategoryDto });

    await SyncService.syncContext();

    const categoriesFetchedFromOfflineBrowserDb = await offlineContext.categories.filter(c => c.ProductsCount == 0).toArray();

    this.products = await context.products.toArray();

    const someConfig = clientAppProfile.getConfig<boolean>("SomeConfig");
  }
}
