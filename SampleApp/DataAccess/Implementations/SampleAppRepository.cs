using Bit.Data.EntityFrameworkCore.Implementations;
using Bit.Model.Contracts;

namespace SampleApp.DataAccess.Implementations
{
    public class SampleAppRepository<TEntity> : EfCoreRepository<TEntity>
        where TEntity : class, IEntity
    {

    }
}
