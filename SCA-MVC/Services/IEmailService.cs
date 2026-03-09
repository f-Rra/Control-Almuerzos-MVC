namespace SCA_MVC.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Envía un email con el PDF del reporte como adjunto.
        /// </summary>
        Task EnviarReporteAsync(string destinatario, string asunto, string cuerpoHtml, byte[] adjuntoPdf, string nombreArchivo);
    }
}
