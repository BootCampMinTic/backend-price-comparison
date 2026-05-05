using Backend.PriceComparison.Application.Client.Dtos;

namespace Backend.PriceComparison.Domain.Test;

public class ApiResponseDtoTests
{
    [Fact]
    public void Constructor_WithData_SetsSuccessResponse()
    {
        var response = new ApiResponseDto<int[]>([1, 2], "Loaded");

        Assert.Equal("success", response.Status);
        Assert.Equal("Loaded", response.Message);
        Assert.NotNull(response.Data);
        Assert.Equal([1, 2], response.Data);
        Assert.False(string.IsNullOrWhiteSpace(response.CorrelationId));
    }

    [Fact]
    public void Constructor_WithMessage_SetsErrorResponseByDefault()
    {
        var response = new ApiResponseDto<object>("Failed");

        Assert.Equal("error", response.Status);
        Assert.Equal("Failed", response.Message);
        Assert.Null(response.Data);
    }
}
