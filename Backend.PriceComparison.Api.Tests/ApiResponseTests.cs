using Backend.PriceComparison.Api.Common.Wrappers;

namespace Backend.PriceComparison.Api.Tests;

public class ApiResponseTests
{
    [Fact]
    public void SuccessResponse_SetsSuccessMessageAndData()
    {
        var response = ApiResponse<string>.SuccessResponse("payload", "created");

        Assert.True(response.Success);
        Assert.Equal("created", response.Message);
        Assert.Equal("payload", response.Data);
        Assert.Null(response.Errors);
    }

    [Fact]
    public void ErrorResponse_WithMessage_SetsFailureState()
    {
        var response = ApiResponse<object>.ErrorResponse("invalid request");

        Assert.False(response.Success);
        Assert.Equal("invalid request", response.Message);
        Assert.Null(response.Data);
    }

    [Fact]
    public void PagedResponse_WithTotalRecords_CalculatesTotalPages()
    {
        var response = new PagedResponse<int[]>([1, 2, 3], pageNumber: 2, pageSize: 10, totalRecords: 21);

        Assert.True(response.Success);
        Assert.Equal(2, response.PageNumber);
        Assert.Equal(10, response.PageSize);
        Assert.Equal(21, response.TotalRecords);
        Assert.Equal(3, response.TotalPages);
        Assert.NotNull(response.Data);
        Assert.Equal([1, 2, 3], response.Data);
    }
}
