# Documentación inicial del módulo de categorías

## Objetivo

Documentar el módulo de categorías del sistema, diferenciando categorías de producto y categorías de tienda.

Este aporte se realiza desde la rama `feature/category_ricaurte` como evidencia de documentación técnica y apoyo al proceso de pruebas del módulo.

---

## 1. Categorías de producto

Las categorías de producto permiten clasificar los productos registrados en el sistema.

Ejemplos:

- Bebidas
- Aseo
- Tecnología
- Alimentos
- Ropa
- Lácteos
- Granos

Una categoría de producto responde a la pregunta:

¿Qué tipo de producto es?

Ejemplo:

Producto: Coca-Cola 1.5L  
Categoría de producto: Bebidas

---

## 2. Categorías de tienda

Las categorías de tienda permiten clasificar los establecimientos o negocios registrados en el sistema.

Ejemplos:

- Supermercado
- Farmacia
- Restaurante
- Ferretería
- Tienda de tecnología
- Zapatería

Una categoría de tienda responde a la pregunta:

¿Qué tipo de tienda o negocio es?

Ejemplo:

Tienda: Droguería La Salud  
Categoría de tienda: Farmacia

---

## 3. Endpoints propuestos para categorías de producto

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/product-categories | Consultar todas las categorías de producto |
| GET | /api/v1/product-categories/{id} | Consultar una categoría de producto por ID |
| POST | /api/v1/product-categories | Crear una nueva categoría de producto |
| PUT | /api/v1/product-categories/{id} | Actualizar una categoría de producto |
| DELETE | /api/v1/product-categories/{id} | Eliminar o desactivar una categoría de producto |

---

## 4. Endpoints propuestos para categorías de tienda

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/store-categories | Consultar todas las categorías de tienda |
| GET | /api/v1/store-categories/{id} | Consultar una categoría de tienda por ID |
| POST | /api/v1/store-categories | Crear una nueva categoría de tienda |
| PUT | /api/v1/store-categories/{id} | Actualizar una categoría de tienda |
| DELETE | /api/v1/store-categories/{id} | Eliminar o desactivar una categoría de tienda |

---

## 5. Observaciones de calidad

- Las rutas deben mantener nombres claros y consistentes.
- Los nombres de categorías no deben enviarse vacíos.
- Las descripciones deben ser comprensibles.
- No se deben duplicar categorías con el mismo nombre.
- Si una categoría tiene productos o tiendas asociadas, se recomienda usar eliminación lógica en lugar de borrado físico.