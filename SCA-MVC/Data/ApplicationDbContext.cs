using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SCA_MVC.Models;
using System;
using System.Collections.Generic;

namespace SCA_MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Lugar> Lugares { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Registro> Registros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Llama a base para que Identity configure sus propias tablas primero
            base.OnModelCreating(modelBuilder);

            // Aplica todas las clases IEntityTypeConfiguration<T> del ensamblado
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

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
