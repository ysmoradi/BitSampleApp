/// <reference path="node_modules/@bit/bitframework/typings.all.d.ts" />

declare module SampleApp.Dto {
	
	class ProductDto extends $data.Entity {
				    
			Id : string;
			static Id : any;
				    
			Name : string;
			static Name : any;
				    
			IsActive : boolean;
			static IsActive : any;
				    
			CategoryId : string;
			static CategoryId : any;
				    
			CategoryName : string;
			static CategoryName : any;
			}
}


declare module SampleApp.Dto {
	
	class CategoryDto extends $data.Entity {
				    
			Id : string;
			static Id : any;
				    
			Name : string;
			static Name : any;
				    
			Products : Array<SampleApp.Dto.ProductDto>;
			static Products : any;
				    
			AllProductsAreActive : boolean;
			static AllProductsAreActive : any;
				    
			ProductsCount : number;
			static ProductsCount : any;
				    
			IsArchived : boolean;
			static IsArchived : any;
				    
			Version : string;
			static Version : any;
				    
			IsSynced : boolean;
			static IsSynced : any;
			}
}




    
	interface CategoriesEntitySet extends $data.EntitySet<SampleApp.Dto.CategoryDto>{
				    /**
    It returns categories without product.
    */
		    getEmptyCategories():  $data.Queryable<SampleApp.Dto.CategoryDto> ;
			}
    
	interface ProductsEntitySet extends $data.EntitySet<SampleApp.Dto.ProductDto>{
				    
		    deactivateProductById(id : string):  Promise<void> ;
				    
		    hashSample(Id : string,Hash : string):  Promise<void> ;
			}

declare class SampleAppContext extends $data.EntityContext {

		    
		categories: CategoriesEntitySet;
		    
		products: ProductsEntitySet;
	
}

	import SampleAppModel = SampleApp.Dto;

