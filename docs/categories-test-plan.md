# Plan de pruebas del módulo de categorías

## Objetivo

Verificar que el módulo de categorías permita consultar, crear, actualizar y eliminar categorías de producto y categorías de tienda de forma correcta, segura y consistente.

Este documento hace parte del aporte realizado en la rama `feature/category_ricaurte`, orientado a documentación, pruebas y validación funcional del módulo de categorías.

---

## Alcance

El plan de pruebas cubre las siguientes funcionalidades:

- Consulta de categorías de producto.
- Consulta de categorías de tienda.
- Creación de nuevas categorías.
- Actualización de categorías existentes.
- Eliminación o desactivación de categorías.
- Validación de campos obligatorios.
- Validación de respuestas HTTP esperadas.

---

## Tipos de pruebas

| Tipo de prueba | Descripción |
|---|---|
| Prueba funcional | Verifica que cada endpoint cumpla con su propósito |
| Prueba de validación | Verifica que el sistema rechace datos incorrectos |
| Prueba de error | Verifica respuestas ante registros inexistentes |
| Prueba de integración manual | Verifica el comportamiento usando herramientas como Postman |
| Prueba de documentación | Verifica que las rutas estén claras y correctamente descritas |

---

## Criterios de aceptación

El módulo se considera aprobado si cumple con los siguientes criterios:

- Permite listar categorías de producto.
- Permite listar categorías de tienda.
- Permite consultar una categoría por ID.
- Permite crear categorías con datos válidos.
- Rechaza categorías sin nombre.
- Permite actualizar categorías existentes.
- Retorna error cuando se intenta actualizar una categoría inexistente.
- Retorna error cuando se intenta eliminar una categoría inexistente.
- Retorna códigos HTTP adecuados.
- Mantiene nombres de rutas claros y consistentes.

---

## Códigos HTTP esperados

| Código | Significado | Uso esperado |
|---|---|---|
| 200 OK | Solicitud correcta | Consultar o actualizar información |
| 201 Created | Recurso creado | Crear una nueva categoría |
| 204 No Content | Sin contenido | Eliminar o desactivar una categoría |
| 400 Bad Request | Solicitud incorrecta | Datos incompletos o inválidos |
| 404 Not Found | No encontrado | Categoría inexistente |
| 500 Internal Server Error | Error interno | Fallo no controlado del servidor |

---

## Recomendaciones de calidad

- No permitir nombres de categorías vacíos.
- No permitir categorías duplicadas.
- Usar nombres claros para las rutas.
- Mantener consistencia entre categorías de producto y categorías de tienda.
- Evitar eliminación física cuando existan productos o tiendas relacionadas.
- Documentar los resultados de las pruebas realizadas.