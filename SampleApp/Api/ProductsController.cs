using Bit.Core.Exceptions;
using Bit.OData.ODataControllers;
using SampleApp.Dto;
using SampleApp.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp.Api
{
    public class ProductsController : DtoSetController<ProductDto, Product, Guid>
    {
        public class DeactivateProductByIdArgs
        {
            public Guid id { get; set; }
        }

        [Action]
        public virtual async Task DeactivateProductById(DeactivateProductByIdArgs args, CancellationToken cancellationToken)
        {
            try
            {
                Product product = await Repository.GetByIdAsync(cancellationToken, args.id);

                if (product == null)
                    throw new ResourceNotFoundException();

                if (product.IsActive == false)
                    throw new DomainLogicException("ProductIsDeactiveAlready");

                product.IsActive = false;

                await Repository.UpdateAsync(product, cancellationToken);
            }
            catch (InvalidOperationException ex) when (ex.Message == "Source sequence doesn't contain any elements.")
            {
                throw new ResourceNotFoundException("ProductNotFound", ex);
            }
        }
    }
}
