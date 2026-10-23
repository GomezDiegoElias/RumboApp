# Contribuir a RumboApp

## Flujo de trabajo

Usamos un flujo basado en GitHub Flow con `develop` como rama principal de integración. `main` representa únicamente versiones estables o entregables. Cada cambio se desarrolla en una rama corta, creada desde la última versión de `develop`, y se integra mediante un Pull Request hacia `develop`.

Ninguna rama de tarea debe partir directamente desde `main` ni debe abrir su Pull Request directamente contra `main`, salvo una corrección urgente previamente acordada por el equipo.

### Ciclo de integración

1. Actualizar la copia local de `develop`.
2. Crear la rama de tarea desde `develop`.
3. Implementar, probar y documentar el cambio.
4. Abrir el Pull Request hacia `develop`.
5. Integrar en `develop` después de la revisión y las validaciones.
6. Promover `develop` a `main` únicamente para una versión estable, control o entrega.

### Ramas

Usar el prefijo que describa el propósito:

| Prefijo | Uso |
|---|---|
| `feat/` | funcionalidad nueva |
| `fix/` | corrección de un defecto |
| `docs/` | documentación |
| `test/` | pruebas |
| `ci/` | automatización, build o workflows |
| `refactor/` | reorganización sin cambio funcional |
| `chore/` | mantenimiento |
| `perf/` | mejora de rendimiento |
| `security/` | corrección o endurecimiento de seguridad |

Ejemplo: `feat/trips-scheduled-worker`.

Comandos de referencia:

```bash
git switch develop
git pull --ff-only origin develop
git switch -c feat/trips-scheduled-worker
```

## Commits

Usamos Conventional Commits:

```text
<tipo>(<alcance>): <descripción en imperativo>
```

Tipos admitidos: `feat`, `fix`, `docs`, `test`, `ci`, `refactor`, `chore`, `perf`, `security`, `build`.

La descripción debe explicar el cambio observable. El cuerpo debe explicar el motivo cuando no sea evidente. Los commits deben ser pequeños, compilables cuando sea posible y trazables a un requisito `REQ-*`, tarea o issue. En cambios normales, dividir el trabajo en varios commits coherentes y autocontenidos; no acumular funcionalidades, tests, documentación y configuración no relacionada en un único commit. La excepción es una carga inicial, migración o consolidación solicitada explícitamente.

Cada commit debe representar una intención única y revisable. El orden recomendado es: configuración/base, dominio o producción, tests, documentación y automatización, siempre que puedan validarse de forma independiente.

Ejemplos:

```text
feat(trips): agregar asignación idempotente REQ-V-11
test(concurrency): verificar 50 aceptaciones simultáneas
docs(adr): registrar decisión de restricción EXCLUDE
```

## Pull Requests

Todo cambio entra por Pull Request hacia `develop` y requiere revisión real de al menos otro integrante. La descripción debe incluir:

- objetivo y requisitos afectados;
- decisiones o ADR relacionados;
- pruebas ejecutadas y resultado;
- riesgos, pendientes y evidencia relevante;
- impacto sobre documentación, configuración o datos de prueba.

No aprobar cambios que oculten errores, incorporen secretos, rompan la secuencia de clon limpio o no tengan pruebas cuando agregan comportamiento.

## Calidad mínima

Antes de solicitar revisión:

1. leer el módulo y los requisitos afectados;
2. actualizar documentación y trazabilidad;
3. ejecutar el comando de validación aplicable;
4. revisar que no haya secretos, `bin/`, `obj/` ni datos personales;
5. mantener el cambio dentro del alcance de la rama.

Las decisiones arquitectónicas se registran en un ADR al momento de decidir y siempre incluyen alternativas reales descartadas.
