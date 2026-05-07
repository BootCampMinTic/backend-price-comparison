using AutoMapper;
using Backend.PriceComparison.Application.Store.Commands.CreateCategoryProduct;
using Backend.PriceComparison.Application.Store.Commands.CreateCategoryStore;
using Backend.PriceComparison.Application.Store.Commands.CreateProduct;
using Backend.PriceComparison.Application.Store.Commands.CreateSale;
using Backend.PriceComparison.Application.Store.Commands.CreateStore;
using Backend.PriceComparison.Application.Store.Commands.CreateUser;
using Backend.PriceComparison.Application.Store.Dtos;
using Backend.PriceComparison.Domain.Store.Entities;

namespace Backend.PriceComparison.Application.Store.Mappers;

public class StoreProfile : Profile
{
    public StoreProfile()
    {
        CreateMap<StateEntity, StateDto>();
        CreateMap<TypeUserEntity, TypeUserDto>();
        CreateMap<CategoryProductEntity, CategoryProductDto>();
        CreateMap<CategoryStoreEntity, CategoryStoreDto>();

        CreateMap<StoreEntity, StoreDto>()
            .ForMember(dest => dest.CategoryStoreDescription, opt => opt.MapFrom(src => src.CategoryStore != null ? src.CategoryStore.Description : null));

        CreateMap<ProductEntity, ProductDto>()
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null))
            .ForMember(dest => dest.CategoryProductDescription, opt => opt.MapFrom(src => src.CategoryProduct != null ? src.CategoryProduct.Description : null));

        CreateMap<UserEntity, UserDto>()
            .ForMember(dest => dest.TypeUserDescription, opt => opt.MapFrom(src => src.TypeUser != null ? src.TypeUser.Description : null));

        CreateMap<SaleEntity, SaleDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : null))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null))
            .ForMember(dest => dest.StateDescription, opt => opt.MapFrom(src => src.State != null ? src.State.Description : null))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.ProductSales));

        CreateMap<ProductSaleEntity, ProductSaleDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0));

        CreateMap<CreateCategoryProductCommand, CategoryProductEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<CreateCategoryStoreCommand, CategoryStoreEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<CreateStoreCommand, StoreEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryStore, opt => opt.Ignore());
        CreateMap<CreateProductCommand, ProductEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Store, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryProduct, opt => opt.Ignore());
        CreateMap<CreateUserCommand, UserEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TypeUser, opt => opt.Ignore());
        CreateMap<CreateSaleCommand, SaleEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Store, opt => opt.Ignore())
            .ForMember(dest => dest.State, opt => opt.Ignore())
            .ForMember(dest => dest.ProductSales, opt => opt.Ignore());
    }
}
