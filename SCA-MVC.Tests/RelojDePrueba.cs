using Microsoft.Extensions.Time.Testing;
using SCA_MVC.Helpers;

namespace SCA_MVC.Tests;

// Un reloj que no avanza: siempre marca la hora que le pidas. Así un test puede decir
// "son las 23:30 del 10 de marzo en Argentina" y dar el mismo resultado en cualquier
// máquina y a cualquier hora, sin esperar a que sea esa hora de verdad.
public static class RelojDePrueba
{
    // La hora se escribe como se lee en un reloj de Argentina. Por dentro el reloj
    // guarda el instante en UTC, igual que el de la aplicación.
    public static FakeTimeProvider EnArgentina(int anio, int mes, int dia, int hora, int minuto)
    {
        var enArgentina = new DateTime(anio, mes, dia, hora, minuto, 0);
        var utc = TimeZoneInfo.ConvertTimeToUtc(enArgentina, RelojArgentina.Zona);

        var reloj = new FakeTimeProvider(new DateTimeOffset(utc));
        reloj.SetLocalTimeZone(RelojArgentina.Zona);
        return reloj;
    }

    // Para los tests que necesitan un reloj pero no prueban nada de la hora
    public static FakeTimeProvider Cualquiera() => EnArgentina(2026, 3, 10, 12, 0);
}
