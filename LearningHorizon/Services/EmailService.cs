using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Threading.Tasks;

namespace LearningHorizon.Services
{
    public static class EmailService
    {
        private static readonly string SenderEmail = "osama.reda.abdelghany@gmail.com";
        private static readonly string SenderPassword = "mifw vwzg lktc avtm";
        private static readonly string SmtpHost = "smtp.gmail.com";
        //private static readonly string SenderEmail = "noreply@learning-horizon.net";
        //private static readonly string SenderPassword = ";M=X!a0b";
        //private static readonly string SmtpHost = "smtp.hostinger.com";
        private static readonly int SmtpPort = 587;

        public static async Task SendEmailAsync(string email, string subject, BodyBuilder body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Learning Horizon", SenderEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = body.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(SmtpHost, SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(SenderEmail, SenderPassword);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while sending the email: " + ex.Message);
            }

        }

    }
}
