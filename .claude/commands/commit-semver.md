---
description: Crea un commit siguiendo Conventional Commits + Semantic Versioning
argument-hint: [mensaje opcional o tipo:scope]
---

Eres un asistente experto en **Conventional Commits** y **Semantic Versioning (SemVer)**. Tu tarea es analizar los cambios actuales del repositorio y crear un commit correctamente formateado.

## Pasos a seguir

### 1. Analiza el estado del repo (en paralelo)
- `git status` (sin `-uall`)
- `git diff` (cambios staged y unstaged)
- `git log -5 --oneline` para ver el estilo de commits previos
- `git tag --sort=-v:refname | head -5` para ver el último tag de versión, si existe

### 2. Determina el TIPO de cambio según Conventional Commits

| Tipo | Cuándo usarlo | Impacto SemVer |
|---|---|---|
| `feat` | Nueva funcionalidad para el usuario | **MINOR** (0.X.0) |
| `fix` | Corrección de bug | **PATCH** (0.0.X) |
| `perf` | Mejora de rendimiento | **PATCH** |
| `refactor` | Cambio de código sin alterar funcionalidad | sin bump |
| `docs` | Solo documentación | sin bump |
| `style` | Formato, espacios, comas (no afecta lógica) | sin bump |
| `test` | Agregar o corregir tests | sin bump |
| `build` | Cambios al sistema de build, dependencias | sin bump |
| `ci` | Cambios en CI/CD (GitHub Actions, etc.) | sin bump |
| `chore` | Tareas de mantenimiento (no src ni tests) | sin bump |
| `revert` | Revertir un commit anterior | depende |

**Breaking changes:** Si rompe compatibilidad agrega `!` después del tipo/scope (`feat!:`, `fix(api)!:`) **o** una sección `BREAKING CHANGE:` en el footer. Esto fuerza **MAJOR** (X.0.0).

### 3. Determina el SCOPE (opcional pero recomendado)

Inferir del path de archivos modificados:
- Cambios en `Backend.PriceComparison.Api/` → `api`
- Cambios en `Backend.PriceComparison.Application/` → `application`
- Cambios en `Backend.PriceComparison.Domain/` → `domain`
- Cambios en `Backend.PriceComparison.Infrastructure.Persistence.Mysql/` → `persistence`
- Cambios en `WorkerServiceBilling/` → `worker`
- Cambios en `Dockerfile`, `.github/`, `.dockerignore` → `infra` o `ci`
- Cambios en múltiples capas → omitir scope

### 4. Calcula la versión próxima

1. Lee la versión actual desde el último tag git (ej. `v1.2.3` → `1.2.3`).
2. Si no hay tags, asume `0.1.0` como base.
3. Aplica bump según el tipo del commit:
   - **MAJOR:** breaking changes → `2.0.0`
   - **MINOR:** `feat` → `1.3.0`
   - **PATCH:** `fix`, `perf` → `1.2.4`
   - **Sin bump:** `chore`, `docs`, `style`, etc. → mantener `1.2.3`

### 5. Construye el mensaje del commit

**Formato estándar:**
```
<tipo>(<scope>): <descripción imperativa, minúscula, sin punto final>

[cuerpo opcional explicando el qué y el por qué]

[footer opcional con BREAKING CHANGE: o refs a issues]
```

**Reglas:**
- **Header ≤ 72 caracteres**, en imperativo presente ("add X" no "added X" ni "adds X")
- **Cuerpo** envuelto a 72 columnas, separa el "qué" del "por qué"
- **NO incluir** el número de versión en el mensaje del commit (eso lo lleva el tag)
- **Co-author** al final:
  ```
  Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>
  ```

### 6. Muestra al usuario un resumen ANTES de commitear

Reporta en este formato exacto:

```
Tipo:        feat
Scope:       api
Breaking:    no
Versión:     1.2.3 → 1.3.0 (MINOR)

Mensaje propuesto:
─────────────────────────────────────
feat(api): agregar endpoint de health check versionado

Expone /api/v1/health/{ready,live} para readiness y
liveness probes en Kubernetes.
─────────────────────────────────────

Archivos a incluir:
  M  Backend.PriceComparison.Api/Endpoints/HealthEndpoints.cs
  A  Backend.PriceComparison.Api/HealthChecks/ApplicationHealthCheck.cs
```

### 7. Procede con el commit

- Stagea archivos por nombre (NUNCA `git add .` ni `-A`).
- **Excluye** archivos sensibles: `.env`, `*.pfx`, `secrets.*`, `appsettings.*.local.json`, archivos con credenciales detectadas en el diff.
- Crea el commit con HEREDOC para preservar formato:
  ```bash
  git commit -m "$(cat <<'EOF'
  <header>

  <body>

  Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>
  EOF
  )"
  ```
- Si el hook de pre-commit falla: **NO uses `--amend`**. Arregla el problema, re-stagea y crea un commit nuevo.
- Después del commit ejecuta `git status` y `git log -1` para confirmar.

### 8. Push automático al remoto

Después de un commit exitoso:

1. Detecta la rama actual: `git branch --show-current`
2. Detecta si tiene upstream configurado:
   ```bash
   git rev-parse --abbrev-ref --symbolic-full-name @{u}
   ```
3. **Push según el caso:**
   - **Con upstream:** `git push`
   - **Sin upstream:** `git push -u origin <rama-actual>`
4. **Si la rama actual es `main` o `master`**, antes de pushear muestra una advertencia:
   ```
   ⚠️  Vas a pushear directo a main. ¿Continuar? (auto-yes en este flujo)
   ```
   Procede igual (el usuario invocó el comando sabiendo que pushea), pero deja el aviso visible en el output.
5. **Nunca** uses `--force`, `--force-with-lease`, ni `--no-verify`. Si el push es rechazado por non-fast-forward, **detente** y avisa al usuario para que decida (`git pull --rebase` o resolver manualmente).
6. **No pushees tags automáticamente** — solo el commit.

Si el push falla por cualquier razón, reporta el error completo y deja el commit local intacto (no lo deshagas).

### 9. Sugiere el tag (NO lo crees automáticamente)

Termina con:

```
✅ Commit creado: <hash>
✅ Push a <remote>/<rama> exitoso

Para tagear esta versión ejecuta:
  git tag -a v1.3.0 -m "Release 1.3.0"
  git push origin v1.3.0
```

## Argumentos del usuario

Si el usuario pasó argumentos (`$ARGUMENTS`):
- Si parece un mensaje libre: úsalo como base pero **valida y reformatea** al estilo Conventional Commits.
- Si parece `tipo:scope` o `tipo(scope)`: úsalo como prefijo y deduce el resto del análisis.
- Si está vacío: deduce todo del diff.

## Restricciones

- **Nunca** ejecutes `git push --force` ni `--force-with-lease` automáticamente.
- **Nunca** ejecutes con `--no-verify` salvo petición explícita.
- **Nunca** crees commits vacíos.
- **Nunca** pushees tags automáticamente (solo el commit, los tags requieren confirmación).
- Si no hay cambios, dilo claramente y termina.
- Si detectas secretos en el diff (API keys, passwords, tokens), **detente** y avisa al usuario antes de commitear.
- Si el push es rechazado (non-fast-forward, permission denied, etc.), **no intentes resolverlo automáticamente**: reporta el error y deja al usuario decidir.
