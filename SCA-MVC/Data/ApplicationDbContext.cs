using Microsoft.EntityFrameworkCore;
using SCA_MVC.Models;
using System;
using System.Collections.Generic;

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

            // ===== CONFIGURACIÓN DE CHECK CONSTRAINTS =====
            // Relajamos el check de fecha para permitir datos de prueba futuros si es necesario
            modelBuilder.Entity<Servicio>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Servicio_Fecha",
                    "[Fecha] <= '2030-01-01'"));

            // ===== SEEDING DE DATOS (HASDATA) =====
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // 1. Lugares
            modelBuilder.Entity<Lugar>().HasData(
                new Lugar { IdLugar = 1, Nombre = "Comedor", Estado = true },
                new Lugar { IdLugar = 2, Nombre = "Quincho", Estado = true }
            );

            // 2. Empresas
            modelBuilder.Entity<Empresa>().HasData(
                new Empresa { IdEmpresa = 1, Nombre = "Roemmers", Estado = true },
                new Empresa { IdEmpresa = 2, Nombre = "Gema", Estado = true },
                new Empresa { IdEmpresa = 3, Nombre = "Siegfried", Estado = true },
                new Empresa { IdEmpresa = 4, Nombre = "Gramon", Estado = true },
                new Empresa { IdEmpresa = 5, Nombre = "Simmer", Estado = true },
                new Empresa { IdEmpresa = 6, Nombre = "Ethical", Estado = true }
            );

            // 3. Empleados (Muestra representativa para no saturar el archivo de migración)
            // Se agregan los 10 de Roemmers y algunos de otras empresas como ejemplo
            var empleados = new List<Empleado>
            {
                new Empleado { IdEmpleado = 1, IdEmpresa = 1, IdCredencial = "RF001", Nombre = "Juan", Apellido = "Perez", Estado = true },
                new Empleado { IdEmpleado = 2, IdEmpresa = 1, IdCredencial = "RF002", Nombre = "María", Apellido = "García", Estado = true },
                new Empleado { IdEmpleado = 3, IdEmpresa = 1, IdCredencial = "RF003", Nombre = "Carlos", Apellido = "López", Estado = true },
                new Empleado { IdEmpleado = 4, IdEmpresa = 1, IdCredencial = "RF004", Nombre = "Ana", Apellido = "Martínez", Estado = true },
                new Empleado { IdEmpleado = 5, IdEmpresa = 1, IdCredencial = "RF005", Nombre = "Luis", Apellido = "Rodríguez", Estado = true },
                new Empleado { IdEmpleado = 6, IdEmpresa = 1, IdCredencial = "RF006", Nombre = "Sofía", Apellido = "Fernández", Estado = true },
                new Empleado { IdEmpleado = 7, IdEmpresa = 1, IdCredencial = "RF007", Nombre = "Diego", Apellido = "González", Estado = true },
                new Empleado { IdEmpleado = 8, IdEmpresa = 1, IdCredencial = "RF008", Nombre = "Valentina", Apellido = "Silva", Estado = true },
                new Empleado { IdEmpleado = 9, IdEmpresa = 1, IdCredencial = "RF009", Nombre = "Andrés", Apellido = "Morales", Estado = true },
                new Empleado { IdEmpleado = 10, IdEmpresa = 1, IdCredencial = "RF010", Nombre = "Camila", Apellido = "Vargas", Estado = true },
                
                new Empleado { IdEmpleado = 11, IdEmpresa = 2, IdCredencial = "RF011", Nombre = "Roberto", Apellido = "Herrera", Estado = true },
                new Empleado { IdEmpleado = 12, IdEmpresa = 2, IdCredencial = "RF012", Nombre = "Patricia", Apellido = "Castro", Estado = true },
                new Empleado { IdEmpleado = 13, IdEmpresa = 3, IdCredencial = "RF013", Nombre = "Miguel", Apellido = "Torres", Estado = true },
                new Empleado { IdEmpleado = 14, IdEmpresa = 4, IdCredencial = "RF014", Nombre = "Isabella", Apellido = "Reyes", Estado = true },
                new Empleado { IdEmpleado = 15, IdEmpresa = 5, IdCredencial = "RF015", Nombre = "Francisco", Apellido = "Jiménez", Estado = true },
                new Empleado { IdEmpleado = 16, IdEmpresa = 6, IdCredencial = "RF016", Nombre = "Daniela", Apellido = "Moreno", Estado = true }
            };
            modelBuilder.Entity<Empleado>().HasData(empleados);

            // 4. Servicios y Registros (Muestra para Febrero 2026)
            // Se agregan un par de días para demostrar la funcionalidad
            var febreros = new[] { 2, 3, 4 }; // Días de febrero 2026 (Mon, Tue, Wed)
            int servicioIdCounter = 1;
            int registroIdCounter = 1;

            foreach (var dia in febreros)
            {
                var fecha = new DateTime(2026, 2, dia);
                
                // Servicio Comedor
                var sComedorId = servicioIdCounter++;
                modelBuilder.Entity<Servicio>().HasData(new Servicio 
                { 
                    IdServicio = sComedorId, IdLugar = 1, Fecha = fecha, 
                    Proyeccion = 50, DuracionMinutos = 60, TotalComensales = 5, TotalInvitados = 1 
                });

                // Registros para ese servicio
                for (int i = 1; i <= 5; i++)
                {
                    modelBuilder.Entity<Registro>().HasData(new Registro 
                    { 
                        IdRegistro = registroIdCounter++, IdEmpleado = i, IdEmpresa = empleados[i-1].IdEmpresa, 
                        IdServicio = sComedorId, IdLugar = 1, Fecha = fecha, Hora = new TimeSpan(12, 10 + i, 0) 
                    });
                }

                // Servicio Quincho
                var sQuinchoId = servicioIdCounter++;
                modelBuilder.Entity<Servicio>().HasData(new Servicio 
                { 
                    IdServicio = sQuinchoId, IdLugar = 2, Fecha = fecha, 
                    Proyeccion = 30, DuracionMinutos = 45, TotalComensales = 3, TotalInvitados = 0 
                });

                // Registros para Quincho
                for (int i = 6; i <= 8; i++)
                {
                    modelBuilder.Entity<Registro>().HasData(new Registro 
                    { 
                        IdRegistro = registroIdCounter++, IdEmpleado = i, IdEmpresa = empleados[i-1].IdEmpresa, 
                        IdServicio = sQuinchoId, IdLugar = 2, Fecha = fecha, Hora = new TimeSpan(13, 05 + i, 0) 
                    });
                }
            }
        }
    }
}
