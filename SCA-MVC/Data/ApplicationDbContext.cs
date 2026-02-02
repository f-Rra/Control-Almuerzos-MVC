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
        }
    }
}
