using Backend.PriceComparison.Domain.ClientPos.Entities;

namespace Backend.PriceComparison.Domain.Test;

public class ClientEntityTests
{
    [Fact]
    public void IsValid_WithValidData_ReturnsTrue()
    {
        var entity = new ClientNaturalPosEntity
        {
            DocumentNumber = "123456789",
            DocumentTypeId = 1,
            Name = "Ana"
        };

        Assert.True(entity.IsValid());
    }

    [Fact]
    public void IsValid_WithNullDocumentNumber_ReturnsFalse()
    {
        var entity = new ClientNaturalPosEntity
        {
            DocumentNumber = null,
            DocumentTypeId = 1
        };

        Assert.False(entity.IsValid());
    }

    [Fact]
    public void IsValid_WithEmptyDocumentNumber_ReturnsFalse()
    {
        var entity = new ClientNaturalPosEntity
        {
            DocumentNumber = "",
            DocumentTypeId = 1
        };

        Assert.False(entity.IsValid());
    }

    [Fact]
    public void IsValid_WithWhitespaceDocumentNumber_ReturnsFalse()
    {
        var entity = new ClientNaturalPosEntity
        {
            DocumentNumber = "   ",
            DocumentTypeId = 1
        };

        Assert.False(entity.IsValid());
    }

    [Fact]
    public void IsValid_WithZeroDocumentTypeId_ReturnsFalse()
    {
        var entity = new ClientNaturalPosEntity
        {
            DocumentNumber = "123",
            DocumentTypeId = 0
        };

        Assert.False(entity.IsValid());
    }

    [Fact]
    public void IsValid_WithNegativeDocumentTypeId_ReturnsFalse()
    {
        var entity = new ClientNaturalPosEntity
        {
            DocumentNumber = "123",
            DocumentTypeId = -1
        };

        Assert.False(entity.IsValid());
    }

    [Fact]
    public void IsValid_LegalClient_WithValidData_ReturnsTrue()
    {
        var entity = new ClientLegalPosEntity
        {
            DocumentNumber = "900123456",
            DocumentTypeId = 3,
            CompanyName = "Empresa ABC"
        };

        Assert.True(entity.IsValid());
    }

    [Fact]
    public void UpdateElectronicInvoiceEmail_WithValidEmail_UpdatesProperty()
    {
        var entity = new ClientNaturalPosEntity
        {
            ElectronicInvoiceEmail = "old@example.com"
        };

        entity.UpdateElectronicInvoiceEmail("new@example.com");

        Assert.Equal("new@example.com", entity.ElectronicInvoiceEmail);
    }

    [Fact]
    public void UpdateElectronicInvoiceEmail_WithNullEmail_ThrowsArgumentException()
    {
        var entity = new ClientNaturalPosEntity();

        var ex = Assert.Throws<ArgumentException>(() => entity.UpdateElectronicInvoiceEmail(null!));

        Assert.Contains("Invalid email format", ex.Message);
    }

    [Fact]
    public void UpdateElectronicInvoiceEmail_WithEmptyEmail_ThrowsArgumentException()
    {
        var entity = new ClientNaturalPosEntity();

        var ex = Assert.Throws<ArgumentException>(() => entity.UpdateElectronicInvoiceEmail(""));

        Assert.Contains("Invalid email format", ex.Message);
    }

    [Fact]
    public void UpdateElectronicInvoiceEmail_WithoutAtSymbol_ThrowsArgumentException()
    {
        var entity = new ClientNaturalPosEntity();

        var ex = Assert.Throws<ArgumentException>(() => entity.UpdateElectronicInvoiceEmail("invalid-email"));

        Assert.Contains("Invalid email format", ex.Message);
    }

    [Fact]
    public void UpdateElectronicInvoiceEmail_WithWhitespaceOnly_ThrowsArgumentException()
    {
        var entity = new ClientNaturalPosEntity();

        var ex = Assert.Throws<ArgumentException>(() => entity.UpdateElectronicInvoiceEmail("   "));

        Assert.Contains("Invalid email format", ex.Message);
    }

    [Fact]
    public void ClientNaturalPosEntity_InheritsFromClientEntity()
    {
        var entity = new ClientNaturalPosEntity();

        Assert.IsAssignableFrom<ClientEntity>(entity);
    }

    [Fact]
    public void ClientLegalPosEntity_InheritsFromClientEntity()
    {
        var entity = new ClientLegalPosEntity();

        Assert.IsAssignableFrom<ClientEntity>(entity);
    }
}
