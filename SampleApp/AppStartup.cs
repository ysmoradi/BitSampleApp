using Bit.Core;
using Bit.Core.Contracts;
using Bit.Core.Exceptions;
using Bit.Core.Models;
using Bit.IdentityServer.Contracts;
using Bit.IdentityServer.Implementations;
using Bit.Model.Implementations;
using Bit.OData.ActionFilters;
using Bit.OData.Contracts;
using Bit.Owin;
using Bit.Owin.Contracts;
using Bit.Owin.Implementations;
using Bit.WebApi.ActionFilters;
using IdentityServer3.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using SampleApp.DataAccess;
using SampleApp.DataAccess.Implementations;
using SampleApp.Dto.Implementations;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

[assembly: ODataModule("SampleApp")]

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

    public class SampleAppModulesProvider : IAppModule, IAppModulesProvider
    {
        public IEnumerable<IAppModule> GetAppModules()
        {
            yield return this;
        }

        public virtual void ConfigureDependencies(IServiceCollection services, IDependencyManager dependencyManager)
        {
            AssemblyContainer.Current.Init();

            dependencyManager.RegisterMinimalDependencies();

            dependencyManager.RegisterDefaultLogger(typeof(DebugLogStore).GetTypeInfo(), typeof(ConsoleLogStore).GetTypeInfo());

            dependencyManager.RegisterDefaultAspNetCoreApp();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            }).Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            }).Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            dependencyManager.RegisterAspNetCoreMiddlewareUsing(aspNetCoreApp =>
            {
                aspNetCoreApp.UseResponseCompression();
            });

            services.AddCors();
            dependencyManager.RegisterAspNetCoreMiddlewareUsing(aspNetCoreApp =>
            {
                aspNetCoreApp.UseCors(c => c.AllowAnyOrigin());
            });

            dependencyManager.RegisterMinimalAspNetCoreMiddlewares();

            dependencyManager.RegisterAspNetCoreSingleSignOnClient();

            dependencyManager.RegisterMetadata();

            dependencyManager.RegisterDefaultWebApiAndODataConfiguration();

            dependencyManager.RegisterWebApiMiddleware(webApiDependencyManager =>
            {
                webApiDependencyManager.RegisterWebApiMiddlewareUsingDefaultConfiguration();

                webApiDependencyManager.RegisterGlobalWebApiActionFiltersUsing(httpConfiguration =>
                {
                    httpConfiguration.Filters.Add(new AppExceptionHandlerFilterAttribute());
                    httpConfiguration.Filters.Add(new System.Web.Http.AuthorizeAttribute());
                });

                webApiDependencyManager.RegisterGlobalWebApiCustomizerUsing(httpConfiguration =>
                {
                    httpConfiguration.EnableMultiVersionWebApiSwaggerWithUI();
                });
            });

            dependencyManager.RegisterODataMiddleware(odataDependencyManager =>
            {
                odataDependencyManager.RegisterGlobalWebApiActionFiltersUsing(httpConfiguration =>
                {
                    httpConfiguration.Filters.Add(new AppExceptionHandlerFilterAttribute());
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

                odataDependencyManager.RegisterWebApiODataMiddlewareUsingDefaultConfiguration();
            });

            //dependencyManager.Register<IDbConnectionProvider, DefaultDbConnectionProvider<SqlConnection>>(); // InMemory: See line below!
            dependencyManager.RegisterEfCoreDbContext<SampleAppDbContext>((serviceProvider, options) =>
            {
                options.UseInMemoryDatabase("SampleAppDb");
            });
            dependencyManager.RegisterAppEvents<SampleAppDbContextInitializer>();
            dependencyManager.RegisterRepository(typeof(SampleAppRepository<>).GetTypeInfo());

            dependencyManager.RegisterDtoEntityMapper();
            dependencyManager.RegisterMapperConfiguration<DefaultMapperConfiguration>();
            dependencyManager.RegisterMapperConfiguration<SampleAppMapperConfiguration>();

            dependencyManager.RegisterSingleSignOnServer<SampleAppUserService, SampleAppClientProvider>();

            dependencyManager.RegisterIndexPageMiddlewareUsingDefaultConfiguration();
        }
    }

    public class SampleAppClientProvider : OAuthClientsProvider
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
        public async override Task<BitJwtToken> LocalLogin(LocalAuthenticationContext context, CancellationToken cancellationToken)
        {
            string username = context.UserName;
            string password = context.Password;

            if (string.IsNullOrEmpty(username))
                throw new ArgumentException(nameof(username));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException(nameof(password));

            if (username == password)
                return new BitJwtToken { UserId = username };

            throw new DomainLogicException("LoginFailed");
        }
    }

    public class AppExceptionHandlerFilterAttribute : ExceptionHandlerFilterAttribute
    {
        protected override HttpResponseMessage CreateErrorResponseMessage(HttpActionExecutedContext actionExecutedContext, IExceptionToHttpErrorMapper exceptionToHttpErrorMapper, Exception exception)
        {
            IDependencyResolver resolver = actionExecutedContext.Request.GetOwinContext().GetDependencyResolver();
            ILogger logger = resolver.Resolve<ILogger>();
            HttpContext httpContext = resolver.Resolve<IHttpContextAccessor>().HttpContext;
            string logNumber = $"{(DateTimeOffset.UtcNow.UtcTicks / 10) % 1000000000:d9}";
            httpContext.Request.Headers.Remove("X-Correlation-ID");
            httpContext.Request.Headers.Add("X-Correlation-ID", new string[] { logNumber });
            logger.LogData.Single(i => i.Key == "X-Correlation-ID").Value = logNumber;

            return actionExecutedContext.Request.CreateResponse(exceptionToHttpErrorMapper.GetStatusCode(exception), new
            {
                Code = logNumber,
                Message = exceptionToHttpErrorMapper.GetMessage(exception)
            }, new JsonMediaTypeFormatter());
        }
    }
}
