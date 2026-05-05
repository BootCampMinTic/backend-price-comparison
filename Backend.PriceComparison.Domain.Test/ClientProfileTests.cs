using AutoMapper;
using Backend.PriceComparison.Application.Client.Commands.CreateClientPos;
using Backend.PriceComparison.Application.Client.Mappers;
using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Domain.Test;

public class ClientProfileTests
{
    private readonly IMapper _mapper = new MapperConfiguration(config => config.AddProfile<ClientProfile>()).CreateMapper();

    [Fact]
    public void CreateClientNaturalPosCommand_MapsToEntity()
    {
        var command = new CreateClientNaturalPosCommand(
            "Ana",
            "Maria",
            "Lopez",
            "Diaz",
            "123",
            "ana@example.com",
            1,
            "CO");

        var entity = _mapper.Map<ClientNaturalPosEntity>(command);

        Assert.Equal("Ana", entity.Name);
        Assert.Equal("Maria", entity.MiddleName);
        Assert.Equal("Lopez", entity.LastName);
        Assert.Equal("Diaz", entity.SecondSurname);
        Assert.Equal("123", entity.DocumentNumber);
        Assert.Equal("ana@example.com", entity.ElectronicInvoiceEmail);
        Assert.Equal(1, entity.DocumentTypeId);
        Assert.Equal("CO", entity.DocumentCountry);
        Assert.Equal(0, entity.Id);
        Assert.Null(entity.DocumentType);
    }

    [Fact]
    public void CreateClientLegalPosCommand_MapsToEntity()
    {
        var command = new CreateClientLegalPosCommand(
            "Empresa SAS",
            7,
            "900123",
            "billing@example.com",
            true,
            false,
            true,
            false,
            2,
            true,
            "CO");

        var entity = _mapper.Map<ClientLegalPosEntity>(command);

        Assert.Equal("Empresa SAS", entity.CompanyName);
        Assert.Equal(7, entity.VerificationDigit);
        Assert.Equal("900123", entity.DocumentNumber);
        Assert.Equal("billing@example.com", entity.ElectronicInvoiceEmail);
        Assert.True(entity.VATResponsibleParty);
        Assert.False(entity.SelfRetainer);
        Assert.True(entity.WithholdingAgent);
        Assert.False(entity.SimpleTaxRegime);
        Assert.Equal(2, entity.DocumentTypeId);
        Assert.True(entity.LargeTaxpayer);
        Assert.Equal("CO", entity.DocumentCountry);
    }
}
