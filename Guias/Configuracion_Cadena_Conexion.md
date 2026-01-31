# Configuración de la Cadena de Conexión

## 📋 Descripción General

Este documento describe la configuración de la cadena de conexión a SQL Server para el proyecto **Sistema de Control de Almuerzos MVC**.

---

## 🔧 Configuración Actual

### Archivo: `appsettings.json`

Se han configurado **dos cadenas de conexión** para diferentes escenarios de autenticación:

#### 1. **DefaultConnection** (Windows Authentication) - RECOMENDADA

```json
"DefaultConnection": "Server=localhost;Database=SistemaControlAlmuerzos;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true;Connection Timeout=30;"
```

**Uso:** Esta es la cadena de conexión **activa por defecto** y utiliza autenticación de Windows.

#### 2. **DefaultConnection_SQLAuth** (SQL Server Authentication)

```json
"DefaultConnection_SQLAuth": "Server=localhost;Database=SistemaControlAlmuerzos;User Id=sa;Password=TuPassword;TrustServerCertificate=true;MultipleActiveResultSets=true;Connection Timeout=30;"
```

**Uso:** Cadena alternativa para autenticación con usuario y contraseña de SQL Server.

---

## 📖 Explicación de Parámetros

### Parámetros Principales

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| **Server** | `localhost` | Nombre o dirección del servidor SQL Server. Puede ser `localhost`, `.`, `(local)`, o una dirección IP/nombre de red |
| **Database** | `SistemaControlAlmuerzos` | Nombre de la base de datos que se creará/utilizará |
| **Integrated Security** | `true` | Usa la autenticación de Windows del usuario actual |
| **User Id** | `sa` | Usuario de SQL Server (solo para SQL Auth) |
| **Password** | `TuPassword` | Contraseña del usuario (solo para SQL Auth) |

### Parámetros de Seguridad y Configuración

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| **TrustServerCertificate** | `true` | Permite conexiones sin validar el certificado SSL del servidor. **Útil en desarrollo local** |
| **MultipleActiveResultSets** | `true` | Permite múltiples conjuntos de resultados activos en la misma conexión |
| **Connection Timeout** | `30` | Tiempo máximo (en segundos) para establecer la conexión antes de fallar |

---

## 🔐 Tipos de Autenticación

### Windows Authentication (Integrated Security)

**✅ Ventajas:**
- No requiere gestión de credenciales en el código
- Más segura para desarrollo local
- Usa las credenciales del usuario de Windows actual
- No hay contraseñas en archivos de configuración

**❌ Desventajas:**
- Requiere que el usuario de Windows tenga permisos en SQL Server
- Menos portable entre diferentes entornos

**Cuándo usar:** Desarrollo local en Windows con SQL Server instalado localmente.

### SQL Server Authentication

**✅ Ventajas:**
- Funciona en cualquier plataforma
- Más control sobre las credenciales
- Útil para entornos de producción y contenedores

**❌ Desventajas:**
- Requiere gestión segura de contraseñas
- Las credenciales deben protegerse (usar User Secrets o variables de entorno)

**Cuándo usar:** Producción, contenedores Docker, o cuando no se puede usar Windows Authentication.

---

## 🛠️ Personalización de la Configuración

### Cambiar el Servidor SQL Server

Si tu SQL Server está en otra máquina o instancia:

```json
// SQL Server Express con instancia nombrada
"Server": ".\\SQLEXPRESS"

// SQL Server en otra máquina de la red
"Server": "192.168.1.100"

// SQL Server con puerto específico
"Server": "localhost,1433"
```

### Cambiar el Nombre de la Base de Datos

```json
"Database": "MiBaseDeDatos"
```

### Usar SQL Server Authentication

1. Cambia el nombre de la cadena de conexión en `Program.cs`:
   ```csharp
   // De:
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   
   // A:
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection_SQLAuth")));
   ```

2. Actualiza el usuario y contraseña en `appsettings.json`:
   ```json
   "DefaultConnection_SQLAuth": "Server=localhost;Database=SistemaControlAlmuerzos;User Id=miUsuario;Password=miContraseña;..."
   ```

---

## 🔒 Seguridad en Producción

### ⚠️ NUNCA incluyas contraseñas en `appsettings.json` en producción

**Opciones seguras:**

#### 1. User Secrets (Desarrollo)

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Password=MiPasswordSeguro;..."
```

#### 2. Variables de Entorno (Producción)

```bash
# Linux/Mac
export ConnectionStrings__DefaultConnection="Server=...;Password=..."

# Windows
set ConnectionStrings__DefaultConnection="Server=...;Password=..."
```

#### 3. Azure Key Vault / AWS Secrets Manager

Para entornos cloud, usa servicios de gestión de secretos.

---

## 📝 Logging de Entity Framework

En `appsettings.Development.json` se ha configurado logging detallado:

```json
"Microsoft.EntityFrameworkCore.Database.Command": "Information"
```

Esto permite ver las **consultas SQL generadas** en la consola durante el desarrollo, útil para:
- Debugging
- Optimización de consultas
- Aprendizaje de cómo EF Core traduce LINQ a SQL

**⚠️ En producción, cambia este nivel a `Warning` o `Error`** para evitar logs excesivos.

---

## ✅ Verificación de la Configuración

Para verificar que la cadena de conexión funciona correctamente:

1. Asegúrate de que SQL Server esté ejecutándose
2. Verifica que el usuario tenga permisos adecuados
3. Ejecuta las migraciones (próximo paso):
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

---

## 🔄 Próximos Pasos

Una vez configurada la cadena de conexión:

1. ✅ Paquetes NuGet instalados
2. ✅ Cadena de conexión configurada
3. ⏭️ Crear el DbContext
4. ⏭️ Definir las entidades del modelo
5. ⏭️ Ejecutar migraciones
6. ⏭️ Crear la base de datos

---

## 📚 Referencias

- [Connection Strings - Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/connection-strings)
- [Entity Framework Core - Connection Strings](https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-strings)
- [SQL Server Connection Strings](https://www.connectionstrings.com/sql-server/)
