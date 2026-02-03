using Microsoft.EntityFrameworkCore;
using SCA_MVC.Models;

namespace SCA_MVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor: recibe opciones de configuración
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets: representan las tablas en la BD
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Lugar> Lugares { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Registro> Registros { get; set; }

        // OnModelCreating: configuración de relaciones con Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== CONFIGURACIÓN DE RELACIONES =====

            // Relación: Empresa → Empleados (1:N)
            // Una empresa tiene muchos empleados, un empleado pertenece a una empresa
            modelBuilder.Entity<Empresa>()
                .HasMany(e => e.Empleados)           // Una Empresa tiene muchos Empleados
                .WithOne(emp => emp.Empresa)         // Cada Empleado tiene una Empresa
                .HasForeignKey(emp => emp.IdEmpresa) // La FK es IdEmpresa
                .OnDelete(DeleteBehavior.Restrict)   // NO permitir eliminar Empresa si tiene Empleados
                .HasConstraintName("FK_Empleados_Empresa"); // Nombre del constraint en BD

            // Relación: Empresa → Registros (1:N)
            // Una empresa tiene muchos registros, un registro pertenece a una empresa
            modelBuilder.Entity<Empresa>()
                .HasMany(e => e.Registros)
                .WithOne(r => r.Empresa)
                .HasForeignKey(r => r.IdEmpresa)
                .OnDelete(DeleteBehavior.Restrict)   // Preservar registros históricos
                .HasConstraintName("FK_Registros_Empresa");

            // Relación: Empleado → Registros (1:N, NULLABLE)
            // Un empleado puede tener muchos registros, un registro puede tener empleado (o ser invitado)
            // ⚠️ IMPORTANTE: IdEmpleado es nullable para permitir invitados
            modelBuilder.Entity<Empleado>()
                .HasMany(e => e.Registros)
                .WithOne(r => r.Empleado)
                .HasForeignKey(r => r.IdEmpleado)
                .OnDelete(DeleteBehavior.SetNull)    // Si eliminas empleado, IdEmpleado = null
                .IsRequired(false)                    // La relación es opcional (permite invitados)
                .HasConstraintName("FK_Registros_Empleado");

            // Relación: Lugar → Servicios (1:N)
            // Un lugar tiene muchos servicios, un servicio pertenece a un lugar
            modelBuilder.Entity<Lugar>()
                .HasMany(l => l.Servicios)
                .WithOne(s => s.Lugar)
                .HasForeignKey(s => s.IdLugar)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Servicios_Lugar");

            // Relación: Lugar → Registros (1:N)
            // Un lugar tiene muchos registros, un registro pertenece a un lugar
            modelBuilder.Entity<Lugar>()
                .HasMany(l => l.Registros)
                .WithOne(r => r.Lugar)
                .HasForeignKey(r => r.IdLugar)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Registros_Lugar");

            // Relación: Servicio → Registros (1:N)
            // Un servicio tiene muchos registros, un registro pertenece a un servicio
            modelBuilder.Entity<Servicio>()
                .HasMany(s => s.Registros)
                .WithOne(r => r.Servicio)
                .HasForeignKey(r => r.IdServicio)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Registros_Servicio");

            // ===== CONFIGURACIÓN DE ÍNDICES ÚNICOS =====

            // Índice único en Empleado.IdCredencial
            // Garantiza que no haya dos empleados con la misma credencial RFID
            modelBuilder.Entity<Empleado>()
                .HasIndex(e => e.IdCredencial)
                .IsUnique()
                .HasDatabaseName("IX_Empleado_IdCredencial");

            // Índice único compuesto en Registro (IdEmpleado, IdServicio)
            // Evita que un empleado se registre dos veces en el mismo servicio
            // ⚠️ IMPORTANTE: HasFilter para excluir nulls (permite múltiples invitados)
            modelBuilder.Entity<Registro>()
                .HasIndex(r => new { r.IdEmpleado, r.IdServicio })
                .IsUnique()
                .HasFilter("[IdEmpleado] IS NOT NULL")  // Solo aplica cuando IdEmpleado no es null
                .HasDatabaseName("IX_Registro_Empleado_Servicio");

            // ===== CONFIGURACIÓN DE VALORES POR DEFECTO =====

            // Valores por defecto para Estado = true
            modelBuilder.Entity<Empresa>()
                .Property(e => e.Estado)
                .HasDefaultValue(true);

            modelBuilder.Entity<Empleado>()
                .Property(e => e.Estado)
                .HasDefaultValue(true);

            modelBuilder.Entity<Lugar>()
                .Property(l => l.Estado)
                .HasDefaultValue(true);

            // Valores por defecto para totales = 0
            modelBuilder.Entity<Servicio>()
                .Property(s => s.TotalComensales)
                .HasDefaultValue(0);

            modelBuilder.Entity<Servicio>()
                .Property(s => s.TotalInvitados)
                .HasDefaultValue(0);

            // ===== CONFIGURACIÓN DE CHECK CONSTRAINTS =====

            // Check constraint: Fecha no puede ser futura
            // Validación a nivel de base de datos
            modelBuilder.Entity<Servicio>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Servicio_Fecha",
                    "[Fecha] <= CAST(GETDATE() AS DATE)"));

            // ===== CONFIGURACIÓN DE ÍNDICES DE PERFORMANCE =====

            // Índice compuesto para búsquedas por Fecha y Lugar
            // Optimiza consultas como "servicios de un lugar en una fecha"
            modelBuilder.Entity<Servicio>()
                .HasIndex(s => new { s.Fecha, s.IdLugar })
                .HasDatabaseName("IX_Servicio_Fecha_Lugar");

            // Índice para búsquedas por Fecha en Registros
            // Optimiza consultas como "registros de una fecha específica"
            modelBuilder.Entity<Registro>()
                .HasIndex(r => r.Fecha)
                .HasDatabaseName("IX_Registro_Fecha");
        }
    }
}
