using Bit.Model.Contracts;
using SampleApp.Model.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Model
{
    public class Category : IEntity, ISyncableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual bool IsArchived { get; set; }

        public virtual long Version { get; set; }
    }
}
