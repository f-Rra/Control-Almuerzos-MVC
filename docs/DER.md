# Modelo de datos

Diagrama entidad-relación de SCA-MVC, armado a partir del modelo de EF Core (el último snapshot de las migraciones). Muestra las seis tablas del negocio y las tres de ASP.NET Core Identity que la aplicación usa para los usuarios y sus roles.

```mermaid
erDiagram
    EMPRESA ||--o{ EMPLEADO : tiene
    EMPRESA ||--o{ REGISTRO : "de la empresa"
    EMPLEADO |o--o{ REGISTRO : "almuerza en"
    LUGAR ||--o{ SERVICIO : "se hace en"
    LUGAR ||--o{ REGISTRO : "en el lugar"
    LUGAR ||--o{ LECTOR : "instalado en"
    SERVICIO ||--o{ REGISTRO : contiene
    ASPNETUSERS ||--o{ ASPNETUSERROLES : tiene
    ASPNETROLES ||--o{ ASPNETUSERROLES : "se asigna en"

    EMPRESA {
        int IdEmpresa PK
        nvarchar(100) Nombre
        bit Estado "por defecto 1"
        int CantidadEmpleados
    }
    EMPLEADO {
        int IdEmpleado PK
        nvarchar(100) Nombre
        nvarchar(100) Apellido
        nvarchar(50) IdCredencial UK "la del RFID"
        int IdEmpresa FK
        bit Estado "por defecto 1"
    }
    LUGAR {
        int IdLugar PK
        nvarchar(50) Nombre
        bit Estado "por defecto 1"
    }
    SERVICIO {
        int IdServicio PK
        int IdLugar FK
        datetime2 Fecha "CHECK hasta 2030"
        datetime2 HoraInicio "nulo"
        int Proyeccion "nulo"
        int DuracionMinutos "nulo mientras está activo"
        int TotalComensales "por defecto 0"
        int TotalInvitados "por defecto 0"
    }
    REGISTRO {
        int IdRegistro PK
        int IdEmpleado FK "nulo en los invitados"
        int IdEmpresa FK
        int IdServicio FK
        int IdLugar FK
        datetime2 Fecha
        time Hora
    }
    LECTOR {
        int IdLector PK
        nvarchar(100) Nombre
        int IdLugar FK
        nvarchar(64) ApiKeyHash UK "SHA-256"
        bit Activo "por defecto 1"
        datetime2 FechaAlta
    }
    ASPNETUSERS {
        nvarchar(450) Id PK
        nvarchar(256) UserName UK
        nvarchar(256) Email
        nvarchar(max) PasswordHash "PBKDF2"
        nvarchar(max) Nombre
        nvarchar(max) Apellido
        datetimeoffset LockoutEnd "nulo; bloquea al usuario"
    }
    ASPNETROLES {
        nvarchar(450) Id PK
        nvarchar(256) Name "Admin, Usuario"
    }
    ASPNETUSERROLES {
        nvarchar(450) UserId PK, FK
        nvarchar(450) RoleId PK, FK
    }
```

## Restricciones

| Qué | Dónde | Para qué |
|---|---|---|
| Índice único `(IdEmpleado, IdServicio)`, ignorando los nulos | `REGISTRO` | Un empleado no almuerza dos veces en el mismo servicio; los invitados (empleado nulo) no chocan entre sí |
| Índice único `IdCredencial` | `EMPLEADO` | Una credencial RFID, un empleado |
| Índice único `ApiKeyHash` | `LECTOR` | No puede haber dos lectores con la misma clave |
| `CHECK (Fecha <= '2030-01-01')` | `SERVICIO` | Evita errores de tipeo absurdos |
| Claves foráneas con `ON DELETE RESTRICT` | Todas | La base no deja borrar algo que tiene historial |
| `ON DELETE SET NULL` | `REGISTRO.IdEmpleado` | Si se eliminara un empleado, sus asistencias no se pierden |
| Trigger `TR_ValidarRegistroUnico` (`INSTEAD OF INSERT`) | `REGISTRO` | Rechaza un registro repetido. Está en `SQL/Procedimientos_Vistas_Triggers.sql`, no en las migraciones; EF Core solo lo declara con `HasTrigger` |

Las otras cuatro tablas de Identity (`AspNetRoleClaims`, `AspNetUserClaims`, `AspNetUserLogins` y `AspNetUserTokens`) y las columnas estándar de `AspNetUsers` que no se usan acá se omitieron para que el diagrama se lea.
