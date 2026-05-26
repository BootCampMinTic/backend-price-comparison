# Avance del módulo de categorías

## Objetivo

Implementar el módulo de categorías del sistema, permitiendo administrar categorías de productos y categorías de tiendas dentro del backend.

## Categorías identificadas

El proyecto manejará dos tipos principales de categorías:

1. Categorías de producto
2. Categorías de tienda

## Rutas API propuestas

### Categorías de producto

GET /api/v1/product-categories  
GET /api/v1/product-categories/{id}  
POST /api/v1/product-categories  
PUT /api/v1/product-categories/{id}  
DELETE /api/v1/product-categories/{id}  

### Categorías de tienda

GET /api/v1/store-categories  
GET /api/v1/store-categories/{id}  
POST /api/v1/store-categories  
PUT /api/v1/store-categories/{id}  
DELETE /api/v1/store-categories/{id}  

## Distribución del trabajo

| Desarrollador | Rama | Responsabilidad |
|---|---|---|
| Desarrollador 1 | feature/categories-domain | Entidades y contratos |
| Desarrollador 2 | feature/categories-application | DTOs, comandos, queries y validaciones |
| Desarrollador 3 | feature/categories-infrastructure | Repositorios y acceso a base de datos |
| Desarrollador 4 | feature/categories-api | Endpoints y documentación Swagger |
| Desarrollador 5 | feature/categories-tests-docs | Pruebas, documentación y Postman |

## Buenas prácticas acordadas

- No trabajar directamente sobre la rama main.
- Cada cambio debe hacerse en una rama feature.
- Todo avance debe subirse mediante Pull Request.
- Los nombres de ramas deben ser claros.
- Los commits deben tener mensajes descriptivos.
- No subir contraseñas ni cadenas de conexión reales.
- No subir carpetas bin, obj, .vs o archivos temporales.
- Validar el código antes de solicitar revisión.