using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Configurations
{
    public class LugarConfiguration : IEntityTypeConfiguration<Lugar>
    {
        public void Configure(EntityTypeBuilder<Lugar> builder)
        {
            // Relación: Lugar → Servicios (1:N)
            builder.HasMany(l => l.Servicios)
                .WithOne(s => s.Lugar)
                .HasForeignKey(s => s.IdLugar)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Servicios_Lugar");

            // Relación: Lugar → Registros (1:N)
            builder.HasMany(l => l.Registros)
                .WithOne(r => r.Lugar)
                .HasForeignKey(r => r.IdLugar)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Registros_Lugar");

            // Estado activo por defecto
            builder.Property(l => l.Estado)
                .HasDefaultValue(true);
        }
    }
}
