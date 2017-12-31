using Bit.Model.Contracts;

namespace SampleApp.Model.Contracts
{
    public interface IVersionableEntity : IEntity
    {
        long Version { get; set; }
    }
}
