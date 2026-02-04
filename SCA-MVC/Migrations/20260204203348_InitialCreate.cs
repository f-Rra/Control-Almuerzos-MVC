using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SCA_MVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.IdEmpresa);
                });

            migrationBuilder.CreateTable(
                name: "Lugares",
                columns: table => new
                {
                    IdLugar = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lugares", x => x.IdLugar);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    IdEmpleado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdCredencial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.IdEmpleado);
                    table.ForeignKey(
                        name: "FK_Empleados_Empresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresas",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    IdServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLugar = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Proyeccion = table.Column<int>(type: "int", nullable: true),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: true),
                    TotalComensales = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalInvitados = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.IdServicio);
                    table.CheckConstraint("CK_Servicio_Fecha", "[Fecha] <= CAST(GETDATE() AS DATE)");
                    table.ForeignKey(
                        name: "FK_Servicios_Lugar",
                        column: x => x.IdLugar,
                        principalTable: "Lugares",
                        principalColumn: "IdLugar",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Registros",
                columns: table => new
                {
                    IdRegistro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: true),
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdServicio = table.Column<int>(type: "int", nullable: false),
                    IdLugar = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registros", x => x.IdRegistro);
                    table.ForeignKey(
                        name: "FK_Registros_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleados",
                        principalColumn: "IdEmpleado",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Registros_Empresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresas",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Registros_Lugar",
                        column: x => x.IdLugar,
                        principalTable: "Lugares",
                        principalColumn: "IdLugar",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Registros_Servicio",
                        column: x => x.IdServicio,
                        principalTable: "Servicios",
                        principalColumn: "IdServicio",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_IdCredencial",
                table: "Empleados",
                column: "IdCredencial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_IdEmpresa",
                table: "Empleados",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_Registro_Empleado_Servicio",
                table: "Registros",
                columns: new[] { "IdEmpleado", "IdServicio" },
                unique: true,
                filter: "[IdEmpleado] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Registro_Fecha",
                table: "Registros",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Registros_IdEmpresa",
                table: "Registros",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_Registros_IdLugar",
                table: "Registros",
                column: "IdLugar");

            migrationBuilder.CreateIndex(
                name: "IX_Registros_IdServicio",
                table: "Registros",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_Fecha_Lugar",
                table: "Servicios",
                columns: new[] { "Fecha", "IdLugar" });

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_IdLugar",
                table: "Servicios",
                column: "IdLugar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Registros");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Lugares");
        }
    }
}
