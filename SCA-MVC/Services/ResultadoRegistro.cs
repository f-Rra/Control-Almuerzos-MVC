namespace SCA_MVC.Services
{
    // Qué pasó al intentar registrar a un empleado en un servicio. Con un bool no
    // alcanza: hay varios motivos para no registrar y la pantalla le explica cuál al usuario.
    public enum ResultadoRegistro
    {
        Registrado,
        YaRegistrado,
        ServicioInexistente,
        ServicioFinalizado,
        LugarIncorrecto
    }

    public static class ResultadoRegistroExtensiones
    {
        // El texto que se le muestra al usuario cuando no se pudo registrar
        public static string Mensaje(this ResultadoRegistro resultado) => resultado switch
        {
            ResultadoRegistro.YaRegistrado => "Ya registrado en este servicio",
            ResultadoRegistro.ServicioInexistente => "El servicio no existe",
            ResultadoRegistro.ServicioFinalizado => "El servicio ya terminó",
            ResultadoRegistro.LugarIncorrecto => "El lugar no corresponde a ese servicio",
            _ => string.Empty
        };
    }
}
