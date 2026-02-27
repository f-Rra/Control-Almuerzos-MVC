using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SCA_MVC.Helpers
{
    public static class MensajesUI
    {
        public const string KEY_TIPO = "ToastType";
        public const string KEY_TITULO = "ToastTitle";
        public const string KEY_CONTENIDO = "ToastMessage";

        public static void MostrarExito(this ITempDataDictionary tempData, string mensaje, string titulo = "Éxito")
        {
            tempData[KEY_TIPO] = "success";
            tempData[KEY_TITULO] = titulo;
            tempData[KEY_CONTENIDO] = mensaje;
        }

        public static void MostrarError(this ITempDataDictionary tempData, string mensaje, string titulo = "Error")
        {
            tempData[KEY_TIPO] = "error";
            tempData[KEY_TITULO] = titulo;
            tempData[KEY_CONTENIDO] = mensaje;
        }

        public static void MostrarAdvertencia(this ITempDataDictionary tempData, string mensaje, string titulo = "Advertencia")
        {
            tempData[KEY_TIPO] = "warning";
            tempData[KEY_TITULO] = titulo;
            tempData[KEY_CONTENIDO] = mensaje;
        }

        public static void MostrarInfo(this ITempDataDictionary tempData, string mensaje, string titulo = "Información")
        {
            tempData[KEY_TIPO] = "info";
            tempData[KEY_TITULO] = titulo;
            tempData[KEY_CONTENIDO] = mensaje;
        }
    }
}
