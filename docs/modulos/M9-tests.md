# M9 — Tests

**Alcance:** suites de dominio, integración, concurrencia, seguridad y arquitectura, con un comando único.  
**Requisitos:** REQ-B8-01..04 y evidencias de todos los bloques.  
**Dependencias:** progresivamente M0–M6; no bloquear tests unitarios por infraestructura.

**Tareas:** separar tests sin HTTP/BD de integración; usar PostgreSQL real en carreras; agregar NetArchTest; cubrir autorización, inyección, campos desconocidos, prototype pollution, 429 e idempotencia.  
**Aceptación:** el comando único descubre todas las suites; los tests concurrentes verifican datos finales; el estado inicial de integración es conocido.  
**Tests:** esta carpeta es la propia matriz ejecutable; cada `REQ-*` debe apuntar a un test concreto al implementarse.

