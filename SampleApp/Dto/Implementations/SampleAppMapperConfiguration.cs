using AutoMapper;
using Bit.Model.Contracts;
using SampleApp.Model;
using System.Linq;

namespace SampleApp.Dto.Implementations
{
    public class SampleAppMapperConfiguration : IMapperConfiguration
    {
        public virtual void Configure(IMapperConfigurationExpression mapperConfigExpression)
        {
            mapperConfigExpression.CreateMap<Category, CategoryDto>()
                .ForMember(category => category.AllProductsAreActive, config => config.MapFrom(category => category.Products.All(p => p.IsActive == true)))
                .ReverseMap();

            mapperConfigExpression.CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
