using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Configurations
{
    public class ServicioConfiguration : IEntityTypeConfiguration<Servicio>
    {
        public void Configure(EntityTypeBuilder<Servicio> builder)
        {
            // Relación: Servicio → Registros (1:N)
            builder.HasMany(s => s.Registros)
                .WithOne(r => r.Servicio)
                .HasForeignKey(r => r.IdServicio)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Registros_Servicio");

            // Valores por defecto: totales arrancan en 0
            builder.Property(s => s.TotalComensales)
                .HasDefaultValue(0);

            builder.Property(s => s.TotalInvitados)
                .HasDefaultValue(0);

            // Check constraint: fecha no puede superar 2030 (evita errores de tipeo absurdos)
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Servicio_Fecha",
                "[Fecha] <= '2030-01-01'"));
        }
    }
}
