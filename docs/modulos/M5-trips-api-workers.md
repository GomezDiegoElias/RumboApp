# M5 — Trips.Api y workers

**Alcance:** endpoints, validación de entrada, autorización por recurso, ofertas con vencimiento, scheduler, tiempo real y reconexión.  
**Requisitos:** REQ-B4, REQ-B5-02/04..08, REQ-V-04/06/23/31/32, REQ-B-01/03/04.  
**Dependencias:** M0, M2, M3, M4 y M6.

**Tareas:** publicar contratos y errores; implementar workers independientes del cliente; elegir SignalR/SSE en ADR; recuperar estado tras reconexión; restringir seguimiento; documentar API.  
**Aceptación:** campos desconocidos se rechazan; no se filtran recursos ajenos; una oferta vence aunque el cliente esté cerrado; dos instancias no procesan el mismo programado.  
**Tests:** API, autorización, validación, expiración, reconexión, worker con reloj falso y carreras.

