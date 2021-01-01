import { Component } from '@angular/core';
import jsSHA from "jssha";
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

    await SecurityService.loginWithCredentials("Test", "Test", "SampleApp-ResOwner", "secret");

    const context = await EntityContextProvider.getContext<SampleAppContext>("SampleApp");

    const id = "9cc7f05f-512f-4ad1-bba5-7c8cb02039d6";

    const alg: any = "SHA-256";

    const shaObj = new jsSHA(alg, "TEXT", { encoding: "UTF8" });

    shaObj.update(id.split("-")[0]);

    await context.products.hashSample(id, shaObj.getHash("HEX")); // the logic of your app is something else. This code is for testing purposes only.

    return;

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
      // error messages
      console.log(e.message);
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
