using SCA_MVC.Models;

namespace SCA_MVC.Tests;

// Los tests más simples que hay: prueban una clase sola, sin base de datos.
public class ServicioTests
{
    [Fact]
    public void Estado_SinDuracionEsActivo()
    {
        var servicio = new Servicio { DuracionMinutos = null };

        Assert.Equal("Activo", servicio.Estado);
    }

    [Fact]
    public void Estado_ConDuracionEsFinalizado()
    {
        var servicio = new Servicio { DuracionMinutos = 45 };

        Assert.Equal("Finalizado", servicio.Estado);
    }

    [Fact]
    public void TotalGeneral_SumaComensalesEInvitados()
    {
        var servicio = new Servicio { TotalComensales = 40, TotalInvitados = 3 };

        Assert.Equal(43, servicio.TotalGeneral);
    }
}
