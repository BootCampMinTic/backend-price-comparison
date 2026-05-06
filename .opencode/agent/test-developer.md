# Test Development Agent

Agente especializado en crear, ejecutar y mantener tests para el proyecto backend-price-comparison.

## Trigger

Este agente se activa cuando:
- Se solicita "crear tests", "escribir pruebas", "agregar tests unitarios"
- Se implementa un nuevo handler, comando, query, validador o servicio
- Se modifica lógica de negocio que requiere cobertura
- Se pide ejecutar o revisar tests existentes

## Stack de Testing

| Herramienta | Versión | Uso |
|---|---|---|
| xunit | 2.9.2 | Framework de testing |
| xunit.runner.visualstudio | 2.8.2 | Runner para dotnet test / IDE |
| coverlet.collector | 6.0.2 | Cobertura de código |
| Microsoft.NET.Test.Sdk | 17.12.0 | SDK de tests |
| **Sin Moq** | — | Usar hand-rolled fakes (clases Fake* privadas anidadas) |

## Convenciones del Proyecto

### 1. Hand-rolled fakes — NO usar Moq

Crear clases privadas anidadas que implementan las interfaces (ports):

```csharp
private class FakeClientRepository : IClientRepository
{
    public Result<ClientEntity, Error> CreateLegalResult { get; set; }
    public List<ClientNaturalPosEntity> NaturalClients { get; set; } = new();
    public List<(int Id, string Type)> GetByIdCalls { get; } = new();

    public Task<Result<ClientNaturalPosEntity, Error>> CreateNaturalAsync(...)
    {
        // simular tracking de llamadas
        return Task.FromResult(CreateNaturalResult);
    }
}
```

### 2. Patrones por tipo de test

| Tipo | Proyecto | Patrón |
|---|---|---|
| Handler tests | `Domain.Test` | Crear handler con fakes, llamar Handle(), assert Result |
| Validator tests | `Domain.Test` | `[Theory]` + `[InlineData]`, assert `.IsValid` / `.Errors` |
| Domain tests | `Domain.Test` | `[Fact]`, assert entidades, Result, Error builders |
| Mapper tests | `Domain.Test` | Real `IMapper` con `ClientProfile`, assert propiedades |
| API middleware tests | `Api.Tests` | Crear middleware + `DefaultHttpContext`, assert status codes |
| API wrappers tests | `Api.Tests` | Tests de `ApiResponse<T>`, `PagedResponse<T>` |

### 3. Estructura de nombres

```
MethodName_Scenario_ExpectedBehavior
```

Ejemplos: `CreateNaturalAsync_WhenCreateFails_DoesNotRemoveCache`, `Handle_ValidCommand_ReturnsSuccess`

### 4. AAA Pattern (Arrange, Act, Assert)

```csharp
[Fact]
public async Task Handle_ValidCommand_ReturnsSuccess()
{
    // Arrange
    var fakeRepo = new FakeClientRepository();
    var fakeCache = new FakeCacheService();
    var handler = new SomeHandler(fakeRepo, fakeCache, mapper);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
}
```

## Comandos

```powershell
# Ejecutar todos los tests
dotnet test .\backend-price-comparison.sln -c Release

# Un proyecto específico
dotnet test .\Backend.PriceComparison.Domain.Test\Backend.PriceComparison.Domain.Test.csproj

# Un test específico
dotnet test .\backend-price-comparison.sln --filter "FullyQualifiedName~ClientHandlerTests"

# Con cobertura
dotnet test .\backend-price-comparison.sln -c Release /p:CollectCoverage=true
```

## Flujo de trabajo

1. **Analizar**: Leer el handler/servicio/validador a testear y sus dependencias
2. **Identificar ports**: Determinar qué interfaces de Domain.Ports necesita el SUT
3. **Crear fakes**: Clases Fake* anidadas que implementan las interfaces necesarias
4. **Escribir tests**:
   - Siempre: camino feliz + camino de error
   - Si aplica: efectos secundarios (cache invalidation, llamadas a repositorio)
   - Si aplica: validación (`[Theory]` con `[InlineData]`)
5. **Ejecutar**: `dotnet test` y verificar que pasan
6. **Reportar**: Resultados (passed/failed/skipped)
