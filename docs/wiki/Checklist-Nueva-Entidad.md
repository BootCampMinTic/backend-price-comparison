# Checklist Nueva Entidad

Usa este checklist cada vez que implementes una nueva entidad en el sistema. Marca cada item al completarlo.

## Domain

- [ ] **Entidad** creada en `Domain/{Feature}/Entities/{Entity}Entity.cs`
  - `sealed class`
  - Propiedades con `{ get; set; }`
  - FK como `{Related}Id`
  - Navigation props como `{Related}Entity?`
  - XML doc si aporta valor

- [ ] **Puerto** creado en `Domain/Ports/I{Entity}Repository.cs`
  - `Task<Result<...>>` en cada metodo
  - `CancellationToken` en cada metodo
  - Metodos CRUD necesarios (no añadir de mas)

- [ ] **ErrorBuilder** extendido (si la entidad tiene errores nuevos)
  - `NotFound(id, entity)`, `NoRecordsFound(entity)`, `CreationFailed(entity)`

## Application

- [ ] **DTO** creado en `Application/{Feature}/Dtos/{Entity}Dto.cs`
  - Props aplanadas: `StoreName` no `Store.Name`
  - Excluye `Password` y campos sensibles

- [ ] **Query GetAll** + **Handler**
  - Cache-aside con key correcta
  - `AsNoTracking()` en repo (no aca, es responsabilidad del repo)

- [ ] **Query GetById** + **Handler** (si aplica)
  - Cache key individual

- [ ] **Command Create** + **Handler** + **Validator**
  - Mapea command → entity (ignorando `Id` y nav props)
  - Invalida cache prefix en exito
  - Validator con reglas de negocio minimas

- [ ] **AutoMapper Profile** extendido
  - Entity → DTO (con `.ForMember` para FK aplanadas)
  - Command → Entity (con `.Ignore()` para `Id` y nav props)
  - Registrado en `Application/DependencyInjectionService.cs`

- [ ] **CacheKeys** extendido
  - Constantes y metodos estaticos para cada patron de clave

## Infrastructure

- [ ] **Repositorio** en `Infrastructure/{Feature}/Repositories/{Entity}Repository.cs`
  - `internal sealed class`
  - Implementa `I{Entity}Repository`
  - `AsNoTracking()` en reads
  - `Include()` para FKs
  - `LogDebug`/`LogWarning`/`LogInformation`
  - Retorna `VoidResult` o `ErrorBuilder`

- [ ] **Mock Repository** en `Infrastructure/Mock/Mock{Entity}Repository.cs`
  - Datos hardcoded realistas
  - Navigation props populadas
  - `CreateAsync` asigna ID

- [ ] **DbSet** en `ClientDbContext`
  - `public DbSet<{Entity}Entity> {Entity}s { get; set; }`

- [ ] **Mapeo EF** en `StoreEntityConfiguration()`
  - `.ToTable("tabla_mysql")`
  - `.HasColumnName("columna_mysql")` para cada propiedad
  - `.HasOne().WithMany().HasForeignKey()` para cada FK

- [ ] **DI real** en `DependencyInjectionService.AddPersistence()`
  - `services.AddScoped<I{Entity}Repository, {Entity}Repository>();`

- [ ] **DI mock** en el path `UseMockInfrastructure=true`
  - `services.AddScoped<I{Entity}Repository, Mock{Entity}Repository>();`

## API

- [ ] **Endpoints** en `Api/Endpoints/{Feature}Endpoints.cs`
  - Clase `static` con metodo `Map{Feature}Endpoints(this IEndpointRouteBuilder app)`
  - `MapGroup("api/v1").WithTags("{Tag}")`
  - `WithName` + `WithSummary`
  - `Produces<T>` correcto

- [ ] **Program.cs** actualizado
  - `app.Map{Feature}Endpoints();`

## Cross-cutting

- [ ] Build exitoso: `dotnet build .\backend-price-comparison.sln -c Debug`
- [ ] Mock mode funciona: `UseMockInfrastructure=true`
- [ ] Cache invalidado correctamente en writes
- [ ] Scalar UI muestra los nuevos endpoints
