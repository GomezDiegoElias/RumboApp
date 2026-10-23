# Architecture Decision Records

Cada decisión se guarda en un archivo numerado `NNN-titulo.md`, se escribe cuando se decide y contiene:

1. título que nombre la decisión;
2. estado (`Propuesto`, `Aceptado` o `Reemplazado por ...`);
3. fecha y participantes;
4. contexto y restricciones;
5. al menos dos alternativas reales con ventajas y desventajas;
6. decisión;
7. consecuencias;
8. condición que haría cambiar la decisión.

## Catálogo inicial

Los siguientes ADR deben crearse como parte del diseño, sin marcarlos aceptados hasta resolver los pendientes correspondientes:

1. estructura de solución .NET (reemplazo de módulos CommonJS/ESM);
2. persistencia y acceso a datos;
3. autenticación y autorización;
4. asignación concurrente de viaje y chofer;
5. aislamiento de la tarea pesada;
6. tiempo real;
7. búsqueda geográfica;
8. candidatos para viajes futuros;
9. capas frente a hexagonal por servicio;
10. AppHost frente a Docker Compose.

No crear ADR de relleno: cada uno debe documentar alternativas descartadas reales.

