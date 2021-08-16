using Bit.Core.Exceptions;
using Bit.Core.Exceptions.Contracts;
using Bit.OData.ODataControllers;
using SampleApp.Dto;
using SampleApp.Model;
using System;
using System.Net;
using System.Runtime.Serialization;
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
            throw new TooManyRequestsException("/-:");

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

    [Serializable]
    public class TooManyRequestsException : AppException, IHttpStatusCodeAwareException
    {
        public TooManyRequestsException()
            : this(ExceptionMessageKeys.DomainLogicException)
        {
        }

        public TooManyRequestsException(string message)
            : base(message)
        {
        }

        public TooManyRequestsException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        protected TooManyRequestsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.TooManyRequests;
    }
}
