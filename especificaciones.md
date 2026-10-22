# RumboApp — Especificación consolidada del TP Final (Programación IV, UTN FRRe, 2026)

> **Propósito de este documento:** es la fuente única de verdad del proyecto, resumida y ordenada a partir del enunciado oficial (versión 1.0, 23/09/2026) y de las decisiones técnicas tomadas hasta ahora. Está pensado para que un agente de IA lo divida en módulos y genere la documentación y el plan de trabajo.
>
> **Regla de oro del enunciado:** *todo lo que se evalúa está escrito; si no está, no se exige; si está, se exige.* Ante cualquier duda, el enunciado oficial manda sobre este resumen.

---

## 0. Instrucciones para el agente

1. Lee el documento completo antes de generar nada.
2. Divide el proyecto en **módulos** (ver sección 13 como punto de partida) y, por cada módulo, genera: alcance, requisitos con su ID, dependencias con otros módulos, tareas, criterios de aceptación y tests exigidos.
3. Genera la documentación obligatoria listada en la sección 12 (ADR, `modelo.md`, `api.md`, `mediciones.md`, `ataque.md`, README), **con plantillas y contenido inicial** donde ya se pueda deducir, y marcando claramente qué debe completar el equipo.
4. **No inventes decisiones.** Donde hay un punto en la sección 3 (*pendiente de confirmar con la cátedra*), propón opciones y márcalo como pendiente.
5. Respeta el orden del cronograma (sección 11): lo que se pide en cada control condiciona la prioridad.
6. Todo ADR debe tener **alternativas descartadas reales**. Un ADR sin ellas no cuenta.
7. El equipo debe poder **explicar cada línea** en la defensa oral individual. Prefiere código simple y explicable a código ingenioso. Documenta el *por qué*.
8. Los requisitos están numerados (`REQ-…`) para poder referenciarlos en tareas, tests y commits.

---

## 1. Contexto y objetivo

### 1.1 Qué se construye
Una **plataforma de viajes**: los pasajeros solicitan viajes y el sistema los asigna a choferes disponibles cercanos. El primero que acepta lo toma. El viaje avanza por estados hasta completarse, y pasajero y chofer ven el estado en tiempo real.

### 1.2 Qué evalúa la materia (no es un CRUD)
No se evalúa el caso feliz. Se evalúa que el sistema funcione correctamente bajo condiciones adversas: muchos pedidos simultáneos, usuarios malintencionados, servicios externos que fallan y tareas pesadas. Tres ejes:

1. **El sistema:** cumple los requisitos comunes y los del proyecto, con tests, y funciona en una máquina limpia.
2. **La evidencia:** ADR, informe de mediciones y bitácora de ataque.
3. **La comprensión individual:** defensa oral individual (predicción, diagnóstico de una falla introducida, modificación en vivo, justificación de un ADR).

Un sistema que solo expone CRUD sin resolver los problemas de la sección 6 obtiene la nota mínima.

### 1.3 Equipo y variante
- **Proyecto:** Plataforma de viajes (sección 7 de este documento).
- **Grupo:** 2 de la comisión de viajes, 3 integrantes (Bizjan Kevin, Flores Sebastian, Gomez Diego).
- **Variante obligatoria:** **B — Viajes programados** (sección 8).
- **Alcance:** grupo de tres → sin reducción de alcance; se cumple todo.

### 1.4 Fuera de alcance
- Frontend: **opcional y sin nota**. Para la demo basta una colección de requests (Bruno, Insomnia, Postman o `.http`) o una UI mínima.
- Despliegue en nube paga: no se exige. Basta con levantar en una máquina del laboratorio y que esté accesible durante la semana de ataque cruzado.
- App móvil y entrenamiento de modelos de IA: no se piden.

### 1.5 Uso de IA
Permitido para programar, sin declararlo. Lo que no se delega es **entender**: la defensa es individual, se introduce una falla en el código, y los artefactos exigen decisiones y mediciones propias.

---

## 2. Decisiones técnicas ya tomadas

El enunciado original está escrito para Node.js/Express. **El equipo eligió .NET** (ver pendientes en sección 3 para la validación con la cátedra).

### 2.1 Stack
| Aspecto | Decisión |
|---|---|
| Runtime | .NET 10 (`net10.0`), SDK `10.0.402` fijado en `global.json` (`rollForward: latestFeature`) |
| Orquestación | .NET Aspire 13.x (`Aspire.AppHost.Sdk` 13.6.3) |
| Base de datos | PostgreSQL, una base por servicio (`identitydb`, `locationdb`, `tripsdb`), con imagen **PostGIS** (incluye `btree_gist`) |
| Acceso a datos | EF Core + Npgsql (a justificar en ADR; SQL crudo donde enseñe un mecanismo) |
| Solución | `RumboApp.slnx` |
| Centralización | `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`, `.gitignore`, `global.json` |
| Lock de dependencias | `RestorePackagesWithLockFile=true` → `packages.lock.json` versionado; restore con `--locked-mode` |
| Auditoría de paquetes | `NuGetAudit` (modo `all`, nivel `high`) + `dotnet list package --vulnerable --include-transitive` |

### 2.2 Arquitectura de microservicios

```
/rumbo-app
  /src
    RumboApp.AppHost             # orquestador Aspire
    RumboApp.ServiceDefaults     # OpenTelemetry, health, resiliencia, service discovery
    /RumboApp.Identity           # Servicio 1 — arquitectura en capas
        Identity.Api · Identity.Business · Identity.Contracts · Identity.Data · Identity.Entities
    /RumboApp.Location           # Servicio 2 — arquitectura en capas
        Location.Api · Location.Business · Location.Contracts · Location.Data · Location.Entities
    /RumboApp.Trips              # Servicio 3 — arquitectura HEXAGONAL
        Trips.Domain · Trips.Application · Trips.Infrastructure · Trips.Api
    /RumboApp.Web                # Blazor (opcional, sin nota)
  /tests
  /docs
  RumboApp.slnx
```

**Dirección de dependencias**
- Capas (Identity, Location): `Api → Business → Data → Entities`; `Business → Contracts`; `Api` referencia `Data` solo para composición.
- Hexagonal (Trips): `Api → Infrastructure → Application → Domain`. **`Trips.Domain` no tiene ninguna referencia ni `PackageReference`.**
- El AppHost referencia solo los proyectos de entrada (`*.Api` y `Web`). La clase generada es `Projects.Identity_Api`, `Projects.Location_Api`, `Projects.Trips_Api`, `Projects.RumboApp_Web`.
- Los servicios **no** se referencian entre sí; se comunican por HTTP con service discovery (`https+http://location`). `Contracts` no se comparte entre servicios.

### 2.3 Reparto de responsabilidades (decisión de diseño clave)
- **Trips** es dueño de viajes, ofertas, **y del estado activo/ocupado del chofer**. La asignación es **una sola transacción** en su propia base (no cruza servicios).
- **Location** solo ingesta posiciones y responde "candidatos cercanos". **No decide nada transaccional.**
- **Identity** gestiona usuarios y roles, y emite JWT; los demás servicios validan el JWT localmente (clave/authority compartida vía parámetro del AppHost).

### 2.4 Estado del repositorio al momento de redactar
- Hecho: estructura de solución, AppHost, ServiceDefaults, Postgres + 3 bases + pgAdmin, los 4 servicios levantan en verde en el dashboard de Aspire, `Directory.Build.props` base, división de proyectos de Identity/Location/Trips.
- Propuesto (verificar su estado real en el repo): código inicial de `Trips.Domain` (`TripState`, `TripStateMachine`, `Trip`, errores de dominio, `Money`, `GeoPoint`, puertos) y un test de máquina de estados que recorre todas las combinaciones.
- Pendiente: todo lo demás (ver sección 13).

---

## 3. Puntos pendientes de confirmar con la cátedra

Las siguientes adaptaciones Node → .NET **no están cubiertas literalmente** por el enunciado. Hay que pedir **confirmación por escrito** y registrarlo en un ADR.

| # | Tema | Propuesta del equipo |
|---|---|---|
| P1 | **`docker compose up` como comando único** (REQ-B1-02). Aspire levanta con `dotnet run` sobre el AppHost. | Aceptar el AppHost como equivalente, o generar un `docker-compose.yml` (p. ej. con `Aspire.Hosting.Docker`). Definir antes del control 1. |
| P2 | **Hash de contraseñas:** el enunciado exige argon2 o bcrypt; el hasher por defecto de ASP.NET Identity es PBKDF2. | Usar `BCrypt.Net-Next` con costo declarado. |
| P3 | **Prototype pollution:** no existe en .NET, pero el test es obligatorio. | Test que envía `__proto__` y `constructor.prototype`; verificar rechazo con `UnmappedMemberHandling.Disallow` y ADR explicando por qué la clase de ataque no aplica. |
| P4 | **Lag del event loop** (métrica central del bloque 7) es específica de Node. | Equivalente: *thread pool starvation* (`ThreadPool.PendingWorkItemCount`, latencia p50/p95/p99 del endpoint liviano, GC y memoria vía `dotnet-counters`). |
| P5 | **Hexagonal en todo el proyecto:** el enunciado habla de hexagonal con puertos de doble adaptador; aquí Identity y Location usan capas. | Confirmar que es aceptable. Si no, migrar primero **Location** (tiene el puerto de búsqueda con dos adaptadores). |
| P6 | **ADR 1 "Sistema de módulos (CommonJS/ESM)"** no aplica a .NET. | Reemplazarlo por otra decisión equivalente (p. ej. Minimal APIs vs controllers, o .NET 10 + estructura de solución). |
| P7 | **Idioma/estructura de carpetas sugerida** (`src/dominio`, etc.) es solo sugerencia. | Usar la estructura .NET de la sección 2.2. |
| P8 | **Dos servicios de datos distintos** vs. "un solo repositorio": se cumple (un repo). | Sin acción; solo dejarlo claro en el README. |

---

## 4. Reglas generales

### 4.1 Repositorio y entrega
- **REQ-G-01** Un único repositorio Git **privado**, con la cátedra como colaboradora desde el primer control.
- **REQ-G-02** Tag `entrega-final` marca el estado que se corrige; lo posterior no se mira.
- **REQ-G-03** Todos los integrantes con commits propios, distribuidos durante el cuatrimestre. Mensajes que digan qué cambió y por qué.
- **REQ-G-04** Integración por **pull requests** revisados por al menos otro integrante, con comentarios reales.
- **REQ-G-05** Sin secretos reales en el repo **ni en su historial**. Existe `.env.example` con todas las variables y valores de ejemplo.
- **REQ-G-06** Sin `bin/`, `obj/` ni volcados con datos personales reales.
- **REQ-G-07** Reutilizar código previo es válido si se declara en el README y se puede explicar.

### 4.2 Secuencia de corrección en clon limpio (CRÍTICA)
Si esta secuencia falla, **el trabajo no se aprueba**, sin importar el código. Debe funcionar sin preguntas, en una máquina que nunca vio el proyecto:

1. Copiar `.env.example` a `.env` y ajustar, como máximo, valores obvios.
2. Levantar los servicios (`docker compose up` o equivalente aprobado, ver P1).
3. Ejecutar un comando de preparación que crea el esquema y carga datos de prueba.
4. Ejecutar los tests con **un comando único**, y que pasen.
5. Levantar el servidor y ejecutar la colección de requests de la demo.

El equipo debe probarla en **otra máquina** antes de entregar.

### 4.3 Documentación mínima del repositorio
- `README.md`: qué hace el sistema (5 líneas), comandos exactos de la secuencia anterior, **roles y usuarios de prueba con credenciales**, índice de `docs/`, tabla de roles y permisos, justificación de vulnerabilidades de dependencias si queda alguna.
- `docs/adr/`: decisiones (sección 9.1).
- `docs/mediciones.md`: informe de medición (sección 9.2).
- `docs/ataque.md`: bitácora de ataque y respuestas (sección 10).
- `docs/modelo.md`: modelo de dominio (entidades, invariantes, estados) con diagrama.
- `docs/api.md` **o** especificación OpenAPI: endpoints, códigos de respuesta y errores posibles.

---

## 5. Conceptos que se evalúan (glosario)

- **Condición de carrera:** dos o más operaciones sobre el mismo dato cuyo resultado depende del orden de intercalado. Ser monohilo no la evita: se mueve a la base de datos. Resoluciones aceptadas: (a) transacción con aislamiento adecuado, (b) bloqueo explícito de fila (`SELECT … FOR UPDATE`), (c) restricción de la base que haga imposible el estado inválido (`CHECK`, único, **`EXCLUDE`**), (d) concurrencia optimista con número de versión. Se evalúa saber cuál se eligió, por qué y qué pasa cuando dos pedidos chocan.
- **Idempotencia:** ejecutar varias veces con los mismos datos produce el mismo resultado que una vez. Mecanismo exigido: encabezado **`Idempotency-Key`**; el servidor guarda la clave con el resultado y, si llega de nuevo, devuelve el resultado guardado sin re-ejecutar. Se demuestra con un test que envía la misma petición dos veces y verifica que el efecto ocurrió una vez.
- **Bloqueo / lag (tarea pesada):** una tarea que ocupa el hilo degrada a todos. En Node son cuatro salidas (chunking, streams, worker threads, cola aparte). En .NET el análogo es evitar *thread pool starvation* y CPU bound en el hilo del request (ver P4).
- **Arquitectura hexagonal:** el dominio no conoce framework web, driver de BD ni servicios externos. **Puerto** = interfaz definida por el dominio en términos del dominio (no menciona SQL ni HTTP). **Adaptador** = implementación concreta (real y falso/en memoria). **Composición** = único lugar que crea adaptadores e inyecta. Prueba práctica: se puede correr la lógica de negocio sin servidor ni BD, y se cambia un adaptador tocando solo la composición.
- **ADR:** registro corto de una decisión, opciones consideradas y por qué se eligió una; escrito **en el momento** de decidir.
- **Percentiles:** se informa siempre **p50, p95 y p99**, nunca solo el promedio.
- **Fallar cerrado:** si un servicio del que depende una decisión de seguridad/cobro/acceso no responde, se **niega** y se registra. Nunca fallar abierto.

---

## 6. Requisitos comunes (8 bloques obligatorios)

> Un bloque **ausente por completo** limita la nota de la entrega grupal a 5 y es condición que impide aprobar. Cada requisito incluye cómo se demuestra y su traducción a .NET.

### Bloque 1 — Proyecto, dependencias y arranque
- **REQ-B1-01** Proyecto que arranca en máquina limpia siguiendo el README.
- **REQ-B1-02** Servicios auxiliares levantados con un solo comando (ver P1).
- **REQ-B1-03** Archivo de bloqueo de dependencias versionado (`packages.lock.json`, restore `--locked-mode`).
- **REQ-B1-04** Comandos habituales documentados/scripteados: arrancar en desarrollo, arrancar en producción, preparar la base, cargar datos de prueba, correr tests, correr linter/analizadores.
- *Defensa:* diferencia entre dependencias de desarrollo y producción; por qué una versión flotante puede cambiar entre instalaciones (en .NET: rangos de versión sin lock file).

### Bloque 2 — Arquitectura hexagonal
- **REQ-B2-01** Ningún archivo del dominio referencia infraestructura (verificable leyendo el `.csproj`/`using`; reforzar con **NetArchTest**).
- **REQ-B2-02** Toda dependencia externa (BD, mapas/ubicación, correo/notificaciones, pagos, reloj) entra por un **puerto con al menos dos adaptadores**: real y falso/en memoria.
- **REQ-B2-03** **Un único archivo/método de composición** que crea adaptadores y los inyecta. Cambiar real ↔ falso toca solo ese lugar o una variable de entorno.
- *Demostración:* tests de dominio sin BD ni HTTP. *Defensa:* reemplazar el adaptador de notificaciones por uno que escriba a archivo, en pocas líneas y en un solo lugar.

### Bloque 3 — Configuración y ciclo de vida del proceso
- **REQ-B3-01** Toda la configuración viene de variables de entorno; **ningún** valor de conexión, clave o URL en el código.
- **REQ-B3-02** Validación al arrancar con esquema (`Options` + `ValidateDataAnnotations().ValidateOnStart()`): si falta o es inválida, mensaje que indica **exactamente qué variable**, y termina con **código de salida ≠ 0**. No arranca a medias.
- **REQ-B3-03** Dos endpoints distintos: **`/health`** (proceso vivo) y **`/ready`** (BD disponible **y migraciones aplicadas**). No responden lo mismo.
- **REQ-B3-04** Apagado ordenado ante SIGTERM: deja de aceptar conexiones, termina las en curso, cierra conexiones a BD y sale; si excede un tiempo límite definido (`HostOptions.ShutdownTimeout`), sale igual y lo registra.
- *Demostración:* borrar una variable y mostrar el error; mandar SIGTERM durante un pedido lento y mostrar en el log que terminó antes de salir.

### Bloque 4 — Manejo de errores y observabilidad
- **REQ-B4-01** **Un único** manejador global de errores (`IExceptionHandler` + `ProblemDetails`) consistente en todo el proyecto. Ninguna tarea asíncrona deja excepciones sin capturar.
- **REQ-B4-02** Los errores de dominio se traducen a códigos HTTP **en la capa web**, no en el dominio.
- **REQ-B4-03** Formato de error único y documentado, con **identificador de correlación**; **nunca** stack trace ni mensaje crudo de la BD.
- **REQ-B4-04** Logs estructurados **JSON** con identificador de pedido propagado por scope (`AsyncLocal` por debajo), no pasado de función en función. El log del pedido y el del error se correlacionan por ese id.
- *Defensa:* por qué el contexto asíncrono (`AsyncLocal`/`AsyncLocalStorage`) funciona a través de `await` y una variable global/estática no.

### Bloque 5 — Seguridad
- **REQ-B5-01** Autenticación por JWT o sesiones (elección justificada en ADR). Contraseñas con **argon2 o bcrypt**, **factor de costo declarado** (ver P2). Nunca texto plano, MD5 ni SHA sin derivación.
- **REQ-B5-02** **Autorización por recurso**, siempre en servidor. Pedir un recurso ajeno cambiando el id debe devolver **403 o 404**, nunca el recurso. **Test automatizado obligatorio.**
- **REQ-B5-03** **Autorización por rol:** al menos tres roles con permisos distintos, definidos en una tabla del README. No se verifica en el frontend.
- **REQ-B5-04** **Validación de entrada** de todo (cuerpo, ruta, query, encabezados relevantes) con esquema; los **campos no declarados se rechazan**, no se ignoran (`UnmappedMemberHandling.Disallow`).
- **REQ-B5-05** **Prototype pollution:** test con `__proto__` y `constructor.prototype` en distintas posiciones; verifica rechazo. La defensa **no puede ser una lista negra de nombres** (ver P3).
- **REQ-B5-06** **Inyección SQL:** todas las consultas parametrizadas; test con carga de inyección típica en un campo de búsqueda.
- **REQ-B5-07** **Límite de tasa** en login y endpoints costosos, con respuesta **429** y encabezados informativos; test o demostración que lo dispare (`Microsoft.AspNetCore.RateLimiting`).
- **REQ-B5-08** Encabezados de seguridad configurados y **CORS restringido a orígenes concretos**.
- **REQ-B5-09** Dependencias sin vulnerabilidades altas ni críticas; si queda alguna, justificada por escrito en el README.

### Bloque 6 — Concurrencia, consistencia e idempotencia
- **REQ-B6-01** **ADR** que identifique qué operaciones pueden entrar en condición de carrera y cómo se resuelve cada una.
- **REQ-B6-02** Test automatizado con **≥ 50 pedidos simultáneos** sobre la carrera principal que **verifica el dato final** (no basta con "sin error").
- **REQ-B6-03** **Idempotencia** (`Idempotency-Key`) en todas las operaciones que **crean algo, cobran algo o disparan un efecto externo**, con test.
- *Defensa:* predecir y luego mostrar qué pasa si se quita el bloqueo/transacción.

### Bloque 7 — Tareas pesadas / aislamiento del tráfico
- **REQ-B7-01** Implementar **primero la versión ingenua** (la que degrada) y medirla.
- **REQ-B7-02** Implementar la versión que **no degrada el servicio** y justificarla en ADR.
- **REQ-B7-03** Medir ambas bajo carga (k6, NBomber o similar): latencia **p50/p95/p99** de un endpoint liviano (p. ej. `/health`), **pedidos por segundo**, equivalente al lag del event loop (ver P4) y **memoria**.
- **REQ-B7-04** Hipótesis **antes** de medir y conclusión **después**, en `docs/mediciones.md`.
- *Demostración:* medición en vivo con comandos ejecutables tal como están escritos.

### Bloque 8 — Tests
- **REQ-B8-01** **Un único comando** que corre todos los tests.
- **REQ-B8-02** **Dominio:** unitarios, sin HTTP ni BD, que cubren reglas de negocio y casos de borde.
- **REQ-B8-03** **Integración:** flujos principales contra la **BD real** en contenedor (Testcontainers o `Aspire.Hosting.Testing`), con la base en estado conocido al iniciar cada test.
- **REQ-B8-04** **Seguridad y concurrencia:** autorización por recurso, prototype pollution, inyección, límite de tasa, carrera e idempotencia.
- La cobertura **no** se evalúa como número: se evalúa **qué se eligió testear y por qué**. Un test que falla en la entrega cuenta como requisito no cumplido.

---

## 7. Dominio: plataforma de viajes

### 7.1 Roles mínimos
**Pasajero, chofer, operador** (soporte: ve viajes en curso y puede cancelar con motivo) y **administrador** (configura tarifas, zonas y parámetros).

### 7.2 Alcance funcional mínimo
- **REQ-V-01** Registro y autenticación de pasajeros y choferes; el chofer tiene datos del vehículo y un **estado de habilitación** que otorga el administrador.
- **REQ-V-02** El chofer se declara disponible/no disponible y, mientras está disponible, **envía su ubicación periódicamente**.
- **REQ-V-03** Solicitud de viaje con origen, destino y **estimación de precio y tiempo antes de confirmar**.
- **REQ-V-04** Búsqueda de choferes cercanos y **oferta con tiempo límite de aceptación**.
- **REQ-V-05** Máquina de estados del viaje: solicitado, asignado, chofer en camino, en curso, finalizado, cancelado por pasajero, cancelado por chofer, sin choferes disponibles (**más** `Programado` y `BuscandoChofer` por la variante B).
- **REQ-V-06** Seguimiento en tiempo real de estado y posición del chofer, para pasajero y chofer.
- **REQ-V-07** Cálculo de la tarifa final al terminar y **registro del cobro (simulado)**.
- **REQ-V-08** Historial de viajes por pasajero y por chofer, y panel del operador con viajes en curso.

### 7.3 Problema central (lo primero que se mira y se pregunta)
**Un viaje se asigna a un solo chofer, y un chofer tiene un solo viaje activo**, incluso con decenas de solicitudes y aceptaciones simultáneas y con cancelaciones justo mientras un chofer acepta. Es el problema de "la última unidad", pero con **dos recursos a la vez** (viaje y chofer).

- **REQ-V-10** **ADR del mecanismo de asignación:** qué se bloquea, en qué orden, qué pasa si dos choferes aceptan en el mismo milisegundo, por qué no puede haber doble asignación, y **cómo se evita el interbloqueo** (el orden de los bloqueos importa).
- **REQ-V-11** Test con **≥ 50 aceptaciones simultáneas** sobre el mismo viaje: exactamente **una** tiene éxito y las otras reciben **rechazo claro**.
- **REQ-V-12** Test de **cancelación simultánea** (pasajero cancela mientras un chofer acepta): el resultado es **uno de dos estados definidos y documentados**; nunca un viaje asignado a un chofer sin pasajero, ni un chofer ocupado sin viaje.
- **REQ-V-13** Test de que **un chofer con un viaje activo no puede aceptar otro**.

### 7.4 Búsqueda geográfica y ubicaciones
- **REQ-V-20** Ubicaciones periódicas (sugerido cada 2–5 s), soportando concurrencia; se **mide** (ver 7.6).
- **REQ-V-21** Búsqueda de cercanos con mecanismo apropiado (PostGIS, geohash o estructura en memoria), **ADR que compare ≥ 2 opciones y explique el costo de cada una**. No se acepta traer todos los choferes y calcular distancia en código sin justificarlo y medirlo.
- **REQ-V-22** **Política de retención o agregación** de ubicaciones históricas, definida y documentada.
- **REQ-V-23** El endpoint de seguimiento **no expone** la ubicación de un chofer a quien no es su pasajero actual, ni la de un pasajero a quien no es su chofer asignado. **Test obligatorio.**

### 7.5 Tarifa, oferta y tiempo real
- **REQ-V-30** **Tarifa calculada siempre en el servidor** a partir de distancia, tiempo y parámetros configurados. La **estimación y el cálculo final usan la misma función**; la diferencia queda explicada. Tests con casos concretos y bordes: distancia cero y viaje que supera el máximo permitido. Importes en **enteros (centavos) o decimal exacto**, nunca punto flotante.
- **REQ-V-31** Cada oferta tiene **tiempo límite** (sugerido 15–30 s). Si el chofer no responde, la oferta vence y se ofrece al siguiente candidato; si se agota la lista, el viaje pasa a *sin choferes disponibles*. **El vencimiento ocurre aunque el chofer haya cerrado la app** (no depende del cliente → worker en servidor).
- **REQ-V-32** Tiempo real por **WebSocket/SignalR o SSE** (elección justificada en ADR). Documentar **qué pasa al cortarse y reconectarse la conexión**: el cliente debe poder recuperar el estado actual sin quedar desincronizado.

### 7.6 Tarea pesada obligatoria
Recepción masiva de ubicaciones **combinada** con búsqueda de candidatos.

- **REQ-V-40** **Simulador propio** de **≥ 500 choferes** enviando ubicación cada 2 s, sostenido varios minutos.
- **REQ-V-41** Medir **con y sin la solución elegida**: latencia p50/p95/p99 del endpoint de **solicitud de viaje**, **tiempo hasta la asignación**, lag/starvation, pedidos por segundo y memoria.
- **REQ-V-42** Demostrar que, bajo esa carga, **las asignaciones ocurren en un tiempo razonable definido y declarado por el grupo** (el número lo eligen; se evalúa que se haya medido y se cumpla).
- **REQ-V-43** Implementar y medir también la **versión ingenua** (sin ella no hay comparación).

### 7.7 Qué se muestra en la demo (toda la comisión)
- Ciclo completo de un viaje con seguimiento en tiempo real del pasajero.
- Test de 50 aceptaciones simultáneas sobre el mismo viaje.
- Caso de cancelación simultánea a la aceptación.
- Oferta que vence y pasa al siguiente chofer **con la app del chofer cerrada**.
- Intento de ver la ubicación de un viaje ajeno, rechazado.
- Carga de 500 choferes corriendo, con medición de latencia y lag antes y después de la solución.
- **La variante obligatoria del grupo, con su condición de carrera.**

---

## 8. Variante B — Viajes programados (obligatoria para este grupo)

La variante **se suma** al alcance común y trae su propia condición de carrera, que también hay que resolver y testear.

### 8.1 Requisitos
- **REQ-B-01** El pasajero **agenda un viaje para una fecha y hora futura**. El sistema **busca chofer con la antelación configurada**.
- **REQ-B-02** Un viaje programado **no puede pisar un viaje en curso del mismo chofer**, ni dos programados pueden solaparse para el mismo chofer.
- **REQ-B-03** Si no se encuentra chofer, **el pasajero se entera con antelación suficiente**, no en el momento (aviso idempotente a un umbral configurable).
- **REQ-B-04** **Carrera propia (dos casos):**
  1. Dos viajes programados que se solapan intentan asignarse al **mismo chofer a la vez**.
  2. Un viaje programado **empieza a buscar chofer mientras ese chofer acepta un viaje inmediato**.

### 8.2 Diseño propuesto (a validar en ADR)
- **Tabla `driver_commitments`** con intervalo de tiempo por chofer y restricción de exclusión en PostgreSQL:
  ```sql
  CREATE EXTENSION IF NOT EXISTS btree_gist;
  ALTER TABLE driver_commitments
    ADD CONSTRAINT no_overlap
    EXCLUDE USING gist (driver_id WITH =, period WITH &&);
  ```
  Tanto la **aceptación de un viaje inmediato** como la **asignación de uno programado** insertan en esta misma tabla. El segundo `INSERT` conflictivo falla con SQLSTATE `23P01`, que el adaptador traduce a un error de dominio (`DriverNotAvailable`). Así ambas carreras se resuelven con **un solo mecanismo** y **sin interbloqueo** (cada operación toma una sola restricción, no dos bloqueos en orden distinto).
- Para un viaje inmediato la duración se desconoce: documentar una regla (p. ej. `[ahora, ahora + duración estimada + margen]`) y cerrar el rango real al finalizar.
- **Worker de programados** (`BackgroundService`) que funcione sin que nadie abra la app: consulta con `FOR UPDATE SKIP LOCKED` para que dos instancias no tomen el mismo viaje; segundo umbral para el aviso al pasajero (columna tipo `no_driver_notified_at` para idempotencia).
- **Tiempo:** todo en UTC (`timestamptz`) y `TimeProvider` como puerto de reloj (`FakeTimeProvider` en tests).
- **Candidatos para un viaje futuro:** no sirve la ubicación actual. Decidir y justificar en ADR (última ubicación conocida, zona habilitada, o choferes habilitados sin conflicto en la ventana).
- **Máquina de estados:** `Scheduled → SearchingDriver → Assigned | NoDriversAvailable`, con cancelación posible desde `Scheduled`.

### 8.3 Tests específicos
- 50 intentos paralelos de asignar **dos programados solapados** al mismo chofer → exactamente uno tiene éxito; se verifica la tabla.
- **Programado vs. inmediato** en paralelo → estado final verificado (nunca dos compromisos solapados, nunca un chofer ocupado sin viaje).
- Programado **no puede pisar** un viaje en curso.
- **Scheduler con `FakeTimeProvider`:** avanza el reloj, verifica que busca chofer y que avisa a tiempo.
- Todo con **Postgres real** (Testcontainers). Un test de concurrencia con base en memoria no demuestra nada.

---

## 9. Artefactos de proceso (20 % de la nota)

### 9.1 ADR — mínimo 6, uno por archivo, numerados, en `docs/adr/`
Obligatorios para todos (adaptados):
1. Sistema de módulos → **reemplazar** por decisión equivalente en .NET (ver P6).
2. **Persistencia y acceso a datos:** base elegida y EF Core vs. SQL directo vs. query builder.
3. **Autenticación y autorización:** JWT vs. sesiones y cómo se verifica el acceso por recurso.
4. **Resolución de la condición de carrera principal** (asignación viaje/chofer + variante B).
5. **Solución de la tarea pesada** (recepción masiva de ubicaciones + búsqueda).
6. Uno o más a elección: tiempo real (SignalR vs. SSE), búsqueda geográfica (PostGIS vs. geohash vs. memoria), candidatos para viajes futuros, capas vs. hexagonal por servicio, `docker compose` vs. AppHost, etc.

**Formato obligatorio de cada ADR:**
- **Título:** una frase que nombra la *decisión* (ej.: "Usamos una restricción EXCLUDE para impedir solapamientos de choferes"), no el tema ("Concurrencia").
- **Estado:** propuesto, aceptado o reemplazado por (nº).
- **Fecha y quiénes participaron.**
- **Contexto:** problema y restricciones.
- **Opciones consideradas:** al menos dos, con ventajas y desventajas **reales**.
- **Decisión:** una o dos oraciones.
- **Consecuencias:** qué gana, qué pierde, qué queda más difícil.
- **Qué nos haría cambiar de decisión.**

> Un ADR **sin alternativas descartadas no cuenta**. Escribirlos **cuando se decide**, no después.

### 9.2 Informe de medición — `docs/mediciones.md`
Cubre la tarea pesada y la condición de carrera principal. Para cada una:
1. **Hipótesis** escrita **antes** de medir, **con un número**.
2. **Entorno:** máquina, SO, versión de .NET, versión de la BD, si corría en Docker, qué más corría.
3. **Comando exacto**, copiable y ejecutable.
4. **Resultados** en tabla para versión ingenua y final: p50, p95, p99, req/s, lag/starvation (máx y p99) y memoria, **con unidades**.
5. **Conclusión** honesta. Una hipótesis refutada con números es un buen resultado; ajustarla después de ver los datos es un mal resultado.

Reglas de método: **≥ 30 s** de carga sostenida, **≥ 3 repeticiones** por escenario (registrar las tres), línea de base sin la tarea pesada, misma carga de datos en ambas corridas, nada más corriendo en la máquina, sin capturas sin contexto.

### 9.3 Historial de trabajo
Es el historial de Git: commits de todos distribuidos en el tiempo, mensajes claros y PRs con revisión real.

---

## 10. Fase de ataque cruzado (semana 11 aprox.)

- **Emparejamiento:** el grupo *n* ataca al *n+1*; el último ataca al primero. Cada grupo es atacante de uno y defensor de otro.
- **Qué se busca:** acceso a recursos ajenos cambiando ids; escalada de privilegios; inyección (SQL, prototype pollution); romper consistencia con concurrencia (asignar dos veces, cobrar dos veces); saltear validaciones (campos no declarados, tipos raros, cuerpos enormes); reutilizar/falsificar credenciales (tokens vencidos, de otro usuario, reenvío de pedidos); degradar con una operación costosa disparada pocas veces.

### 10.1 Reglas (cumplimiento obligatorio; incumplirlas invalida la fase y es falta académica)
1. Atacar **solo** el entorno publicado por el defensor, con sus usuarios de prueba.
2. Prohibido atacar infraestructura compartida del laboratorio o la red de la facultad.
3. Prohibidos los **ataques de volumen** para tirar el servicio; sí se permite demostrar que una operación costosa degrada con pocos pedidos.
4. Prohibido acceder al repositorio, máquinas o cuentas del otro grupo: **caja negra**.
5. Prohibida la ingeniería social.
6. Si un ataque provoca caída o pérdida de datos, **avisar de inmediato** al defensor y a la cátedra y documentarlo.
7. Lo encontrado se usa solo para este trabajo.

### 10.2 Bitácora del atacante — `docs/ataque.md` (una entrada por intento, **incluidos los fallidos**)
Campos: número y fecha · hipótesis · petición exacta (copiable) · resultado esperado · resultado obtenido · veredicto (hallazgo confirmado / defensa efectiva / no concluyente) · severidad (alta/media/baja con justificación) · impacto.

### 10.3 Respuesta del defensor (mismo archivo), por cada hallazgo confirmado
Severidad acordada o discutida · **causa raíz** (no basta "faltaba una validación"; explicar por qué faltaba) · **commit que lo corrige** (id) · **test automatizado que falla antes y pasa después** · si no se corrige, justificación explícita del riesgo aceptado.

### 10.4 Puntuación
El atacante suma por hallazgos confirmados, calidad de bitácora e intentos bien documentados aunque fallen. El defensor no pierde por tener vulnerabilidades sino por **no corregirlas, no entender la causa o no poder explicar cómo entró el atacante**. **Tener el entorno arriba esa semana es responsabilidad del defensor.**

---

## 11. Cronograma y controles

Los controles no tienen nota propia, pero uno salteado descuenta del 10 % de "historial y controles".

| Control | Semana | Qué se muestra |
|---|---|---|
| 1. Diseño | 2 | Repo con la cátedra como colaboradora, **modelo de dominio, puertos definidos, ADR de módulos (o su reemplazo) y de persistencia** |
| 2. Núcleo | 5 | Flujos principales funcionando, **tests de dominio sin BD**, autenticación y **autorización por recurso**, configuración validada al arrancar |
| 3. Problema difícil | 8 | **Carrera principal resuelta con test concurrente.** Tarea pesada en sus dos versiones con primera medición |
| 4. Congelamiento | 10 | Sistema desplegado, accesible y congelado. Usuarios de prueba y documentación de API publicados para el grupo atacante |
| 5. Ataque cruzado | 11 | Bitácora del atacante entregada. El defensor recibe hallazgos y empieza a corregir |
| 6. Entrega final | 12 | Correcciones con tests, informe de medición completo, todos los ADR, **tag `entrega-final`** |
| 7. Defensa oral | 13 | Defensa individual, en grupo |

*(Las fechas exactas las publica la cátedra en el aula virtual; vale esa fecha ante cualquier diferencia.)*

---

## 12. Evaluación

### 12.1 Composición de la nota (individual)
| Componente | Peso | Alcance |
|---|---|---|
| Entrega grupal (sistema y requisitos) | 40 % | Grupal |
| Artefactos de proceso | 20 % | Grupal |
| Defensa oral | 40 % | **Individual** |

Se aprueba con **6 o más**. La **defensa oral requiere 6 o más por sí sola.**

### 12.2 Rúbrica de la entrega grupal (40 %)
| Criterio | Peso |
|---|---|
| Problema central del proyecto (resuelto, con tests, demostrable) | 30 % |
| Seguridad (bloque 5, autorización por recurso, resultado de ataques recibidos) | 20 % |
| Concurrencia e idempotencia (bloque 6) | 20 % |
| Arquitectura y configuración (bloques 2 y 3) | 15 % |
| Tests, documentación y reproducibilidad (bloques 1, 4 y 8) | 15 % |

### 12.3 Rúbrica de artefactos (20 %)
| Criterio | Peso |
|---|---|
| ADR con alternativas reales (6 o más) | 30 % |
| Informe de medición | 30 % |
| Fase de ataque cruzado | 30 % |
| Historial de Git y controles | 10 % |

> **Si no se llega con la variante:** se descuenta del problema central (30 % de la entrega). Conviene entregar la variante con alcance mínimo y bien resuelta antes que a medio hacer.

### 12.4 Defensa oral — cuatro tipos de pregunta
- **Predicción:** qué pasa ante una entrada u orden de eventos concreto; se predice en voz alta y luego se ejecuta. Se evalúa el razonamiento.
- **Diagnóstico:** el docente introduce una falla (quitar un `await`, quitar el bloqueo de asignación, aflojar una validación, invertir una condición de autorización, cambiar el orden de middlewares) y hay que encontrarla desde el síntoma.
- **Modificación en vivo:** cambio chico de requisito (nueva regla, cambiar adaptador de notificaciones, agregar un rol, cambiar criterio de orden). Se evalúa si la arquitectura lo permite en pocos lugares.
- **Justificación:** defender un ADR frente a la alternativa descartada.

Reglas: cada integrante es preguntado sobre código que figure a su nombre en Git; no se puede consultar IA ni internet durante la defensa. Se lleva el sistema corriendo con datos de prueba, la colección de requests, los tests, el generador de carga y toda la documentación accesible **sin internet**.

---

## 13. Condiciones que impiden aprobar (sin compensación)

1. El sistema no levanta siguiendo el README desde un clon limpio (sección 4.2).
2. Hay **secretos reales** versionados en el repo o su historial.
3. Un integrante no puede explicar código que figura a su nombre.
4. Falta por completo alguno de los **ocho bloques** de requisitos comunes.
5. No se realizó la fase de ataque cruzado (ni como atacante ni como defensor).
6. Se incumplió alguna regla de la sección 10.1.

---

## 14. Propuesta de módulos (punto de partida para el agente)

> El agente debe refinar esta división, no ignorarla. Cada módulo debe tener alcance, requisitos (`REQ-…`), tareas, tests y criterios de aceptación.

| Módulo | Contenido | Servicio/proyecto | Control objetivo |
|---|---|---|---|
| **M0 — Transversal** | Opciones validadas al arranque, `/health` y `/ready`, `IExceptionHandler`+`ProblemDetails`+correlation id, logs JSON, rate limiting, headers de seguridad, CORS, middleware de **idempotencia**, apagado ordenado | `ServiceDefaults` | 2 |
| **M1 — Identity** | Registro/login de los 4 roles, JWT con claims de rol, BCrypt con costo declarado, rate limit en login, tabla de roles y permisos | `RumboApp.Identity` | 2 |
| **M2 — Trips.Domain** | `Trip`, `Driver`, `Offer`, `DriverCommitment`, `Fare`; máquina de estados (incl. `Scheduled`/`SearchingDriver`); reglas de tarifa; errores de dominio; **puertos** | `Trips.Domain` | 1 |
| **M3 — Trips.Application** | Casos de uso: solicitar, ofrecer, aceptar, cancelar, iniciar, finalizar, agendar programado, buscar chofer, tarifa final, cobro simulado, historial, consulta del operador | `Trips.Application` | 2 |
| **M4 — Trips.Infrastructure** | DbContext, migraciones, repositorios, `driver_commitments` con `EXCLUDE`, traducción de `23P01`, `IUnitOfWork`, adaptadores reales y falsos, cliente HTTP hacia Location | `Trips.Infrastructure` | 2–3 |
| **M5 — Trips.Api y workers** | Endpoints con validación, autorización por recurso, worker de ofertas (vencimiento), worker de programados (`SKIP LOCKED` + aviso), tiempo real (SignalR/SSE) con reconexión, seguimiento restringido | `Trips.Api` | 3 |
| **M6 — Location** | Ingesta de posiciones (upsert + historial con retención), búsqueda de cercanos con **dos adaptadores** (ingenuo y PostGIS), escritura por lotes (`Channel<T>`) | `RumboApp.Location` | 3 |
| **M7 — Web (opcional)** | Login, solicitar viaje, estado en tiempo real, vistas de chofer/operador | `RumboApp.Web` | 4+ |
| **M8 — Herramientas y mediciones** | Simulador de 500 choferes, scripts de carga, endpoint de diagnóstico (thread pool, GC, memoria), hipótesis y corridas | `/tools`, `docs/mediciones.md` | 3 |
| **M9 — Tests** | Dominio, integración (Postgres real), concurrencia, seguridad, arquitectura (NetArchTest); comando único | `/tests` | 2–3 |
| **M10 — Documentación** | ADR, `modelo.md`, `api.md`, `mediciones.md`, `ataque.md`, README con secuencia de arranque y usuarios de prueba | `/docs` | 1 en adelante |
| **M11 — Infra de arranque** | Resolución de P1 (`docker compose`/AppHost), `.env.example`, comando de preparación (esquema + datos de prueba), prueba en clon limpio | raíz | 1 y 4 |

### Estructura de tests propuesta
```
/tests
  RumboApp.Trips.Domain.Tests          # sin BD ni HTTP
  RumboApp.Trips.Integration.Tests     # Testcontainers / Aspire.Hosting.Testing
  RumboApp.Concurrency.Tests           # 50 aceptaciones, cancelación vs aceptación, variante B
  RumboApp.Security.Tests              # recurso ajeno, inyección, campos no declarados, __proto__, 429, idempotencia
  RumboApp.Architecture.Tests          # NetArchTest: Domain no referencia infraestructura
```

---

## 15. Entregables que se espera que genere el agente

1. **Plan por módulos** (un archivo por módulo) con tareas ordenadas y dependencias.
2. **Matriz de trazabilidad:** cada `REQ-…` → módulo → test que lo verifica → artefacto/documento.
3. **Borradores de ADR** (con alternativas) para: persistencia, autenticación, asignación y variante B (`EXCLUDE`), tarea pesada, tiempo real, búsqueda geográfica, candidatos de viajes futuros, capas vs hexagonal, `docker compose` vs AppHost.
4. **`docs/modelo.md`** con entidades, invariantes y diagrama de estados (Mermaid).
5. **`docs/api.md`** (o plan de OpenAPI) con endpoints, códigos y errores.
6. **Plantilla de `docs/mediciones.md`** y de **`docs/ataque.md`**.
7. **README** con secuencia de arranque, roles, permisos y usuarios de prueba.
8. **Checklist de autoevaluación** (sección 16) convertido en tareas verificables.
9. **Backlog por control** (1 a 7) alineado con el cronograma.
10. **Lista de preguntas abiertas** para la cátedra (sección 3), redactada lista para enviar.

---

## 16. Checklist de autoevaluación antes de entregar

- [ ] Clon limpio en otra máquina: la secuencia de 4.2 funciona tocando solo `.env`.
- [ ] Todos los tests pasan y un solo comando los corre.
- [ ] Borrar una variable de entorno hace fallar el arranque con mensaje claro y código ≠ 0.
- [ ] SIGTERM durante un pedido lento: el pedido termina antes de que el proceso salga.
- [ ] Acceso a recurso ajeno por id devuelve 403/404 y hay test.
- [ ] El test concurrente de la carrera principal verifica el **dato final**.
- [ ] Misma operación dos veces con la misma `Idempotency-Key`: el efecto ocurre una sola vez.
- [ ] Tarea pesada medida en ambas versiones, con números y comandos en `docs/mediciones.md`.
- [ ] Al menos 6 ADR, todos con alternativas descartadas.
- [ ] Bitácora de ataque completa y hallazgos recibidos respondidos, con tests.
- [ ] Sin secretos en repo ni historial; existe `.env.example` completo.
- [ ] `dotnet list package --vulnerable --include-transitive` sin altas/críticas (o justificadas en el README).
- [ ] Variante B con su carrera resuelta, testeada y demostrable.
- [ ] Cada integrante puede explicar, sin ayuda, el fragmento más complicado del sistema.

---

## 17. Errores frecuentes (aprendidos de años anteriores)

- Empezar por el frontend (no suma nota y consume el tiempo del bloque 6).
- Dejar la seguridad para el final: la autorización por recurso atraviesa todos los endpoints.
- Confundir **autenticación** con **autorización**: verificar el token no es verificar que el recurso sea de quien lo pide.
- Escribir los ADR después de decidir (se nota: no tienen alternativas).
- Medir una sola vez, con tres pedidos, sin línea de base.
- Creer que ser monohilo evita las carreras (las mueve a la base de datos).
- Usar `double`/`float` para dinero.
- Que **un solo integrante** entienda el proyecto (la defensa es individual).
- No probar el clon limpio hasta el día de la entrega.