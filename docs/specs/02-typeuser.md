# TypeUser Entity Specification

## Database Schema

**Table:** `type_user`

| Column | Type | Nullable | Key | Extra |
|--------|------|----------|-----|-------|
| `id_type_user` | `int(11)` | NO | PRI | auto_increment |
| `description` | `varchar(45)` | NO | | |

**Relationships:**
- `user.id_type_user` → `type_user.id_type_user` (one type has many users)

**Seed Data:**
| id_type_user | description |
|-------------|-------------|
| 1 | Administrador |
| 2 | Cliente |

---

## C# Entity

**Namespace:** `Backend.PriceComparison.Domain.Store.Entities`

```csharp
public sealed class TypeUserEntity
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
}
```

**EF Core Mapping:**

```csharp
modelBuilder.Entity<TypeUserEntity>(entity =>
{
    entity.ToTable("type_user");
    entity.Property(e => e.Id).HasColumnName("id_type_user");
    entity.Property(e => e.Description).HasColumnName("description");
});
```

---

## Port (Domain Interface)

**File:** `Domain/Ports/ITypeUserRepository.cs`

```csharp
public interface ITypeUserRepository
{
    Task<Result<IEnumerable<TypeUserEntity>, Error>> GetAllAsync(CancellationToken ct);
}
```

---

## Infrastructure

**File:** `Infrastructure/Store/Repositories/TypeUserRepository.cs`
- `AsNoTracking()`, returns `StoreErrorBuilder.NoRecordsFound("type user")` on empty.

**Mock:** `Infrastructure/Mock/MockTypeUserRepository.cs` - Returns Administrador + Cliente.

---

## Application

### Query
```csharp
public record GetAllTypeUsersQuery : IRequest<Result<IEnumerable<TypeUserDto>, Error>>;
```

### Handler
- Cache key: `CacheKeys.TypeUsersAll` (`"typeusers:all"`)
- Cache-aside pattern

### DTO

```csharp
public sealed class TypeUserDto { int Id; string Description; }
```

---

## API Endpoint

```
GET /api/v1/type-users
```

Tags: `Catalog`.

---

## Cache

| Key | Pattern | Invalidated by |
|-----|---------|----------------|
| `typeusers:all` | Constant | N/A (read-only catalog) |

---

## Notes

- Read-only catalog. Used as FK by `UserEntity`.
- No create/update/delete endpoints.
