using SCA_MVC.Helpers;

namespace SCA_MVC.Tests;

// Prueban el reloj de la aplicación tal cual, sin falsearlo: el instante en UTC que
// le llega a un servidor de Azure tiene que salir convertido a la hora de Argentina.
public class RelojArgentinaTests
{
    [Fact]
    public void LaZona_EsUtcMenosTresHoras()
    {
        // 02:30 UTC del 15 de octubre son las 23:30 del 14 en Argentina
        var utc = new DateTimeOffset(2026, 10, 15, 2, 30, 0, TimeSpan.Zero);

        var enArgentina = TimeZoneInfo.ConvertTime(utc, new RelojArgentina().LocalTimeZone);

        Assert.Equal(new DateTime(2026, 10, 14, 23, 30, 0), enArgentina.DateTime);
    }

    [Fact]
    public void Hoy_NoCambiaDeDiaAntesDeLaMedianocheDeArgentina()
    {
        // Es el 15 en UTC, pero en Argentina todavía es el 14
        var reloj = RelojDePrueba.EnArgentina(2026, 10, 14, 23, 30);

        Assert.Equal(new DateTime(2026, 10, 15, 2, 30, 0), reloj.GetUtcNow().UtcDateTime);
        Assert.Equal(new DateTime(2026, 10, 14), reloj.Hoy());
        Assert.Equal(new DateTime(2026, 10, 14, 23, 30, 0), reloj.Ahora());
    }
}
