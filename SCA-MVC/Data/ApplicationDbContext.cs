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
                new Empresa { IdEmpresa = 1, Nombre = "Roemmers",  Estado = true },
                new Empresa { IdEmpresa = 2, Nombre = "Gema",      Estado = true },
                new Empresa { IdEmpresa = 3, Nombre = "Siegfried", Estado = true },
                new Empresa { IdEmpresa = 4, Nombre = "Gramon",    Estado = true },
                new Empresa { IdEmpresa = 5, Nombre = "Simmer",    Estado = true },
                new Empresa { IdEmpresa = 6, Nombre = "Ethical",   Estado = true }
            );

            // 3. Empleados
            // IDs 1–60: seed original (mantenidos para no romper migraciones existentes)
            // IDs 61–504: empleados adicionales para alcanzar escala real (~500)
            var empList = new List<Empleado>();

            string[] nombresOrig  = { "Juan", "María", "Carlos", "Ana", "Luis", "Sofía", "Diego", "Valentina", "Andrés", "Camila" };
            string[] apellidosOrig = { "Perez", "García", "López", "Martínez", "Rodríguez", "Fernández", "González", "Silva", "Morales", "Vargas" };
            string[] nombresOrig2  = { "Roberto", "Patricia", "Miguel", "Isabella", "Francisco", "Daniela", "Alejandro", "Gabriela", "Ricardo", "Natalia" };
            string[] apellidosOrig2 = { "Herrera", "Castro", "Torres", "Reyes", "Jiménez", "Moreno", "Ruiz", "Díaz", "Flores", "Cruz" };

            // Seed original (IDs 1–60, 10 por empresa)
            int idEmp = 1;
            for (int i = 1; i <= 6; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    empList.Add(new Empleado
                    {
                        IdEmpleado   = idEmp,
                        IdEmpresa    = i,
                        IdCredencial = $"RF{idEmp:D3}",
                        Nombre       = i % 2 == 0 ? nombresOrig2[j]  : nombresOrig[j],
                        Apellido     = i % 2 == 0 ? apellidosOrig2[j] : apellidosOrig[j],
                        Estado       = true
                    });
                    idEmp++;
                }
            }

            // Empleados adicionales (IDs 61–504, 74 más por empresa)
            string[] nombresExt = {
                "Martín", "Paula", "Sergio", "Florencia", "Fernando", "Agustina",
                "Gustavo", "Romina", "Pablo", "Claudia", "Hernán", "Valeria",
                "Federico", "Carolina", "Leonardo", "Marcela", "Santiago", "Vanesa",
                "Maximiliano", "Silvina", "Nicolás", "Lucía", "Matías", "Verónica",
                "Sebastián", "Cecilia", "Ariel", "Mónica", "Emilio", "Graciela"
            };
            string[] apellidosExt = {
                "Ramos", "Romero", "Acosta", "Molina", "Gutiérrez", "Navarro",
                "Sánchez", "Medina", "Suárez", "Ortega", "Vega", "Campos",
                "Castillo", "Ríos", "Núñez", "Aguirre", "Mendoza", "Ibáñez",
                "Paredes", "Rojas", "Ávila", "Carrillo", "Fuentes", "Ponce",
                "Salinas", "Espinoza", "Contreras", "Barrera", "Leal", "Moya"
            };

            for (int i = 1; i <= 6; i++)
            {
                for (int j = 0; j < 74; j++)
                {
                    empList.Add(new Empleado
                    {
                        IdEmpleado   = idEmp,
                        IdEmpresa    = i,
                        IdCredencial = $"RF{idEmp:D3}",
                        Nombre       = nombresExt[(j + i * 7)   % nombresExt.Length],
                        Apellido     = apellidosExt[(j + i * 11) % apellidosExt.Length],
                        Estado       = true
                    });
                    idEmp++;
                }
            }
            modelBuilder.Entity<Empleado>().HasData(empList);

            // 4. Servicios y Registros
            // Bloque A (IDs 1–32): servicios de febrero 2026 — idéntico al seed original
            // Bloque B (IDs 33+): enero, marzo y abril 2026 — datos adicionales
            var rng = new Random(42);

            // ── Bloque A: febrero 2026 (idéntico al InitialCreate) ──
            int sId = 1;
            int rId = 1;
            var diasFeb = new[] { 2, 3, 4, 5, 6, 10, 11, 12, 13, 19, 20, 23, 24, 25, 26, 27 };

            foreach (var dia in diasFeb)
            {
                var fecha = new DateTime(2026, 2, dia);

                int proyC = 45 + rng.Next(20);
                int comC  = 0;
                int invC  = rng.Next(1, 5);
                int sIdC  = sId++;

                for (int eId = 1; eId <= 6; eId++)
                {
                    int cant = 2 + rng.Next(4);
                    for (int k = 0; k < cant; k++)
                    {
                        int empIdx = ((eId - 1) * 10) + ((dia + k + eId) % 10);
                        modelBuilder.Entity<Registro>().HasData(new Registro
                        {
                            IdRegistro = rId++, IdEmpleado = empList[empIdx].IdEmpleado,
                            IdEmpresa  = eId,   IdServicio  = sIdC, IdLugar = 1,
                            Fecha      = fecha, Hora        = new TimeSpan(12, 10 + rng.Next(40), 0)
                        });
                        comC++;
                    }
                }
                modelBuilder.Entity<Servicio>().HasData(new Servicio
                {
                    IdServicio = sIdC, IdLugar = 1, Fecha = fecha,
                    Proyeccion = proyC, DuracionMinutos = 60, TotalComensales = comC, TotalInvitados = invC
                });

                int proyQ = 25 + rng.Next(15);
                int comQ  = 0;
                int invQ  = rng.Next(0, 3);
                int sIdQ  = sId++;

                for (int eId = 1; eId <= 6; eId++)
                {
                    int cant = 1 + rng.Next(3);
                    for (int k = 0; k < cant; k++)
                    {
                        int empIdx = ((eId - 1) * 10) + 5 + ((dia + k + eId) % 5);
                        modelBuilder.Entity<Registro>().HasData(new Registro
                        {
                            IdRegistro = rId++, IdEmpleado = empList[empIdx].IdEmpleado,
                            IdEmpresa  = eId,   IdServicio  = sIdQ, IdLugar = 2,
                            Fecha      = fecha, Hora        = new TimeSpan(13, 15 + rng.Next(30), 0)
                        });
                        comQ++;
                    }
                }
                modelBuilder.Entity<Servicio>().HasData(new Servicio
                {
                    IdServicio = sIdQ, IdLugar = 2, Fecha = fecha,
                    Proyeccion = proyQ, DuracionMinutos = 45, TotalComensales = comQ, TotalInvitados = invQ
                });
            }

            // ── Bloque B: enero, marzo y abril 2026 (datos adicionales) ──
            // sId ahora vale 33 (los 32 primeros los usó el bloque A)
            const int empPorEmpresa = 84; // total con los 74 nuevos
            var diasExtra = new List<DateTime>();
            foreach (var (anio, mes) in new[] { (2026, 1), (2026, 3), (2026, 4) })
            {
                var c = new DateTime(anio, mes, mes == 1 ? 2 : 1);
                var f = new DateTime(anio, mes, DateTime.DaysInMonth(anio, mes));
                while (c <= f)
                {
                    if (c.DayOfWeek != DayOfWeek.Saturday && c.DayOfWeek != DayOfWeek.Sunday)
                        diasExtra.Add(c);
                    c = c.AddDays(1);
                }
            }

            foreach (var fecha in diasExtra)
            {
                int diaIdx = (int)(fecha - new DateTime(2026, 1, 1)).TotalDays;

                int proyC = 280 + rng.Next(60);
                int comC  = 0;
                int invC  = rng.Next(1, 8);
                int sIdC  = sId++;

                for (int eId = 1; eId <= 6; eId++)
                {
                    int cant = 6 + rng.Next(8);
                    for (int k = 0; k < cant; k++)
                    {
                        int empIdx = ((eId - 1) * empPorEmpresa) + ((diaIdx * 7 + k * 3 + eId * 13) % empPorEmpresa);
                        modelBuilder.Entity<Registro>().HasData(new Registro
                        {
                            IdRegistro = rId++, IdEmpleado = empList[empIdx].IdEmpleado,
                            IdEmpresa  = eId,   IdServicio  = sIdC, IdLugar = 1,
                            Fecha      = fecha, Hora        = new TimeSpan(12, 5 + rng.Next(55), 0)
                        });
                        comC++;
                    }
                }
                modelBuilder.Entity<Servicio>().HasData(new Servicio
                {
                    IdServicio = sIdC, IdLugar = 1, Fecha = fecha,
                    Proyeccion = proyC, DuracionMinutos = 55 + rng.Next(20),
                    TotalComensales = comC, TotalInvitados = invC
                });

                int proyQ = 160 + rng.Next(40);
                int comQ  = 0;
                int invQ  = rng.Next(0, 5);
                int sIdQ  = sId++;

                for (int eId = 1; eId <= 6; eId++)
                {
                    int cant = 4 + rng.Next(5);
                    for (int k = 0; k < cant; k++)
                    {
                        int empIdx = ((eId - 1) * empPorEmpresa) + ((diaIdx * 5 + k * 7 + eId * 17 + 42) % empPorEmpresa);
                        modelBuilder.Entity<Registro>().HasData(new Registro
                        {
                            IdRegistro = rId++, IdEmpleado = empList[empIdx].IdEmpleado,
                            IdEmpresa  = eId,   IdServicio  = sIdQ, IdLugar = 2,
                            Fecha      = fecha, Hora        = new TimeSpan(13, 10 + rng.Next(40), 0)
                        });
                        comQ++;
                    }
                }
                modelBuilder.Entity<Servicio>().HasData(new Servicio
                {
                    IdServicio = sIdQ, IdLugar = 2, Fecha = fecha,
                    Proyeccion = proyQ, DuracionMinutos = 40 + rng.Next(20),
                    TotalComensales = comQ, TotalInvitados = invQ
                });
            }
        }
    }
}
