using System;
using ApiEcommerce.Model;
using ApiEcommerce.Model.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>().
        ForMember(destination => destination.CategoryName, option => option.MapFrom(source => source.Category.Name)).ReverseMap();

        CreateMap<Product, CreateProductDto>().ReverseMap();
        CreateMap<Product, UpdateProductDto>().ReverseMap();
    }

}

