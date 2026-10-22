# Instrucciones para asistentes de código

- Leer `especificaciones.md` y `docs/README.md` antes de proponer cambios.
- Asumir `develop` como rama base: las ramas de tarea parten de `develop` y los Pull Requests apuntan a `develop`; `main` solo recibe promociones estables o entregas.
- No desarrollar fuera del módulo solicitado ni cerrar pendientes de `docs/preguntas-catedra.md` por inferencia.
- Buscar requisitos `REQ-*`, registrar trazabilidad y respetar las dependencias del módulo.
- Preferir cambios pequeños, explícitos, tipados y coherentes con los patrones existentes.
- En cambios normales, crear varios commits pequeños y autocontenidos por intención; no agrupar producción, tests y documentación no relacionada. Aceptar un único commit grande solo cuando el usuario lo solicite explícitamente.
- No introducir secretos, valores de conexión hardcodeados, silenciamiento de excepciones ni fallos abiertos.
- Para comportamiento nuevo o corregido, agregar pruebas enfocadas; para concurrencia, verificar el dato final.
- Actualizar ADR, API, modelo, mediciones o bitácora cuando el cambio los afecte.
- No modificar código de producción durante una tarea explícitamente documental.
- Antes de informar terminado, ejecutar la validación mínima relevante y reportar cualquier bloqueo.
