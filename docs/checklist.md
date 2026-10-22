# Checklist de autoevaluación

## Por control

- [ ] Control 1: modelo, puertos y ADR iniciales revisados.
- [ ] Control 2: núcleo, auth, autorización por recurso y configuración validada.
- [ ] Control 3: carrera principal, variante B, 50+ operaciones y mediciones iniciales.
- [ ] Control 4: entorno publicado, usuarios de prueba y API accesibles.
- [ ] Control 5: bitácora atacante y respuestas defensoras.
- [ ] Control 6: correcciones, documentación completa y tag `entrega-final`.
- [ ] Control 7: cada integrante puede explicar el código asignado.

## Antes de entregar

- [ ] Clon limpio funciona tocando solo `.env`.
- [ ] Un comando ejecuta todos los tests.
- [ ] Configuración faltante detiene el arranque con mensaje claro y código distinto de cero.
- [ ] Existen `/health` y `/ready` con semánticas diferentes.
- [ ] Autorización por recurso devuelve 403/404 y tiene test.
- [ ] Idempotencia verifica un solo efecto.
- [ ] Tarea pesada tiene versión ingenua y final, hipótesis, tres repeticiones y percentiles.
- [ ] Hay al menos seis ADR con alternativas reales.
- [ ] No hay secretos en árbol ni historial.
- [ ] La variante B tiene tests con PostgreSQL real.

