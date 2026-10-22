# Backlog por controles

| Control | Resultado mínimo | Módulos prioritarios | Evidencia |
|---|---|---|---|
| 1. Diseño | modelo, puertos, ADR de estructura y persistencia | M2, M4, M10, M11 | ADR, modelo, diagrama, solución reproducible |
| 2. Núcleo | flujos principales, dominio sin BD, auth, autorización y configuración | M0, M1, M2, M3, M5, M9 | tests y demo |
| 3. Problema difícil | carrera principal, variante B y medición ingenua/final | M4, M5, M6, M8, M9 | tests concurrentes y mediciones |
| 4. Congelamiento | sistema accesible, usuarios y API publicados | M7, M10, M11 | README, API, entorno publicado |
| 5. Ataque cruzado | ataque y defensa documentados | M0, M1, M5, M9, M10 | `docs/ataque.md`, tests y commits |
| 6. Entrega final | correcciones, ADR, mediciones y tag | todos | checklist y `entrega-final` |
| 7. Defensa | comprensión individual | todos | historial de commits y demo offline |

No comenzar por el frontend opcional. La prioridad es seguridad, dominio, concurrencia, reproducibilidad y evidencia.

