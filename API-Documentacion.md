# 📄 API REST - Documentación de Endpoints

**Base URL:**
`http://localhost:5064/`

Todos los endpoints, salvo el login, requieren el header:
`Authorization: Bearer <token>`

---

## Autenticación

### Login

- **Método:** `POST`
- **Ruta:** `/api/login`
- **Tipo de envío:** `application/json`
- **Parámetros (body):**
  - `usuario`: Tecnico
  - `clave`: Tecnico123!
- **Respuesta:** `string` (token JWT)
- **Descripción:** Inicia sesión y devuelve un token de autorización. El token incluye el rol del usuario (por ejemplo `Tecnico`) y es necesario para acceder al resto de los endpoints.

---

## Préstamos

### Listar préstamos pendientes

- **Método:** `GET`
- **Ruta:** `/api/prestamos`
- **Headers:** `Authorization: Bearer <token>`
- **Parámetros (query, opcionales):** `pageIndex`, `pageSize`
- **Respuesta:** `{ total, items }` (lista de `Prestamo`)
- **Descripción:** Devuelve los préstamos que todavía no fueron devueltos (`FechaDevolucionReal` es nulo).

### Listar préstamos vencidos

- **Método:** `GET`
- **Ruta:** `/api/prestamos/vencidos`
- **Headers:** `Authorization: Bearer <token>`
- **Respuesta:** `{ total, items }` (lista de `Prestamo`)
- **Descripción:** Devuelve los préstamos sin devolver cuya fecha estimada de devolución ya pasó.

### Registrar devolución

- **Método:** `POST`
- **Ruta:** `/api/prestamos/{id}/devolucion`
- **Headers:** `Authorization: Bearer <token>`
- **Cuerpo:** ninguno
- **Respuesta:** `{ mensaje }` (200) / `{ error }` (400 si ya estaba devuelto) / 404 si no existe
- **Descripción:** Marca el préstamo como devuelto (fecha de devolución real = ahora) y pone el equipo asociado como "Disponible".

---

## Solicitudes

### Listar solicitudes pendientes

- **Método:** `GET`
- **Ruta:** `/api/solicitudes`
- **Headers:** `Authorization: Bearer <token>`
- **Parámetros (query, opcionales):** `pageIndex`, `pageSize`
- **Respuesta:** `{ total, items }` (lista de `Solicitud`)
- **Descripción:** Devuelve las solicitudes en estado `Pendiente`.

### Aprobar solicitud

- **Método:** `POST`
- **Ruta:** `/api/solicitudes/{id}/aprobar`
- **Headers:** `Authorization: Bearer <token>`
- **Cuerpo:** ninguno
- **Respuesta:** `{ mensaje }` (200) / `{ error }` (400 si no estaba Pendiente) / 404 si no existe
- **Descripción:** Cambia el estado de la solicitud a `Aprobada`. Solo se permite si estaba en estado `Pendiente`.

### Rechazar solicitud

- **Método:** `POST`
- **Ruta:** `/api/solicitudes/{id}/rechazar`
- **Headers:** `Authorization: Bearer <token>`
- **Cuerpo:** ninguno
- **Respuesta:** `{ mensaje }` (200) / `{ error }` (400 si no estaba Pendiente) / 404 si no existe
- **Descripción:** Cambia el estado de la solicitud a `Rechazada`. Solo se permite si estaba en estado `Pendiente`.

---

## Notas de seguridad

- Todos los endpoints (excepto el login) requieren rol `Tecnico` o `Admin` (policy `TecnicoOAdmin`).
