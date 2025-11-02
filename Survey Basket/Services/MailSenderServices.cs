
namespace Survey_Basket.Services
{
    public class MailSenderServices(IOptions<MailSettings> mailSettings, ILogger<MailSenderServices> logger) : IEmailSender
    {

        private readonly MailSettings mailSettings = mailSettings.Value;
        private readonly ILogger<MailSenderServices> _logger = logger;


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(mailSettings.Mail),
                Subject = subject,
            };

            message.To.Add(MailboxAddress.Parse(email));

            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            //logger.LogInformation("Sending Email to {Email}", email);

            // smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);

            // smtp.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);

            //await smtp.SendAsync(message);
            // smtp.DisconnectAsync(true);

            try
            {
                logger.LogInformation("Connecting to SMTP server {Host}:{Port}", mailSettings.Host, mailSettings.Port);

                smtp.ServerCertificateValidationCallback = (s, c, h, e) =>
                {
                    logger.LogWarning("SSL Certificate validation bypassed: {Error}", e);
                    return true;
                };

                logger.LogInformation("Email sent successfully to {Email}", email);

                await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);

                await smtp.SendAsync(message);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email to {Email}", email);
            }
            finally
            {
                if (smtp.IsConnected)
                {
                    await smtp.DisconnectAsync(true);
                }
            }
        }

    }

}
