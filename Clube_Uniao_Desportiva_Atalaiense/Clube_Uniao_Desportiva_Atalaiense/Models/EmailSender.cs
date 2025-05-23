// Models/EmailSender.cs
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Clube_Uniao_Desportiva_Atalaiense.Models
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> settings, ILogger<EmailSender> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        // MUITO IMPORTANTE: O método DEVE ser `async Task` e não `Task`
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("Attempting to send email to {RecipientEmail} with subject {Subject} from EmailSender.", email, subject);
            var mail = new MailMessage()
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mail.To.Add(email);

            // É uma boa prática envolver o SmtpClient num using para garantir o Dispose
            using (var smtp = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort))
            {
                smtp.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                smtp.EnableSsl = true;
                // Podes ajustar o timeout se necessário, mas o padrão (100s) deve ser suficiente
                // smtp.Timeout = 30000; // 30 segundos

                try
                {
                    // MUITO IMPORTANTE: Usar `await` aqui
                    await smtp.SendMailAsync(mail);
                    _logger.LogInformation("Email successfully sent to {RecipientEmail} from EmailSender.", email);
                }
                catch (SmtpException smtpEx)
                {
                    _logger.LogError(smtpEx, "SmtpException in EmailSender for {RecipientEmail}. Status Code: {StatusCode}", email, smtpEx.StatusCode);
                    throw; // Re-lança para que o chamador saiba
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Generic exception in EmailSender for {RecipientEmail}", email);
                    throw; // Re-lança
                }
            }
        }
    }
}