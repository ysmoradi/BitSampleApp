

		 var   SampleApp = SampleApp || {};
		 SampleApp.Dto = SampleApp.Dto || {};

SampleApp.Dto.ProductDto = $data.Entity.extend("SampleApp.Dto.ProductDto", {
	 
		Id: {
			"type": "Edm.Guid" , nullable: false
															, "key": true
			, "required" : true
			, "computed": true
										},
	 
		Name: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
	 
		IsActive: {
			"type": "Edm.Boolean" , nullable: false
																			},
	 
		CategoryId: {
			"type": "Edm.Guid" , nullable: false
																			},
	 
		CategoryName: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
		});


		 var   SampleApp = SampleApp || {};
		 SampleApp.Dto = SampleApp.Dto || {};

SampleApp.Dto.CategoryDto = $data.Entity.extend("SampleApp.Dto.CategoryDto", {
	 
		Id: {
			"type": "Edm.Guid" , nullable: false
															, "key": true
			, "required" : true
			, "computed": true
										},
	 
		Name: {
			"type": "Edm.String" , nullable: true
			  , defaultValue: null
																			},
	 
		Products: {
			"type": "Array" , nullable: true
			  , defaultValue: []
						 , "elementType": "SampleApp.Dto.ProductDto"
						 , "inverseProperty": "$$unbound"
													},
	 
		AllProductsAreActive: {
			"type": "Edm.Boolean" , nullable: false
																			},
	 
		ProductsCount: {
			"type": "Edm.Int32" , nullable: false
																			},
	 
		IsArchived: {
			"type": "Edm.Boolean" , nullable: false
																			},
	 
		Version: {
			"type": "Edm.Int64" , nullable: false
																			},
	 
		IsSynced: {
			"type": "Edm.Boolean" , nullable: false
																			},
		});



SampleAppContext = $data.EntityContext.extend("SampleAppContext", {
			categories : {
			"type": "$data.EntitySet",
			"elementType": "SampleApp.Dto.CategoryDto",
							"actions": {
													"getEmptyCategories": {
								"type": "$data.ServiceFunction",
								"namespace": "Default",
								"returnType":  "$data.Queryable" ,
								 "elementType": "SampleApp.Dto.CategoryDto", 
																	"params": [
																			]
						},
										}
					},
			products : {
			"type": "$data.EntitySet",
			"elementType": "SampleApp.Dto.ProductDto",
							"actions": {
													"deactivateProductById": {
								"type": "$data.ServiceAction",
								"namespace": "Default",
								"returnType":  null ,
																	"params": [
																					{
												"name": "id",
												"type": "Edm.Guid",
																							},									
																			]
						},
										}
					},
	});

	SampleAppModel = SampleApp.Dto;

