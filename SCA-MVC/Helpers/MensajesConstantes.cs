namespace SCA_MVC.Helpers
{
    public static class MensajesConstantes
    {
        // Validaciones
        public const string VAL_REQUERIDO = "Este campo es obligatorio.";
        public const string VAL_LONGITUD = "La longitud no es válida.";

        // Éxito
        public const string EXITO_CREAR = "Registro creado correctamente.";
        public const string EXITO_ACTUALIZAR = "Registro actualizado correctamente.";
        public const string EXITO_ELIMINAR = "Registro desactivado correctamente.";

        // Error
        public const string ERROR_INESPERADO = "Ha ocurrido un error inesperado. Intente nuevamente.";
        public const string ERROR_NOT_FOUND = "No se encontró el registro solicitado.";
        public const string ERROR_DUPLICADO = "El registro ya existe.";

        // Advertencia
        public const string ADV_INACTIVO = "El registro ya se encuentra desactivado.";
    }
}
