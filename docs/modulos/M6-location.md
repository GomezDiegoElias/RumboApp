# M6 — Location

**Alcance:** ingesta de posiciones, historial/retención, búsqueda de cercanos y adaptadores ingenuo y optimizado.  
**Requisitos:** REQ-B2-02, REQ-V-02, REQ-V-20..23, REQ-V-40..43.  
**Dependencias:** M0, M4 para persistencia y M8 para carga.

**Tareas:** decidir PostGIS/geohash/memoria comparando al menos dos opciones; definir retención; upsert y lotes con `Channel<T>` si se confirma; exponer candidatos sin decidir asignación.  
**Aceptación:** Location no posee reglas transaccionales de Trips; ubicación ajena queda protegida; ambas implementaciones del puerto son sustituibles y medibles.  
**Tests:** autorización de seguimiento, concurrencia de ingesta, distancia/bordes, retención y comparación de adaptadores.

