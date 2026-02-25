namespace SCA_MVC.Helpers
{
    public static class MensajesUI
    {
        public const string KEY_TIPO = "TipoMensaje";
        public const string KEY_CONTENIDO = "ContenidoMensaje";

        public enum TipoMensaje
        {
            Exito,
            Error,
            Advertencia,
            Info
        }
    }
}
