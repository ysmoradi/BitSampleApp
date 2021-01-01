using Bit.Core.Exceptions;
using Bit.OData.ODataControllers;
using SampleApp.Dto;
using SampleApp.Model;
using System;
using System.Security.Cryptography;
using System.Text;
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

        public class HashSampleArgs
        {
            public Guid Id { get; set; }

            public string Hash { get; set; }
        }

        [Action]
        public async Task HashSample(HashSampleArgs args)
        {
            string input = args.Id.ToString().Split('-')[0];

            string hashedInput = input.Hash();

            if (args.Hash != hashedInput)
                throw new BadRequestException("invalid hash");

            // the rest of the logic...
        }
    }

    public static class HashExtensions
    {
        public static string Hash(this string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            using SHA256Managed generator = new SHA256Managed();

            byte[] hash = generator.ComputeHash(bytes);

            return BytesToHex(hash);
        }

        private static string BytesToHex(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
    }
}
