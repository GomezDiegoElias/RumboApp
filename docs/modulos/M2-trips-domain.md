# M2 — Trips.Domain

**Estado:** primera iteración implementada en `feat/trips-domain-foundation`: `Trip`, `TripState`, `TripStateMachine`, `Money`, `GeoPoint`, errores, puertos y tests unitarios sin infraestructura.
**Alcance:** entidades, value objects, máquina de estados, tarifa, invariantes, errores de dominio y puertos.  
**Requisitos:** REQ-B2-01..03, REQ-V-05, REQ-V-10, REQ-V-30, REQ-B-02..04.  
**Dependencias:** ninguna de infraestructura, framework o paquetes externos.

**Tareas:** completar estados inmediatos/programados; modelar viaje, oferta, compromiso y tarifa exacta; decidir reglas de intervalos y tiempo UTC. Los puertos iniciales son `IClock`, `ITripRepository` e `IDriverCandidateProvider`, con contratos expresados en términos del dominio.
**Aceptación:** el dominio corre sin HTTP ni BD; estados inválidos no son representables o son rechazados; dinero no usa punto flotante; ningún `.csproj` referencia infraestructura.  
**Tests:** transiciones, bordes de tarifa, cancelaciones, solapamientos y reloj falso.
