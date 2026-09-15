# IT Lend — Sistema de Préstamo de Equipos

Sistema web desarrollado en **ASP.NET Core MVC** para la gestión de préstamos de equipos informáticos dentro de una empresa: solicitudes de empleados, aprobación/rechazo por técnicos o administradores, seguimiento de préstamos y devoluciones, y una API REST con autenticación JWT para uso de los técnicos.

## Características Principales

- **Gestión de Equipos**: catálogo por categoría, estado (Disponible/Prestado/En Mantenimiento/Baja) y archivo de garantía adjunto
- **Gestión de Solicitudes**: los empleados piden equipos, técnicos/admins aprueban o rechazan
- **Gestión de Préstamos**: registro de devoluciones, filtro de pendientes/devueltos/vencidos y busqueda por dni de empleado
- **Sistema de Usuarios**: roles Admin / Técnico / Empleado, con avatar de perfil
- **API REST con JWT**: pensada para que los técnicos gestionen préstamos y solicitudes fuera del sistema web (ver `API-Documentacion.md`)

## Tecnologías Utilizadas

- **Backend**: ASP.NET Core (.NET 10) MVC
- **Base de Datos**: MySQL
- **Frontend**: HTML5, CSS3, Bootstrap 5, JavaScript, Vue.js (ABM de Categorías), Select2 (búsqueda Ajax)
- **Autenticación**: Cookie Authentication (sistema web) + JWT Bearer (API REST)
- **Patrones y Arquitectura**: Diseño en N-Capas (Controladores, Servicios, Repositorios), Filtros Globales de Excepciones (IExceptionFilter), y manejo de transacciones con TransactionScope.

## Requisitos Previos

- Visual Studio 2022 o VS Code
- .NET 10 SDK
- MySQL Server (local)
- Git

## Configuración del Proyecto

### 1. Clonar el repositorio
```bash
git clone https://github.com/Debora36/final-LAB2.git
cd final-LAB2
```

### 2. Configurar la base de datos

Este proyecto se conecta a una base de datos **MySQL local**. Los pasos:

1. Creá la base ejecutando el script `itlenddb.sql` incluido en la raíz del proyecto contra tu instancia local de MySQL.
2. En `appsettings.json` (o `appsettings.Development.json`), configurá tu cadena de conexión local en:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=itlenddb;User=TU_USUARIO;Password=TU_CONTRASEÑA;"
   }
   ```

### 3. Restaurar dependencias
```bash
dotnet restore
```

### 4. Ejecutar el proyecto
```bash
dotnet run
```

La aplicación va a estar disponible en `http://localhost:5064` (verificá el puerto real en la consola al ejecutar).

## Pruebas Unitarias

El proyecto incluye un proyecto de tests (`final_LAB2.Tests`) con pruebas unitarias sobre la capa de servicios usando **xUnit** y **Moq**. Se cubren los servicios `SolicitudService`, `PrestamoService`, `UsuarioService` y `EmpleadoService`.

Para correrlas:
```bash
cd final_LAB2.Tests
dotnet test
```

## Roles y Permisos

- **Admin**: acceso completo, incluye gestión de usuarios y edición de roles
- **Técnico**: gestiona equipos, préstamos y solicitudes (vía sistema web y vía API)
- **Empleado**: crea sus propias solicitudes y ve sus propios préstamos

## Estructura del Proyecto

```
final-LAB2/
├── Controllers/
│   └── Api/             # Controllers de la API REST (JWT)
├──Filters               # Filtros globales (GlobalExceptionFilter)
├── Models/
│   └── ViewModels/
├── Views/
├── Repository/          # Acceso a datos (MySqlConnector)
├── Services/            # Lógica de negocio
├── wwwroot/
└── itlenddb.sql         # Script de base de datos
```

## API REST

El sistema expone una API REST autenticada con JWT para uso de los técnicos. Ver el detalle completo de endpoints en [`API-Documentacion.md`](./API-Documentacion.md).

## Cumplimiento de Requisitos Mínimos

| Requisito | Dónde está desarrollado |
|---|---|
| 4+ clases relacionadas con al menos una relación 1 a muchos | `Models/` (`Usuario`, `Empleado`, `Categoria`, `Equipo`, `Solicitud`, `Prestamo`) — relaciones 1 a N: Categoría→Equipo, Categoría→Solicitud, Empleado→Solicitud, Empleado→Préstamo, Equipo→Préstamo |
| Seguridad con login, `[Authorize]` y roles, funcionalidad restringida por rol | Login vía Cookie Authentication (`Program.cs`); `[Authorize(Roles = "...")]` en `EquipoController`, `PrestamoController`, `SolicitudController`, `UsuarioController`; ejemplos de restricción por rol: solo Admin edita roles de usuario, solo Admin/Técnico aprueban o rechazan solicitudes |
| Avatar en los usuarios | `Usuario.AvatarUrl`, carga de imagen en `Views/Usuario/Edit.cshtml` |
| Uso de archivos adicional al avatar | Archivo de garantía por equipo: `Equipo.RutaArchivoGarantia`, carga/reemplazo en `Views/Equipo/Edit.cshtml` |
| ABM con Vue.js y funcionalidad vía Ajax | Gestión de **Categorías** |
| Listados con paginado (server-side, por página) | `Prestamo`, `Solicitud` y `Equipo` — `ObtenerPaginado` en Repository/Service, con filtros por estado |
| Búsqueda vía Ajax de entidad relacionada (sin traer todos los elementos) | Selección de **Equipo** al aprobar una **Solicitud**: Select2 + `GET /Equipo/BuscarDisponibles?q=...&categoriaId=...`, con `LIMIT 10` en la consulta SQL | búsqueda de préstamos por DNI de empleado en el Home |
| Uso de API con JWT | API REST completa en `Controllers/Api/` (login, préstamos, solicitudes) — ver `API-Documentacion.md` |

## Notas de Seguridad

- Las contraseñas de los usuarios están hasheadas; cada usuario cambia la propia desde su perfil (no se permite que un Admin cambie la contraseña de otro usuario).
- Los endpoints de la API validan rol (`Tecnico`/`Admin`) además del token JWT.
- Las excepciones de negocio (por ejemplo, préstamo ya devuelto o solicitud ya resuelta) se devuelven como errores controlados (`400`) tanto en la API como en el sistema web, sin exponer detalles internos del servidor.
- Filtro de Excepciones Global (IExceptionFilter).
