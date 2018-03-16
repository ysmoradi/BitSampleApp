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

        public SampleAppDbContext(IDbContextObjectsProvider dbContextCreationOptionsProvider)
              : base("SampleAppDatabase", dbContextCreationOptionsProvider)
        {

        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Product> Products { get; set; }
    }
}
