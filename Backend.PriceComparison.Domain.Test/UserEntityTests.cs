using Backend.PriceComparison.Domain.Common.Results.Errors;
using Backend.PriceComparison.Domain.Store.Entities;
using System.Net;

namespace Backend.PriceComparison.Domain.Test;

public class UserEntityTests
{
    [Fact]
    public void UserEntity_DefaultValues_AreSet()
    {
        var user = new UserEntity();

        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Name);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal("User", user.Role);
        Assert.True(user.IsActive);
        Assert.NotEqual(default, user.CreatedAt);
    }

    [Fact]
    public void UserEntity_CanSetProperties()
    {
        var now = DateTime.UtcNow;
        var user = new UserEntity
        {
            Id = 1,
            Name = "Test User",
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            Role = "Admin",
            IsActive = false,
            CreatedAt = now
        };

        Assert.Equal(1, user.Id);
        Assert.Equal("Test User", user.Name);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("hashed_password", user.PasswordHash);
        Assert.Equal("Admin", user.Role);
        Assert.False(user.IsActive);
        Assert.Equal(now, user.CreatedAt);
    }

    [Fact]
    public void UserErrorBuilder_NotFound_CorrectError()
    {
        var error = UserErrorBuilder.NotFound(1, "User");

        Assert.Equal("UserRecordNotFound", error.Code);
        Assert.Equal("User with ID 1 was not found.", error.Description);
        Assert.Equal(HttpStatusCode.NotFound, error.HttpStatusCode);
    }

    [Fact]
    public void UserErrorBuilder_NoRecordsFound_CorrectError()
    {
        var error = UserErrorBuilder.NoRecordsFound("user");

        Assert.Equal("UserRecordNotFound", error.Code);
        Assert.Equal("No user records were found.", error.Description);
        Assert.Equal(HttpStatusCode.NotFound, error.HttpStatusCode);
    }

    [Fact]
    public void UserErrorBuilder_CreationFailed_CorrectError()
    {
        var error = UserErrorBuilder.CreationFailed("user");

        Assert.Equal("UserCreationError", error.Code);
        Assert.Equal("Failed to create user due to an internal error.", error.Description);
        Assert.Equal(HttpStatusCode.InternalServerError, error.HttpStatusCode);
    }
}
