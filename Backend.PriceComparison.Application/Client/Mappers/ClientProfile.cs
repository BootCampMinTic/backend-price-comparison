using AutoMapper;
using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;
using Backend.PriceComparison.Application.Client.Dtos;
using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Application.Client.Mappers;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        CreateMap<CreateClientLegalPosCommand, ClientLegalPosEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentType, opt => opt.Ignore());

        CreateMap<CreateClientNaturalPosCommand, ClientNaturalPosEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentType, opt => opt.Ignore());

        CreateMap<ClientEntity, ClientDto>();
        CreateMap<ClientLegalPosEntity, ClientDto>();
        CreateMap<ClientNaturalPosEntity, ClientDto>();
    }
}
