using ApiEcommerce.Model;
using ApiEcommerce.Model.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public class ProductProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(destination => destination.CategoryName, source => source.Category.Name);
        config.NewConfig<ProductDto, Product>();
        config.NewConfig<Product, CreateProductDto>();
        config.NewConfig<CreateProductDto, Product>();
        config.NewConfig<Product, UpdateProductDto>();
        config.NewConfig<UpdateProductDto, Product>();
    }
}
