## 📌 Contexto
<!-- Describe el objetivo del cambio y el problema de negocio/técnico que resuelve. -->

## ✅ Checklist de arquitectura hexagonal
- [ ] El cambio respeta la dirección de dependencias: `Domain` no depende de `Application`, `Infrastructure` ni `Api`.
- [ ] Los puertos (`Domain/Ports`) siguen siendo contratos del dominio y no exponen detalles de infraestructura.
- [ ] Los adaptadores de infraestructura implementan puertos sin filtrar tipos específicos de frameworks al dominio.
- [ ] No se mezclan responsabilidades de capas (por ejemplo: lógica de dominio en endpoints/controladores).
- [ ] Los casos de uso/handlers en `Application` orquestan, pero no contienen lógica de infraestructura.

## 🧱 Checklist SOLID y code smells
- [ ] **S**RP: cada clase tiene una única razón de cambio.
- [ ] **O**CP: se evitó modificar comportamiento existente cuando se podía extender.
- [ ] **L**SP: las implementaciones respetan el contrato de sus abstracciones.
- [ ] **I**SP: interfaces específicas y pequeñas; se evitó crear “interfaces gordas”.
- [ ] **D**IP: el código depende de abstracciones, no de concretos.
- [ ] Se evitaron code smells comunes: clases/métodos largos, nombres ambiguos, duplicación, magia de strings/números.
- [ ] Se manejan errores de forma explícita (por ejemplo, usando `Result`/errores de dominio) y sin ocultar excepciones.

## 🧪 Calidad y pruebas
- [ ] Se agregaron/ajustaron pruebas unitarias para cubrir comportamiento nuevo o modificado.
- [ ] Se verificaron escenarios borde y casos de error.
- [ ] Los cambios mantienen o mejoran legibilidad y mantenibilidad.

## 💬 Comentarios de code review (obligatorio)
<!--
Incluye aquí al menos 2 observaciones de mejora encontradas durante la auto-revisión.
Formato recomendado:
1. [Tipo] Archivo:Línea - Observación - Sugerencia.
2. [Tipo] Archivo:Línea - Observación - Sugerencia.

Tipos sugeridos: Arquitectura, SOLID, Code Smell, Pruebas, Rendimiento, Seguridad.
-->
1. 
2. 

## 🔍 Riesgos / impacto
<!-- Describe impacto en rendimiento, seguridad, contratos API, migraciones, etc. -->

## 📝 Evidencia
<!-- Pega resultados de pruebas o comandos ejecutados. -->
