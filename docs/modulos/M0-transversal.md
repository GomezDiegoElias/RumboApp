# M0 — Transversal

**Alcance:** configuración validada al arranque, health/readiness, errores, correlación, logs JSON, rate limiting, headers, CORS, idempotencia y apagado ordenado.  
**Requisitos:** REQ-B3-01..04, REQ-B4-01..04, REQ-B5-07..09, REQ-B6-03.  
**Dependencias:** ninguna funcional; usado por M1, M3, M5 y M11.

**Tareas:** definir opciones y variables; configurar `IExceptionHandler`/ProblemDetails; correlation id y scopes; `/health`/`/ready`; políticas de seguridad; middleware de idempotencia; shutdown y errores explícitos.  
**Aceptación:** configuración inválida impide arrancar; health y ready difieren; errores no filtran secretos; repetir una operación con la misma clave produce un único efecto.  
**Tests:** arranque inválido, endpoint ready sin BD, formato de error, correlación, 429, idempotencia y SIGTERM.

