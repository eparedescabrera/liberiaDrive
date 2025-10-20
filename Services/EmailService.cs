using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace LiberiaDriveMVC.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public bool EnviarCorreo(string destinatario, string asunto, string cuerpoHtml)
        {
            try
            {
                var smtpSection = _config.GetSection("SmtpSettings");
                string host = smtpSection["Host"];
                int port = int.Parse(smtpSection["Port"]);
                bool enableSsl = bool.Parse(smtpSection["EnableSsl"]);
                string user = smtpSection["User"];
                string pass = smtpSection["Password"];

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = enableSsl;
                    client.Credentials = new NetworkCredential(user, pass);

                    var mail = new MailMessage();
                    mail.From = new MailAddress(user, "Liberia Drive 🚗");
                    mail.To.Add(destinatario);
                    mail.Subject = asunto;
                    mail.Body = cuerpoHtml;
                    mail.IsBodyHtml = true;

                    client.Send(mail);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
