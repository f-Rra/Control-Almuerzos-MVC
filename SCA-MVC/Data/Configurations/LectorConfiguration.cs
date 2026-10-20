using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Configurations
{
    public class LectorConfiguration : IEntityTypeConfiguration<Lector>
    {
        public void Configure(EntityTypeBuilder<Lector> builder)
        {
            // La API busca al lector por el hash de la clave que manda: tiene que ser único y
            // el índice hace que esa búsqueda no recorra la tabla
            builder.HasIndex(l => l.ApiKeyHash)
                .IsUnique()
                .HasDatabaseName("IX_Lector_ApiKeyHash");

            // Relación: Lugar → Lectores (1:N). Un lugar con lectores no se puede borrar.
            builder.HasOne(l => l.Lugar)
                .WithMany()
                .HasForeignKey(l => l.IdLugar)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Lectores_Lugar");

            // Activo por defecto
            builder.Property(l => l.Activo)
                .HasDefaultValue(true);
        }
    }
}
