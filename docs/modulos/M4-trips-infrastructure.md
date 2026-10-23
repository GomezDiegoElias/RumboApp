# M4 — Trips.Infrastructure

**Estado:** primera iteración implementada: `TripsDbContext`, `EfTripRepository`, composición de persistencia y validación de `FarePolicyOptions`.
**Alcance:** PostgreSQL, EF Core/SQL donde corresponda, migraciones, repositorios, unidad de trabajo, `driver_commitments`, adaptadores reales/falsos y cliente hacia Location.
**Requisitos:** REQ-B2-02..03, REQ-B6-01..02, REQ-B-02..04.  
**Dependencias:** M2, M3 y M11.

**Tareas:** registrar ADR de persistencia; crear restricción EXCLUDE y traducir `23P01`; implementar `FOR UPDATE SKIP LOCKED`; composición única; Testcontainers/Aspire Testing; estado conocido por prueba.  
**Aceptación:** asignación e inmediatos/programados no generan solapamientos; los adaptadores se cambian desde composición; migraciones son reproducibles.  
**Tests:** integración con PostgreSQL real, 50+ aceptaciones, cancelación concurrente, programado vs. inmediato y scheduler.
