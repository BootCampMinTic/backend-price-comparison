# backend-api-eds-client

API de comparacion de precios. El proyecto expone endpoints HTTP para crear y consultar clientes naturales, clientes juridicos y tipos de documento, usando ASP.NET Core Minimal APIs, MediatR, Entity Framework Core, MySQL y Redis.

## Contenido

- [Arquitectura](#arquitectura)
- [Tecnologias principales](#tecnologias-principales)
- [Requisitos](#requisitos)
- [Configuracion](#configuracion)
- [Ejecucion local](#ejecucion-local)
- [Documentacion de la API](#documentacion-de-la-api)
- [Endpoints principales](#endpoints-principales)
- [Health checks](#health-checks)
- [Pruebas](#pruebas)
- [Docker](#docker)
- [CI/CD](#cicd)
- [Notas de mantenimiento](#notas-de-mantenimiento)

## Arquitectura

La solucion principal es `backend-price-comparison.sln` y esta organizada por capas:

| Proyecto | Responsabilidad |
| --- | --- |
| `Backend.PriceComparison.Api` | Punto de entrada HTTP. Configura Minimal APIs, CORS, Scalar/OpenAPI, Redis, health checks y middleware de token Bearer. |
| `Backend.PriceComparison.Application` | Casos de uso, comandos, queries, servicios de aplicacion, MediatR, AutoMapper y validaciones. |
| `Backend.PriceComparison.Domain` | Entidades, contratos de dominio, modelos comunes y resultados. |
| `Backend.PriceComparison.Infraestructure.Persistence.Mysql` | Persistencia MySQL con Entity Framework Core y servicios de dominio concretos. |
| `Backend.PriceComparison.Api.Tests` | Proyecto de pruebas xUnit para la API. |
| `Backend.PriceComparison.Domain.Test` | Proyecto de pruebas xUnit para dominio. |

Tambien existen carpetas auxiliares como `LoadTest`, `WorkerServiceBilling`, `Backend.PriceComparison.Common`, `Backend.PriceComparison.Infraestructure.External.Plemsi` y `Backend.PriceComparison.Infraestructure.External.TNS`. No todas estan incluidas en la solucion principal.

## Tecnologias principales

- .NET `10.0` para la API y los proyectos principales de cliente.
- ASP.NET Core Minimal APIs.
- Entity Framework Core `9.0` con `Pomelo.EntityFrameworkCore.MySql`.
- MySQL como base de datos.
- Redis con `StackExchange.Redis`.
- MediatR para comandos y queries.
- AutoMapper para mapeos entre comandos, entidades y DTOs.
- FluentValidation para el pipeline de validacion.
- Scalar para visualizar la documentacion OpenAPI.
- xUnit y coverlet para pruebas.
- Docker y GitHub Actions para build/deploy.

## Requisitos

- .NET SDK `10.0.x`.
- MySQL accesible desde el entorno local o contenedor.
- Redis accesible desde el entorno local o contenedor.
- Docker, opcional para ejecutar en contenedor.

## Configuracion

La API toma configuracion desde `appsettings.json`, variables de entorno y `appsettings.Development.json`.

> Importante: no publiques credenciales reales en el README, issues, logs o commits. Para desarrollo local, usa variables de entorno, User Secrets o un archivo local no versionado.

Variables y claves relevantes:

| Variable/clave | Uso |
| --- | --- |
| `MYSQL_CONNECTION` | Sobrescribe `ConnectionStrings:MysqlConnection`. Cadena de conexion a MySQL usada por EF Core. |
| `REDIS_CONNECTION` | Sobrescribe `Redis:ConnectionString`. Conexion a Redis. |
| `ConnectionStrings:MysqlConnection` | Cadena de conexion MySQL por configuracion. |
| `Redis:ConnectionString` | Conexion Redis por configuracion. |
| `Redis:CacheExpirationMinutes` | Tiempo por defecto de expiracion del cache. |
| `AllowedOrigins` | Origenes permitidos por CORS. |
| `ApiPlemsi:ApiKey` | Clave de integracion Plemsi, si aplica. |
| `ApiPlemsi:PosUrl` | URL POS de Plemsi, si aplica. |
| `ApiPlemsi:ApiUrl` | URL base/consulta de Plemsi, si aplica. |

Ejemplo en PowerShell:

```powershell
$env:MYSQL_CONNECTION="Server=localhost;Port=3306;Database=clients;User Id=root;Password=local_password;ConvertZeroDateTime=True;SslMode=Disabled"
$env:REDIS_CONNECTION="localhost:6379"
$env:ASPNETCORE_ENVIRONMENT="Development"
```

## Ejecucion local

Restaura dependencias:

```powershell
dotnet restore .\backend-price-comparison.sln
```

Compila la solucion:

```powershell
dotnet build .\backend-price-comparison.sln -c Debug
```

Ejecuta la API:

```powershell
dotnet run --project .\Backend.PriceComparison.Api\Backend.PriceComparison.Api.csproj --launch-profile http
```

Con el perfil `http`, la API queda publicada en:

- `http://localhost:5062`
- `https://localhost:5000`

## Documentacion de la API

La documentacion interactiva esta disponible con Scalar:

```text
http://localhost:5062/scalar/v1
```

El documento OpenAPI se expone mediante `MapOpenApi`:

```text
http://localhost:5062/openapi/v1.json
```

## Autenticacion

La API usa `BearerTokenMiddleware`. Todos los endpoints no publicos requieren el header:

```http
Authorization: Bearer <token>
```

Endpoints publicos:

- `/health`
- `/health/ready`
- `/health/live`
- `/api/v1/health`
- `/openapi`
- `/scalar`
- recursos estaticos usados por la UI

El middleware actual valida la presencia del token Bearer; no valida firma, expiracion ni claims.

## Endpoints principales

Base path:

```text
/api/v1
```

| Metodo | Ruta | Descripcion |
| --- | --- | --- |
| `POST` | `/client/natural` | Crea un cliente natural. |
| `POST` | `/client/legal` | Crea un cliente juridico. |
| `GET` | `/client/natural?pageNumber=1&pageSize=10` | Lista clientes naturales paginados. |
| `GET` | `/client/legal?pageNumber=1&pageSize=10` | Lista clientes juridicos paginados. |
| `GET` | `/client/natural/{id}` | Consulta cliente natural por id. |
| `GET` | `/client/legal/{id}` | Consulta cliente juridico por id. |
| `GET` | `/client/natural/{number}/document-number` | Consulta cliente natural por numero de documento. |
| `GET` | `/client/legal/{number}/document-number` | Consulta cliente juridico por numero de documento. |
| `GET` | `/client/document-type` | Lista tipos de documento. |

Ejemplo de consulta:

```bash
curl -H "Authorization: Bearer dev-token" "http://localhost:5062/api/v1/client/natural?pageNumber=1&pageSize=10"
```

Ejemplo de creacion de cliente natural:

```json
{
  "name": "Juan",
  "middleName": "Carlos",
  "lastName": "Perez",
  "secondSurname": "Gomez",
  "documentNumber": "123456789",
  "electronicInvoiceEmail": "cliente@example.com",
  "documentTypeId": 1,
  "documentCountry": "CO"
}
```

Ejemplo de creacion de cliente juridico:

```json
{
  "companyName": "Empresa Demo SAS",
  "verificationDigit": 5,
  "documentNumber": "900123456",
  "electronicInvoiceEmail": "facturacion@example.com",
  "vatResponsibleParty": true,
  "selfRetainer": false,
  "withholdingAgent": false,
  "simpleTaxRegime": false,
  "documentTypeId": 1,
  "largeTaxpayer": false,
  "documentCountry": "CO"
}
```

## Health checks

La API configura health checks de aplicacion, base de datos y Entity Framework.

| Metodo | Ruta | Uso |
| --- | --- | --- |
| `GET` | `/health` | Estado completo y detallado. |
| `GET` | `/health/ready` | Readiness, incluye dependencias como base de datos. |
| `GET` | `/health/live` | Liveness de la aplicacion. |
| `GET` | `/api/v1/health` | Estado completo mediante endpoint versionado. |
| `GET` | `/api/v1/health/ready` | Readiness versionado. |
| `GET` | `/api/v1/health/live` | Liveness versionado. |

Ejemplo:

```bash
curl -i http://localhost:5062/health/ready
```

Hay documentacion adicional en:

- `Backend.PriceComparison.Api/HealthChecks/README.md`
- `Backend.PriceComparison.Api/HealthChecks/TESTING.md`

## Pruebas

Ejecuta todas las pruebas de la solucion:

```powershell
dotnet test .\backend-price-comparison.sln -c Release
```

Los proyectos de pruebas actuales usan xUnit:

- `Backend.PriceComparison.Api.Tests`
- `Backend.PriceComparison.Domain.Test`

## Docker

Construye la imagen de la API:

```powershell
docker build -t backend-api-eds-client .
```

Ejecuta el contenedor:

```powershell
docker run --rm -p 8080:8080 `
  -e ASPNETCORE_URLS="http://+:8080" `
  -e MYSQL_CONNECTION="Server=host.docker.internal;Port=3306;Database=clients;User Id=root;Password=local_password;ConvertZeroDateTime=True;SslMode=Disabled" `
  -e REDIS_CONNECTION="host.docker.internal:6379" `
  backend-api-eds-client
```

Luego valida:

```text
http://localhost:8080/health/live
```

## CI/CD

El flujo principal esta en `.github/workflows/deploy-client-api.yml`.

Resumen:

- Restaura, compila y ejecuta pruebas con .NET `10.0.x`.
- Construye una imagen Docker.
- Publica la imagen en Amazon ECR.
- Despliega el servicio de desarrollo en AWS ECS.

El workflow se ejecuta en pushes a ramas configuradas y pull requests hacia `main`, `release/*` o `releasecandidate/*`.

## Notas de mantenimiento

- `DocumentCountry` existe en los comandos y entidades, pero `ClientDbContext` lo ignora temporalmente para `ClientNaturalPosEntity` y `ClientLegalPosEntity`. Hasta que la base de datos tenga esas columnas, el valor puede recibirse por API pero no se persiste.
- `WorkerServiceBilling` y algunos proyectos externos contienen referencias a namespaces/proyectos `Backend.PriceComparison.*`. Revisa esas referencias antes de agregarlos a la solucion principal o al pipeline.
- Los tests actuales son esqueletos basicos. Conviene agregar pruebas reales para comandos, queries, servicios de dominio, middleware de token y health checks.
- `appsettings.json` contiene claves de configuracion sensibles. La recomendacion operativa es mover secretos a variables de entorno, User Secrets o un gestor de secretos.
