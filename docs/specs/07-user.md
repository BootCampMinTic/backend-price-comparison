# User Entity Specification

## Database Schema

**Table:** `user`

| Column | Type | Nullable | Key | Extra |
|--------|------|----------|-----|-------|
| `id_user` | `int(11)` | NO | PRI | auto_increment |
| `name` | `varchar(45)` | NO | | |
| `password` | `varchar(45)` | NO | | |
| `id_type_user` | `int(11)` | NO | MUL | FK → type_user |

> **WARNING:** Password stored in plaintext (`varchar(45)`). **Must be hashed** (e.g. BCrypt) before production use.

**Relationships:**
- **FK:** `id_type_user` → `type_user.id_type_user`
- **HasMany:** `sale` (purchases made by this user)

---

## C# Entity

```csharp
public sealed class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int TypeUserId { get; set; }
    public TypeUserEntity? TypeUser { get; set; }
}
```

**EF Core Mapping:**

```csharp
modelBuilder.Entity<UserEntity>(entity =>
{
    entity.ToTable("user");
    entity.Property(e => e.Id).HasColumnName("id_user");
    entity.Property(e => e.Name).HasColumnName("name");
    entity.Property(e => e.Password).HasColumnName("password");
    entity.Property(e => e.TypeUserId).HasColumnName("id_type_user");
    entity.HasOne(e => e.TypeUser).WithMany().HasForeignKey(e => e.TypeUserId);
});
```

> **Security Note:** The `password` property should NOT be serialized in any DTO/API response.

---

## Port

```csharp
public interface IUserRepository
{
    Task<Result<IEnumerable<UserEntity>, Error>> GetAllAsync(CancellationToken ct);
    Task<Result<UserEntity, Error>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<VoidResult, Error>> CreateAsync(UserEntity entity, CancellationToken ct);
}
```

---

## Infrastructure

**File:** `Infrastructure/Store/Repositories/UserRepository.cs`
- `GetAllAsync`: `AsNoTracking().Include(u => u.TypeUser)`
- `GetByIdAsync`: `AsNoTracking().Include(u => u.TypeUser).FirstOrDefaultAsync()`
- `CreateAsync`: standard

**Mock:** `MockUserRepository.cs` — 2 users (Carlos/Admin, Ana/Cliente).

---

## Application

### Queries

**GetAll:** `GetAllUsersQuery` — Cache key: `CacheKeys.UsersAll` (`"users:all"`)

**GetById:** `GetUserByIdQuery(int Id)` — Cache key: `CacheKeys.UserById(id)` (`"user:{id}"`)

### Command

**Create:**
```csharp
public record CreateUserCommand(
    string Name, string Password, int TypeUserId
) : IRequest<Result<VoidResult, Error>>;
```

**Handler:** Invalidates `CacheKeys.UsersPrefix` (`"users"`) on success.

**Validator:**
- `Name.NotEmpty().MinimumLength(2)`
- `Password.NotEmpty().MinimumLength(3)`
- `TypeUserId.GreaterThan(0)`

---

## API Endpoints

```
GET    /api/v1/users       → GetAll
GET    /api/v1/users/{id}  → GetById
POST   /api/v1/users       → Create
```

Tags: `User`.

---

## UserDto

```csharp
public sealed class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TypeUserId { get; set; }
    public string? TypeUserDescription { get; set; }  // flattened
    // NOTE: Password is intentionally excluded
}
```

---

## Security Considerations (TODO)

1. Hash password with `BCrypt.Net` before persisting
2. Never return password in any API response
3. Add authentication endpoint (`POST /api/v1/auth/login`)
4. Consider `[JsonIgnore]` on `Password` property
5. Minimize password length from `varchar(45)` to `varchar(255)` for hashed storage

---

## Notes

- Used as FK by `SaleEntity` (user who made the purchase).
- User type (`Administrador` vs `Cliente`) is a catalog lookup via `TypeUserId`.
