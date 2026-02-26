using Microsoft.AspNetCore.Mvc;
using SCA_MVC.Services;
using SCA_MVC.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SCA_MVC.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IReporteNegocio _reporteNegocio;
        private readonly ILugarNegocio _lugarNegocio;
        private readonly IServicioNegocio _servicioNegocio;

        public ReporteController(IReporteNegocio reporteNegocio, ILugarNegocio lugarNegocio, IServicioNegocio servicioNegocio)
        {
            _reporteNegocio = reporteNegocio;
            _lugarNegocio = lugarNegocio;
            _servicioNegocio = servicioNegocio;
        }

        // GET: Reporte
        public async Task<IActionResult> Index(DateTime? desde, DateTime? hasta, int? idLugar, string tipo = "servicios")
        {
            var model = new ReporteViewModel
            {
                // Ajustado a febrero 2026 para coincidir con los datos del Seed
                FechaDesde  = desde   ?? new DateTime(2026, 2, 1),
                FechaHasta  = hasta   ?? new DateTime(2026, 2, 28),
                IdLugar     = idLugar,
                TipoReporte = tipo
            };

            ViewBag.Lugares = await _lugarNegocio.ListarAsync();

            if (tipo == "servicios")
            {
                model.ListaServicios  = await _reporteNegocio.ObtenerListaServiciosAsync(model.FechaDesde, model.FechaHasta, model.IdLugar);
                model.TotalServicios  = model.ListaServicios.Count;
            }
            else if (tipo == "empresas")
            {
                model.AsistenciasPorEmpresa = await _reporteNegocio.ObtenerAsistenciasPorEmpresaAsync(model.FechaDesde, model.FechaHasta, model.IdLugar);
            }
            else if (tipo == "dias")
            {
                model.AsistenciasPorDia = await _reporteNegocio.ObtenerDistribucionPorDiaAsync(model.FechaDesde, model.FechaHasta, model.IdLugar);
            }
            else // cobertura
            {
                model.CoberturaVsProyeccion = await _reporteNegocio.ObtenerCoberturaVsProyeccionAsync(model.FechaDesde, model.FechaHasta, model.IdLugar);
            }

            return View(model);
        }

        // GET: Reporte/ExportarPDF
        public async Task<IActionResult> ExportarPDF(DateTime desde, DateTime hasta, int? idLugar, string tipo = "cobertura")
        {
            // ── Paleta del proyecto ──────────────────────────────────
            var colorPrimary   = Color.FromHex("#FFC107");  // Amarillo ámbar
            var colorPrimaryDk = Color.FromHex("#E6A800");  // Ámbar oscuro para header
            var colorBgFaint   = Color.FromHex("#FFFBF0");  // Fondo cálido filas pares
            var colorText      = Color.FromHex("#2D2D2D");  // Texto oscuro
            var colorMuted     = Color.FromHex("#6B6B6B");  // Gris secundario
            var colorBorder    = Color.FromHex("#E0D5B0");  // Borde suave

            // ── Datos y metadatos ────────────────────────────────────
            var lugares = await _lugarNegocio.ListarAsync();
            var lugarNombre = idLugar.HasValue
                ? lugares.FirstOrDefault(l => l.IdLugar == idLugar)?.Nombre ?? "Todos"
                : "Todos los lugares";

            var tipoNombre = tipo switch
            {
                "servicios" => "Lista de Servicios",
                "empresas"  => "Asistencias por Empresa",
                "dias"      => "Distribución por Día de Semana",
                _           => "Cobertura vs Proyección"
            };

            var tipoDescripcion = tipo switch
            {
                "servicios" => "Todos los servicios del período: fecha, lugar, proyección, duración, comensales, invitados y total general. Útil para revisión histórica día por día.",
                "empresas"  => "Total de asistencias por cada empresa del predio. Permite comparativa, ranking y análisis de participación corporativa. Útil para facturación segmentada.",
                "dias"      => "Distribución y patrones de asistencia por día de la semana. Identifica días pico y días bajos para optimizar compras y proyecciones.",
                _           => "Comparación entre proyección inicial y asistencia real por jornada. Muestra diferencia absoluta y porcentaje de cobertura. Mejora la planificación futura."
            };

            // Cargar solo los datos del tipo seleccionado
            var servicios = tipo == "servicios"
                ? await _reporteNegocio.ObtenerListaServiciosAsync(desde, hasta, idLugar)
                : new List<ServicioReporte>();

            var cobertura = tipo == "cobertura" || tipo == ""
                ? await _reporteNegocio.ObtenerCoberturaVsProyeccionAsync(desde, hasta, idLugar)
                : new List<CoberturaReporte>();

            var empresas = tipo == "empresas"
                ? await _reporteNegocio.ObtenerAsistenciasPorEmpresaAsync(desde, hasta, idLugar)
                : new List<AsistenciaPorEmpresa>();

            var dias = tipo == "dias"
                ? await _reporteNegocio.ObtenerDistribucionPorDiaAsync(desde, hasta, idLugar)
                : new List<AsistenciaPorDia>();

            int totalRegistros = tipo switch
            {
                "servicios" => servicios.Count,
                "empresas"  => empresas.Sum(x => x.TotalAsistencias),
                "dias"      => dias.Sum(x => x.Total),
                _           => cobertura.Sum(x => x.Atendidos)
            };

            // ── Documento ────────────────────────────────────────────
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(1.5f, Unit.Centimetre);
                    page.MarginVertical(1.2f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9.5f).FontFamily(Fonts.Verdana).FontColor(colorText));

                    // ══ HEADER ══════════════════════════════════════════
                    page.Header().Column(hdr =>
                    {
                        // Barra superior con color primario
                        hdr.Item()
                            .Background(colorPrimaryDk)
                            .PaddingHorizontal(12).PaddingVertical(10)
                            .Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("SISTEMA CONTROL DE ALMUERZOS")
                                        .FontSize(13).Bold()
                                        .FontColor(Colors.White);
                                });
                                // Info reporte (alineado derecha)
                                row.RelativeItem().AlignRight().Column(col =>
                                {
                                    col.Item().Text(tipoNombre.ToUpper())
                                        .FontSize(11).Bold().FontColor(Colors.White);
                                    col.Item().Text($"Período: {desde:dd/MM/yyyy}  →  {hasta:dd/MM/yyyy}")
                                        .FontSize(8.5f).FontColor(Color.FromHex("#FFF8DC"));
                                    col.Item().Text($"Lugar: {lugarNombre}")
                                        .FontSize(8.5f).FontColor(Color.FromHex("#FFF8DC"));
                                });
                            });

                        // Línea decorativa dorada
                        hdr.Item().Height(3).Background(colorPrimary);

                        // Franja de metadatos: período + total + emisión
                        hdr.Item()
                            .Background(colorBgFaint)
                            .BorderBottom(1).BorderColor(colorBorder)
                            .PaddingHorizontal(12).PaddingVertical(5)
                            .Row(row =>
                            {
                                row.RelativeItem().AlignLeft().Text(text =>
                                {
                                    text.Span("Período: ").Bold().FontSize(8);
                                    text.Span($"{desde:dd/MM/yyyy} – {hasta:dd/MM/yyyy}").FontSize(8);
                                    text.Span("   Lugar: ").Bold().FontSize(8);
                                    text.Span(lugarNombre).FontSize(8);
                                });
                                row.RelativeItem().AlignCenter().Text(text =>
                                {
                                    text.Span("Total: ").Bold().FontSize(8);
                                    text.Span(totalRegistros.ToString()).FontSize(8);
                                });
                                row.RelativeItem().AlignRight().Text(text =>
                                {
                                    text.Span("Emitido: ").Bold().FontSize(8);
                                    text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8);
                                });
                            });

                        // Descripción del reporte
                        hdr.Item()
                            .Background(Colors.White)
                            .BorderBottom(2).BorderColor(colorBorder)
                            .PaddingHorizontal(12).PaddingVertical(6)
                            .Text(text =>
                            {
                                text.Span("Descripción: ").Bold().FontSize(8.5f);
                                text.Span(tipoDescripcion).FontSize(8.5f).FontColor(colorMuted);
                            });

                        hdr.Item().Height(6); // Separador
                    });

                    // ══ CONTENIDO ════════════════════════════════════════
                    page.Content().Column(col =>
                    {
                        col.Spacing(6);

                        // ── Tabla: Lista de Servicios ──────────────────────
                        if (tipo == "servicios")
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(22);
                                    c.RelativeColumn(2.2f);
                                    c.RelativeColumn(2f);
                                    c.RelativeColumn(1.5f);
                                    c.RelativeColumn(1.5f);
                                    c.RelativeColumn(1.5f);
                                    c.RelativeColumn(1.5f);
                                    c.RelativeColumn(1.8f);
                                });

                                table.Header(header =>
                                {
                                    void HC(string txt) =>
                                        header.Cell().Background(colorPrimary).Padding(5)
                                            .Text(txt).Bold().FontSize(8).FontColor(Colors.White);
                                    HC("#"); HC("Fecha"); HC("Lugar"); HC("Proyección");
                                    HC("Duración"); HC("Comensales"); HC("Invitados"); HC("Total");
                                });

                                int i = 1;
                                foreach (var item in servicios)
                                {
                                    var bg = i % 2 == 0 ? colorBgFaint : Colors.White;
                                    void RC(string txt, bool r = false)
                                    {
                                        var c = table.Cell().Background(bg).BorderBottom(1).BorderColor(colorBorder).Padding(3);
                                        if (r) c.AlignRight().Text(txt).FontSize(8.5f);
                                        else c.Text(txt).FontSize(8.5f);
                                    }
                                    RC(i.ToString());
                                    RC(item.Fecha.ToString("dd/MM/yyyy"));
                                    RC(item.Lugar);
                                    RC(item.Proyeccion.ToString(), true);
                                    RC((item.DuracionMinutos?.ToString() ?? "–"), true);
                                    RC(item.TotalComensales.ToString(), true);
                                    RC(item.TotalInvitados.ToString(), true);
                                    RC(item.TotalGeneral.ToString(), true);
                                    i++;
                                }
                            });
                        }

                        // ── Tabla: Cobertura vs Proyección ─────────────────
                        if (tipo == "cobertura" || tipo == "")
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(30);    // #
                                    c.RelativeColumn(2.5f);  // Fecha
                                    c.RelativeColumn(2f);    // Lugar
                                    c.RelativeColumn(1.5f);  // Proyección
                                    c.RelativeColumn(1.5f);  // Atendidos
                                    c.RelativeColumn(1.5f);  // Diferencia
                                    c.RelativeColumn(1.8f);  // Cobertura %
                                });

                                // Encabezado
                                table.Header(header =>
                                {
                                    void HCell(string txt) =>
                                        header.Cell()
                                            .Background(colorPrimary)
                                            .Padding(5)
                                            .Text(txt).Bold().FontSize(9).FontColor(Colors.White);

                                    HCell("#");
                                    HCell("Fecha");
                                    HCell("Lugar");
                                    HCell("Proyectados");
                                    HCell("Atendidos");
                                    HCell("Diferencia");
                                    HCell("Cobertura %");
                                });

                                // Filas
                                int i = 1;
                                foreach (var item in cobertura)
                                {
                                    var bg = i % 2 == 0 ? colorBgFaint : Colors.White;
                                    void RCell(string txt, bool right = false)
                                    {
                                        var c = table.Cell().Background(bg).BorderBottom(1).BorderColor(colorBorder).Padding(4);
                                        if (right) c.AlignRight().Text(txt).FontSize(9);
                                        else c.Text(txt).FontSize(9);
                                    }

                                    RCell(i.ToString());
                                    RCell(item.Fecha.ToString("dd/MM/yyyy"));
                                    RCell(item.Lugar);
                                    RCell(item.Proyeccion.ToString(), true);
                                    RCell(item.Atendidos.ToString(), true);
                                    var dif = item.Diferencia;
                                    var difTxt = (dif > 0 ? "+" : "") + dif;
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor(colorBorder).Padding(4)
                                        .AlignRight()
                                        .Text(difTxt).FontSize(9)
                                        .FontColor(dif < 0 ? Color.FromHex("#D32F2F") : dif > 0 ? Color.FromHex("#388E3C") : colorMuted);
                                    RCell((item.CoberturaPorcentaje?.ToString("F1") ?? "—") + "%", true);
                                    i++;
                                }
                            });
                        }

                        // ── Tabla: Asistencias por Empresa ────────────────
                        if (tipo == "empresas")
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(30);
                                    c.RelativeColumn(4f);
                                    c.RelativeColumn(2f);
                                });

                                table.Header(header =>
                                {
                                    void HCell(string txt) =>
                                        header.Cell().Background(colorPrimary).Padding(5)
                                            .Text(txt).Bold().FontSize(9).FontColor(Colors.White);
                                    HCell("#");
                                    HCell("Empresa");
                                    HCell("Total Almuerzos");
                                });

                                int i = 1;
                                foreach (var item in empresas)
                                {
                                    var bg = i % 2 == 0 ? colorBgFaint : Colors.White;
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor(colorBorder).Padding(4).Text(i.ToString()).FontSize(9);
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor(colorBorder).Padding(4).Text(item.Empresa).FontSize(9);
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor(colorBorder).Padding(4).AlignRight().Text(item.TotalAsistencias.ToString()).FontSize(9).Bold();
                                    i++;
                                }
                            });
                        }

                        // ── Tabla: Distribución por Día de Semana ─────────
                        if (tipo == "dias")
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(30);
                                    c.RelativeColumn(4f);
                                    c.RelativeColumn(2f);
                                });

                                table.Header(header =>
                                {
                                    void HCell(string txt) =>
                                        header.Cell().Background(colorPrimary).Padding(5)
                                            .Text(txt).Bold().FontSize(9).FontColor(Colors.White);
                                    HCell("#");
                                    HCell("Día de la semana");
                                    HCell("Total Almuerzos");
                                });

                                int i = 1;
                                foreach (var item in dias)
                                {
                                    var bg = i % 2 == 0 ? colorBgFaint : Colors.White;
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor(colorBorder).Padding(4).Text(i.ToString()).FontSize(9);
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor(colorBorder).Padding(4).Text(item.Dia).FontSize(9);
                                    table.Cell().Background(bg).BorderBottom(1).BorderColor(colorBorder).Padding(4).AlignRight().Text(item.Total.ToString()).FontSize(9).Bold();
                                    i++;
                                }
                            });
                        }
                    });

                    // ══ FOOTER ═══════════════════════════════════════════
                    page.Footer()
                        .BorderTop(1).BorderColor(colorBorder)
                        .PaddingTop(6).PaddingHorizontal(12)
                        .Row(row =>
                        {
                            row.RelativeItem().Text("SCA — Sistema de Control de Almuerzos")
                                .FontSize(8).FontColor(colorMuted);
                            row.RelativeItem().AlignRight().Text(x =>
                            {
                                x.Span("Página ").FontSize(8).FontColor(colorMuted);
                                x.CurrentPageNumber().FontSize(8).FontColor(colorMuted);
                                x.Span(" de ").FontSize(8).FontColor(colorMuted);
                                x.TotalPages().FontSize(8).FontColor(colorMuted);
                            });
                        });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            var fileName = $"Reporte_{tipo}_{desde:yyyyMMdd}_{hasta:yyyyMMdd}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
