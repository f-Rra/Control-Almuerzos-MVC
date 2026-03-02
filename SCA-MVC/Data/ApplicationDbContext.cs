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
        // ------------------------------------------------------------------------------------------
        // ESTA SECCIÓN CONTIENE LA EXPLICACIÓN DIDÁCTICA DEL MODELADO DE CLASES CON EF CORE.
        // Aquí personalizamos cómo nuestras clases (Entidades) se mapean a la Base de Datos.
        // Sobrepasamos las convenciones automáticas de EF Core para configurar:
        // - Claves foráneas específicas (HasForeignKey)
        // - Comportamientos de eliminación en cascada (OnDelete)
        // - Índices únicos para evitar duplicados en la BD (HasIndex.IsUnique)
        // - Valores por defecto y restricciones a nivel de SQL Server
        // ------------------------------------------------------------------------------------------
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

            // 3. Empleados (10 por empresa = 60 empleados)
            var empList = new List<Empleado>();
            string[] nombres = { "Juan", "María", "Carlos", "Ana", "Luis", "Sofía", "Diego", "Valentina", "Andrés", "Camila" };
            string[] apellidos = { "Perez", "García", "López", "Martínez", "Rodríguez", "Fernández", "González", "Silva", "Morales", "Vargas" };
            string[] nombres2 = { "Roberto", "Patricia", "Miguel", "Isabella", "Francisco", "Daniela", "Alejandro", "Gabriela", "Ricardo", "Natalia" };
            string[] apellidos2 = { "Herrera", "Castro", "Torres", "Reyes", "Jiménez", "Moreno", "Ruiz", "Díaz", "Flores", "Cruz" };

            int idEmp = 1;
            for (int i = 1; i <= 6; i++) // 6 empresas
            {
                for (int j = 0; j < 10; j++) // 10 empleados por empresa
                {
                    empList.Add(new Empleado 
                    { 
                        IdEmpleado = idEmp, 
                        IdEmpresa = i, 
                        IdCredencial = $"RF{idEmp:D3}", 
                        Nombre = i % 2 == 0 ? nombres2[j] : nombres[j], 
                        Apellido = i % 2 == 0 ? apellidos2[j] : apellidos[j], 
                        Estado = true 
                    });
                    idEmp++;
                }
            }
            modelBuilder.Entity<Empleado>().HasData(empList);

            // 4. Servicios y Registros (Demo Febrero 2026)
            int sId = 1;
            int rId = 1;
            // Incluimos Lunes a Viernes de varias semanas para que los reportes sean completos
            var diasDemo = new[] { 2, 3, 4, 5, 6, 10, 11, 12, 13, 19, 20, 23, 24, 25, 26, 27 };

            var rng = new Random(42); // Seed fija para consistencia en cada generación

            foreach (var dia in diasDemo)
            {
                var fecha = new DateTime(2026, 2, dia);
                
                // --- COMEDOR ---
                int proyComedor = 45 + rng.Next(20); // Variación entre 45 y 65
                int comensalesComedor = 0;
                int invitadosComedor = rng.Next(1, 5);
                int currentSIdComedor = sId++;

                // Registros Comedor (variado por empresa)
                for (int eId = 1; eId <= 6; eId++)
                {
                    int cantPorEmpresa = 2 + rng.Next(4); // 2 a 5 registros por empresa
                    for (int k = 0; k < cantPorEmpresa; k++)
                    {
                        int empIdx = ((eId - 1) * 10) + ((dia + k + eId) % 10);
                        modelBuilder.Entity<Registro>().HasData(new Registro 
                        { 
                            IdRegistro = rId++, IdEmpleado = empList[empIdx].IdEmpleado, 
                            IdEmpresa = eId, IdServicio = currentSIdComedor, IdLugar = 1, 
                            Fecha = fecha, Hora = new TimeSpan(12, 10 + rng.Next(40), 0) 
                        });
                        comensalesComedor++;
                    }
                }

                modelBuilder.Entity<Servicio>().HasData(new Servicio 
                { 
                    IdServicio = currentSIdComedor, IdLugar = 1, Fecha = fecha, 
                    Proyeccion = proyComedor, DuracionMinutos = 60, 
                    TotalComensales = comensalesComedor, TotalInvitados = invitadosComedor 
                });

                // --- QUINCHO ---
                int proyQuincho = 25 + rng.Next(15); // Variación entre 25 y 40
                int comensalesQuincho = 0;
                int invitadosQuincho = rng.Next(0, 3);
                int currentSIdQuincho = sId++;

                // Registros Quincho (variado por empresa)
                for (int eId = 1; eId <= 6; eId++)
                {
                    int cantPorEmpresa = 1 + rng.Next(3); // 1 a 3 registros por empresa
                    for (int k = 0; k < cantPorEmpresa; k++)
                    {
                        int empIdx = ((eId - 1) * 10) + 5 + ((dia + k + eId) % 5);
                        modelBuilder.Entity<Registro>().HasData(new Registro 
                        { 
                            IdRegistro = rId++, IdEmpleado = empList[empIdx].IdEmpleado, 
                            IdEmpresa = eId, IdServicio = currentSIdQuincho, IdLugar = 2, 
                            Fecha = fecha, Hora = new TimeSpan(13, 15 + rng.Next(30), 0) 
                        });
                        comensalesQuincho++;
                    }
                }

                modelBuilder.Entity<Servicio>().HasData(new Servicio 
                { 
                    IdServicio = currentSIdQuincho, IdLugar = 2, Fecha = fecha, 
                    Proyeccion = proyQuincho, DuracionMinutos = 45, 
                    TotalComensales = comensalesQuincho, TotalInvitados = invitadosQuincho 
                });
            }
        }
    }
}
