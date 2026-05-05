using Backend.PriceComparison.Domain.Common.Results;
using Backend.PriceComparison.Domain.Common.Results.Errors;
using System.Net;

namespace Backend.PriceComparison.Domain.Test;

public class ResultTests
{
    [Fact]
    public void Success_CreatesSuccessfulResultWithValue()
    {
        var result = Result<string, Error>.Success("ok");

        Assert.True(result.IsSuccess);
        Assert.Equal("ok", result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_CreatesFailedResultWithError()
    {
        var error = Error.CreateInstance("NotFound", "The resource was not found.", HttpStatusCode.NotFound);

        var result = Result<string, Error>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void VoidResult_Instance_ReturnsSingleton()
    {
        Assert.Same(VoidResult.Instance, VoidResult.Instance);
    }
}
