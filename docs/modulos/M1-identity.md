# M1 — Identity

**Alcance:** registro, autenticación, roles pasajero/chofer/operador/administrador, habilitación de choferes y emisión de JWT.  
**Requisitos:** REQ-B5-01, REQ-B5-03 y REQ-V-01.  
**Dependencias:** M0; contratos propios del servicio.

**Tareas:** modelar usuario, roles y vehículo; decidir JWT/sesiones en ADR; implementar hash bcrypt/argon2 según confirmación P2; claims y validación; límites de login; datos de prueba.  
**Aceptación:** permisos distintos están documentados; credenciales nunca se almacenan en texto plano; tokens inválidos, vencidos o insuficientes son rechazados.  
**Tests:** registro/login, roles, costo del hash, rate limit, token vencido y usuario no habilitado.

