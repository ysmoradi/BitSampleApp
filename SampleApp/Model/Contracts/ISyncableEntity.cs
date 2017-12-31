using Bit.Model.Contracts;

namespace SampleApp.Model.Contracts
{
    public interface ISyncableEntity : IArchivableEntity, IVersionableEntity
    {
    }
}
