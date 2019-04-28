using Bit.Data.EntityFrameworkCore.Implementations;
using Microsoft.EntityFrameworkCore;
using SampleApp.Model;

namespace SampleApp.DataAccess
{
    public class SampleAppDbContext : EfCoreDbContextBase
    {
        public SampleAppDbContext(DbContextOptions<SampleAppDbContext> options)
              : base(options)
        {

        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Product> Products { get; set; }
    }
}
