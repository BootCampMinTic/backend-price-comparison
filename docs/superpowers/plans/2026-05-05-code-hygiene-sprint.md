# Code Hygiene & CVE Mitigation Sprint — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Close the top decision-free items left after the recent dotnet-best-practices code review: the AutoMapper CVE, missing direct tests for `ClientRepository`, `<inheritdoc/>` on adapter implementations, the `DocumentTypeEntity → DocumentTypeDto` AutoMapper gap, and a documented cache-key contract.

**Architecture:** Incremental refactors over the existing Clean + Hexagonal layout. No new projects. One new test infrastructure dependency (`Microsoft.EntityFrameworkCore.InMemory`) so `ClientRepository` can be tested without MySQL.

**Tech Stack:** .NET 10, xUnit (project convention — we keep it instead of MSTest from the skill, see Notes), AutoMapper (bump), Microsoft.EntityFrameworkCore.InMemory 9.0.

**Out of scope (separate plans needed):** credentials migration to User Secrets, JWT validation, xUnit→MSTest migration, removing the `ClientCommandService`/`QueryService` MediatR wrappers.

**Notes:**
- The `dotnet-best-practices` skill recommends MSTest + FluentAssertions + Moq, but the rest of the solution uses xUnit with native asserts (`Backend.PriceComparison.Domain.Test`, `Backend.PriceComparison.Api.Tests`). For consistency we stay on xUnit. A migration to MSTest is its own plan.
- We are working directly on `main`. The `commit-semver` slash command in this repo auto-pushes after each commit. Let it.

---

## File Structure

| File | Action | Responsibility |
|---|---|---|
| `Backend.PriceComparison.Application/Backend.PriceComparison.Application.csproj` | modify | Bump `AutoMapper` |
| `Backend.PriceComparison.Application/Client/Mappers/ClientProfile.cs` | modify | Add `DocumentTypeEntity → DocumentTypeDto` map |
| `Backend.PriceComparison.Application/Client/Queries/DocumentType/GetAllDocumentTypeQueryHandler.cs` | modify | Replace manual mapping with `IMapper` |
| `Backend.PriceComparison.Application/Common/Constants/CacheKeys.cs` | create | Centralized cache-key contract (constants + helpers) |
| `Backend.PriceComparison.Application/Client/Commands/CreateClientPos/CreateClientNaturalPosHandle.cs` | modify | Use `CacheKeys.NaturalCollectionPrefix` |
| `Backend.PriceComparison.Application/Client/Commands/CreateClientPos/CreateClientLegalPosHandle.cs` | modify | Use `CacheKeys.LegalCollectionPrefix` |
| `Backend.PriceComparison.Application/Client/Queries/Client/GetClientByIdQueryHandler.cs` | modify | Use `CacheKeys.ClientById(...)` |
| `Backend.PriceComparison.Application/Client/Queries/Client/GetClientByDocumentNumberQueryHandler.cs` | modify | Use `CacheKeys.ClientByDocument(...)` |
| `Backend.PriceComparison.Application/Client/Queries/Client/GetAllClientNaturalQueryHandler.cs` | modify | Use `CacheKeys.NaturalPage(...)` |
| `Backend.PriceComparison.Application/Client/Queries/Client/GetAllClientLegalQueryHandler.cs` | modify | Use `CacheKeys.LegalPage(...)` |
| `Backend.PriceComparison.Application/Client/Queries/DocumentType/GetAllDocumentTypeQueryHandler.cs` | modify | Use `CacheKeys.DocumentTypeAll` |
| `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Client/Repositories/ClientRepository.cs` | modify | Replace manual XML on impl methods with `<inheritdoc/>` (already has none — adds them) |
| `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Adapter/Cache/RedisCacheService.cs` | modify | Add `<inheritdoc/>` to public methods |
| `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Client/Repositories/ClientRepository.cs` | modify | Make class `public` (was `internal`) so it is visible from the test project — alternative: add `InternalsVisibleTo`. We pick `InternalsVisibleTo` to preserve current visibility |
| `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Backend.PriceComparison.Infrastructure.Persistence.Mysql.csproj` | modify | Add `<InternalsVisibleTo Include="Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests" />` |
| `Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests.csproj` | create | xUnit test project for the repository |
| `Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/ClientRepositoryTests.cs` | create | Direct integration tests via EF InMemory |
| `Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/CacheKeysTests.cs` | create | Contract tests pinning the cache-key shape |
| `backend-price-comparison.sln` | modify | Add the new test project |

---

## Task 1 — CVE: bump AutoMapper

**Files:**
- Modify: `Backend.PriceComparison.Application/Backend.PriceComparison.Application.csproj`

- [ ] **Step 1.1: Confirm the advisory still applies**

Run:
```powershell
dotnet list backend-price-comparison.sln package --vulnerable --include-transitive
```
Expected output contains a line referencing `AutoMapper 13.0.1` and advisory `GHSA-rvv3-g6hj-g44x`.

- [ ] **Step 1.2: Try the safest bump first — latest patched 13.x**

Run:
```powershell
dotnet add Backend.PriceComparison.Application/Backend.PriceComparison.Application.csproj package AutoMapper --version 13.0.2
```
Expected: package resolves, project file shows `<PackageReference Include="AutoMapper" Version="13.0.2" />`.

- [ ] **Step 1.3: Re-check vulnerabilities**

Run:
```powershell
dotnet list backend-price-comparison.sln package --vulnerable --include-transitive
```
- If `AutoMapper` is no longer flagged → continue to Step 1.5.
- If it is still flagged → continue to Step 1.4 (must move to 14.x and absorb breaking changes).

- [ ] **Step 1.4 (only if 13.0.2 is still flagged): bump to 14.x**

Run:
```powershell
dotnet add Backend.PriceComparison.Application/Backend.PriceComparison.Application.csproj package AutoMapper --version 14.0.0
```

AutoMapper 14 dropped the static `Mapper` API and changed how `MapperConfiguration` is constructed. The codebase only uses **instance** `IMapper` injection (`DependencyInjectionService.cs:17-22`), so the impact is contained to that registration. Update it:

```csharp
// Backend.PriceComparison.Application/DependencyInjectionService.cs
// before:
var mapper = new MapperConfiguration(config =>
{
    config.AddProfile(new ClientProfile());
});
services.AddSingleton(mapper.CreateMapper());

// after (AutoMapper 14):
services.AddAutoMapper(cfg => cfg.AddProfile<ClientProfile>(), Assembly.GetExecutingAssembly());
```

- [ ] **Step 1.5: Build the solution**

Run:
```powershell
dotnet build backend-price-comparison.sln -c Debug --nologo
```
Expected: `0 Error(s)` and the `NU1903` warning for AutoMapper is gone.

- [ ] **Step 1.6: Run the test suite**

Run:
```powershell
dotnet test backend-price-comparison.sln -c Debug --nologo --no-build
```
Expected: `Passed: 24, Failed: 0` total across both test assemblies.

- [ ] **Step 1.7: Commit (auto-pushes via /commit-semver convention)**

```powershell
git add Backend.PriceComparison.Application/Backend.PriceComparison.Application.csproj Backend.PriceComparison.Application/DependencyInjectionService.cs
git commit -m "fix(deps): bump AutoMapper to close GHSA-rvv3-g6hj-g44x"
git push origin main
```

---

## Task 2 — Centralize cache keys

Goal: replace the ad-hoc string interpolations with constants/helpers in one file. Pins the contract and lets us write a contract test.

**Files:**
- Create: `Backend.PriceComparison.Application/Common/Constants/CacheKeys.cs`

- [ ] **Step 2.1: Create the constants file**

Write:
```csharp
// Backend.PriceComparison.Application/Common/Constants/CacheKeys.cs
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;

namespace Backend.PriceComparison.Application.Common.Constants;

/// <summary>
/// Single source of truth for cache keys consumed by query handlers.
/// Keep these stable: changing a key invalidates every cached entry on
/// every running instance the next time it is requested.
/// </summary>
public static class CacheKeys
{
    public const string NaturalCollectionPrefix = "clients:natural";
    public const string LegalCollectionPrefix = "clients:legal";
    public const string DocumentTypeAll = "documenttypes:all";

    public static string ClientById(ClientType type, int id)
        => $"client:{type}:{id}";

    public static string ClientByDocument(ClientType type, string documentNumber)
        => $"client:{type}:document:{documentNumber}";

    public static string NaturalPage(int pageNumber, int pageSize)
        => $"{NaturalCollectionPrefix}:page:{pageNumber}:size:{pageSize}";

    public static string LegalPage(int pageNumber, int pageSize)
        => $"{LegalCollectionPrefix}:page:{pageNumber}:size:{pageSize}";
}
```

- [ ] **Step 2.2: Build to confirm the file compiles**

Run:
```powershell
dotnet build Backend.PriceComparison.Application/Backend.PriceComparison.Application.csproj -c Debug --nologo
```
Expected: `0 Error(s)`.

- [ ] **Step 2.3: Wire it into each handler**

Update the six handlers and two commands to import the constants and call them. Show each replacement explicitly so the engineer doesn't grep blind:

**`CreateClientNaturalPosHandle.cs`** — replace `"clients:natural"` with `CacheKeys.NaturalCollectionPrefix`. Add `using Backend.PriceComparison.Application.Common.Constants;` at the top.

**`CreateClientLegalPosHandle.cs`** — same with `"clients:legal"` → `CacheKeys.LegalCollectionPrefix`.

**`GetClientByIdQueryHandler.cs`** — replace
```csharp
var cacheKey = $"client:{request.Type}:{request.Id}";
```
with
```csharp
var cacheKey = CacheKeys.ClientById(request.Type, request.Id);
```

**`GetClientByDocumentNumberQueryHandler.cs`** — replace
```csharp
var cacheKey = $"client:{request.Type}:document:{request.DocumentNumber}";
```
with
```csharp
var cacheKey = CacheKeys.ClientByDocument(request.Type, request.DocumentNumber);
```

**`GetAllClientNaturalQueryHandler.cs`** — replace
```csharp
var cacheKey = $"clients:natural:page:{request.PageNumber}:size:{request.PageSize}";
```
with
```csharp
var cacheKey = CacheKeys.NaturalPage(request.PageNumber, request.PageSize);
```

**`GetAllClientLegalQueryHandler.cs`** — replace
```csharp
var cacheKey = $"clients:legal:page:{request.PageNumber}:size:{request.PageSize}";
```
with
```csharp
var cacheKey = CacheKeys.LegalPage(request.PageNumber, request.PageSize);
```

**`GetAllDocumentTypeQueryHandler.cs`** — replace
```csharp
private const string CacheKey = "documenttypes:all";
```
with `using Backend.PriceComparison.Application.Common.Constants;` and use `CacheKeys.DocumentTypeAll` everywhere `CacheKey` was referenced. Delete the local constant.

- [ ] **Step 2.4: Build**

Run:
```powershell
dotnet build backend-price-comparison.sln -c Debug --nologo
```
Expected: `0 Error(s)`.

- [ ] **Step 2.5: Run tests**

Run:
```powershell
dotnet test backend-price-comparison.sln -c Debug --nologo --no-build
```
Expected: 24/24 still passing — the keys produce the same strings.

- [ ] **Step 2.6: Commit**

```powershell
git add Backend.PriceComparison.Application/
git commit -m "refactor(application): centralize cache keys in CacheKeys constants"
git push origin main
```

---

## Task 3 — `DocumentTypeEntity → DocumentTypeDto` mapping in profile

**Files:**
- Modify: `Backend.PriceComparison.Application/Client/Mappers/ClientProfile.cs`
- Modify: `Backend.PriceComparison.Application/Client/Queries/DocumentType/GetAllDocumentTypeQueryHandler.cs`

- [ ] **Step 3.1: Add the map to the profile**

Append inside `ClientProfile()` constructor:
```csharp
CreateMap<DocumentTypeEntity, DocumentTypeDto>();
```
(All property names match by convention, no `ForMember` needed.)

- [ ] **Step 3.2: Inject `IMapper` into the DocumentType handler**

The handler currently takes only `IDocumentTypeRepository` and `ICacheService`. Add `IMapper`:

```csharp
public class GetAllDocumentTypeQueryHandler(
    IDocumentTypeRepository _documentTypeRepository,
    ICacheService _cacheService,
    IMapper _mapper)
    : IRequestHandler<GetAllDocumentTypeQuery, Result<ApiResponseDto<IEnumerable<DocumentTypeDto>>, Error>>
```
Add `using AutoMapper;` at the top.

- [ ] **Step 3.3: Replace the manual mapping**

```csharp
// before
var dtos = result.Value!.Select(entity => new DocumentTypeDto
{
    Id = entity.Id,
    Name = entity.Name,
    DocumentType = entity.DocumentType,
    HelpTextHeader = entity.HelpTextHeader,
    HelpText = entity.HelpText,
    Regex = entity.Regex,
    Fields = entity.Fields
}).ToList();

// after
var dtos = _mapper.Map<IEnumerable<DocumentTypeDto>>(result.Value!).ToList();
```

- [ ] **Step 3.4: Build**

Run:
```powershell
dotnet build backend-price-comparison.sln -c Debug --nologo
```
Expected: `0 Error(s)`.

- [ ] **Step 3.5: Run tests**

Run:
```powershell
dotnet test backend-price-comparison.sln -c Debug --nologo --no-build
```
Expected: 24/24 passing.

- [ ] **Step 3.6: Smoke test in container**

Run (PowerShell):
```powershell
docker stop backend-api-eds-client; docker rm backend-api-eds-client
docker build -t backend-api-eds-client:local .
docker run -d --name backend-api-eds-client -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development -e UseMockInfrastructure=true backend-api-eds-client:local
Start-Sleep -Seconds 3
curl -H "Authorization: Bearer t" "http://localhost:8080/api/v1/client/document-type"
```
Expected: HTTP 200, three document types (CC / CE / NIT) in the response.

- [ ] **Step 3.7: Commit**

```powershell
git add Backend.PriceComparison.Application/Client/Mappers/ClientProfile.cs Backend.PriceComparison.Application/Client/Queries/DocumentType/GetAllDocumentTypeQueryHandler.cs
git commit -m "refactor(application): map DocumentType via AutoMapper profile"
git push origin main
```

---

## Task 4 — `<inheritdoc/>` on adapter implementations

The interfaces in `Backend.PriceComparison.Domain/Ports/` already have full XML docs (committed earlier in the session). The adapter implementations don't, so IntelliSense on a concrete instance shows nothing. Adding `<inheritdoc/>` propagates the docs.

**Files:**
- Modify: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Client/Repositories/ClientRepository.cs`
- Modify: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Adapter/Cache/RedisCacheService.cs`
- Modify: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Client/Repositories/DocumentTypeRepository.cs`
- Modify: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Adapter/MessageProvider.cs`
- Modify: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Mock/MockClientRepository.cs`
- Modify: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Mock/MockDocumentTypeRepository.cs`
- Modify: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Mock/InMemoryCacheService.cs`

- [ ] **Step 4.1: Add `<inheritdoc/>` above each public/explicit interface member**

Pattern — for each method (and each property in `MessageProvider`/`InMemoryCacheService`) place exactly:
```csharp
/// <inheritdoc/>
public async Task<Result<VoidResult, Error>> CreateClientLegalAsync(...)
```

Do this for every member that comes from an interface in `Backend.PriceComparison.Domain.Ports`. Do **not** add `<inheritdoc/>` to private helpers or to constructors.

- [ ] **Step 4.2: Build**

Run:
```powershell
dotnet build backend-price-comparison.sln -c Debug --nologo
```
Expected: `0 Error(s)` and `0 Warning(s)` from the new docs (no `CS1591` introduced because the interfaces already document the members).

- [ ] **Step 4.3: Commit**

```powershell
git add Backend.PriceComparison.Infrastructure.Persistence.Mysql/
git commit -m "docs(persistence): inherit XML docs on port implementations"
git push origin main
```

---

## Task 5 — Test project for `Backend.PriceComparison.Infrastructure.Persistence.Mysql`

We need a test project + EF InMemory + visibility. Tests will live separately from the existing `Domain.Test` to keep the dependency graph clean (Domain.Test must not see EF Core).

### Subtask 5.1 — Create the test project

**Files:**
- Create: `Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests.csproj`
- Modify: `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Backend.PriceComparison.Infrastructure.Persistence.Mysql.csproj`
- Modify: `backend-price-comparison.sln`

- [ ] **Step 5.1.1: Generate the project**

Run:
```powershell
dotnet new xunit -n Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests -f net10.0
```

- [ ] **Step 5.1.2: Add the project reference and EF InMemory**

Run:
```powershell
dotnet add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests reference Backend.PriceComparison.Infrastructure.Persistence.Mysql/Backend.PriceComparison.Infrastructure.Persistence.Mysql.csproj
dotnet add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests reference Backend.PriceComparison.Domain/Backend.PriceComparison.Domain.csproj
dotnet add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests package Microsoft.EntityFrameworkCore.InMemory --version 9.0.0
dotnet add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests package Microsoft.Extensions.Logging.Abstractions --version 10.0.0
```

- [ ] **Step 5.1.3: Make the repository visible to the test project**

Edit `Backend.PriceComparison.Infrastructure.Persistence.Mysql/Backend.PriceComparison.Infrastructure.Persistence.Mysql.csproj` and add inside the existing `<Project>` element:

```xml
<ItemGroup>
  <InternalsVisibleTo Include="Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests" />
</ItemGroup>
```

- [ ] **Step 5.1.4: Add the project to the solution**

Run:
```powershell
dotnet sln backend-price-comparison.sln add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests.csproj
```

- [ ] **Step 5.1.5: Verify the empty project builds**

Run:
```powershell
dotnet build backend-price-comparison.sln -c Debug --nologo
```
Expected: `0 Error(s)`. The new `*.Tests` assembly appears under `bin/Debug/net10.0/`.

- [ ] **Step 5.1.6: Commit the scaffolding**

```powershell
git add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/ Backend.PriceComparison.Infrastructure.Persistence.Mysql/Backend.PriceComparison.Infrastructure.Persistence.Mysql.csproj backend-price-comparison.sln
git commit -m "chore(test): scaffold Persistence.Mysql.Tests with EF InMemory"
git push origin main
```

### Subtask 5.2 — Test fixture for `ClientRepository`

**Files:**
- Create: `Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/ClientRepositoryTests.cs`

- [ ] **Step 5.2.1: Write the fixture and one happy-path GetById test (TDD red)**

Replace the autogenerated `UnitTest1.cs` with `ClientRepositoryTests.cs`:

```csharp
using Backend.PriceComparison.Domain.ClientPos.Entities;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Client.Repositories;
using Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests;

public class ClientRepositoryTests
{
    private static (ClientRepository repo, ClientDbContext db) NewSut()
    {
        var options = new DbContextOptionsBuilder<ClientDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var db = new ClientDbContext(options);
        var repo = new ClientRepository(db, NullLogger<ClientRepository>.Instance);
        return (repo, db);
    }

    [Fact]
    public async Task GetByIdAsync_NaturalClientExists_ReturnsClient()
    {
        var (repo, db) = NewSut();
        db.ClientNaturalPos.Add(new ClientNaturalPosEntity
        {
            Id = 1,
            DocumentNumber = "12345",
            Name = "Ana",
            DocumentTypeId = 1
        });
        await db.SaveChangesAsync();

        var result = await repo.GetByIdAsync(1, ClientType.Natural, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("12345", result.Value!.DocumentNumber);
    }
}
```

- [ ] **Step 5.2.2: Run only this test class to confirm it passes**

Run:
```powershell
dotnet test Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests.csproj -c Debug --nologo --filter "FullyQualifiedName~ClientRepositoryTests"
```
Expected: `Passed: 1, Failed: 0`.

If the test fails because `ClientDbContext` requires the model configuration that ignores `DocumentCountry`, the InMemory provider will surface it. In that case, ensure the entity-side `DocumentCountry` is set to `null` before saving (it is nullable).

- [ ] **Step 5.2.3: Commit**

```powershell
git add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/
git commit -m "test(persistence): cover ClientRepository.GetByIdAsync happy path"
git push origin main
```

### Subtask 5.3 — Round out repository coverage

Add the remaining tests, one commit per group so the diff stays reviewable.

- [ ] **Step 5.3.1: GetByIdAsync — not-found returns ClientNotFound error**

Append to `ClientRepositoryTests.cs`:

```csharp
[Fact]
public async Task GetByIdAsync_NaturalClientMissing_ReturnsNotFound()
{
    var (repo, _) = NewSut();

    var result = await repo.GetByIdAsync(999, ClientType.Natural, CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.Error);
}

[Fact]
public async Task GetByIdAsync_QueryingWrongTypeReturnsNotFound()
{
    var (repo, db) = NewSut();
    db.ClientLegalPos.Add(new ClientLegalPosEntity
    {
        Id = 1,
        DocumentNumber = "900",
        CompanyName = "Acme",
        DocumentTypeId = 3
    });
    await db.SaveChangesAsync();

    var result = await repo.GetByIdAsync(1, ClientType.Natural, CancellationToken.None);

    Assert.False(result.IsSuccess);
}
```

Run:
```powershell
dotnet test Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests -c Debug --nologo --filter "FullyQualifiedName~ClientRepositoryTests"
```
Expected: `Passed: 3`.

Commit:
```powershell
git add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/ClientRepositoryTests.cs
git commit -m "test(persistence): cover ClientRepository.GetByIdAsync error paths"
git push origin main
```

- [ ] **Step 5.3.2: GetByDocumentNumberAsync — happy + not-found**

Append:

```csharp
[Fact]
public async Task GetByDocumentNumberAsync_LegalClientExists_ReturnsClient()
{
    var (repo, db) = NewSut();
    db.ClientLegalPos.Add(new ClientLegalPosEntity
    {
        Id = 7,
        DocumentNumber = "900111",
        CompanyName = "Acme",
        DocumentTypeId = 3
    });
    await db.SaveChangesAsync();

    var result = await repo.GetByDocumentNumberAsync("900111", ClientType.Legal, CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal(7, result.Value!.Id);
}

[Fact]
public async Task GetByDocumentNumberAsync_Missing_ReturnsNotFound()
{
    var (repo, _) = NewSut();

    var result = await repo.GetByDocumentNumberAsync("does-not-exist", ClientType.Natural, CancellationToken.None);

    Assert.False(result.IsSuccess);
}
```

Run + commit:
```powershell
dotnet test Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests -c Debug --nologo
git add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/ClientRepositoryTests.cs
git commit -m "test(persistence): cover GetByDocumentNumberAsync"
git push origin main
```

- [ ] **Step 5.3.3: GetAll[Natural|Legal]Async — pagination + empty**

Append:

```csharp
[Fact]
public async Task GetAllNaturalAsync_ReturnsRequestedPage()
{
    var (repo, db) = NewSut();
    for (var i = 1; i <= 25; i++)
    {
        db.ClientNaturalPos.Add(new ClientNaturalPosEntity
        {
            Id = i,
            DocumentNumber = i.ToString(),
            Name = $"User{i}",
            DocumentTypeId = 1
        });
    }
    await db.SaveChangesAsync();

    var result = await repo.GetAllNaturalAsync(pageNumber: 2, pageSize: 10, CancellationToken.None);

    Assert.True(result.IsSuccess);
    var items = result.Value!.ToList();
    Assert.Equal(10, items.Count);
    Assert.Equal("11", items.First().DocumentNumber);
}

[Fact]
public async Task GetAllLegalAsync_NoRecords_ReturnsError()
{
    var (repo, _) = NewSut();

    var result = await repo.GetAllLegalAsync(1, 10, CancellationToken.None);

    Assert.False(result.IsSuccess);
}
```

Run + commit:
```powershell
dotnet test Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests -c Debug --nologo
git add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/ClientRepositoryTests.cs
git commit -m "test(persistence): cover GetAll[Natural|Legal]Async pagination"
git push origin main
```

- [ ] **Step 5.3.4: Create[Natural|Legal] — persist and idempotency on second save**

Append:

```csharp
[Fact]
public async Task CreateClientNaturalAsync_PersistsEntity()
{
    var (repo, db) = NewSut();
    var entity = new ClientNaturalPosEntity
    {
        DocumentNumber = "55",
        Name = "Lu",
        DocumentTypeId = 1
    };

    var result = await repo.CreateClientNaturalAsync(entity, CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal(1, await db.ClientNaturalPos.CountAsync());
}

[Fact]
public async Task CreateClientLegalAsync_PersistsEntity()
{
    var (repo, db) = NewSut();
    var entity = new ClientLegalPosEntity
    {
        DocumentNumber = "900",
        CompanyName = "Acme",
        DocumentTypeId = 3
    };

    var result = await repo.CreateClientLegalAsync(entity, CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal(1, await db.ClientLegalPos.CountAsync());
}
```

Run + commit:
```powershell
dotnet test Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests -c Debug --nologo
git add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/ClientRepositoryTests.cs
git commit -m "test(persistence): cover Create[Natural|Legal] persistence"
git push origin main
```

- [ ] **Step 5.3.5: Verify the global suite still green**

Run:
```powershell
dotnet test backend-price-comparison.sln -c Debug --nologo
```
Expected: a single line per assembly, all `Passed!`. Total tests should now read `24 + 9 = 33` passing, 0 failing.

---

## Task 6 — Cache key contract test

Pin the public shape of every key so an accidental rename of `ClientType.Natural` (or anyone "fixing" formatting) breaks the test instead of silently invalidating production caches.

**Files:**
- Create: `Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/CacheKeysTests.cs` (lives here so it has access to the application reference; if you'd rather, put it under a new `Application.Tests` project — single-test-project for now keeps the diff small)
- Modify: `Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests.csproj`

- [ ] **Step 6.1: Add reference to the Application project from the test project**

Run:
```powershell
dotnet add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests reference Backend.PriceComparison.Application/Backend.PriceComparison.Application.csproj
```

- [ ] **Step 6.2: Write the contract tests**

```csharp
// Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/CacheKeysTests.cs
using Backend.PriceComparison.Application.Common.Constants;
using Backend.PriceComparison.Domain.ClientPos.Models.Enums;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests;

public class CacheKeysTests
{
    [Fact]
    public void NaturalCollectionPrefix_IsStable()
        => Assert.Equal("clients:natural", CacheKeys.NaturalCollectionPrefix);

    [Fact]
    public void LegalCollectionPrefix_IsStable()
        => Assert.Equal("clients:legal", CacheKeys.LegalCollectionPrefix);

    [Fact]
    public void DocumentTypeAll_IsStable()
        => Assert.Equal("documenttypes:all", CacheKeys.DocumentTypeAll);

    [Theory]
    [InlineData(ClientType.Natural, 5, "client:Natural:5")]
    [InlineData(ClientType.Legal, 42, "client:Legal:42")]
    public void ClientById_FormatsAsExpected(ClientType type, int id, string expected)
        => Assert.Equal(expected, CacheKeys.ClientById(type, id));

    [Theory]
    [InlineData(ClientType.Natural, "123", "client:Natural:document:123")]
    [InlineData(ClientType.Legal, "900", "client:Legal:document:900")]
    public void ClientByDocument_FormatsAsExpected(ClientType type, string doc, string expected)
        => Assert.Equal(expected, CacheKeys.ClientByDocument(type, doc));

    [Fact]
    public void NaturalPage_FormatsAsExpected()
        => Assert.Equal("clients:natural:page:1:size:10", CacheKeys.NaturalPage(1, 10));

    [Fact]
    public void LegalPage_FormatsAsExpected()
        => Assert.Equal("clients:legal:page:2:size:25", CacheKeys.LegalPage(2, 25));
}
```

- [ ] **Step 6.3: Run only this test class**

Run:
```powershell
dotnet test Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests -c Debug --nologo --filter "FullyQualifiedName~CacheKeysTests"
```
Expected: `Passed: 9` (3 facts + 4 theory rows + 2 page facts).

- [ ] **Step 6.4: Commit**

```powershell
git add Backend.PriceComparison.Infrastructure.Persistence.Mysql.Tests/
git commit -m "test(application): pin CacheKeys contract"
git push origin main
```

---

## Task 7 — Final verification

- [ ] **Step 7.1: Build everything fresh**

Run:
```powershell
dotnet build backend-price-comparison.sln -c Release --nologo
```
Expected: `0 Error(s)`. CVE warning gone.

- [ ] **Step 7.2: Run every test**

Run:
```powershell
dotnet test backend-price-comparison.sln -c Release --nologo
```
Expected: total `33+ Passed, 0 Failed` across three assemblies (Domain.Test, Api.Tests, Persistence.Mysql.Tests).

- [ ] **Step 7.3: Smoke test the container**

```powershell
docker stop backend-api-eds-client; docker rm backend-api-eds-client
docker build -t backend-api-eds-client:local .
docker run -d --name backend-api-eds-client -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development -e UseMockInfrastructure=true backend-api-eds-client:local
Start-Sleep -Seconds 3
curl http://localhost:8080/health/live
curl -H "Authorization: Bearer t" "http://localhost:8080/api/v1/client/document-type"
curl -H "Authorization: Bearer t" "http://localhost:8080/api/v1/client/natural?pageNumber=1&pageSize=10"
```
Expected: each call returns HTTP 200 with the expected mock payload.

- [ ] **Step 7.4: Tag the release**

The accumulated work in this plan is a `fix` (CVE) plus several `refactor`/`test`/`docs` commits — net effect a PATCH bump.

```powershell
git tag -a v0.1.2 -m "Release 0.1.2 - hygiene & CVE sprint"
git push origin v0.1.2
```

---

## Self-Review Checklist (run before executing)

**Spec coverage:**
- [x] AutoMapper CVE → Task 1
- [x] Direct ClientRepository tests → Task 5
- [x] `<inheritdoc/>` on impls → Task 4
- [x] DocumentType profile mapping → Task 3
- [x] Cache-key contract documented + tested → Tasks 2 and 6

**Placeholder scan:** none of `TBD`, `implement later`, "fill in details" appear in this plan.

**Type consistency:**
- `CacheKeys.ClientById(ClientType type, int id)` is referenced from `GetClientByIdQueryHandler` in Task 2 and pinned in Task 6. ✓
- `CacheKeys.ClientByDocument(ClientType type, string documentNumber)` matches Task 2 and Task 6. ✓
- `CacheKeys.NaturalCollectionPrefix` / `LegalCollectionPrefix` referenced from create handlers and pinned in Task 6. ✓
- `ClientRepository(ClientDbContext, ILogger<ClientRepository>)` constructor matches the live source. ✓
- `Microsoft.EntityFrameworkCore.InMemory 9.0.0` aligns with EF Core 9.0 already in `Backend.PriceComparison.Infrastructure.Persistence.Mysql.csproj`. ✓
