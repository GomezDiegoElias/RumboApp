# M4 — Trips.Infrastructure

**Estado:** primera iteración implementada: `TripsDbContext`, `EfTripRepository`, composición de persistencia, validación de `FarePolicyOptions`, migración inicial versionada y una prueba de integración con PostgreSQL real en contenedor.
**Alcance:** PostgreSQL, EF Core/SQL donde corresponda, migraciones, repositorios, unidad de trabajo, `driver_commitments`, adaptadores reales/falsos y cliente hacia Location.
**Requisitos:** REQ-B2-02..03, REQ-B6-01..02, REQ-B-02..04.  
**Dependencias:** M2, M3 y M11.

**Tareas:** registrar ADR de persistencia; crear restricción EXCLUDE y traducir `23P01`; implementar `FOR UPDATE SKIP LOCKED`; composición única; Testcontainers/Aspire Testing; estado conocido por prueba.  
**Aceptación:** asignación e inmediatos/programados no generan solapamientos; los adaptadores se cambian desde composición; migraciones son reproducibles.  
**Tests:** integración con PostgreSQL real, 50+ aceptaciones, cancelación concurrente, programado vs. inmediato y scheduler.

## Estrategia de esquema

El esquema se versiona mediante migraciones de EF Core almacenadas en
`Trips.Infrastructure/Data/Migrations`. La API ejecuta `MigrateAsync` durante el
arranque y falla explícitamente si no puede aplicar una migración; no continúa con
un esquema posiblemente incompleto. Las pruebas de integración aplican las mismas
migraciones contra PostgreSQL real, evitando `EnsureCreatedAsync`.

La fábrica de diseño requiere `TRIPS_DB_CONNECTION` para generar nuevas migraciones:

```powershell
$env:TRIPS_DB_CONNECTION = "Host=localhost;Port=5432;Database=tripsdb;Username=postgres;Password=postgres"
dotnet ef migrations add <Nombre> `
  --project src\RumboApp.Trips\Trips.Infrastructure\Trips.Infrastructure.csproj `
  --startup-project src\RumboApp.Trips\Trips.Infrastructure\Trips.Infrastructure.csproj `
  --output-dir Data\Migrations
```
