# ADR-004 — Impedir solapamientos de choferes con `EXCLUDE`

- **Estado:** Propuesto
- **Fecha:** 2026-10-22
- **Participantes:** Equipo RumboApp

## Contexto

Los viajes programados y los viajes inmediatos pueden intentar reservar el
mismo chofer en paralelo. La regla REQ-B-02 exige impedir que un chofer tenga
dos compromisos temporales solapados, incluso cuando las solicitudes llegan a
instancias distintas del servicio. La solución debe resolver la carrera en la
base de Trips, sin depender de comprobaciones previas en memoria.

## Opciones consideradas

### Opción A — Comprobación previa en la aplicación

- Ventajas:
  - Es sencilla de leer y probar en una sola instancia.
  - No requiere SQL específico de PostgreSQL.
- Desventajas:
  - Dos solicitudes concurrentes pueden observar el mismo hueco y aprobarse.
  - Requiere bloqueos adicionales y aumenta el riesgo de interbloqueos.

### Opción B — Bloqueo de filas con `FOR UPDATE`

- Ventajas:
  - Permite serializar la consulta de compromisos existentes.
  - Es una herramienta nativa de PostgreSQL.
- Desventajas:
  - No existe una fila única que represente todos los huecos posibles.
  - La implementación debe coordinar cuidadosamente transacciones y rangos.

### Opción C — Restricción de exclusión `EXCLUDE`

- Ventajas:
  - La base impide directamente cualquier estado inválido.
  - Funciona igual para aceptación inmediata y asignación programada.
  - La carrera se resuelve con una única restricción, sin ordenar múltiples
    bloqueos.
- Desventajas:
  - Requiere PostgreSQL, `btree_gist` y tipos de rango.
  - El adaptador debe traducir SQLSTATE `23P01` a un error de dominio.

## Decisión

Se propone usar `driver_commitments` con un período `tstzrange` y una
restricción `EXCLUDE USING gist (driver_id WITH =, period WITH &&)`. El
repositorio agrega el compromiso y `IUnitOfWork` confirma la operación; un
conflicto `23P01` se traduce a `DriverNotAvailableException`. La prueba de
integración con PostgreSQL debe ser la evidencia de la carrera, no una prueba
en memoria.

La duración del intervalo para un viaje inmediato y el flujo completo de
asignación quedan pendientes de cerrar en el caso de uso correspondiente.

## Consecuencias

- Los solapamientos se rechazan de forma atómica en la base de datos.
- La aplicación debe tratar `DriverNotAvailableException` como resultado
  esperado de una carrera, no como un error ocultable.
- El modelo queda acoplado a capacidades de PostgreSQL para esta invariancia.
- Las pruebas de concurrencia requieren PostgreSQL real, como Testcontainers.

## Qué haría cambiar la decisión

Se reconsiderará esta decisión si el despliegue deja de usar PostgreSQL, si la
consigna exige portabilidad entre motores sin equivalentes de `EXCLUDE`, o si
las mediciones demuestran que la contención de la restricción no cumple los
objetivos de latencia.
