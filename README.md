# RumboApp

Plataforma de viajes de la comisión de Programación IV (UTN FRRe, 2026). El sistema permite solicitar, programar, asignar y seguir viajes, con foco en seguridad, concurrencia, idempotencia, observabilidad y medición de tareas pesadas.

## Estado actual

Este repositorio se encuentra en fase de organización documental. La fuente funcional completa es [`especificaciones.md`](especificaciones.md). No iniciar implementación sin revisar el módulo correspondiente, sus dependencias y los pendientes de la cátedra.

## Documentación

- [Mapa documental y plan modular](docs/README.md)
- [Reglas de desarrollo de calidad](docs/quality.md)
- [Flujo de trabajo y convenciones Git](CONTRIBUTING.md)
- [Backlog por controles](docs/backlog-controles.md)
- [Matriz de trazabilidad](docs/matriz-trazabilidad.md)
- [Preguntas abiertas para la cátedra](docs/preguntas-catedra.md)
- [Checklist de preparación](docs/checklist.md)
- [Decisiones arquitectónicas (ADR)](docs/adr/README.md)

## Regla de precedencia

El enunciado oficial de la cátedra prevalece sobre cualquier resumen o decisión del repositorio. Las decisiones todavía no confirmadas están registradas en [`docs/preguntas-catedra.md`](docs/preguntas-catedra.md) y no deben tratarse como requisitos cerrados.

## Estructura

```text
src/       código de la solución
tests/     pruebas automatizadas
docs/      planificación, arquitectura y evidencias
tools/     simuladores y herramientas de medición (cuando corresponda)
```

