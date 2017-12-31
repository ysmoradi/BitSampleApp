using Bit.OData.ODataControllers;
using SampleApp.Dto;
using SampleApp.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp.Api
{
    public class CategoriesController : DtoSetController<CategoryDto, Category, Guid>
    {
        /// <summary>
        /// It returns categories without product.
        /// </summary>
        [Function]
        public virtual async Task<IQueryable<CategoryDto>> GetEmptyCategories(CancellationToken cancellationToken)
        {
            return DtoEntityMapper.FromEntityQueryToDtoQuery((await Repository.GetAllAsync(cancellationToken))
                .Where(c => !c.Products.Any()));
        }
    }
}
