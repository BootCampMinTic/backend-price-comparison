using AutoMapper;
using Backend.PriceComparison.Application.Store.Commands.CreateCategoryProduct;
using Backend.PriceComparison.Application.Store.Commands.CreateCategoryStore;
using Backend.PriceComparison.Application.Store.Commands.CreateProduct;
using Backend.PriceComparison.Application.Store.Commands.CreateSale;
using Backend.PriceComparison.Application.Store.Commands.UpdateCategoryProduct;
using Backend.PriceComparison.Application.Store.Commands.UpdateCategoryStore;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Application.Store.Mappers;

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

        CreateMap<SaleEntity, SaleDto>();

        CreateMap<CreateSaleCommand, SaleEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<CategoryProductEntity, CategoryProductDto>();
        CreateMap<CategoryStoreEntity, CategoryStoreDto>();
    }
}