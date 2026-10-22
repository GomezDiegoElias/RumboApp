# Matriz de trazabilidad

La matriz completa debe crecer con cada implementación. Esta primera versión fija el dueño principal y la evidencia esperada por bloque.

| Bloque/requisitos | Módulo principal | Evidencia |
|---|---|---|
| B1: arranque, lock y comandos | M11 | README, lock, clon limpio |
| B2: hexagonal y adaptadores | M2, M4, M9 | arquitectura, tests de dominio, NetArchTest |
| B3: configuración, health, shutdown | M0, M11 | tests de arranque y demostración SIGTERM |
| B4: errores y observabilidad | M0 | ProblemDetails, logs y correlación |
| B5: seguridad | M0, M1, M5, M9 | tests de auth, autorización, entrada, inyección y rate limit |
| B6: concurrencia e idempotencia | M3, M4, M5, M9 | ADR, tests de 50+ operaciones y estado final |
| B7: tarea pesada | M5, M6, M8 | hipótesis, carga, p50/p95/p99 y memoria |
| B8: tests | M9 | comando único y suites separadas |
| V-01..V-08: plataforma | M1, M3, M5, M6 | API, integración y demo |
| V-10..V-13: asignación | M2, M4, M5, M9 | ADR y concurrencia |
| V-20..V-23: ubicación | M6, M5, M9 | adaptadores, retención y autorización |
| V-30..V-32: tarifa, ofertas, tiempo real | M2, M3, M5 | dominio, workers, API y ADR |
| V-40..V-43: carga masiva | M6, M8, M9 | simulador y medición comparativa |
| B-01..B-04: programados | M2, M3, M4, M5, M9 | EXCLUDE, scheduler y carreras |

Cada fila debe ampliarse a nivel de `REQ-*` cuando se implemente el módulo y debe incluir el nombre exacto del test y del artefacto.

