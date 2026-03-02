using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Configurations
{
    public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            // Relación: Empresa → Empleados (1:N)
            builder.HasMany(e => e.Empleados)
                .WithOne(emp => emp.Empresa)
                .HasForeignKey(emp => emp.IdEmpresa)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Empleados_Empresa");

            // Relación: Empresa → Registros (1:N)
            builder.HasMany(e => e.Registros)
                .WithOne(r => r.Empresa)
                .HasForeignKey(r => r.IdEmpresa)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Registros_Empresa");

            // Estado activo por defecto
            builder.Property(e => e.Estado)
                .HasDefaultValue(true);
        }
    }
}
