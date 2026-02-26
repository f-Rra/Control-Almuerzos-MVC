using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                    { 13, "Torres", true, "RF013", 2, "Miguel" },
                    { 14, "Reyes", true, "RF014", 2, "Isabella" },
                    { 15, "Jiménez", true, "RF015", 2, "Francisco" },
                    { 16, "Moreno", true, "RF016", 2, "Daniela" },
                    { 17, "Ruiz", true, "RF017", 2, "Alejandro" },
                    { 18, "Díaz", true, "RF018", 2, "Gabriela" },
                    { 19, "Flores", true, "RF019", 2, "Ricardo" },
                    { 20, "Cruz", true, "RF020", 2, "Natalia" },
                    { 21, "Perez", true, "RF021", 3, "Juan" },
                    { 22, "García", true, "RF022", 3, "María" },
                    { 23, "López", true, "RF023", 3, "Carlos" },
                    { 24, "Martínez", true, "RF024", 3, "Ana" },
                    { 25, "Rodríguez", true, "RF025", 3, "Luis" },
                    { 26, "Fernández", true, "RF026", 3, "Sofía" },
                    { 27, "González", true, "RF027", 3, "Diego" },
                    { 28, "Silva", true, "RF028", 3, "Valentina" },
                    { 29, "Morales", true, "RF029", 3, "Andrés" },
                    { 30, "Vargas", true, "RF030", 3, "Camila" },
                    { 31, "Herrera", true, "RF031", 4, "Roberto" },
                    { 32, "Castro", true, "RF032", 4, "Patricia" },
                    { 33, "Torres", true, "RF033", 4, "Miguel" },
                    { 34, "Reyes", true, "RF034", 4, "Isabella" },
                    { 35, "Jiménez", true, "RF035", 4, "Francisco" },
                    { 36, "Moreno", true, "RF036", 4, "Daniela" },
                    { 37, "Ruiz", true, "RF037", 4, "Alejandro" },
                    { 38, "Díaz", true, "RF038", 4, "Gabriela" },
                    { 39, "Flores", true, "RF039", 4, "Ricardo" },
                    { 40, "Cruz", true, "RF040", 4, "Natalia" },
                    { 41, "Perez", true, "RF041", 5, "Juan" },
                    { 42, "García", true, "RF042", 5, "María" },
                    { 43, "López", true, "RF043", 5, "Carlos" },
                    { 44, "Martínez", true, "RF044", 5, "Ana" },
                    { 45, "Rodríguez", true, "RF045", 5, "Luis" },
                    { 46, "Fernández", true, "RF046", 5, "Sofía" },
                    { 47, "González", true, "RF047", 5, "Diego" },
                    { 48, "Silva", true, "RF048", 5, "Valentina" },
                    { 49, "Morales", true, "RF049", 5, "Andrés" },
                    { 50, "Vargas", true, "RF050", 5, "Camila" },
                    { 51, "Herrera", true, "RF051", 6, "Roberto" },
                    { 52, "Castro", true, "RF052", 6, "Patricia" },
                    { 53, "Torres", true, "RF053", 6, "Miguel" },
                    { 54, "Reyes", true, "RF054", 6, "Isabella" },
                    { 55, "Jiménez", true, "RF055", 6, "Francisco" },
                    { 56, "Moreno", true, "RF056", 6, "Daniela" },
                    { 57, "Ruiz", true, "RF057", 6, "Alejandro" },
                    { 58, "Díaz", true, "RF058", 6, "Gabriela" },
                    { 59, "Flores", true, "RF059", 6, "Ricardo" },
                    { 60, "Cruz", true, "RF060", 6, "Natalia" }
                });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales", "TotalInvitados" },
                values: new object[,]
                {
                    { 1, 60, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 58, 22, 1 },
                    { 2, 45, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 35, 11, 1 },
                    { 3, 60, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 60, 19, 2 }
                });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales" },
                values: new object[] { 4, 45, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 25, 13 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales", "TotalInvitados" },
                values: new object[] { 5, 60, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 62, 17, 4 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales" },
                values: new object[] { 6, 45, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 25, 9 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales", "TotalInvitados" },
                values: new object[,]
                {
                    { 7, 60, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 61, 18, 4 },
                    { 8, 45, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 30, 12, 2 },
                    { 9, 60, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 64, 23, 4 },
                    { 10, 45, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 27, 11, 1 },
                    { 11, 60, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 49, 24, 2 },
                    { 12, 45, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 38, 13, 1 },
                    { 13, 60, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 58, 24, 1 },
                    { 14, 45, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 39, 11, 2 },
                    { 15, 60, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 53, 18, 1 },
                    { 16, 45, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 34, 11, 2 },
                    { 17, 60, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 50, 21, 3 },
                    { 18, 45, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 37, 14, 1 },
                    { 19, 60, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 56, 20, 3 }
                });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales" },
                values: new object[] { 20, 45, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 38, 14 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales", "TotalInvitados" },
                values: new object[] { 21, 60, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 55, 22, 2 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales" },
                values: new object[] { 22, 45, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 26, 13 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales", "TotalInvitados" },
                values: new object[] { 23, 60, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 49, 17, 3 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales" },
                values: new object[] { 24, 45, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 27, 10 });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "IdServicio", "DuracionMinutos", "Fecha", "HoraInicio", "IdLugar", "Proyeccion", "TotalComensales", "TotalInvitados" },
                values: new object[,]
                {
                    { 25, 60, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 47, 16, 4 },
                    { 26, 45, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 27, 11, 2 },
                    { 27, 60, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 45, 20, 2 },
                    { 28, 45, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 27, 12, 2 },
                    { 29, 60, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 55, 21, 2 },
                    { 30, 45, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 36, 12, 2 },
                    { 31, 60, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, 46, 18, 1 },
                    { 32, 45, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, 32, 13, 2 }
                });

            migrationBuilder.InsertData(
                table: "Registros",
                columns: new[] { "IdRegistro", "Fecha", "Hora", "IdEmpleado", "IdEmpresa", "IdLugar", "IdServicio" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 30, 0, 0), 4, 1, 1, 1 },
                    { 2, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 16, 0, 0), 5, 1, 1, 1 },
                    { 3, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 15, 2, 1, 1 },
                    { 4, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 30, 0, 0), 16, 2, 1, 1 },
                    { 5, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 16, 0, 0), 17, 2, 1, 1 },
                    { 6, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 19, 0, 0), 26, 3, 1, 1 },
                    { 7, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 20, 0, 0), 27, 3, 1, 1 },
                    { 8, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 30, 0, 0), 28, 3, 1, 1 },
                    { 9, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 22, 0, 0), 29, 3, 1, 1 },
                    { 10, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 25, 0, 0), 30, 3, 1, 1 },
                    { 11, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 30, 0, 0), 37, 4, 1, 1 },
                    { 12, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 38, 4, 1, 1 },
                    { 13, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 39, 4, 1, 1 },
                    { 14, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 25, 0, 0), 48, 5, 1, 1 },
                    { 15, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 16, 0, 0), 49, 5, 1, 1 },
                    { 16, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 50, 5, 1, 1 },
                    { 17, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 41, 5, 1, 1 },
                    { 18, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 31, 0, 0), 59, 6, 1, 1 },
                    { 19, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 60, 6, 1, 1 },
                    { 20, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 51, 6, 1, 1 },
                    { 21, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 15, 0, 0), 52, 6, 1, 1 },
                    { 22, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 46, 0, 0), 53, 6, 1, 1 },
                    { 23, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 9, 1, 2, 2 },
                    { 24, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 32, 0, 0), 20, 2, 2, 2 },
                    { 25, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 15, 0, 0), 26, 3, 2, 2 },
                    { 26, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 31, 0, 0), 27, 3, 2, 2 },
                    { 27, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 39, 0, 0), 28, 3, 2, 2 },
                    { 28, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 26, 0, 0), 37, 4, 2, 2 },
                    { 29, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 19, 0, 0), 38, 4, 2, 2 },
                    { 30, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 15, 0, 0), 39, 4, 2, 2 },
                    { 31, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 38, 0, 0), 48, 5, 2, 2 },
                    { 32, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 59, 6, 2, 2 },
                    { 33, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 22, 0, 0), 60, 6, 2, 2 },
                    { 34, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 15, 0, 0), 5, 1, 1, 3 },
                    { 35, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 6, 1, 1, 3 },
                    { 36, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 34, 0, 0), 7, 1, 1, 3 },
                    { 37, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 30, 0, 0), 8, 1, 1, 3 },
                    { 38, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 16, 2, 1, 3 },
                    { 39, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 17, 2, 1, 3 },
                    { 40, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 18, 0, 0), 27, 3, 1, 3 },
                    { 41, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 10, 0, 0), 28, 3, 1, 3 },
                    { 42, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 24, 0, 0), 29, 3, 1, 3 },
                    { 43, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 20, 0, 0), 30, 3, 1, 3 },
                    { 44, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 38, 4, 1, 3 },
                    { 45, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 12, 0, 0), 39, 4, 1, 3 },
                    { 46, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 10, 0, 0), 49, 5, 1, 3 },
                    { 47, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 41, 0, 0), 50, 5, 1, 3 },
                    { 48, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 27, 0, 0), 41, 5, 1, 3 },
                    { 49, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 29, 0, 0), 60, 6, 1, 3 },
                    { 50, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 10, 0, 0), 51, 6, 1, 3 },
                    { 51, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 43, 0, 0), 52, 6, 1, 3 },
                    { 52, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 10, 0, 0), 53, 6, 1, 3 },
                    { 53, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 10, 1, 2, 4 },
                    { 54, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 42, 0, 0), 6, 1, 2, 4 },
                    { 55, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 28, 0, 0), 16, 2, 2, 4 },
                    { 56, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 26, 0, 0), 17, 2, 2, 4 },
                    { 57, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 33, 0, 0), 18, 2, 2, 4 },
                    { 58, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 26, 0, 0), 27, 3, 2, 4 },
                    { 59, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 28, 0, 0), 38, 4, 2, 4 },
                    { 60, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 30, 0, 0), 49, 5, 2, 4 },
                    { 61, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 27, 0, 0), 50, 5, 2, 4 },
                    { 62, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 23, 0, 0), 46, 5, 2, 4 },
                    { 63, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 32, 0, 0), 60, 6, 2, 4 },
                    { 64, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 37, 0, 0), 56, 6, 2, 4 },
                    { 65, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 15, 0, 0), 57, 6, 2, 4 },
                    { 66, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 6, 1, 1, 5 },
                    { 67, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 16, 0, 0), 7, 1, 1, 5 },
                    { 68, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 8, 1, 1, 5 },
                    { 69, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 23, 0, 0), 17, 2, 1, 5 },
                    { 70, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 41, 0, 0), 18, 2, 1, 5 },
                    { 71, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 15, 0, 0), 19, 2, 1, 5 },
                    { 72, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 18, 0, 0), 28, 3, 1, 5 },
                    { 73, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 33, 0, 0), 29, 3, 1, 5 },
                    { 74, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 49, 0, 0), 39, 4, 1, 5 },
                    { 75, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 40, 0, 0), 40, 4, 1, 5 },
                    { 76, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 31, 4, 1, 5 },
                    { 77, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 32, 4, 1, 5 },
                    { 78, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 32, 0, 0), 50, 5, 1, 5 },
                    { 79, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 49, 0, 0), 41, 5, 1, 5 },
                    { 80, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 42, 5, 1, 5 },
                    { 81, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 23, 0, 0), 51, 6, 1, 5 },
                    { 82, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 52, 6, 1, 5 },
                    { 83, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 6, 1, 2, 6 },
                    { 84, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 32, 0, 0), 17, 2, 2, 6 },
                    { 85, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 17, 0, 0), 28, 3, 2, 6 },
                    { 86, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 44, 0, 0), 29, 3, 2, 6 },
                    { 87, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 39, 4, 2, 6 },
                    { 88, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 24, 0, 0), 50, 5, 2, 6 },
                    { 89, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 39, 0, 0), 56, 6, 2, 6 },
                    { 90, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 15, 0, 0), 57, 6, 2, 6 },
                    { 91, new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 58, 6, 2, 6 },
                    { 92, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 7, 1, 1, 7 },
                    { 93, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 43, 0, 0), 8, 1, 1, 7 },
                    { 94, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 31, 0, 0), 18, 2, 1, 7 },
                    { 95, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 35, 0, 0), 19, 2, 1, 7 },
                    { 96, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 35, 0, 0), 20, 2, 1, 7 },
                    { 97, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 21, 0, 0), 29, 3, 1, 7 },
                    { 98, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 30, 3, 1, 7 },
                    { 99, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 19, 0, 0), 40, 4, 1, 7 },
                    { 100, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 31, 4, 1, 7 },
                    { 101, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 41, 0, 0), 41, 5, 1, 7 },
                    { 102, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 15, 0, 0), 42, 5, 1, 7 },
                    { 103, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 43, 5, 1, 7 },
                    { 104, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 44, 5, 1, 7 },
                    { 105, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 21, 0, 0), 45, 5, 1, 7 },
                    { 106, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 32, 0, 0), 52, 6, 1, 7 },
                    { 107, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 53, 6, 1, 7 },
                    { 108, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 54, 6, 1, 7 },
                    { 109, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 33, 0, 0), 55, 6, 1, 7 },
                    { 110, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 25, 0, 0), 7, 1, 2, 8 },
                    { 111, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 43, 0, 0), 8, 1, 2, 8 },
                    { 112, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 31, 0, 0), 18, 2, 2, 8 },
                    { 113, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 43, 0, 0), 19, 2, 2, 8 },
                    { 114, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 29, 3, 2, 8 },
                    { 115, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 30, 3, 2, 8 },
                    { 116, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 15, 0, 0), 26, 3, 2, 8 },
                    { 117, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 43, 0, 0), 40, 4, 2, 8 },
                    { 118, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 27, 0, 0), 46, 5, 2, 8 },
                    { 119, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 32, 0, 0), 47, 5, 2, 8 },
                    { 120, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 27, 0, 0), 48, 5, 2, 8 },
                    { 121, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 42, 0, 0), 57, 6, 2, 8 },
                    { 122, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 23, 0, 0), 8, 1, 1, 9 },
                    { 123, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 31, 0, 0), 9, 1, 1, 9 },
                    { 124, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 19, 2, 1, 9 },
                    { 125, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 20, 2, 1, 9 },
                    { 126, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 39, 0, 0), 11, 2, 1, 9 },
                    { 127, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 21, 0, 0), 12, 2, 1, 9 },
                    { 128, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 31, 0, 0), 30, 3, 1, 9 },
                    { 129, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 21, 3, 1, 9 },
                    { 130, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 19, 0, 0), 22, 3, 1, 9 },
                    { 131, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 31, 4, 1, 9 },
                    { 132, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 32, 4, 1, 9 },
                    { 133, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 21, 0, 0), 33, 4, 1, 9 },
                    { 134, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 19, 0, 0), 34, 4, 1, 9 },
                    { 135, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 43, 0, 0), 35, 4, 1, 9 },
                    { 136, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 42, 5, 1, 9 },
                    { 137, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 43, 5, 1, 9 },
                    { 138, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 44, 5, 1, 9 },
                    { 139, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 45, 5, 1, 9 },
                    { 140, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 48, 0, 0), 53, 6, 1, 9 },
                    { 141, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 54, 6, 1, 9 },
                    { 142, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 28, 0, 0), 55, 6, 1, 9 },
                    { 143, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 56, 6, 1, 9 },
                    { 144, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 48, 0, 0), 57, 6, 1, 9 },
                    { 145, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 34, 0, 0), 8, 1, 2, 10 },
                    { 146, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 36, 0, 0), 19, 2, 2, 10 },
                    { 147, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 20, 2, 2, 10 },
                    { 148, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 25, 0, 0), 30, 3, 2, 10 },
                    { 149, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 30, 0, 0), 26, 3, 2, 10 },
                    { 150, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 36, 0, 0), 27, 3, 2, 10 },
                    { 151, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 30, 0, 0), 36, 4, 2, 10 },
                    { 152, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 32, 0, 0), 47, 5, 2, 10 },
                    { 153, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 20, 0, 0), 48, 5, 2, 10 },
                    { 154, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 43, 0, 0), 49, 5, 2, 10 },
                    { 155, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 58, 6, 2, 10 },
                    { 156, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 21, 0, 0), 2, 1, 1, 11 },
                    { 157, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 20, 0, 0), 3, 1, 1, 11 },
                    { 158, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 28, 0, 0), 13, 2, 1, 11 },
                    { 159, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 24, 0, 0), 14, 2, 1, 11 },
                    { 160, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 28, 0, 0), 15, 2, 1, 11 },
                    { 161, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 40, 0, 0), 16, 2, 1, 11 },
                    { 162, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 17, 2, 1, 11 },
                    { 163, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 24, 0, 0), 24, 3, 1, 11 },
                    { 164, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 44, 0, 0), 25, 3, 1, 11 },
                    { 165, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 22, 0, 0), 26, 3, 1, 11 },
                    { 166, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 27, 3, 1, 11 },
                    { 167, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 28, 3, 1, 11 },
                    { 168, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 34, 0, 0), 35, 4, 1, 11 },
                    { 169, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 25, 0, 0), 36, 4, 1, 11 },
                    { 170, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 48, 0, 0), 37, 4, 1, 11 },
                    { 171, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 38, 4, 1, 11 },
                    { 172, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 46, 5, 1, 11 },
                    { 173, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 22, 0, 0), 47, 5, 1, 11 },
                    { 174, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 46, 0, 0), 48, 5, 1, 11 },
                    { 175, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 57, 6, 1, 11 },
                    { 176, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 10, 0, 0), 58, 6, 1, 11 },
                    { 177, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 21, 0, 0), 59, 6, 1, 11 },
                    { 178, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 33, 0, 0), 60, 6, 1, 11 },
                    { 179, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 10, 0, 0), 51, 6, 1, 11 },
                    { 180, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 34, 0, 0), 7, 1, 2, 12 },
                    { 181, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 44, 0, 0), 8, 1, 2, 12 },
                    { 182, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 23, 0, 0), 9, 1, 2, 12 },
                    { 183, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 37, 0, 0), 18, 2, 2, 12 },
                    { 184, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 15, 0, 0), 19, 2, 2, 12 },
                    { 185, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 25, 0, 0), 20, 2, 2, 12 },
                    { 186, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 29, 3, 2, 12 },
                    { 187, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 35, 0, 0), 30, 3, 2, 12 },
                    { 188, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 25, 0, 0), 40, 4, 2, 12 },
                    { 189, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 25, 0, 0), 36, 4, 2, 12 },
                    { 190, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 33, 0, 0), 46, 5, 2, 12 },
                    { 191, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 33, 0, 0), 57, 6, 2, 12 },
                    { 192, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 24, 0, 0), 58, 6, 2, 12 },
                    { 193, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 43, 0, 0), 3, 1, 1, 13 },
                    { 194, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 21, 0, 0), 4, 1, 1, 13 },
                    { 195, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 24, 0, 0), 14, 2, 1, 13 },
                    { 196, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 10, 0, 0), 15, 2, 1, 13 },
                    { 197, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 33, 0, 0), 16, 2, 1, 13 },
                    { 198, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 24, 0, 0), 17, 2, 1, 13 },
                    { 199, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 16, 0, 0), 18, 2, 1, 13 },
                    { 200, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 35, 0, 0), 25, 3, 1, 13 },
                    { 201, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 26, 3, 1, 13 },
                    { 202, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 36, 4, 1, 13 },
                    { 203, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 37, 4, 1, 13 },
                    { 204, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 38, 4, 1, 13 },
                    { 205, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 35, 0, 0), 39, 4, 1, 13 },
                    { 206, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 40, 4, 1, 13 },
                    { 207, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 24, 0, 0), 47, 5, 1, 13 },
                    { 208, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 34, 0, 0), 48, 5, 1, 13 },
                    { 209, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 49, 5, 1, 13 },
                    { 210, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 27, 0, 0), 50, 5, 1, 13 },
                    { 211, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 41, 5, 1, 13 },
                    { 212, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 32, 0, 0), 58, 6, 1, 13 },
                    { 213, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 31, 0, 0), 59, 6, 1, 13 },
                    { 214, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 60, 6, 1, 13 },
                    { 215, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 51, 6, 1, 13 },
                    { 216, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 52, 6, 1, 13 },
                    { 217, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 24, 0, 0), 8, 1, 2, 14 },
                    { 218, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 39, 0, 0), 19, 2, 2, 14 },
                    { 219, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 35, 0, 0), 20, 2, 2, 14 },
                    { 220, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 32, 0, 0), 30, 3, 2, 14 },
                    { 221, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 28, 0, 0), 36, 4, 2, 14 },
                    { 222, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 44, 0, 0), 37, 4, 2, 14 },
                    { 223, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 30, 0, 0), 38, 4, 2, 14 },
                    { 224, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 47, 5, 2, 14 },
                    { 225, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 27, 0, 0), 48, 5, 2, 14 },
                    { 226, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 27, 0, 0), 49, 5, 2, 14 },
                    { 227, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 31, 0, 0), 58, 6, 2, 14 },
                    { 228, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 28, 0, 0), 4, 1, 1, 15 },
                    { 229, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 22, 0, 0), 5, 1, 1, 15 },
                    { 230, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 29, 0, 0), 6, 1, 1, 15 },
                    { 231, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 7, 1, 1, 15 },
                    { 232, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 8, 1, 1, 15 },
                    { 233, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 15, 2, 1, 15 },
                    { 234, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 41, 0, 0), 16, 2, 1, 15 },
                    { 235, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 46, 0, 0), 26, 3, 1, 15 },
                    { 236, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 27, 3, 1, 15 },
                    { 237, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 30, 0, 0), 28, 3, 1, 15 },
                    { 238, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 37, 4, 1, 15 },
                    { 239, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 38, 4, 1, 15 },
                    { 240, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 35, 0, 0), 48, 5, 1, 15 },
                    { 241, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 43, 0, 0), 49, 5, 1, 15 },
                    { 242, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 30, 0, 0), 50, 5, 1, 15 },
                    { 243, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 22, 0, 0), 41, 5, 1, 15 },
                    { 244, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 59, 6, 1, 15 },
                    { 245, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 46, 0, 0), 60, 6, 1, 15 },
                    { 246, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 19, 0, 0), 9, 1, 2, 16 },
                    { 247, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 26, 0, 0), 10, 1, 2, 16 },
                    { 248, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 26, 0, 0), 20, 2, 2, 16 },
                    { 249, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 15, 0, 0), 26, 3, 2, 16 },
                    { 250, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 15, 0, 0), 37, 4, 2, 16 },
                    { 251, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 22, 0, 0), 38, 4, 2, 16 },
                    { 252, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 39, 0, 0), 48, 5, 2, 16 },
                    { 253, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 39, 0, 0), 49, 5, 2, 16 },
                    { 254, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 26, 0, 0), 50, 5, 2, 16 },
                    { 255, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 59, 6, 2, 16 },
                    { 256, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 38, 0, 0), 60, 6, 2, 16 },
                    { 257, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 10, 0, 0), 5, 1, 1, 17 },
                    { 258, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 6, 1, 1, 17 },
                    { 259, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 31, 0, 0), 16, 2, 1, 17 },
                    { 260, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 41, 0, 0), 17, 2, 1, 17 },
                    { 261, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 27, 3, 1, 17 },
                    { 262, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 46, 0, 0), 28, 3, 1, 17 },
                    { 263, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 37, 0, 0), 29, 3, 1, 17 },
                    { 264, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 30, 3, 1, 17 },
                    { 265, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 38, 4, 1, 17 },
                    { 266, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 44, 0, 0), 39, 4, 1, 17 },
                    { 267, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 41, 0, 0), 40, 4, 1, 17 },
                    { 268, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 49, 5, 1, 17 },
                    { 269, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 50, 5, 1, 17 },
                    { 270, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 41, 5, 1, 17 },
                    { 271, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 42, 5, 1, 17 },
                    { 272, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 41, 0, 0), 43, 5, 1, 17 },
                    { 273, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 43, 0, 0), 60, 6, 1, 17 },
                    { 274, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 51, 6, 1, 17 },
                    { 275, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 52, 6, 1, 17 },
                    { 276, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 48, 0, 0), 53, 6, 1, 17 },
                    { 277, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 54, 6, 1, 17 },
                    { 278, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 20, 0, 0), 10, 1, 2, 18 },
                    { 279, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 23, 0, 0), 6, 1, 2, 18 },
                    { 280, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 39, 0, 0), 7, 1, 2, 18 },
                    { 281, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 32, 0, 0), 16, 2, 2, 18 },
                    { 282, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 42, 0, 0), 17, 2, 2, 18 },
                    { 283, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 27, 3, 2, 18 },
                    { 284, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 40, 0, 0), 28, 3, 2, 18 },
                    { 285, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 25, 0, 0), 29, 3, 2, 18 },
                    { 286, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 26, 0, 0), 38, 4, 2, 18 },
                    { 287, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 39, 4, 2, 18 },
                    { 288, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 44, 0, 0), 40, 4, 2, 18 },
                    { 289, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 41, 0, 0), 49, 5, 2, 18 },
                    { 290, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 44, 0, 0), 60, 6, 2, 18 },
                    { 291, new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 15, 0, 0), 56, 6, 2, 18 },
                    { 292, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 15, 0, 0), 1, 1, 1, 19 },
                    { 293, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 2, 1, 1, 19 },
                    { 294, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 39, 0, 0), 3, 1, 1, 19 },
                    { 295, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 16, 0, 0), 12, 2, 1, 19 },
                    { 296, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 28, 0, 0), 13, 2, 1, 19 },
                    { 297, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 32, 0, 0), 23, 3, 1, 19 },
                    { 298, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 49, 0, 0), 24, 3, 1, 19 },
                    { 299, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 25, 3, 1, 19 },
                    { 300, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 49, 0, 0), 34, 4, 1, 19 },
                    { 301, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 29, 0, 0), 35, 4, 1, 19 },
                    { 302, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 34, 0, 0), 36, 4, 1, 19 },
                    { 303, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 45, 5, 1, 19 },
                    { 304, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 21, 0, 0), 46, 5, 1, 19 },
                    { 305, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 47, 5, 1, 19 },
                    { 306, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 48, 5, 1, 19 },
                    { 307, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 48, 0, 0), 56, 6, 1, 19 },
                    { 308, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 22, 0, 0), 57, 6, 1, 19 },
                    { 309, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 19, 0, 0), 58, 6, 1, 19 },
                    { 310, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 59, 6, 1, 19 },
                    { 311, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 43, 0, 0), 60, 6, 1, 19 },
                    { 312, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 28, 0, 0), 6, 1, 2, 20 },
                    { 313, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 40, 0, 0), 7, 1, 2, 20 },
                    { 314, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 38, 0, 0), 8, 1, 2, 20 },
                    { 315, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 22, 0, 0), 17, 2, 2, 20 },
                    { 316, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 41, 0, 0), 18, 2, 2, 20 },
                    { 317, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 19, 0, 0), 19, 2, 2, 20 },
                    { 318, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 33, 0, 0), 28, 3, 2, 20 },
                    { 319, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 28, 0, 0), 29, 3, 2, 20 },
                    { 320, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 28, 0, 0), 39, 4, 2, 20 },
                    { 321, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 35, 0, 0), 40, 4, 2, 20 },
                    { 322, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 23, 0, 0), 36, 4, 2, 20 },
                    { 323, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 19, 0, 0), 50, 5, 2, 20 },
                    { 324, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 46, 5, 2, 20 },
                    { 325, new DateTime(2026, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 29, 0, 0), 56, 6, 2, 20 },
                    { 326, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 37, 0, 0), 2, 1, 1, 21 },
                    { 327, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 16, 0, 0), 3, 1, 1, 21 },
                    { 328, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 35, 0, 0), 4, 1, 1, 21 },
                    { 329, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 24, 0, 0), 13, 2, 1, 21 },
                    { 330, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 14, 2, 1, 21 },
                    { 331, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 48, 0, 0), 15, 2, 1, 21 },
                    { 332, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 23, 0, 0), 16, 2, 1, 21 },
                    { 333, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 21, 0, 0), 17, 2, 1, 21 },
                    { 334, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 24, 3, 1, 21 },
                    { 335, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 32, 0, 0), 25, 3, 1, 21 },
                    { 336, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 26, 3, 1, 21 },
                    { 337, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 35, 4, 1, 21 },
                    { 338, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 25, 0, 0), 36, 4, 1, 21 },
                    { 339, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 34, 0, 0), 46, 5, 1, 21 },
                    { 340, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 29, 0, 0), 47, 5, 1, 21 },
                    { 341, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 46, 0, 0), 48, 5, 1, 21 },
                    { 342, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 27, 0, 0), 49, 5, 1, 21 },
                    { 343, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 57, 6, 1, 21 },
                    { 344, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 28, 0, 0), 58, 6, 1, 21 },
                    { 345, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 18, 0, 0), 59, 6, 1, 21 },
                    { 346, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 37, 0, 0), 60, 6, 1, 21 },
                    { 347, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 51, 6, 1, 21 },
                    { 348, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 26, 0, 0), 7, 1, 2, 22 },
                    { 349, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 31, 0, 0), 8, 1, 2, 22 },
                    { 350, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 29, 0, 0), 9, 1, 2, 22 },
                    { 351, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 19, 0, 0), 18, 2, 2, 22 },
                    { 352, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 33, 0, 0), 19, 2, 2, 22 },
                    { 353, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 22, 0, 0), 20, 2, 2, 22 },
                    { 354, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 30, 0, 0), 29, 3, 2, 22 },
                    { 355, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 24, 0, 0), 30, 3, 2, 22 },
                    { 356, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 23, 0, 0), 40, 4, 2, 22 },
                    { 357, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 20, 0, 0), 36, 4, 2, 22 },
                    { 358, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 46, 5, 2, 22 },
                    { 359, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 47, 5, 2, 22 },
                    { 360, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 21, 0, 0), 57, 6, 2, 22 },
                    { 361, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 5, 1, 1, 23 },
                    { 362, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 6, 1, 1, 23 },
                    { 363, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 27, 0, 0), 7, 1, 1, 23 },
                    { 364, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 44, 0, 0), 16, 2, 1, 23 },
                    { 365, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 17, 2, 1, 23 },
                    { 366, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 32, 0, 0), 27, 3, 1, 23 },
                    { 367, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 28, 3, 1, 23 },
                    { 368, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 44, 0, 0), 38, 4, 1, 23 },
                    { 369, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 39, 4, 1, 23 },
                    { 370, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 32, 0, 0), 40, 4, 1, 23 },
                    { 371, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 46, 0, 0), 31, 4, 1, 23 },
                    { 372, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 44, 0, 0), 32, 4, 1, 23 },
                    { 373, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 49, 5, 1, 23 },
                    { 374, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 49, 0, 0), 50, 5, 1, 23 },
                    { 375, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 14, 0, 0), 60, 6, 1, 23 },
                    { 376, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 20, 0, 0), 51, 6, 1, 23 },
                    { 377, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 52, 6, 1, 23 },
                    { 378, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 34, 0, 0), 10, 1, 2, 24 },
                    { 379, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 27, 0, 0), 16, 2, 2, 24 },
                    { 380, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 17, 0, 0), 17, 2, 2, 24 },
                    { 381, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 36, 0, 0), 27, 3, 2, 24 },
                    { 382, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 41, 0, 0), 28, 3, 2, 24 },
                    { 383, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 25, 0, 0), 38, 4, 2, 24 },
                    { 384, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 29, 0, 0), 39, 4, 2, 24 },
                    { 385, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 49, 5, 2, 24 },
                    { 386, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 43, 0, 0), 50, 5, 2, 24 },
                    { 387, new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 32, 0, 0), 60, 6, 2, 24 },
                    { 388, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 28, 0, 0), 6, 1, 1, 25 },
                    { 389, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 7, 1, 1, 25 },
                    { 390, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 19, 0, 0), 8, 1, 1, 25 },
                    { 391, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 34, 0, 0), 9, 1, 1, 25 },
                    { 392, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 33, 0, 0), 10, 1, 1, 25 },
                    { 393, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 17, 2, 1, 25 },
                    { 394, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 18, 2, 1, 25 },
                    { 395, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 46, 0, 0), 28, 3, 1, 25 },
                    { 396, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 34, 0, 0), 29, 3, 1, 25 },
                    { 397, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 39, 4, 1, 25 },
                    { 398, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 40, 4, 1, 25 },
                    { 399, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 50, 5, 1, 25 },
                    { 400, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 18, 0, 0), 41, 5, 1, 25 },
                    { 401, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 39, 0, 0), 51, 6, 1, 25 },
                    { 402, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 29, 0, 0), 52, 6, 1, 25 },
                    { 403, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 16, 0, 0), 53, 6, 1, 25 },
                    { 404, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 21, 0, 0), 6, 1, 2, 26 },
                    { 405, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 27, 0, 0), 7, 1, 2, 26 },
                    { 406, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 17, 2, 2, 26 },
                    { 407, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 20, 0, 0), 18, 2, 2, 26 },
                    { 408, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 36, 0, 0), 28, 3, 2, 26 },
                    { 409, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 15, 0, 0), 29, 3, 2, 26 },
                    { 410, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 25, 0, 0), 30, 3, 2, 26 },
                    { 411, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 35, 0, 0), 39, 4, 2, 26 },
                    { 412, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 42, 0, 0), 50, 5, 2, 26 },
                    { 413, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 46, 5, 2, 26 },
                    { 414, new DateTime(2026, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 22, 0, 0), 56, 6, 2, 26 },
                    { 415, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 48, 0, 0), 7, 1, 1, 27 },
                    { 416, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 48, 0, 0), 8, 1, 1, 27 },
                    { 417, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 20, 0, 0), 9, 1, 1, 27 },
                    { 418, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 34, 0, 0), 18, 2, 1, 27 },
                    { 419, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 43, 0, 0), 19, 2, 1, 27 },
                    { 420, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 12, 0, 0), 20, 2, 1, 27 },
                    { 421, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 11, 2, 1, 27 },
                    { 422, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 25, 0, 0), 29, 3, 1, 27 },
                    { 423, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 30, 3, 1, 27 },
                    { 424, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 22, 0, 0), 40, 4, 1, 27 },
                    { 425, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 31, 4, 1, 27 },
                    { 426, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 40, 0, 0), 32, 4, 1, 27 },
                    { 427, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 33, 4, 1, 27 },
                    { 428, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 41, 5, 1, 27 },
                    { 429, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 10, 0, 0), 42, 5, 1, 27 },
                    { 430, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 31, 0, 0), 52, 6, 1, 27 },
                    { 431, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 18, 0, 0), 53, 6, 1, 27 },
                    { 432, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 33, 0, 0), 54, 6, 1, 27 },
                    { 433, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 39, 0, 0), 55, 6, 1, 27 },
                    { 434, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 33, 0, 0), 56, 6, 1, 27 },
                    { 435, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 20, 0, 0), 7, 1, 2, 28 },
                    { 436, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 26, 0, 0), 8, 1, 2, 28 },
                    { 437, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 29, 0, 0), 9, 1, 2, 28 },
                    { 438, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 36, 0, 0), 18, 2, 2, 28 },
                    { 439, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 34, 0, 0), 19, 2, 2, 28 },
                    { 440, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 30, 0, 0), 29, 3, 2, 28 },
                    { 441, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 37, 0, 0), 40, 4, 2, 28 },
                    { 442, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 37, 0, 0), 36, 4, 2, 28 },
                    { 443, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 20, 0, 0), 37, 4, 2, 28 },
                    { 444, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 33, 0, 0), 46, 5, 2, 28 },
                    { 445, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 33, 0, 0), 57, 6, 2, 28 },
                    { 446, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 43, 0, 0), 58, 6, 2, 28 },
                    { 447, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 16, 0, 0), 8, 1, 1, 29 },
                    { 448, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 35, 0, 0), 9, 1, 1, 29 },
                    { 449, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 45, 0, 0), 10, 1, 1, 29 },
                    { 450, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 27, 0, 0), 1, 1, 1, 29 },
                    { 451, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 33, 0, 0), 19, 2, 1, 29 },
                    { 452, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 48, 0, 0), 20, 2, 1, 29 },
                    { 453, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 25, 0, 0), 30, 3, 1, 29 },
                    { 454, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 31, 0, 0), 21, 3, 1, 29 },
                    { 455, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 22, 3, 1, 29 },
                    { 456, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 27, 0, 0), 23, 3, 1, 29 },
                    { 457, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 24, 3, 1, 29 },
                    { 458, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 46, 0, 0), 31, 4, 1, 29 },
                    { 459, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 32, 4, 1, 29 },
                    { 460, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 48, 0, 0), 42, 5, 1, 29 },
                    { 461, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 49, 0, 0), 43, 5, 1, 29 },
                    { 462, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 36, 0, 0), 44, 5, 1, 29 },
                    { 463, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 45, 5, 1, 29 },
                    { 464, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 19, 0, 0), 46, 5, 1, 29 },
                    { 465, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 27, 0, 0), 53, 6, 1, 29 },
                    { 466, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 13, 0, 0), 54, 6, 1, 29 },
                    { 467, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 55, 6, 1, 29 },
                    { 468, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 43, 0, 0), 8, 1, 2, 30 },
                    { 469, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 38, 0, 0), 9, 1, 2, 30 },
                    { 470, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 10, 1, 2, 30 },
                    { 471, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 19, 2, 2, 30 },
                    { 472, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 38, 0, 0), 20, 2, 2, 30 },
                    { 473, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 31, 0, 0), 16, 2, 2, 30 },
                    { 474, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 30, 3, 2, 30 },
                    { 475, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 27, 0, 0), 26, 3, 2, 30 },
                    { 476, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 36, 0, 0), 36, 4, 2, 30 },
                    { 477, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 19, 0, 0), 47, 5, 2, 30 },
                    { 478, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 34, 0, 0), 58, 6, 2, 30 },
                    { 479, new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 24, 0, 0), 59, 6, 2, 30 },
                    { 480, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 39, 0, 0), 9, 1, 1, 31 },
                    { 481, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 38, 0, 0), 10, 1, 1, 31 },
                    { 482, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 49, 0, 0), 1, 1, 1, 31 },
                    { 483, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 31, 0, 0), 2, 1, 1, 31 },
                    { 484, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 42, 0, 0), 20, 2, 1, 31 },
                    { 485, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 47, 0, 0), 11, 2, 1, 31 },
                    { 486, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 17, 0, 0), 12, 2, 1, 31 },
                    { 487, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 28, 0, 0), 13, 2, 1, 31 },
                    { 488, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 18, 0, 0), 21, 3, 1, 31 },
                    { 489, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 43, 0, 0), 22, 3, 1, 31 },
                    { 490, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 11, 0, 0), 23, 3, 1, 31 },
                    { 491, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 26, 0, 0), 32, 4, 1, 31 },
                    { 492, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 40, 0, 0), 33, 4, 1, 31 },
                    { 493, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 34, 0, 0), 43, 5, 1, 31 },
                    { 494, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 49, 0, 0), 44, 5, 1, 31 },
                    { 495, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 24, 0, 0), 54, 6, 1, 31 },
                    { 496, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 22, 0, 0), 55, 6, 1, 31 },
                    { 497, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 44, 0, 0), 56, 6, 1, 31 },
                    { 498, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 32, 0, 0), 9, 1, 2, 32 },
                    { 499, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 10, 1, 2, 32 },
                    { 500, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 6, 1, 2, 32 },
                    { 501, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 37, 0, 0), 20, 2, 2, 32 },
                    { 502, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 36, 0, 0), 16, 2, 2, 32 },
                    { 503, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 37, 0, 0), 17, 2, 2, 32 },
                    { 504, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 42, 0, 0), 26, 3, 2, 32 },
                    { 505, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 38, 0, 0), 37, 4, 2, 32 },
                    { 506, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 18, 0, 0), 48, 5, 2, 32 },
                    { 507, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 24, 0, 0), 49, 5, 2, 32 },
                    { 508, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 25, 0, 0), 59, 6, 2, 32 },
                    { 509, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 16, 0, 0), 60, 6, 2, 32 },
                    { 510, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 13, 35, 0, 0), 56, 6, 2, 32 }
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
