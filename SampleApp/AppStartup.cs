using Bit.Core;
using Bit.Core.Contracts;
using Bit.Data.EntityFrameworkCore.Implementations;
using Bit.IdentityServer.Contracts;
using Bit.IdentityServer.Implementations;
using Bit.Model.Implementations;
using Bit.OData.ActionFilters;
using Bit.OData.Implementations;
using Bit.Owin.Exceptions;
using Bit.Owin.Implementations;
using Bit.OwinCore;
using Bit.OwinCore.Contracts;
using Bit.OwinCore.Middlewares;
using IdentityServer3.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Api.Implementations;
using SampleApp.DataAccess;
using SampleApp.DataAccess.Implementations;
using SampleApp.Dto.Implementations;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SampleApp
{
    public class AppStartup : AutofacAspNetCoreAppStartup
    {
        public AppStartup(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {

        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            DefaultAppModulesProvider.Current = new SampleAppModulesProvider();

            return base.ConfigureServices(services);
        }
    }

    public class SampleAppModulesProvider : IAspNetCoreAppModule, IAppModulesProvider
    {
        public IEnumerable<IAppModule> GetAppModules()
        {
            yield return this;
        }

        public virtual void ConfigureDependencies(IServiceProvider serviceProvider, IServiceCollection services, IDependencyManager dependencyManager)
        {
            AssemblyContainer.Current.Init();

            dependencyManager.RegisterMinimalDependencies();

            dependencyManager.RegisterDefaultLogger(typeof(DebugLogStore).GetTypeInfo(), typeof(ConsoleLogStore).GetTypeInfo());

            dependencyManager.RegisterDefaultAspNetCoreApp();

            services.AddResponseCompression();
            dependencyManager.RegisterAspNetCoreMiddlewareUsing(aspNetCoreApp =>
            {
                aspNetCoreApp.UseResponseCompression();
            });

            services.AddCors();
            dependencyManager.RegisterAspNetCoreMiddlewareUsing(aspNetCoreApp =>
            {
                aspNetCoreApp.UseCors(c => c.AllowAnyOrigin());
            });

            dependencyManager.RegisterAspNetCoreMiddleware<AspNetCoreStaticFilesMiddlewareConfiguration>();

            dependencyManager.RegisterMinimalAspNetCoreMiddlewares();

            dependencyManager.RegisterAspNetCoreSingleSignOnClient();

            dependencyManager.RegisterMetadata();

            dependencyManager.RegisterDefaultWebApiAndODataConfiguration();

            dependencyManager.RegisterWebApiMiddleware(webApiDependencyManager =>
            {
                webApiDependencyManager.RegisterWebApiMiddlewareUsingDefaultConfiguration();

                webApiDependencyManager.RegisterGlobalWebApiActionFiltersUsing(httpConfiguration =>
                {
                    httpConfiguration.Filters.Add(new System.Web.Http.AuthorizeAttribute());
                });

                webApiDependencyManager.RegisterGlobalWebApiCustomizerUsing(httpConfiguration =>
                {
                    httpConfiguration.EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", $"Swagger-Api");
                        c.ApplyDefaultApiConfig(httpConfiguration);
                    }).EnableBitSwaggerUi();
                });
            });

            dependencyManager.RegisterODataMiddleware(odataDependencyManager =>
            {
                odataDependencyManager.RegisterGlobalWebApiActionFiltersUsing(httpConfiguration =>
                {
                    httpConfiguration.Filters.Add(new DefaultODataAuthorizeAttribute());
                });

                odataDependencyManager.RegisterGlobalWebApiCustomizerUsing(httpConfiguration =>
                {
                    httpConfiguration.EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", $"Swagger-Api");
                        c.ApplyDefaultODataConfig(httpConfiguration);
                    }).EnableBitSwaggerUi();
                });

                odataDependencyManager.RegisterODataServiceBuilder<BitODataServiceBuilder>();
                odataDependencyManager.RegisterODataServiceBuilder<SampleAppODataServiceBuilder>();
                odataDependencyManager.RegisterWebApiODataMiddlewareUsingDefaultConfiguration();
            });

            //dependencyManager.Register<IDbConnectionProvider, DefaultDbConnectionProvider<SqlConnection>>(); // InMemory: See line below!
            dependencyManager.RegisterEfCoreDbContext<SampleAppDbContext, InMemoryDbContextObjectsProvider>();
            dependencyManager.RegisterAppEvents<SampleAppDbContextInitializer>();
            dependencyManager.RegisterRepository(typeof(SampleAppRepository<>).GetTypeInfo());

            dependencyManager.RegisterDtoEntityMapper();
            dependencyManager.RegisterDtoEntityMapperConfiguration<DefaultDtoEntityMapperConfiguration>();
            dependencyManager.RegisterDtoEntityMapperConfiguration<SampleAppDtoEntityMapperConfiguration>();

            dependencyManager.RegisterSingleSignOnServer<SampleAppUserService, SampleAppClientProvider>();

            dependencyManager.RegisterIndexPageMiddlewareUsingDefaultConfiguration();
        }
    }

    public class SampleAppClientProvider : ClientProvider
    {
        public override IEnumerable<Client> GetClients()
        {
            return new[]
            {
                GetResourceOwnerFlowClient(new BitResourceOwnerFlowClient
                {
                    ClientId = "SampleApp-ResOwner",
                    ClientName = "SampleApp-ResOwner",
                    Secret = "secret",
                    TokensLifetime = TimeSpan.FromDays(7),
                    Enabled = true
                })
            };
        }
    }
    public class SampleAppUserService : UserService
    {
        public override async Task<string> GetUserIdByLocalAuthenticationContextAsync(LocalAuthenticationContext context)
        {
            string username = context.UserName;
            string password = context.Password;

            if (string.IsNullOrEmpty(username))
                throw new ArgumentException(nameof(username));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException(nameof(password));

            if (username == password)
                return username;

            throw new DomainLogicException("LoginFailed");
        }

        public override async Task<bool> UserIsActiveAsync(IsActiveContext context, string userId)
        {
            return true;
        }
    }
}
