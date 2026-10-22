# M3 — Trips.Application

**Alcance:** casos de uso de solicitar, agendar, ofertar, aceptar, cancelar, iniciar, finalizar, cobrar simulado, historial y operador.  
**Requisitos:** REQ-V-03..08, REQ-B-01..03, REQ-B6-03.  
**Dependencias:** M0, M2; puertos implementados por M4 y M6.

**Tareas:** orquestar casos de uso sin conocer adaptadores; aplicar autorización de negocio; delimitar transacciones; propagar idempotencia y errores de dominio; definir DTOs de aplicación.  
**Aceptación:** cada caso de uso tiene una prueba aislada; efectos externos y cobros son idempotentes; falla cerrado ante dependencia crítica.  
**Tests:** flujos felices y fallos, reintentos, estados inválidos, cancelación y permisos por recurso.

