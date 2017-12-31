using Bit.Model.Contracts;
using System;
using System.Collections.Generic;

namespace SampleApp.Dto
{
    public class CategoryDto : IDto, ISyncableDto
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual ICollection<ProductDto> Products { get; set; }

        public virtual bool AllProductsAreActive { get; set; }

        public virtual int ProductsCount { get; set; }

        public virtual bool IsArchived { get; set; }

        public virtual long Version { get; set; }
    }
}
