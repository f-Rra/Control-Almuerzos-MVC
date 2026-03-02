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
        }
    }
}
