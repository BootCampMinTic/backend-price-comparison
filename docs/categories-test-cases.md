# Casos de prueba del módulo de categorías

## Objetivo

Definir los casos de prueba manuales para validar el funcionamiento de los endpoints relacionados con categorías de producto y categorías de tienda.

Este documento hace parte del aporte realizado en la rama `feature/category_ricaurte`.

---

## 1. Casos de prueba para categorías de producto

| ID | Caso de prueba | Método | Ruta | Datos de entrada | Resultado esperado |
|---|---|---|---|---|---|
| CP-001 | Listar categorías de producto | GET | /api/v1/product-categories | No aplica | Retorna la lista de categorías de producto |
| CP-002 | Consultar categoría de producto existente | GET | /api/v1/product-categories/{id} | ID válido | Retorna la categoría solicitada |
| CP-003 | Consultar categoría de producto inexistente | GET | /api/v1/product-categories/9999 | ID inexistente | Retorna 404 Not Found |
| CP-004 | Crear categoría de producto válida | POST | /api/v1/product-categories | name y description válidos | Retorna 201 Created |
| CP-005 | Crear categoría de producto sin nombre | POST | /api/v1/product-categories | name vacío | Retorna 400 Bad Request |
| CP-006 | Crear categoría de producto duplicada | POST | /api/v1/product-categories | name ya existente | Retorna error de validación |
| CP-007 | Actualizar categoría de producto existente | PUT | /api/v1/product-categories/{id} | ID válido y datos válidos | Retorna 200 OK o 204 No Content |
| CP-008 | Actualizar categoría de producto inexistente | PUT | /api/v1/product-categories/9999 | ID inexistente | Retorna 404 Not Found |
| CP-009 | Eliminar categoría de producto existente | DELETE | /api/v1/product-categories/{id} | ID válido | Retorna 204 No Content |
| CP-010 | Eliminar categoría de producto inexistente | DELETE | /api/v1/product-categories/9999 | ID inexistente | Retorna 404 Not Found |

---

## 2. Casos de prueba para categorías de tienda

| ID | Caso de prueba | Método | Ruta | Datos de entrada | Resultado esperado |
|---|---|---|---|---|---|
| CT-001 | Listar categorías de tienda | GET | /api/v1/store-categories | No aplica | Retorna la lista de categorías de tienda |
| CT-002 | Consultar categoría de tienda existente | GET | /api/v1/store-categories/{id} | ID válido | Retorna la categoría solicitada |
| CT-003 | Consultar categoría de tienda inexistente | GET | /api/v1/store-categories/9999 | ID inexistente | Retorna 404 Not Found |
| CT-004 | Crear categoría de tienda válida | POST | /api/v1/store-categories | name y description válidos | Retorna 201 Created |
| CT-005 | Crear categoría de tienda sin nombre | POST | /api/v1/store-categories | name vacío | Retorna 400 Bad Request |
| CT-006 | Crear categoría de tienda duplicada | POST | /api/v1/store-categories | name ya existente | Retorna error de validación |
| CT-007 | Actualizar categoría de tienda existente | PUT | /api/v1/store-categories/{id} | ID válido y datos válidos | Retorna 200 OK o 204 No Content |
| CT-008 | Actualizar categoría de tienda inexistente | PUT | /api/v1/store-categories/9999 | ID inexistente | Retorna 404 Not Found |
| CT-009 | Eliminar categoría de tienda existente | DELETE | /api/v1/store-categories/{id} | ID válido | Retorna 204 No Content |
| CT-010 | Eliminar categoría de tienda inexistente | DELETE | /api/v1/store-categories/9999 | ID inexistente | Retorna 404 Not Found |

---

## 3. Datos de prueba sugeridos

### Categoría de producto válida

```json
{
  "name": "Bebidas",
  "description": "Productos líquidos para consumo"
}### Categoría de producto inválida

```json
{
  "name": "",
  "description": "Categoría sin nombre"
}
```

### Categoría de tienda válida

```json
{
  "name": "Supermercado",
  "description": "Establecimiento dedicado a la venta de productos de consumo masivo"
}
```

### Categoría de tienda inválida

```json
{
  "name": "",
  "description": "Categoría sin nombre"
}
```

---

## 4. Checklist de validación

- [ ] Se listan correctamente las categorías de producto.
- [ ] Se listan correctamente las categorías de tienda.
- [ ] Se consulta correctamente una categoría por ID.
- [ ] El sistema retorna 404 cuando la categoría no existe.
- [ ] El sistema permite crear categorías válidas.
- [ ] El sistema rechaza categorías sin nombre.
- [ ] El sistema evita categorías duplicadas.
- [ ] El sistema permite actualizar categorías existentes.
- [ ] El sistema controla errores al actualizar categorías inexistentes.
- [ ] El sistema permite eliminar o desactivar categorías existentes.
- [ ] El sistema controla errores al eliminar categorías inexistentes.