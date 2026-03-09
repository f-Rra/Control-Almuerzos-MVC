using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace SCA_MVC.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarReporteAsync(string destinatario, string asunto, string cuerpoHtml, byte[] adjuntoPdf, string nombreArchivo)
        {
            var settings = _config.GetSection("EmailSettings");
            var host     = settings["SmtpHost"]!;
            var port     = int.Parse(settings["SmtpPort"]!);
            var user     = settings["SmtpUser"]!;
            var pass     = settings["SmtpPass"]!;
            var from     = settings["FromEmail"]!;
            var fromName = settings["FromName"] ?? "Sistema Control de Almuerzos";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, from));
            message.To.Add(MailboxAddress.Parse(destinatario));
            message.Subject = asunto;

            var builder = new BodyBuilder { HtmlBody = cuerpoHtml };
            builder.Attachments.Add(nombreArchivo, adjuntoPdf, ContentType.Parse("application/pdf"));
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(user, pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
