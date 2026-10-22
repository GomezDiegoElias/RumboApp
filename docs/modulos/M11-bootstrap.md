# M11 — Infraestructura de arranque

**Alcance:** solución, AppHost/Compose, variables, `.env.example`, migraciones, datos de prueba, lock de paquetes y prueba de clon limpio.  
**Requisitos:** REQ-B1-01..04, REQ-G-01..08.  
**Dependencias:** decisiones P1 y configuración de M0.

**Tareas:** resolver por escrito AppHost frente a Compose; documentar comandos desarrollo/producción/preparación/tests/análisis; automatizar esquema y datos; probar en otra máquina; auditar dependencias y secretos.  
**Aceptación:** una máquina limpia ejecuta la secuencia de la especificación sin preguntas; valores inválidos fallan de forma explícita; no se versionan secretos ni artefactos de build.  
**Tests:** restore locked, arranque, preparación idempotente, smoke de servicios y checklist de clon limpio.

