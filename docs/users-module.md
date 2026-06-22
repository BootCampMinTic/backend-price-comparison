# Módulo de Usuarios

## Objetivo

El módulo de usuarios permite registrar, consultar, actualizar e inactivar los usuarios del sistema.

## Entidad principal

Usuario.

## Campos propuestos

- Id
- Name
- LastName
- Email
- Password
- Phone
- Role
- IsActive
- CreatedAt
- UpdatedAt

## Reglas de validación

- El nombre es obligatorio.
- El apellido es obligatorio.
- El correo electrónico es obligatorio.
- El correo electrónico debe tener formato válido.
- No se debe permitir registrar dos usuarios con el mismo correo.
- La contraseña es obligatoria.
- La contraseña debe tener mínimo 8 caracteres.
- El rol del usuario debe ser válido.
- El usuario debe tener estado activo o inactivo.

## Rutas API propuestas

GET /api/v1/users  
GET /api/v1/users/{id}  
POST /api/v1/users  
PUT /api/v1/users/{id}  
DELETE /api/v1/users/{id}

## Casos de uso

### Crear usuario

Permite registrar un nuevo usuario en el sistema.

### Consultar usuarios

Permite listar todos los usuarios registrados.

### Consultar usuario por ID

Permite consultar la información de un usuario específico.

### Actualizar usuario

Permite modificar los datos de un usuario existente.

### Inactivar usuario

Permite cambiar el estado del usuario a inactivo sin eliminarlo físicamente de la base de datos.

## Casos de prueba propuestos

- Crear usuario con datos válidos.
- Rechazar usuario sin nombre.
- Rechazar usuario sin correo.
- Rechazar usuario con correo inválido.
- Rechazar usuario con contraseña menor a 8 caracteres.
- Rechazar usuario con correo repetido.
- Consultar usuario existente.
- Consultar usuario inexistente y retornar error 404.
- Actualizar usuario existente.
- Inactivar usuario existente.