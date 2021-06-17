using Bit.Data.Contracts;
using Bit.Http.Contracts;
using Bit.Test;
using FakeItEasy;
using IdentityModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleApp.Api;
using SampleApp.Dto;
using SampleApp.Model;
using SampleApp.Test;
using Simple.OData.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp.Test.Api
{
    [TestClass]
    public class ProductsControllerTests
    {
        [TestMethod]
        public virtual async Task DeactivateProductByIdMustRaiseNotFoundErrorWhenThereIsNoEquivalentProductInDatabase()
        {
            using (BitOwinTestEnvironment testEnvironment = new BitOwinTestEnvironment(new TestEnvironmentArgs
            {
                AdditionalDependencies = (depManager, services) =>
                {
                    IRepository<Product> productsRepository = A.Fake<IRepository<Product>>();

                    A.CallTo(() => productsRepository.GetByIdAsync(A<CancellationToken>.Ignored, A<Guid>.Ignored))
                        .ThrowsAsync(new InvalidOperationException("Source sequence doesn't contain any elements."));

                    depManager.RegisterInstance(productsRepository);
                }
            }))
            {
                Token token = await testEnvironment.Server.LoginWithCredentials("Test", "Test", "SampleApp-ResOwner");

                IODataClient client = testEnvironment.Server.BuildODataClient(odataRouteName: "SampleApp", token: token);

                try
                {
                    await client.Controller<ProductsController, ProductDto>()
                        .Action(nameof(ProductsController.DeactivateProductById))
                        .Set(new ProductsController.DeactivateProductByIdArgs { id = Guid.NewGuid() })
                        .ExecuteAsync();
                }
                catch (WebRequestException ex) when (ex.Response.Contains("ProductNotFound")) { }
            }
        }

        [TestMethod]
        public virtual async Task DeactivateProductByIdMustRaiseProductIsDeactivatedAlreadyWhenLocatedProductIsNotActive()
        {
            using (BitOwinTestEnvironment testEnvironment = new BitOwinTestEnvironment(new TestEnvironmentArgs
            {
                AdditionalDependencies = (depManager, services) =>
                {
                    IRepository<Product> productsRepository = A.Fake<IRepository<Product>>();

                    A.CallTo(() => productsRepository.GetByIdAsync(A<CancellationToken>.Ignored, A<Guid>.Ignored))
                        .Returns(new Product { IsActive = false });

                    depManager.RegisterInstance(productsRepository);
                }
            }))
            {
                Token token = await testEnvironment.Server.LoginWithCredentials("Test", "Test", "SampleApp-ResOwner");

                IODataClient client = testEnvironment.Server.BuildODataClient(odataRouteName: "SampleApp", token: token);

                try
                {
                    await client.Controller<ProductsController, ProductDto>()
                        .Action(nameof(ProductsController.DeactivateProductById))
                        .Set(new ProductsController.DeactivateProductByIdArgs { id = Guid.NewGuid() })
                        .ExecuteAsync();
                }
                catch (WebRequestException ex) when (ex.Response.Contains("ProductIsDeactiveAlready")) { }
            }
        }
    }
}
