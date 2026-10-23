# Mapa documental

## Cómo usar esta carpeta

1. Leer [`../especificaciones.md`](../especificaciones.md), fuente funcional consolidada.
2. Consultar el módulo que se vaya a trabajar.
3. Resolver sus dependencias y pendientes antes de implementar.
4. Actualizar requisitos, tests, evidencias y ADR junto con el cambio.

## Módulos

| ID | Documento | Alcance |
|---|---|---|
| M0 | [transversal](modulos/M0-transversal.md) | configuración, errores, observabilidad y seguridad común |
| M1 | [Identity](modulos/M1-identity.md) | usuarios, roles y autenticación |
| M2 | [Trips.Domain](modulos/M2-trips-domain.md) | dominio, estados, invariantes y puertos |
| M3 | [Trips.Application](modulos/M3-trips-application.md) | casos de uso |
| M4 | [Trips.Infrastructure](modulos/M4-trips-infrastructure.md) | persistencia y adaptadores |
| M5 | [Trips.Api y workers](modulos/M5-trips-api-workers.md) | HTTP, tiempo real y procesos de fondo |
| M6 | [Location](modulos/M6-location.md) | ubicaciones y búsqueda geográfica |
| M7 | [Web](modulos/M7-web.md) | cliente opcional de demostración |
| M8 | [herramientas y mediciones](modulos/M8-tools-measurements.md) | carga, simuladores y evidencia |
| M9 | [tests](modulos/M9-tests.md) | pruebas unitarias, integración, seguridad y arquitectura |
| M10 | [documentación](modulos/M10-documentation.md) | ADR, API, modelo, mediciones y ataque |
| M11 | [infraestructura de arranque](modulos/M11-bootstrap.md) | clon limpio, AppHost/Compose, datos de prueba |

## Documentos operativos

- [`quality.md`](quality.md): reglas de calidad y trabajo asistido por IA.
- [`backlog-controles.md`](backlog-controles.md): prioridad por control de la cátedra.
- [`matriz-trazabilidad.md`](matriz-trazabilidad.md): requisito → módulo → evidencia.
- [`checklist.md`](checklist.md): verificación antes de cada entrega.
- [`preguntas-catedra.md`](preguntas-catedra.md): decisiones pendientes.
- [`adr/README.md`](adr/README.md): formato y catálogo de decisiones.

