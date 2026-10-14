using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SCA_MVC.Data;

namespace SCA_MVC.Tests;

// Una base SQLite en memoria, nueva para cada test: lo que un test guarda no lo ve
// ningún otro, así que el orden en que corran no importa.
//
// No se usa la base en memoria de EF (UseInMemoryDatabase) porque no cumple los
// índices únicos ni acepta SQL crudo, y RegistrarAsync inserta con SQL crudo.
// SQLite es una base de verdad: cumple los índices únicos y las claves foráneas.
//
// La base vive mientras la conexión esté abierta, por eso se abre acá y se cierra
// en Dispose. xUnit llama a Dispose solo cuando el test termina.
public sealed class BaseDeDatos : IDisposable
{
    private readonly SqliteConnection _conexion;

    public ApplicationDbContext Contexto { get; }

    public BaseDeDatos()
    {
        _conexion = new SqliteConnection("DataSource=:memory:");
        _conexion.Open();

        var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_conexion)
            .Options;

        Contexto = new ApplicationDbContext(opciones);

        // Crea las tablas desde el modelo, con los datos de HasData: los 2 lugares,
        // las 6 empresas, los 504 empleados y los servicios históricos
        Contexto.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Contexto.Dispose();
        _conexion.Dispose();
    }
}
