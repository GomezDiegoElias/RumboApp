# Reglas de desarrollo de calidad

## Principios

- La especificación y los `REQ-*` son la fuente de alcance; no se implementan supuestos silenciosos.
- Cada cambio debe ser pequeño, explicable por todos los integrantes y acompañado por evidencia.
- Fallar cerrado en seguridad, autorización, cobro y dependencias críticas.
- No ocultar excepciones ni usar valores por defecto que disimulen configuración inválida.
- No mezclar refactors amplios con cambios funcionales.
- `develop` es la rama de integración; `main` se reserva para versiones estables o entregables.
- Dividir los cambios normales en commits pequeños, coherentes y autocontenidos; un commit grande solo se admite para cargas iniciales, migraciones o consolidaciones solicitadas explícitamente.

## Vibe coding responsable

La IA puede ayudar a explorar, implementar, probar y documentar, pero el integrante que firma el cambio debe poder explicar cada línea relevante. Antes de aceptar una propuesta generada:

1. identificar el requisito y el límite del cambio;
2. revisar patrones existentes y dependencias;
3. pedir o escribir tests que fallen sin la solución;
4. inspeccionar seguridad, concurrencia, cancelación y errores;
5. ejecutar validaciones y registrar comandos;
6. actualizar ADR, API, modelo o trazabilidad si corresponde.

No pegar código generado sin revisar. No delegar a la IA decisiones pendientes de la cátedra ni inventar requisitos.

## Definición de terminado

Un cambio está terminado cuando compila, tiene pruebas proporcionales, actualiza documentación, pasa análisis y no rompe la secuencia de clon limpio. Para requisitos de concurrencia o seguridad, la prueba debe verificar el estado final o la denegación efectiva, no solo la ausencia de excepciones.

## Configuración y secretos

Toda configuración operativa proviene del entorno y se valida al iniciar. Los secretos solo se manejan localmente o mediante el mecanismo aprobado; el repositorio contiene únicamente ejemplos. Revisar historial antes de entregar.

## Evidencia reproducible

Los comandos de build, test, migración, carga y medición deben estar documentados y poder ejecutarse desde una máquina limpia. Las mediciones incluyen hipótesis previa, entorno, repeticiones, percentiles p50/p95/p99 y conclusión.
