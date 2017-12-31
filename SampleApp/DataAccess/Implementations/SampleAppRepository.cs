using Bit.Core.Contracts;
using Bit.Data.EntityFrameworkCore.Implementations;
using Bit.Model.Contracts;
using Microsoft.EntityFrameworkCore;
using SampleApp.Model.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp.DataAccess.Implementations
{
    public class SampleAppRepository<TEntity> : EfCoreRepository<TEntity>
        where TEntity : class, IEntity
    {
        public override void SaveChanges()
        {
            OnSave();

            base.SaveChanges();
        }

        private void OnSave()
        {
            DbContext.ChangeTracker.DetectChanges();

            DbContext.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Deleted || e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .OfType<IVersionableEntity>()
                .ToList()
                .ForEach(e => e.Version = DateTimeProvider.GetCurrentUtcDateTime().UtcTicks);
        }

        public virtual IDateTimeProvider DateTimeProvider { get; set; }

        public override Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            OnSave();

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
