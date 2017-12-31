using Bit.Core.Contracts;
using Bit.Core.Implementations;
using Bit.Data.EntityFrameworkCore.Contracts;
using Bit.Data.EntityFrameworkCore.Implementations;
using Microsoft.EntityFrameworkCore;
using SampleApp.Model;

namespace SampleApp.DataAccess
{
    public class SampleAppDbContext : EfCoreDbContextBase
    {
        public SampleAppDbContext() : base(new DbContextOptionsBuilder().UseInMemoryDatabase("SampleAppDatabase").Options)
        {

        }

        public SampleAppDbContext(IAppEnvironmentProvider appEnvironmentProvider, IDbContextObjectsProvider dbContextCreationOptionsProvider)
              : base(appEnvironmentProvider.GetActiveAppEnvironment().GetConfig<string>("AppConnectionString"), dbContextCreationOptionsProvider)
        {

        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Product> Products { get; set; }
    }
}
