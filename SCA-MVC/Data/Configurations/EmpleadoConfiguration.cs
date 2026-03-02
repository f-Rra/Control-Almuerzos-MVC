using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Configurations
{
    public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
    {
        public void Configure(EntityTypeBuilder<Empleado> builder)
        {
            // Relación: Empleado → Registros (1:N, NULLABLE)
            // IdEmpleado nullable para permitir invitados sin empleado asociado
            builder.HasMany(e => e.Registros)
                .WithOne(r => r.Empleado)
                .HasForeignKey(r => r.IdEmpleado)
                .OnDelete(DeleteBehavior.SetNull)    // Si se elimina el empleado, IdEmpleado queda en null
                .IsRequired(false)
                .HasConstraintName("FK_Registros_Empleado");

            // Índice único: no puede haber dos empleados con el mismo IdCredencial (RFID)
            builder.HasIndex(e => e.IdCredencial)
                .IsUnique()
                .HasDatabaseName("IX_Empleado_IdCredencial");

            // Estado activo por defecto
            builder.Property(e => e.Estado)
                .HasDefaultValue(true);
        }
    }
}
