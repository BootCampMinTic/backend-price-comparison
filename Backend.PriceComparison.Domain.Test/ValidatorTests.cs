using Backend.PriceComparison.Application.Client.Queries.Client;

namespace Backend.PriceComparison.Domain.Test;

public class ValidatorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetClientByIdQueryValidator_RejectsNonPositiveId(int id)
    {
        var validator = new GetClientByIdQueryValidator();

        var result = validator.Validate(new GetClientByIdQuery { Id = id });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetClientByIdQuery.Id));
    }

    [Fact]
    public void GetClientByIdQueryValidator_AcceptsPositiveId()
    {
        var validator = new GetClientByIdQueryValidator();

        var result = validator.Validate(new GetClientByIdQuery { Id = 1 });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetClientByDocumentNumberQueryValidator_RejectsNullDocumentNumber()
    {
        var validator = new GetClientByDocumentNumberQueryValidator();

        var result = validator.Validate(new GetClientByDocumentNumberQuery { DocumentNumber = null! });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetClientByDocumentNumberQuery.DocumentNumber));
    }
}
