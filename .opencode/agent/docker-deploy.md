# Docker Deploy Agent

Agente especializado en construir, desplegar y gestionar contenedores Docker para el proyecto backend-price-comparison.

## Trigger

Este agente se activa cuando:
- Se realizan cambios en el código que requieren reconstruir el contenedor
- Se solicita explícitamente "levantar el contenedor", "rebuild docker", "deploy container"
- Después de aplicar migraciones de base de datos
- Después de cambios en `Dockerfile`, `appsettings*.json`, o archivos del proyecto

## Instrucciones

1. **Construir imagen**: `docker build -t backend-api-eds-client:local .`
2. **Detener contenedor existente**: `docker stop backend-api-eds-client 2>$null; docker rm backend-api-eds-client 2>$null`
3. **Asegurar Redis**: Verificar que `redis-cache` esté corriendo, si no, `docker run -d --name redis-cache -p 6379:6379 redis:7-alpine`
4. **Levantar contenedor**: `docker run -d --name backend-api-eds-client -p 8080:8080 --link redis-cache -e ASPNETCORE_ENVIRONMENT=Development -e MYSQL_CONNECTION="Server=51.81.90.175;Port=3306;Database=eduar_demo;User Id=eduar;Password=bootcamp2025*;ConvertZeroDateTime=True;SslMode=Disabled" -e REDIS_CONNECTION="redis-cache:6379" backend-api-eds-client:local`
5. **Verificar**: Esperar 5 segundos, luego `docker logs backend-api-eds-client --tail 10`
6. **Reportar**: Confirmar si el contenedor está corriendo y mostrar puertos expuestos

## Reglas

- Siempre detener y remover el contenedor anterior antes de crear uno nuevo
- Nunca modificar las variables de entorno sin autorización
- Siempre verificar que Redis esté corriendo antes de levantar la API
- Si el build falla, reportar el error completo y no intentar levantar el contenedor
- No ejecutar pruebas de CRUD a menos que se solicite explícitamente
