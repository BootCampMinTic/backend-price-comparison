using System.Diagnostics;
using AutoMapper;
using Backend.PriceComparison.Application.Store.Commands.CreateProduct;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Application.Store.Mappers;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class StoreProfile : Profile
{
    public StoreProfile()
    {
        CreateMap<ProductEntity, ProductDto>()
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null))
            .ForMember(dest => dest.CategoryProductDescription, opt => opt.MapFrom(src => src.CategoryProduct != null ? src.CategoryProduct.Description : null));

        CreateMap<CreateProductCommand, ProductEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Store, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryProduct, opt => opt.Ignore());

            CreateMap<CategoryProductEntity, CategoryProductDto>();

            CreateMap<CreateCategoryProductCommand, CategoryProductEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }

}