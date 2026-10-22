# M8 — Herramientas y mediciones

**Alcance:** simulador de 500 choferes, cargas de carrera, scripts de medición y diagnóstico de thread pool/GC/memoria.  
**Requisitos:** REQ-B7-01..04, REQ-V-40..43.  
**Dependencias:** M5, M6 y M9.

**Tareas:** definir hipótesis numéricas antes de medir; crear versión ingenua y final; documentar comandos; sostener carga al menos 30 s y repetir tres veces; guardar resultados con unidades.  
**Aceptación:** mismos datos y carga en ambas versiones; se informan p50/p95/p99, req/s, starvation/lag y memoria; conclusión honesta.  
**Tests:** smoke del simulador, reproducibilidad de parámetros y validación de formato de resultados.

