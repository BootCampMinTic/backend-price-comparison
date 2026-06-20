# 📡 GUÍA PRÁCTICA: Solicitudes HTTP para el Caso de Uso "Registrar y Consultar Precios"

Este archivo contiene ejemplos reales de solicitudes HTTP que puedes usar para probar el caso de uso con herramientas como **Postman**, **Insomnia**, o directamente con `curl`.

---

## 🔐 Autenticación

Todos los endpoints requieren un header `Authorization: Bearer <token>`:

```
Authorization: Bearer YOUR_TOKEN_HERE
```

*(El token se valida en `BearerTokenMiddleware` - cualquier token válido funciona en desarrollo)*

---

## 1️⃣ REGISTRAR PRECIO DE UN PRODUCTO

### Descripción
Registra el precio de un producto específico en un supermercado en una fecha determinada.

### Endpoint
```
POST /api/v1/prices
```

### Headers
```
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Body (JSON)
```json
{
  "productId": 1,
  "storeId": 5,
  "price": 45.99,
  "date": "2024-06-19T10:30:00Z"
}
```

### Parámetros Requeridos
| Campo | Tipo | Descripción | Validación |
|-------|------|-------------|-----------|
| `productId` | int | ID del producto | Debe existir en BD, > 0 |
| `storeId` | int | ID del supermercado | Debe existir en BD, > 0 |
| `price` | double | Precio a registrar | Debe ser > 0 |
| `date` | DateTime | Fecha del registro | No puede ser futuro, UTC |

### Respuesta Exitosa (200 OK)
```json
{
  "success": true,
  "message": "Precio registrado exitosamente.",
  "data": {}
}
```

### Respuesta con Error (400 Bad Request)
```json
{
  "success": false,
  "message": "El identificador del producto debe ser mayor a cero.",
  "errors": null
}
```

### Ejemplos cURL

#### Ejemplo 1: Registro válido
```bash
curl -X POST "http://localhost:5062/api/v1/prices" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -d '{
    "productId": 1,
    "storeId": 5,
    "price": 45.99,
    "date": "2024-06-19T10:30:00Z"
  }'
```

#### Ejemplo 2: Precio inválido (negativo)
```bash
curl -X POST "http://localhost:5062/api/v1/prices" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -d '{
    "productId": 1,
    "storeId": 5,
    "price": -10.50,
    "date": "2024-06-19T10:30:00Z"
  }'
```

#### Ejemplo 3: Fecha futura (inválida)
```bash
curl -X POST "http://localhost:5062/api/v1/prices" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -d '{
    "productId": 1,
    "storeId": 5,
    "price": 45.99,
    "date": "2025-12-31T23:59:59Z"
  }'
```

### Códigos de Respuesta
- **200 OK**: Precio registrado correctamente
- **400 Bad Request**: Validación fallida o producto/supermercado no existe
- **401 Unauthorized**: Token no válido o ausente
- **500 Internal Server Error**: Error de servidor (base de datos, etc.)

---

## 2️⃣ CONSULTAR COMPARACIÓN DE PRECIOS

### Descripción
Obtiene la lista de precios de un producto en todos los supermercados, ordenados de menor a mayor precio.

### Endpoint
```
GET /api/v1/products/{productId}/prices
```

### Headers
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Parámetros
| Parámetro | Tipo | Ubicación | Descripción | Validación |
|-----------|------|-----------|-------------|-----------|
| `productId` | int | URL path | ID del producto | Debe ser > 0 |

### Respuesta Exitosa (200 OK)
```json
{
  "success": true,
  "message": "Comparación de precios recuperada exitosamente.",
  "data": [
    {
      "id": 10,
      "productId": 1,
      "productName": "Leche Integral 1L",
      "storeId": 3,
      "storeName": "Carrefour",
      "price": 38.50,
      "date": "2024-06-19T09:15:00Z"
    },
    {
      "id": 11,
      "productId": 1,
      "productName": "Leche Integral 1L",
      "storeId": 5,
      "storeName": "Walmart",
      "price": 41.99,
      "date": "2024-06-19T10:00:00Z"
    },
    {
      "id": 12,
      "productId": 1,
      "productName": "Leche Integral 1L",
      "storeId": 2,
      "storeName": "Éxito",
      "price": 45.99,
      "date": "2024-06-19T10:30:00Z"
    }
  ]
}
```

### Respuesta sin Registros (400 Bad Request)
```json
{
  "success": false,
  "message": "No records found for historial de precios.",
  "errors": null
}
```

### Ejemplos cURL

#### Ejemplo 1: Consultar precios del producto ID 1
```bash
curl -X GET "http://localhost:5062/api/v1/products/1/prices" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

#### Ejemplo 2: Con paginación visual (nota: actualmente no implementada, futura extensión)
```bash
curl -X GET "http://localhost:5062/api/v1/products/1/prices?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

#### Ejemplo 3: Usando PowerShell
```powershell
$headers = @{
    "Authorization" = "Bearer YOUR_TOKEN_HERE"
}

$response = Invoke-RestMethod `
    -Uri "http://localhost:5062/api/v1/products/1/prices" `
    -Headers $headers `
    -Method Get

$response | ConvertTo-Json
```

### Códigos de Respuesta
- **200 OK**: Lista de precios recuperada correctamente
- **400 Bad Request**: Producto no encontrado o sin historial
- **401 Unauthorized**: Token no válido o ausente
- **500 Internal Server Error**: Error de servidor

---

## 📊 FLUJO COMPLETO DE EJEMPLO

### Paso 1: Registrar 3 precios del mismo producto en diferentes supermercados

```bash
# Precio en Carrefour: $38.50
curl -X POST "http://localhost:5062/api/v1/prices" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer token123" \
  -d '{
    "productId": 1,
    "storeId": 3,
    "price": 38.50,
    "date": "2024-06-19T09:15:00Z"
  }'

# Precio en Walmart: $41.99
curl -X POST "http://localhost:5062/api/v1/prices" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer token123" \
  -d '{
    "productId": 1,
    "storeId": 5,
    "price": 41.99,
    "date": "2024-06-19T10:00:00Z"
  }'

# Precio en Éxito: $45.99
curl -X POST "http://localhost:5062/api/v1/prices" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer token123" \
  -d '{
    "productId": 1,
    "storeId": 2,
    "price": 45.99,
    "date": "2024-06-19T10:30:00Z"
  }'
```

### Paso 2: Consultar la comparación de precios

```bash
curl -X GET "http://localhost:5062/api/v1/products/1/prices" \
  -H "Authorization: Bearer token123"
```

**Resultado**: Se obtienen los 3 precios ordenados de menor a mayor, facilitando la comparación:
- Carrefour: $38.50 (más barato) ✓
- Walmart: $41.99
- Éxito: $45.99 (más caro)

---

## 🔍 TESTING CON POSTMAN

### Crear Colección

1. Abre Postman
2. Click en "+" para nueva pestaña
3. Selecciona "Collection" → Nombra: "Precio Comparación API"

### Crear Solicitud 1: Registrar Precio

1. **Método**: POST
2. **URL**: `{{base_url}}/api/v1/prices`
3. **Headers**:
   ```
   Content-Type: application/json
   Authorization: Bearer {{token}}
   ```
4. **Body (raw JSON)**:
   ```json
   {
     "productId": 1,
     "storeId": 5,
     "price": 45.99,
     "date": "2024-06-19T10:30:00Z"
   }
   ```
5. Click **Send**

### Crear Solicitud 2: Consultar Precios

1. **Método**: GET
2. **URL**: `{{base_url}}/api/v1/products/1/prices`
3. **Headers**:
   ```
   Authorization: Bearer {{token}}
   ```
4. Click **Send**

### Configurar Variables de Entorno

1. Click en **Environments** (abajo a la izquierda)
2. Click en "+" para nuevo entorno
3. Nombra: "Local Dev"
4. Agrega variables:
   ```
   base_url: http://localhost:5062
   token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```
5. Selecciona el entorno en la esquina superior derecha

---

## 🧪 TESTING CON INSOMNIA

### Crear Solicitud

1. Abre Insomnia
2. Click en "Create" → "Request"
3. Nombre: "Register Price"
4. Tipo: **POST**
5. URL: `http://localhost:5062/api/v1/prices`
6. Tab **Headers**:
   ```
   Content-Type: application/json
   Authorization: Bearer YOUR_TOKEN
   ```
7. Tab **Body** (JSON):
   ```json
   {
     "productId": 1,
     "storeId": 5,
     "price": 45.99,
     "date": "2024-06-19T10:30:00Z"
   }
   ```
8. Click **Send**

---

## ⚠️ ERRORES COMUNES Y SOLUCIONES

### Error 1: 401 Unauthorized
**Causa**: Token no proporcionado o inválido
**Solución**: Agregar header `Authorization: Bearer <token>`

```bash
# ❌ MAL
curl -X GET "http://localhost:5062/api/v1/products/1/prices"

# ✅ BIEN
curl -X GET "http://localhost:5062/api/v1/products/1/prices" \
  -H "Authorization: Bearer token123"
```

### Error 2: 400 Bad Request - Validación fallida
**Causa**: Datos de entrada inválidos
**Solución**: Verificar que:
- ProductId > 0
- StoreId > 0
- Price > 0
- Date no sea futura

```json
// ❌ INVÁLIDO
{
  "productId": 0,        // Debe ser > 0
  "storeId": 5,
  "price": -10,          // Debe ser > 0
  "date": "2025-01-01T00:00:00Z"  // Futura
}

// ✅ VÁLIDO
{
  "productId": 1,
  "storeId": 5,
  "price": 45.99,
  "date": "2024-06-19T10:30:00Z"
}
```

### Error 3: 400 Bad Request - Producto no existe
**Causa**: El productId no existe en la base de datos
**Solución**: Verificar que el producto existe antes de registrar precio

```bash
# Primero, asegúrate de que el producto existe
# (Usa endpoint GET /api/v1/products o verifica en BD)
```

### Error 4: 500 Internal Server Error
**Causa**: Error de base de datos o servidor
**Solución**: 
1. Verificar que MySQL está corriendo
2. Revisar logs del servidor
3. Verificar connection string en `appsettings.json`

---

## 📈 CASOS DE USO AVANZADOS

### Comparar precios entre múltiples días

```bash
# Día 1: Registrar precio
curl -X POST "http://localhost:5062/api/v1/prices" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer token" \
  -d '{"productId": 1, "storeId": 5, "price": 45.99, "date": "2024-06-18T10:00:00Z"}'

# Día 2: Mismo producto, misma tienda, nuevo precio
curl -X POST "http://localhost:5062/api/v1/prices" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer token" \
  -d '{"productId": 1, "storeId": 5, "price": 48.50, "date": "2024-06-19T10:00:00Z"}'

# Consultar histórico
curl -X GET "http://localhost:5062/api/v1/products/1/prices" \
  -H "Authorization: Bearer token"

# Resultado: Verás ambos registros, permitiendo análisis de tendencias
```

### Monitorear cambios de precio en tiempo real

Implementa un script que registre precios periódicamente:

```powershell
# script-monitoreo-precios.ps1
$token = "YOUR_TOKEN"
$baseUrl = "http://localhost:5062/api/v1"
$productId = 1

# Ejecutar cada hora
$timer = (New-Object System.Timers.Timer 3600000)
$timer.AutoReset = $true

$action = {
    $stores = @(2, 3, 5)  # IDs de tiendas a monitorear
    
    foreach ($storeId in $stores) {
        $price = Get-CurrentPrice -ProductId $productId -StoreId $storeId
        
        $body = @{
            productId = $productId
            storeId = $storeId
            price = $price
            date = (Get-Date).ToUniversalTime().ToString("o")
        } | ConvertTo-Json
        
        Invoke-RestMethod -Uri "$baseUrl/prices" `
            -Method Post `
            -Headers @{"Authorization"="Bearer $token"; "Content-Type"="application/json"} `
            -Body $body
    }
}

Register-ObjectEvent -InputObject $timer -EventName Elapsed -Action $action
$timer.Start()
```

---

## 📚 RECURSOS ADICIONALES

- **Documentación de la API**: http://localhost:5062/scalar/v1
- **OpenAPI/Swagger**: http://localhost:5062/openapi/v1.json
- **Health Check**: http://localhost:5062/health

---

**Última actualización**: 2024-06-19
**Versión API**: 1.0
**Base URL por defecto**: http://localhost:5062
