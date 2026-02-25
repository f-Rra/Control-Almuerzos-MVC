using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SCA_MVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
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
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CantidadEmpleados = table.Column<int>(type: "int", nullable: false)
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
                    HoraInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Proyeccion = table.Column<int>(type: "int", nullable: true),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: true),
                    TotalComensales = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalInvitados = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.IdServicio);
                    table.CheckConstraint("CK_Servicio_Fecha", "[Fecha] <= '2030-01-01'");
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

            migrationBuilder.InsertData(
                table: "Empresas",
                columns: new[] { "IdEmpresa", "CantidadEmpleados", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1, 0, true, "Roemmers" },
                    { 2, 0, true, "Gema" },
                    { 3, 0, true, "Siegfried" },
                    { 4, 0, true, "Gramon" },
                    { 5, 0, true, "Simmer" },
                    { 6, 0, true, "Ethical" }
                });

            migrationBuilder.InsertData(
                table: "Lugares",
                columns: new[] { "IdLugar", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Comedor" },
                    { 2, true, "Quincho" }
                });

            migrationBuilder.InsertData(
                table: "Empleados",
                columns: new[] { "IdEmpleado", "Apellido", "Estado", "IdCredencial", "IdEmpresa", "Nombre" },
                values: new object[,]
                {
                    { 1, "Perez", true, "RF001", 1, "Juan" },
                    { 2, "García", true, "RF002", 1, "María" },
                    { 3, "López", true, "RF003", 1, "Carlos" },
                    { 4, "Martínez", true, "RF004", 1, "Ana" },
                    { 5, "Rodríguez", true, "RF005", 1, "Luis" },
                    { 6, "Fernández", true, "RF006", 1, "Sofía" },
                    { 7, "González", true, "RF007", 1, "Diego" },
                    { 8, "Silva", true, "RF008", 1, "Valentina" },
                    { 9, "Morales", true, "RF009", 1, "Andrés" },
                    { 10, "Vargas", true, "RF010", 1, "Camila" },
                    { 11, "Herrera", true, "RF011", 2, "Roberto" },
                    { 12, "Castro", true, "RF012", 2, "Patricia" },
                    { 13, "Torres", true, "RF013", 3, "Miguel" },
                    { 14, "Reyes", true, "RF014", 4, "Isabella" },
                    { 15, "Jiménez", true, "RF015", 5, "Francisco" },
                    { 16, "Moreno", true, "RF016", 6, "Daniela" }
                });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales", "TotalInvitados" },
                values: new object[] { 1, 60, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 50, 5, 1 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales" },
                values: new object[] { 2, 45, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 30, 3 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales", "TotalInvitados" },
                values: new object[] { 3, 60, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 50, 5, 1 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales" },
                values: new object[] { 4, 45, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 30, 3 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales", "TotalInvitados" },
                values: new object[] { 5, 60, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 50, 5, 1 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales" },
                values: new object[] { 6, 45, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 30, 3 });

            migrationBuilder.InsertData(
                table: "Registros",
                columns: new[] { "IdRegistro", "Fecha", "Hora", "IdEmpleado", "IdEmpresa", "IdLugar", "IdServicio" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 1, 1, 1, 1 },
                    { 2, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 12, 0, 0), 2, 1, 1, 1 },
                    { 3, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 3, 1, 1, 1 },
                    { 4, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 4, 1, 1, 1 },
                    { 5, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 15, 0, 0), 5, 1, 1, 1 },
                    { 6, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 11, 0, 0), 6, 1, 2, 2 },
                    { 7, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 12, 0, 0), 7, 1, 2, 2 },
                    { 8, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 13, 0, 0), 8, 1, 2, 2 },
                    { 9, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 1, 1, 1, 3 },
                    { 10, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 12, 0, 0), 2, 1, 1, 3 },
                    { 11, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 3, 1, 1, 3 },
                    { 12, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 4, 1, 1, 3 },
                    { 13, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 15, 0, 0), 5, 1, 1, 3 },
                    { 14, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 11, 0, 0), 6, 1, 2, 4 },
                    { 15, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 12, 0, 0), 7, 1, 2, 4 },
                    { 16, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 13, 0, 0), 8, 1, 2, 4 },
                    { 17, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 1, 1, 1, 5 },
                    { 18, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 12, 0, 0), 2, 1, 1, 5 },
                    { 19, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 3, 1, 1, 5 },
                    { 20, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 4, 1, 1, 5 },
                    { 21, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 15, 0, 0), 5, 1, 1, 5 },
                    { 22, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 11, 0, 0), 6, 1, 2, 6 },
                    { 23, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 12, 0, 0), 7, 1, 2, 6 },
                    { 24, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 13, 0, 0), 8, 1, 2, 6 }
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
