using Bit.Model.Contracts;
using System;

namespace SampleApp.Dto
{
    public class ProductDto : IDto
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual Guid CategoryId { get; set; }

        public virtual string CategoryName { get; set; }
    }
}
