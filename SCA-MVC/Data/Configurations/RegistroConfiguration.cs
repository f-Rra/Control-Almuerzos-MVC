using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Configurations
{
    public class RegistroConfiguration : IEntityTypeConfiguration<Registro>
    {
        public void Configure(EntityTypeBuilder<Registro> builder)
        {
            // Índice único compuesto: un empleado no puede registrarse dos veces en el mismo servicio.
            // HasFilter excluye los nulls para que los invitados (IdEmpleado = null) no rompan el índice.
            builder.HasIndex(r => new { r.IdEmpleado, r.IdServicio })
                .IsUnique()
                .HasFilter("[IdEmpleado] IS NOT NULL")
                .HasDatabaseName("IX_Registro_Empleado_Servicio");

            // La tabla Registros tiene triggers habilitados en SQL Server.
            // Sin esta configuración, EF Core genera INSERT ... OUTPUT INSERTED.IdRegistro,
            // lo que SQL Server rechaza cuando hay triggers activos en la tabla destino.
            // HasTrigger le indica a EF Core que use una estrategia alternativa para recuperar el ID.
            builder.ToTable(tb => tb.HasTrigger("Registros_Trigger"));
        }
    }
}
