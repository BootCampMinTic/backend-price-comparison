namespace Backend.PriceComparison.Domain.Store.Entities;

public sealed class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int TypeUserId { get; set; }
    public TypeUserEntity? TypeUser { get; set; }
}
