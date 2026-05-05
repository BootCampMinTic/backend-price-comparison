using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using System.Net;

namespace Backend.PriceComparison.Domain.Test;

public class ErrorTests
{
    [Fact]
    public void CreateInstance_SetsCodeDescriptionAndStatusCode()
    {
        var error = Error.CreateInstance("ValidationError", "Invalid input.", HttpStatusCode.BadRequest);

        Assert.Equal("ValidationError", error.Code);
        Assert.Equal("Invalid input.", error.Description);
        Assert.Equal(HttpStatusCode.BadRequest, error.HttpStatusCode);
    }

    [Fact]
    public void ClientNotFound_ById_CreatesNotFoundError()
    {
        var error = ClienErrorBuilder.ClienNotFoundException(10);

        Assert.Equal(ClienErrorBuilder.CLIENT_NOT_FOUND_ERROR, error.Code);
        Assert.Equal(HttpStatusCode.NotFound, error.HttpStatusCode);
        Assert.Contains("10", error.Description);
    }
}
